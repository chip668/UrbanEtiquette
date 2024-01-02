using System.Net;

public class LocationInfo
{
    public double Latitude { get; }
    public double Longitude { get; }
    public string sLatitude => Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public string sLongitude => Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public string Street { get; private set; }
    public string HouseNumber { get; private set; }
    public string PostalCode { get; private set; }
    public string City { get; private set; }
    public bool Valid { get { return (Street != null && HouseNumber != null && PostalCode != null && City != null); } }

    public LocationInfo(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Street = null;
        HouseNumber = null;
        PostalCode = null;
        City = null;
    }

    public void RetrieveAddressSync()
    {
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("user-agent", "YOUR_APP_NAME");

            string apiUrl = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={sLatitude}&lon={sLongitude}";

            string json = client.DownloadString(apiUrl);

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            Street = data.address.road;
            HouseNumber = data.address.house_number;
            PostalCode = data.address.postcode;
            City = data.address.city;
        }
    }
}
