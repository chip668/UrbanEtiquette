using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Anzeige
{
    public class GpxPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime GpxTime { get; set; }        // Original GPX-Zeit
        public DateTime VideoSyncTime { get; set; }  // Zeit synchronisiert zum Video

        public GpxPoint(double lat, double lon, DateTime gpxTime, DateTime videoSyncTime)
        {
            Latitude = lat;
            Longitude = lon;
            GpxTime = gpxTime;
            VideoSyncTime = videoSyncTime;
        }
    }
    public class GpxReader
    {
        public List<GpxPoint> Points { get; private set; } = new List<GpxPoint>();
        // GPX einlesen und Video-Sync berechnen
        public void Load(string filePath, DateTime videoStartTime, DateTime gpxStartTime)
        {
            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = "http://www.topografix.com/GPX/1/1";

            foreach (var trkpt in doc.Descendants(ns + "trkpt"))
            {
                double lat = double.Parse(trkpt.Attribute("lat").Value, CultureInfo.InvariantCulture);
                double lon = double.Parse(trkpt.Attribute("lon").Value, CultureInfo.InvariantCulture);

                DateTime gpxTime = DateTime.MinValue;
                var timeElement = trkpt.Element(ns + "time");
                if (timeElement != null)
                    gpxTime = DateTime.Parse(timeElement.Value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

                // Video-Sync berechnen
                TimeSpan offset = gpxTime - gpxStartTime;
                DateTime videoSyncTime = videoStartTime + offset;

                Points.Add(new GpxPoint(lat, lon, gpxTime, videoSyncTime));
            }
        }
        // 1. Distanz zwischen zwei GPX-Punkten
        public static double Distance(GpxPoint p1, GpxPoint p2)
        {
            double R = 6371000; // Erdradius
            double lat1 = p1.Latitude * Math.PI / 180;
            double lat2 = p2.Latitude * Math.PI / 180;
            double dLat = (p2.Latitude - p1.Latitude) * Math.PI / 180;
            double dLon = (p2.Longitude - p1.Longitude) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        // 2. Strecke zwischen zwei Indizes summieren
        public double DistanceBetweenIndices(int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= Points.Count || startIndex >= endIndex)
                throw new ArgumentException("Ungültige Indizes");

            double distance = 0.0;
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                distance += Distance(Points[i - 1], Points[i]);
            }
            return distance;
        }
        // 3. Gesamte Strecke 0..n
        public double TotalDistance()
        {
            return DistanceBetweenIndices(0, Points.Count - 1);
        }
        // 4. Bestimme Index zu einer GPX-Zeit
        public int IndexAtGpxTime(DateTime gpxTime)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].GpxTime >= gpxTime)
                    return i;
            }
            return Points.Count - 1;
        }
        // 5. Bestimme Index zu einer Video-Zeit
        public int IndexAtVideoTime(DateTime videoTime)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (Points[i].VideoSyncTime >= videoTime)
                    return i;
            }
            return Points.Count - 1;
        }
        // Optional: Distanz zwischen zwei GPX-Zeiten
        public double DistanceBetweenGpxTimes(DateTime startTime, DateTime endTime)
        {
            int startIndex = IndexAtGpxTime(startTime);
            int endIndex = IndexAtGpxTime(endTime);
            return DistanceBetweenIndices(startIndex, endIndex);
        }
        // Optional: Distanz zwischen zwei Video-Zeiten
        public double DistanceBetweenVideoTimes(DateTime startTime, DateTime endTime)
        {
            int startIndex = IndexAtVideoTime(startTime);
            int endIndex = IndexAtVideoTime(endTime);
            return DistanceBetweenIndices(startIndex, endIndex);
        }
    }
}
