using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class WegLiCsvReader
    {
        private const string CsvUrl = "https://www.weg.li/districts.csv";

        public Dictionary<string, District> LoadDistricts()
        {
            var districts = new Dictionary<string, District>();

            using (var client = new WebClient())
            {
                string csvContent = client.DownloadString(CsvUrl);
                var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 2)
                    return districts;

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Replace("\"", "");
                    string[] parts = line.Split(',');

                    if (parts.Length >= 2)
                    {
                        string plz = parts[0].Trim();
                        string city = parts[1];
                        string email = parts[2];


                        if (!districts.ContainsKey(plz))
                        {
                            District district = new District()
                            {
                                PostalCode = plz,
                                City = city,
                                Email = email
                            };
                            districts.Add(plz, district);
                        }
                    }
                }
            }

            return districts;
        }
    }
}
