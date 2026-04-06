using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class FarradPass
    {
        public bool Changed { get; set; }
        public string Name 
        { 
            get
            {
                return ToString();
            } 
        }    
        // Grunddaten
        public string Hersteller { get; set; }
        public string Modell { get; set; }
        public string Farbe { get; set; }
        public string Rahmennummer { get; set; }  // Eindeutige ID
        public string Fahrradtyp { get; set; }    // z.B. Citybike, MTB, Rennrad, E-Bike
        public double Rahmengröße { get; set; }  // optional
        public double Reifengröße { get; set; }  // optional
        public decimal Kaufpreis { get; set; }
        public decimal Zeitwert
        {
            get
            {
                const decimal abschreibung = 0.18m;

                int jahre = Math.Max(
                    0,
                    (int)((DateTime.Today - Kaufdatum).TotalDays / 365.25)
                );

                decimal faktor = (decimal)Math.Pow(
                    (double)(1 - abschreibung),
                    jahre
                );

                return Math.Round(Kaufpreis * faktor, 2);
            }
            set
            {
                // ignore
            }
        }
        public DateTime Kaufdatum { get; set; }
        // Besonderheiten

        public string _Merkmale = ""; // Kratzer, Aufkleber, Umbauten
        public string Merkmale 
        { 
            get { return _Merkmale; }
            set { _Merkmale = value.Replace("\r\n", "[crlf]").Replace("\r", "\n").Replace("\r\n", "[crlf]"); } 
        } // Kratzer, Aufkleber, Umbauten
        public string MerkmaleText
        {
            get { return _Merkmale.Replace("[crlf]", "\r\n"); }
        } // Kratzer, Aufkleber, Umbauten
        // Diebstahlschutz / Codierung
        public bool IstCodiert { get; set; } = false;
        public string CodierungsID { get; set; }  // falls codiert
        public string SchlossTyp { get; set; }    // Bügelschloss, Kette, Faltschloss
        public string SchlossMarke { get; set; }
        // Diebstahlinformationen
        public string Tatort { get; set; }
        public DateTime? TatzeitVon { get; set; }
        public DateTime? TatzeitBis { get; set; }
        public bool WurdeBeschädigt { get; set; } = false;
        // Optionale Tracker / Smart Devices
        public bool HatGpsTracker { get; set; } = false;
        public string TrackerID { get; set; }
        // Fotos
        public List<string> FotoDateien { get; set; } = new List<string>(); // Pfade zu Fotos
        // Ausgabe als Übersicht
        public string Haendler { get; set; }    // z.B. Citybike, MTB, Rennrad, E-Bike
        public override string ToString()
        {
            string cngflg = Changed ? "*" : " ";

            return $"{cngflg}Fahrrad: {Hersteller} {Modell}, Farbe: {Farbe}, Rahmennummer: {Rahmennummer}, Typ: {Fahrradtyp}";
        }
        
        public void Speichern(string dateipfad)
        {
            string backupPfad = dateipfad + ".bak";

            // Alte Backup-Datei löschen, falls vorhanden
            if (File.Exists(backupPfad))
            {
                File.Delete(backupPfad);
            }

            // Vorhandene Original-Datei auf Backup verschieben
            if (File.Exists(dateipfad))
            {
                File.Move(dateipfad, backupPfad);
            }

            // Neue Datei schreiben
            using var writer = new StreamWriter(dateipfad);

            writer.WriteLine($"Hersteller={Hersteller}");
            writer.WriteLine($"Modell={Modell}");
            writer.WriteLine($"Farbe={Farbe}");
            writer.WriteLine($"Rahmennummer={Rahmennummer}");
            writer.WriteLine($"Fahrradtyp={Fahrradtyp}");
            writer.WriteLine($"Rahmengroesse={Rahmengröße}");
            writer.WriteLine($"Reifengroesse={Reifengröße}");
            writer.WriteLine($"Kaufpreis={Kaufpreis.ToString(CultureInfo.InvariantCulture)}");
            writer.WriteLine($"Zeitwert={Zeitwert.ToString(CultureInfo.InvariantCulture)}");
            writer.WriteLine($"Kaufdatum={Kaufdatum:yyyy-MM-dd}");

            writer.WriteLine();
            writer.WriteLine($"IstCodiert={IstCodiert}");
            writer.WriteLine($"CodierungsID={CodierungsID}");
            writer.WriteLine($"SchlossTyp={SchlossTyp}");
            writer.WriteLine($"SchlossMarke={SchlossMarke}");
            writer.WriteLine($"Haendler={Haendler}");

            writer.WriteLine();
            writer.WriteLine($"Merkmale={Merkmale}");

            foreach (var foto in FotoDateien)
                writer.WriteLine($"Foto={foto}");
        }
        public static FarradPass Laden(string dateipfad)
        {
            if (dateipfad == null)
                throw new ArgumentNullException(nameof(dateipfad));

            if (!File.Exists(dateipfad))
                throw new FileNotFoundException("Fahrradpass-Datei nicht gefunden", dateipfad);

            var pass = new FarradPass();

            foreach (var zeile in File.ReadLines(dateipfad))
            {
                if (string.IsNullOrWhiteSpace(zeile))
                    continue;

                int idx = zeile.IndexOf('=');
                if (idx <= 0)
                    continue;

                string key = zeile.Substring(0, idx).Trim();
                string value = zeile.Substring(idx + 1).Trim();

                switch (key)
                {
                    case "Hersteller":
                        pass.Hersteller = value;
                        break;

                    case "Modell":
                        pass.Modell = value;
                        break;

                    case "Farbe":
                        pass.Farbe = value;
                        break;

                    case "Rahmennummer":
                        pass.Rahmennummer = value;
                        break;

                    case "Fahrradtyp":
                        pass.Fahrradtyp = value;
                        break;

                    case "Rahmengroesse":
                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double rahmen))
                            pass.Rahmengröße = rahmen;
                        break;

                    case "Reifengroesse":
                        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double reifen))
                            pass.Reifengröße = reifen;
                        break;

                    case "Kaufpreis":
                        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal preis))
                            pass.Kaufpreis = preis;
                        break;

                    case "Zeitwert":
                        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal zeitwert))
                            pass.Zeitwert = zeitwert;
                        break;

                    case "Kaufdatum":
                        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datum))
                            pass.Kaufdatum = datum;
                        break;

                    case "IstCodiert":
                        if (bool.TryParse(value, out bool codiert))
                            pass.IstCodiert = codiert;
                        break;

                    case "CodierungsID":
                        pass.CodierungsID = value;
                        break;

                    case "Haendler":
                    case "Händler":
                        pass.Haendler = value;
                        break;

                    case "SchlossTyp":
                        pass.SchlossTyp = value;
                        break;

                    case "SchlossMarke":
                        pass.SchlossMarke = value;
                        break;

                    case "Merkmale":
                        pass.Merkmale = value.Replace("[crlf]", "\r\n");
                        break;

                    case "Foto":
                        pass.FotoDateien.Add(value);
                        break;

                    default:
                        // unbekannter Key → bewusst ignorieren (zukunftssicher)
                        break;
                }
            }

            return pass;
        }

        public System.Drawing.Bitmap QRCodeAlsBitmap()
        {
            // QR-Code-Inhalt aus diesem FarradPass
            string qrText =
                $@"H:{Hersteller}
                M:{Modell}
                C:{Farbe}
                N:{Rahmennummer}
                T:{Fahrradtyp}
                S:{Rahmengröße}
                R:{Reifengröße}
                P:{Kaufpreis.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}
                W:{Zeitwert.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}
                D:{Kaufdatum:yyyy-MM-dd}
                O:{MerkmaleText}";

            // QR-Code generieren
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCoder.QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCoder.QRCode(qrCodeData);

            // Bitmap erzeugen und zurückgeben
            return qrCode.GetGraphic(20); // 20 Pixel pro Modul
        }
    }
}
