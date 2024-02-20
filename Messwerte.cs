using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class Messwerte
    {
        public class Messwert
        {
            public enum Kopplung
            {
                leftMean,
                leftMin
            };
            public Kopplung mode { get; set; }
            private int _Abstand;
            public int Abstand
            {
                get 
                { 
                    switch (mode)
                    {
                        case Kopplung.leftMean:
                            return MittelwertLinks;
                            break;

                        case Kopplung.leftMin:
                            return MinimumLinks;
                            break;
                        default:
                            return MinimumLinks;
                            break;
                    }
                }
                set
                {
                    _Abstand = value;
                }
            }
            public int Abstand2
            {
                get { return _Rechts; }
                set
                {
                    _Rechts = value;
                }
            }
            private int _Rechts = 100;
            public int Rechts
            {
                get
                {
                    return _Rechts;
                }
                set
                {
                    _Rechts = value;
                    Abstand2 = _Rechts;
                }
            }
            private int _LinksVorne;
            public int LinksVorne
            {
                get { return _LinksVorne; }
                set
                {
                    _LinksVorne = value;
                }
            }
            private int _LinksHinten;
            public int LinksHinten
            {
                get { return _LinksHinten; }
                set
                {
                    _LinksHinten = value;
                    // Fügen Sie hier zusätzliche Logik ein, wenn nötig
                    // Beispiel: Abstand4 = _LinksHinten;
                }
            }
            public int MittelwertLinks
            {
                get { return (_LinksHinten + _LinksVorne) / 2; }
            }
            public int MaximumLinks
            {
                get { return Math.Max(_LinksHinten, _LinksVorne); }
            }
            public int MinimumLinks
            {
                get { return Math.Min(_LinksHinten, _LinksVorne); }
            }
            public int Delta
            {
                get { return _LinksVorne - _LinksHinten; }
            }
            public double Longitude { get; private set; }
            public double Latitude { get; private set; }
            public String Zeit { get; private set; }
            public string Line
            {
                get => $"{Rechts},{LinksVorne},{LinksHinten},{Longitude},{Latitude},{Zeit}";
                set
                {
                    string[] values = value.Split(',');

                    try
                    {
                        if (values.Length > 0) Rechts = Convert.ToInt32(values[0]);
                        if (values.Length > 1) LinksVorne = Convert.ToInt32(values[1]);
                        if (values.Length > 2) LinksHinten = Convert.ToInt32(values[2]);
                        if (values.Length > 3) Longitude = Convert.ToDouble(values[3]);
                        if (values.Length > 4) Latitude = Convert.ToDouble(values[4]);
                        if (values.Length > 5) Zeit = values[5];
                        Abstand = MinimumLinks;
                    }
                    catch
                    {

                    }
                }
            }
            public Messwert(int abstand, int abstand2 = 120, int rechts = 120, int linksVorne = 150, int linksHinten = 150, double longitude = 0, double latitude = 0, String zeit = "")
            {
                if (zeit == "")
                    zeit = DateTime.Now.ToString();
                Abstand = abstand;
                Abstand2 = abstand2;
                Rechts = rechts;
                LinksVorne = linksVorne;
                LinksHinten = linksHinten;
                Longitude = longitude;
                Latitude = latitude;
                Zeit = zeit;
                mode = Kopplung.leftMin;
            }
            public Messwert(string line)
            {
                Line = line;
                mode = Kopplung.leftMin;
            }
        }

        public String filename { get; set; }
        public String[] data { get; set; }
        public Messwert this[int index]
        {
            get
            {
                if (index >= 0 && index < data.Length)
                {
                    return new Messwert(data[index]);
                }
                else
                {
                    throw new IndexOutOfRangeException("Index ist außerhalb des gültigen Bereichs.");
                }
            }
        }
        public Messwerte(string filename)
        {
            this.filename = filename;
            this.data = File.ReadAllLines(this.filename);
        }
    }
}
