using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class PhotoCollection
    {
        public class PhotoRecord
        {
            public string? FilePath { get; }
            public Bitmap? Image { get; }
            public DateTime? OriginalDateTime { get; }
            public double? Latitude { get; }
            public double? Longitude { get; }
            public string? AddressInfo { get; }
            public bool HasGeo => Latitude.HasValue && Longitude.HasValue;
            // 1️⃣ Konstruktor: Datei mit Pfad
            public PhotoRecord(string filePath)
            {
                FilePath = filePath;
                var extractor = new PhotoMetadataExtractor(filePath);
                OriginalDateTime = ParseDateTime(extractor.Date, extractor.Time);
                if (extractor.Valid)
                {
                    Latitude = extractor.Latitude;
                    Longitude = extractor.Longitude;
                    AddressInfo = $"{extractor.Street} {extractor.HouseNumber}, {extractor.PostalCode} {extractor.City}";
                }
            }
            // 2️⃣ Konstruktor: Bitmap (kein EXIF)
            public PhotoRecord(Bitmap bitmap, DateTime dateTime)
            {
                Image = bitmap;
                OriginalDateTime = dateTime;
            }
            // 3️⃣ Konstruktor: Screenshot aus Video
            public PhotoRecord(Bitmap frame, DateTime videoFileDateTime, TimeSpan frameOffset)
            {
                Image = frame;
                OriginalDateTime = videoFileDateTime + frameOffset;
            }
            private DateTime? ParseDateTime(string? date, string? time)
            {
                if (DateTime.TryParse($"{date} {time}", out var dt))
                    return dt;

                return null;
            }
        }
        private readonly List<PhotoRecord> _items = new();
        public IReadOnlyList<PhotoRecord> Items => _items;
        public void Add(PhotoRecord record)
        {
            _items.Add(record);
        }
        public (double Latitude, double Longitude)? AverageGeo
        {
            get
            {
                var valid = _items
                    .Where(x => x.HasGeo)
                    .ToList();
                if (!valid.Any())
                    return null;
                return (
                    valid.Average(x => x.Latitude!.Value),
                    valid.Average(x => x.Longitude!.Value)
                );
            }
        }
    }
}
