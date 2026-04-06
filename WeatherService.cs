using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json; // Benötigt NuGet-Paket System.Text.Json
using System.Linq;

namespace Anzeige
{
    public class WeatherService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetWeatherReportAsync(double lat, double lon, DateTime timestamp)
        {
            try
            {
                // Open-Meteo Historical API Format: YYYY-MM-DD
                string dateStr = timestamp.ToString("yyyy-MM-dd");
                int hour = timestamp.Hour;

                // Wir fragen Temperatur, Wetter-Code (WMO) und Sichtweite ab
                string url = $"https://archive-api.open-meteo.com/v1/archive?latitude={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&start_date={dateStr}&end_date={dateStr}&hourly=weather_code,visibility&timezone=auto";

                var response = await client.GetStringAsync(url);
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    var hourly = doc.RootElement.GetProperty("hourly");
                    var codes = hourly.GetProperty("weather_code").EnumerateArray().ToList();
                    var visibilities = hourly.GetProperty("visibility").EnumerateArray().ToList();

                    // Wir nehmen den Index der entsprechenden Stunde
                    int weatherCode = codes[hour].GetInt32();
                    double visibilityKm = visibilities[hour].GetDouble() / 1000.0;

                    return InterpretWeather(weatherCode, visibilityKm);
                }
            }
            catch (Exception ex)
            {
                return $"Wetterdaten nicht verfügbar ({ex.Message})";
            }
        }

        private string InterpretWeather(int code, double visibility)
        {
            string condition = code switch
            {
                0 => "klarer Himmel",
                1 or 2 or 3 => "leicht bewölkt",
                45 or 48 => "Nebel/Reif",
                51 or 53 or 55 => "Nieselregen",
                61 or 63 or 65 => "Regen",
                71 or 73 or 75 => "Schneefall",
                95 => "Gewitter",
                _ => "unbekannte Wetterlage"
            };

            return $"{condition}, Sichtweite: ca. {visibility:F1} km";
        }
    }
}
