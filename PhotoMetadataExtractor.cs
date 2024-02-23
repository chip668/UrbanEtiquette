using ExifLib;
using System;
using System.IO;


namespace Anzeige
{

    class PhotoMetadataExtractor
    {
        private string _imagePath;

        public string Time { get; private set; }
        public string Date { get; private set; }
        public string GoogleMapsURL { get; private set; }
        public string Street { get; private set; }
        public string HouseNumber { get; private set; }
        public string PostalCode { get; private set; }
        public string City { get; private set; }
        public bool Valid { get; private set; }


        public PhotoMetadataExtractor(string imagePath)
        {
            _imagePath = imagePath;
            ExtractMetadata();
        }

        private void ExtractMetadata()
        {
            try
            {
                using (FileStream stream = File.OpenRead(_imagePath))
                {
                    var exifReader = new ExifReader(stream);

                    // Datum und Uhrzeit aus den Metadaten extrahieren
                    if (exifReader.GetTagValue(ExifTags.DateTimeOriginal, out DateTime dateTime))
                    {
                        Date = dateTime.Date.ToString("dd MMMM yyyy");
                        Time = dateTime.TimeOfDay.ToString(@"hh\:mm");
                    }

                    // GPS-Koordinaten aus den Metadaten extrahieren
                    if (exifReader.GetTagValue(ExifTags.GPSLatitude, out double[] latitude) &&
                        exifReader.GetTagValue(ExifTags.GPSLongitude, out double[] longitude))
                    {
                        double latitudeValue = ConvertGpsCoordinate(latitude);
                        double longitudeValue = ConvertGpsCoordinate(longitude);

                        // Generiere die Google Maps URL mit den tatsächlichen Geodaten des Bildes
                        GoogleMapsURL = Tools.GenerateGoogleMapsURL(latitudeValue, longitudeValue);

                        LocationInfo lki = new LocationInfo(latitudeValue, longitudeValue);
                        lki.RetrieveAddressSync();
                        Street = lki.Street;
                        HouseNumber = lki.HouseNumber;
                        PostalCode = lki.PostalCode;
                        City = lki.City;
                        Valid = lki.Valid;
                    }
                }
            }
            catch { }
        }

        private double ConvertGpsCoordinate(double[] coordinates)
        {
            double degrees = coordinates[0];
            double minutes = coordinates[1];
            double seconds = coordinates[2];

            return degrees + (minutes / 60.0) + (seconds / 3600.0);
        }
    }
}
