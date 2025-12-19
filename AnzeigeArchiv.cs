using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Anzeige
{
    /// <summary>
    /// Verwaltung eines Anzeige-Archivs mit sofortigem Scan der Zielpfade
    /// </summary>
    public class AnzeigeArchiv
    {
        /// <summary>
        /// Haupt-/Wurzelverzeichnisse, die durchsucht wurden
        /// </summary>
        public IReadOnlyList<string> HauptPfade { get; }

        /// <summary>
        /// Alle gefundenen Anzeigeeinträge
        /// </summary>
        public IReadOnlyList<AnzeigeEintrag> Eintraege { get; }

        /// <summary>
        /// Konstruktor: scannt sofort alle Wurzelpfade nach Anzeige-Verzeichnissen
        /// </summary>
        public AnzeigeArchiv(params string[] wurzelPfade)
        {
            if (wurzelPfade == null || wurzelPfade.Length == 0)
                throw new ArgumentException("Mindestens ein Pfad muss angegeben werden.");

            HauptPfade = wurzelPfade.ToList();

            var eintraege = new List<AnzeigeEintrag>();

            foreach (var wurzel in wurzelPfade)
            {
                if (string.IsNullOrWhiteSpace(wurzel) || !Directory.Exists(wurzel))
                    continue;

                try
                {
                    // Alle Anzeige.txt-Dateien rekursiv finden
                    var dateien = Directory.GetFiles(wurzel, "Anzeige.txt", SearchOption.AllDirectories);

                    foreach (var datei in dateien)
                    {
                        if (eintraege.Count == 226)
                            ;
                        // Kennzeichen korrekt extrahieren
                        AnzeigeEintrag eintrag = ExtractKennzeichen(datei);
                        if (eintrag!=null)
                        eintraege.Add(eintrag);
                    }
                }
                catch
                {
                    // optional: Logging
                    continue;
                }
            }

            Eintraege = eintraege;
        }

        /// <summary>
        /// Suche nach allen Anzeigen für ein bestimmtes Kennzeichen
        /// </summary>
        public IEnumerable<AnzeigeEintrag> SucheNachKennzeichen(string kennzeichen)
        {
            if (string.IsNullOrWhiteSpace(kennzeichen))
                return Enumerable.Empty<AnzeigeEintrag>();

            return Eintraege
                .Where(a => a.Kennzeichen.Equals(kennzeichen, StringComparison.OrdinalIgnoreCase));
        }
        public List<AnzeigeEintrag> SucheNachKennzeichenListe(string kennzeichen)
        {
            // Einfach die Iterator-Version aufrufen und in Liste umwandeln
            return SucheNachKennzeichen(kennzeichen).ToList();
        }
        private AnzeigeEintrag ExtractKennzeichen(string datei)
        {
            AnzeigeEintrag result = null;
            string[] pathiterm = datei.Split("\\");
            for (int i = 0; i < pathiterm.Length; i++)
            {
                if (pathiterm[i].ToLower() == "anzeigen")
                {
                    if ((pathiterm[i].ToLower() == "archiv") ||
                        (pathiterm[i].ToLower() == "neuer-ordner") ||
                        (pathiterm[i].ToLower() == "leer") ||
                        (pathiterm[i].ToLower() == "geschenkt") ||
                        (pathiterm[i].ToLower() == "sonsdtiges") ||
                        (pathiterm[i].ToLower() == "unbekannt")
                        )
                        i++;
                    string ort = string.Empty;
                    string kennzeichen = string.Empty;
                    string datum = string.Empty;
                    bool hassubdirs = false;

                    if (pathiterm.Length > i + 1)
                    {
                        ort = pathiterm[i + 1];
                    }
                    if (pathiterm.Length > i + 2)
                    {
                        if ((pathiterm[i + 2].Length == 8) && (pathiterm[i + 2].All(char.IsDigit)))
                            datum = pathiterm[i + 2];
                        else if (pathiterm[i + 2].ToLower() != "anzeige.txt")
                            kennzeichen =  pathiterm[i + 2];
                    }
                    if (pathiterm.Length > i + 3)
                    {
                        if (pathiterm[i + 3].ToLower() != "anzeige.txt")
                            datum = pathiterm[i + 3];
                    }

                    hassubdirs= (pathiterm.Length > i + 4);
                    result = new AnzeigeEintrag();
                    result.ZielPfad = string.Join("\\", pathiterm.Take(i + 1));
                    result.AnzeigeDatei = datei;
                    result.Kennzeichen = kennzeichen;
                    result.Ort = ort;

                    // if unkown numberplate exchange time and plate number
                    if (result.Kennzeichentyp == AnzeigeEintrag.KennzeichenTyp.Unbekannt)
                    {
                        if ((kennzeichen.Length == 8) && (kennzeichen.All(char.IsDigit)))
                        {
                            string kennz = datum;
                            string dtm = kennzeichen;

                            result.Kennzeichen = kennz;
                        }
                    }
                    try
                    {
                        result.Datum = DateTime.ParseExact(datum, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        if (File.Exists(datei))
                        {
                            FileInfo fi = new FileInfo(datei);
                            result.Datum = fi.CreationTime;
                        }
                    }
                    try
                    {
                        if (File.Exists(datei))
                            result.Text = File.ReadAllText(datei);
                    }
                    catch
                    {
                        // ignore
                    }
                    return result;
                }
            }
            return result;
        }
        private bool IsDatumOrdner(string ordnerName)
        {
            return ordnerName.Length == 8 && ordnerName.All(char.IsDigit);
        }

        // Standard-Mapping: eindeutige Zuordnung ohne Duplikate
        private static readonly Dictionary<string, string> umlauts = new()
        {
            { "ä", "ae" }, { "Ä", "Ae" },
            { "ö", "oe" }, { "Ö", "Oe" },
            { "ü", "ue" }, { "Ü", "Ue" },
            { "ß", "ss" },
            { "å", "aa" }, { "Å", "Aa" },
            { "ø", "oe" }, { "Ø", "Oe" },
            { "æ", "ae" }, { "Æ", "Ae" },
            { "é", "e" }, { "É", "E" },
            { "è", "e" }, { "È", "E" },
            { "ê", "e" }, { "Ê", "E" },
            { "ë", "e" }, { "Ë", "E" },
            { "ñ", "n" }, { "Ñ", "N" }
        };
        private static readonly Dictionary<string, string> specialChars = new()
        {
            // Copyright / Trademark
            { "©", "(C)" },
            { "®", "(R)" },
            { "™", "(TM)" },

            // Maße / Technik
            { "°", "deg" },       // Gradzeichen
            { "‰", "per mille" }, // Promille
            { "‱", "per ten thousand" }, // Basis-Punkte
            { "µ", "u" },         // Mikro
            { "Ω", "Ohm" },       // Ohm
            { "⌀", "/" },         // Durchmesserzeichen
            { "±", "+/-" },       // Plus/Minus
            { "÷", "/" },         // Division
            { "×", "x" },         // Multiplikation
            { "∑", "Sum" },       // Summenzeichen
            { "√", "sqrt" },      // Wurzel
            { "∞", "inf" },       // Unendlichkeit

            // Währungssymbole
            { "€", "EUR" },
            { "$", "USD" },
            { "£", "GBP" },
            { "¥", "JPY" },
            { "₣", "FRF" },
            { "₧", "ESP" },
            { "₤", "ITL" },
            { "₿", "BTC" },

            // Pfeile / Richtung
            { "→", "->" },
            { "←", "<-" },
            { "↑", "^" },
            { "↓", "v" },

            // Sonstige Symbole
            { "§", "Par" },       // Paragraph
            { "¶", "Pilcrow" },   // Absatzzeichen
            { "†", "+" },         // Kreuz
            { "★", "*" },         // Stern
            { "☆", "*" },         // Leerer Stern
            { "♥", "<3" },        // Herz
            { "♦", "<>" },        // Karo
            { "♣", "CL" },        // Kreuz
            { "♠", "SP" }         // Pik
        };
        public static string toUmlauts(string text)
        {
            return Convert(text, umlauts);
        }
        public static string toSpecials(string text)
        {
            return Convert(text, specialChars);
        }

        public static double CompareSimular(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return 0.0;

            int n = source.Length;
            int m = target.Length;
            var d = new int[n + 1, m + 1];

            // Initialisierung
            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            // Matrix füllen
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            int distance = d[n, m];
            int maxLen = Math.Max(n, m);

            // Ähnlichkeit = 1 - (Distanz / maxLen)
            return 1.0 - (double)distance / maxLen;
        }
        public static string Convert(string text, Dictionary<string, string> convertlist)
        {
            string result = text;
            foreach (string key in convertlist.Keys)
                result = result.Replace(key, convertlist[key]);
            return result;
        }

    }

    /// <summary>
    /// Einzelner Anzeigeeintrag
    /// </summary>
    public class AnzeigeEintrag
    {
        public string GetSuchText (bool fulltext)
        {
            string result = $"{Kennzeichen}|{Ort}|{Datum:dd.MM.yyyy HH:mm}|{AnzeigeDatei}";

            if (fulltext)
                result = result + Text;
            return result;

        }
        public string ShowFullinfo
        {
            get => $"{Kennzeichen}|{Ort}|{Datum:dd.MM.yyyy HH:mm}";
        }
        public enum KennzeichenTyp
        {
            StandardPKW,
            ElektroPKW,
            Historisch,
            Auslaender,
            Unbekannt
        }
        // Standardmuster für deutsche Kennzeichen: <Ort>-<ABC><ABC><Zahl1-4><E|H|...> optional
        private static readonly Regex StandardRegex = new Regex(
            @"^(?<Ort>[A-Z]{1,3})-(?<Buchstaben>[A-Z]{1,3})(?<Zahlen>\d{1,4})(?<Suffix>[EH]?)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Beispielmuster für Ausländerkennzeichen (vereinfacht)
        private static readonly Regex AuslaenderRegex = new Regex(
            @"^[A-Z]{1,3}-[A-Z]{1,3}\d{1,4}$", // kann noch erweitert werden
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public KennzeichenTyp Kennzeichentyp
        {
            get => ParseKennzeichen(Kennzeichen);
        }
        public KennzeichenTyp ParseKennzeichen(string kennzeichen)
        {
            if (string.IsNullOrWhiteSpace(kennzeichen))
                return KennzeichenTyp.Unbekannt;

            kennzeichen = kennzeichen.Replace(" ", "").ToUpper();

            var match = StandardRegex.Match(kennzeichen);
            if (match.Success)
            {
                var suffix = match.Groups["Suffix"].Value;
                if (suffix == "E") return KennzeichenTyp.ElektroPKW;
                if (suffix == "H") return KennzeichenTyp.Historisch;
                return KennzeichenTyp.StandardPKW;
            }

            if (AuslaenderRegex.IsMatch(kennzeichen))
                return KennzeichenTyp.Auslaender;

            return KennzeichenTyp.Unbekannt;
        }
        /// <summary>
        /// Der Haupt-/Zielpfad, unter dem diese Anzeige gefunden wurde
        /// </summary>
        public string ZielPfad { get; set; } = string.Empty;

        /// <summary>
        /// Pfad zum Anzeige-Verzeichnis (enthält Anzeige.txt)
        /// </summary>
        public string AnzeigeDatei { get; set; } = string.Empty;

        /// <summary>
        /// Kennzeichen (direkt aus Verzeichnis extrahiert)
        /// </summary>
        private string _kennzeichen = string.Empty;
        public string Kennzeichen
        {
            get => _kennzeichen;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _kennzeichen = string.Empty;
                    return;
                }

                // 1. Außenrum trimmen und ToUpper
                var k = value.Trim().ToUpper();

                // 2. Prüfen, ob schon ein Bindestrich existiert
                if (!k.Contains("-"))
                {
                    // Ort = Buchstaben am Anfang bis zur ersten Ziffer
                    int i = 0;
                    while (i < k.Length && char.IsLetter(k[i]))
                        i++;

                    if (i > 0 && i < k.Length)
                        k = k.Substring(0, i) + "-" + k.Substring(i);
                }

                // 3. Alle übrigen Whitespaces entfernen
                k = k.Replace(" ", "");

                _kennzeichen = k;
            }
        }
        public string Ort { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
        public string Text { get; set; } = string.Empty;

        public override string ToString() => $"{Kennzeichen} @ {AnzeigeDatei}";

        public override bool Equals(object? obj)
        {
            return obj is AnzeigeEintrag other &&
                   AnzeigeDatei.Equals(other.AnzeigeDatei, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => AnzeigeDatei.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}
