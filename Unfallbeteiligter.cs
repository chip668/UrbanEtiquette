using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace Anzeige
{

    public class Unfallbeteiligter
    {
        /* ===================== ENUMS ===================== */
        public enum Fahrzeugart
        {
            Unbekannt,
            Pkw,
            Lkw,
            Bus,
            Motorrad,
            Fahrrad,
            EScooter,
            Traktor,
            Tram,
            Zug,
            Sonstiges,
            KeinFahrzeug // Fußgänger
        }
        public enum Zustand
        {
            InOrdnung,
            NichtInOrdnung,
            Unbekannt
        }
        public enum Kopplungsart
        {
            Anhaenger,
            Auflieger,
            Beiwagen,
            Sonstiges
        }
        /* ===================== KLASSE PERSON ===================== */
        public class Person
        {
            public enum PersonArt
            {
                Fahrer,
                Insasse,
                Halter
            }
            public PersonArt Personart = PersonArt.Insasse;
            public string Name { get; set; }
            public string Ort { get; set; }
            public string Strasse { get; set; }
            public string Telefon { get; set; }
            public bool Verletzt { get; set; }
            public string ArtDerVerletzung { get; set; }
            public string Sicherungssystem { get; set; } = "";
            public string ToString()
            {
                return Personart.ToString() + ":" + Name;
            }
            public string DisplayMember
            {
                get { return ToString(); }
            }
        }
        /* ===================== FORMULARFELDER DES UNFALLBETEILIGTEN ===================== */
        // Fahrzeugdaten direkt eingebettet
        public Fahrzeugart ArtDesFahrzeugs { get; set; } = Fahrzeugart.Unbekannt;
        public string AmtlichesKennzeichen { get; set; } = "";
        public string Hersteller { get; set; } = "";
        public string Typ { get; set; } = "";
        public Zustand Bereifung { get; set; } = Zustand.Unbekannt;
        // Optionaler Anhänger
        public bool HatAnhaenger { get; set; } = false;
        public Kopplungsart AnhaengerArt { get; set; } = Kopplungsart.Anhaenger;
        public string AnhaengerKennzeichen { get; set; } = "";
        // Versicherung (falls relevant)
        public string HaftpflichtversichererNameUndAnschrift { get; set; } = "";
        // Sonstige Schäden
        public string SonstigeSachschaden { get; set; } = "";
        // Alle Personen, die zu diesem Fahrzeug gehören (Fahrer + Mitfahrer)
        public List<Person> Personen { get; set; } = new();
        public string Unfallhergang { get; set; } = "";

        public string ToString()
        {
            return ArtDesFahrzeugs.ToString() + ":" + Hersteller + ";" + AmtlichesKennzeichen;
        }
        public string DisplayMember
        {
            get { return ToString(); }
        }
        public void Speichern(string pfad)
        {
        }

        public static Unfallbeteiligter Laden(string pfad)
        {
            return null;
        }

        public string BerichtAlsText()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Fahrzeug: {ArtDesFahrzeugs} ({Hersteller} {Typ})");
            sb.AppendLine($"Kennzeichen: {AmtlichesKennzeichen}");
            sb.AppendLine($"Bereifung: {Bereifung}");
            if (HatAnhaenger)
                sb.AppendLine($"Anhänger: {AnhaengerArt} ({AnhaengerKennzeichen})");
            sb.AppendLine($"Versicherung: {HaftpflichtversichererNameUndAnschrift}");
            sb.AppendLine($"Sonstige Schäden: {SonstigeSachschaden}");
            sb.AppendLine($"Personen ({Personen.Count}):");
            foreach (var p in Personen)
            {
                sb.AppendLine($"  - {p.Personart}: {p.Name}, {p.Strasse}, {p.Ort}, Tel: {p.Telefon}");
                if (p.Verletzt)
                    sb.AppendLine($"    Verletzung: {p.ArtDerVerletzung}, Sicherungssystem: {p.Sicherungssystem}");
            }
            sb.AppendLine(); 
            sb.AppendLine("Unfallhergang:"); 
            sb.AppendLine(Unfallhergang);
            return sb.ToString();
        }

        public void Drucken()
        {
            var doc = new PrintDocument();
            doc.PrintPage += (s, e) =>
            {
                e.Graphics.DrawString(
                    BerichtAlsText(),
                    new Font("Consolas", 10),
                    Brushes.Black,
                    e.MarginBounds
                );
            };
            doc.Print();
        }
    }
}