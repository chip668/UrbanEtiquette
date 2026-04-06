using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Anzeige
{
    public class PatternEngine
    {
        private static Dictionary<string, string> _patterns = null;
        public static Dictionary<string, string> Patterns
        {
            get
            {
                if (_patterns == null)
                {
                    _patterns = new Dictionary<string, string>();
                    // Standardkennzeichen BRD
                    _patterns["Standard"] = @"^[A-ZÄÖÜ]{1,3}[- ]?[A-ZÄÖÜ]{1,3}[0-9]{1,4}$"; // Kennzeichen
                    _patterns["Elektro"] = @"^[A-ZÄÖÜ]{1,3}[- ]?[A-ZÄÖÜ]{1,3}[0-9]{1,4}E$"; // Elektro
                    _patterns["Oldtimer"] = @"^[A-ZÄÖÜ]{1,3}[- ]?[A-ZÄÖÜ]{1,3}[0-9]{1,4}H$"; // O)ldtimer 
                    _patterns["Bundespolizei"] = @"^BP-[0-9]{2}-[0-9]{3,4}$";
                    // Polizei NRW (NRW-9-XXXX)
                    _patterns["PolizeiNRWL"] = @"^NRW-[0-9]{1,4}-[A-ZÄÖÜ]{1,4}$";
                    _patterns["PolizeiNRWN"] = @"^NRW-[0-9]{1,4}-[0-9]{1,4}$";
                    _patterns["PolizeiDE1"] = @"^(B|HH|HB|KA|S|M|N|A|P|C|F|DA|HRO|SN|H|OL|K|D|MZ|KL|SB|DD|L|MD|HAL|KI|EF|G|WÜ)-[0-9]{1,4}(-[A-ZÄÖÜ0-9]{1,4})?$";
                    _patterns["PolizeiDE2"] = @"^(B|HH|HB|KA|S|M|N|A|P|C|F|DA|HRO|SN|H|OL|K|D|MZ|KL|SB|DD|L|MD|HAL|KI|EF|G|WÜ)-[0-9]{1,5}?$";
                    _patterns["Landes-Bundespolizei1"] = @"^(BWL|BY|B|BBL|HB|HH|MVL|NRW|RPL|SAL|DD|LSA|SH|EF-LP|BP)-[0-9]{1,4}(-[A-ZÄÖÜ0-9]{1,4})?$";
                    _patterns["Landes-Bundespolizei2"] = @"^(BWL|BY|B|BBL|HB|HH|MVL|NRW|RPL|SAL|DD|LSA|SH|EF-LP|BP)-[0-9]{1,6}?$";
                    // Diplomatenkennzeichen (0-XXX 123)
                    _patterns["Diplomat"] = @"^0-[0-9]{3}[ ]?[0-9]{1,3}$";
                    // Diplomatisches Korps – Variante 1 (0 Stadtcode–Landescode–Rangnummer, optional Aliasbuchstabe)
                    _patterns["DiplomatVar1"] = @"^0 ?[A-ZÄÖÜ]{1,2} ?[0-9]{1,3}-[0-9]{1,3}[A-ZÄÖÜ]?$";
                    // Beispiel: 0 17-1 (USA Botschafter), 0 17-37A (Alias nach Diebstahl)
                    // Botschaftspersonal – Variante 2 (Stadtkennung B oder BN, Landescode, Rangnummer)
                    _patterns["DiplomatVar2"] = @"^(B|BN) ?[0-9]{1,3}-[0-9]{1,3}[A-ZÄÖÜ]?$";
                    // Beispiel: B 17-323 (Bediensteter der US‑Botschaft Berlin)
                    // Konsularisches Korps – Variante 3 (Stadtkennung, fünfstelliger Block beginnend mit 9)
                    _patterns["DiplomatVar3"] = @"^[A-ZÄÖÜ]{1,2}-9[0-9]{2,4}$";
                    // Beispiel: F-91234 (Konsulat Frankfurt)
                    // Alias‑Kennzeichen (gestohlene Kennzeichen mit Zusatzbuchstabe A/B/C…)
                    _patterns["DiplomatAlias"] = @"^0 ?[0-9]{1,3}-[0-9]{1,3}[A-ZÄÖÜ]$";
                    // Beispiel: 0 17-37A
                    // Historische Diplomatenfahrzeuge (Kennzeichen mit H am Ende)
                    _patterns["DiplomatHistoric"] = @"^0 ?[0-9]{1,3}-[0-9]{1,3}H$";
                    // Beispiel: 0 45-12H (Frankreich, Oldtimer)
                    // Konsularische Fahrzeuge mit CC-Zusatzschild (immer 9XXX, kein Landescode)
                    _patterns["ConsularCC"] = @"^[A-ZÄÖÜ]{1,2}-9[0-9]{3,4}$";
                    // Beispiel: F-9123 (Konsulat Frankfurt)

                    // -------------------------------
                    // Konsularische & Diplomatenkennzeichen (Deutschland)
                    // -------------------------------
                    // Diplomatenkennzeichen (0-XXX 123)
                    _patterns["Diplomat"] = @"^0[ -][0-9]{1,3}[ -][0-9]{1,3}$";
                    // _patterns["Diplomat"] = @"^0-[0-9]{3}[ ]?[0-9]{1,3}$";
                    // Diplomatisches Korps – Variante 1 (0 Stadtcode–Landescode–Rangnummer, optional Aliasbuchstabe)
                    _patterns["Diplomat1"] = @"^0 ?[A-ZÄÖÜ]{1,2} ?[0-9]{1,3}-[0-9]{1,3}[A-ZÄÖÜ]?$";
                    // Botschaftspersonal – Variante 2 (Stadtkennung B oder BN, Landescode, Rangnummer)
                    _patterns["Diplomat2"] = @"^(B|BN) ?[0-9]{1,3}-[0-9]{1,3}[A-ZÄÖÜ]?$";
                    // Konsularisches Korps – Variante 3 (Stadtkennung, fünfstelliger Block beginnend mit 9)
                    _patterns["Diplomat3"] = @"^[A-ZÄÖÜ]{1,2}-9[0-9]{2,4}$";
                    // Alias-Kennzeichen (gestohlene Kennzeichen mit Zusatzbuchstabe A/B/C…)
                    _patterns["DiplomatAlias"] = @"^0 ?[0-9]{1,3}-[0-9]{1,3}[A-ZÄÖÜ]$";
                    // Historische Diplomatenfahrzeuge (Kennzeichen mit H am Ende)
                    _patterns["DiplomatHistoric"] = @"^0 ?[0-9]{1,3}-[0-9]{1,3}H$";
                    // Konsularische Fahrzeuge mit CC-Zusatzschild (immer 9XXX, kein Landescode)
                    _patterns["ConsularCC"] = @"^[A-ZÄÖÜ]{1,2}-9[0-9]{3,4}$";

                    // Bundeswehr-Kennzeichen (Y-XXXXXX, bis zu 6 Ziffern)
                    // Sonderfälle: Y-1 für Inspekteure, Y-XXX für US-Fahrzeuge, Y-XXXXXX für reguläre Dienstfahrzeuge
                    _patterns["Bundeswehr"] = @"^Y-[0-9]{1,6}$";
                    // Bundeswehr-Kennzeichen Elektrofahrzeuge (Y-XXXXXXE)
                    _patterns["BundeswehrE"] = @"^Y-[0-9]{1,6}E$";
                    // Bundeswehr-Kennzeichen Erprobung (rote Y-Kennzeichen, Format identisch, Farbe nicht im Regex abbildbar)
                    _patterns["BundeswehrTest"] = @"^Y-[0-9]{1,6}$";
                    // NATO-Kennzeichen (X-XXXX, vierstellige Erkennungsnummer)
                    _patterns["NATO"] = @"^X-[0-9]{4}$";
                    // THW-Kennzeichen (Technisches Hilfswerk)
                    // Format: THW-XXXXXX (1 bis 6 Ziffern, keine Ortskennung)
                    // Beispiel: THW-1234, THW-987654
                    _patterns["THW"] = @"^THW-[0-9]{1,6}$";
                    _patterns["Bundesbehoerde"] = @"^BD-[0-9]{1,4}$";
                    _patterns["BundespolizeiAlt"] = @"^BG-[0-9]{1,4}$";
                    _patterns["Zoll"] = @"^Z-[0-9]{1,4}$";
                    // _patterns["Saison"] = @"^[A-ZÄÖÜ]{1,3}-[A-ZÄÖÜ]{1,2}[0-9]{1,4} [0-9]{2}/[0-9]{2}$";
                    // _patterns["Wechsel"] = @"^[A-ZÄÖÜ]{1,3}-[A-ZÄÖÜ]{1,2}[0-9]{1,4}W$";
                    // _patterns["Kurzzeit"] = @"^[A-ZÄÖÜ]{1,3}-[0-9]{4}$"; 
                    // _patterns["Kurzzeit"] = @"^[A-ZÄÖÜ]{1,3}-(03|04|05|06)[0-9]{2}$"; // mit 03/04/05/06 als Präfix
                    // _patterns["HaendlerRot"] = @"^[A-ZÄÖÜ]{1,3}-06[0-9]{3}$";
                    _patterns["Saison"] = @"^[A-ZÄÖÜ]{1,3}-[A-ZÄÖÜ]{1,2}[0-9]{1,4} (1[0-2]|[1-9])/(1[0-2]|[1-9])$";
                    _patterns["Wechsel"] = @"^[A-ZÄÖÜ]{1,3}-[A-ZÄÖÜ]{1,2}[0-9]{1,4}W$";
                    _patterns["Kurzzeit"] = @"^[A-ZÄÖÜ]{1,3}-(03|04)[0-9]{3}$";
                    _patterns["HaendlerRot"] = @"^[A-ZÄÖÜ]{1,3}-06[0-9]{1,4}$";
                    _patterns["Oldtimer"] = @"^[A-ZÄÖÜ]{1,3}-[A-ZÄÖÜ]{1,2}[0-9]{1,4}H$";
                    _patterns["PruefungRot"] = @"^[A-ZÄÖÜ]{1,3}-05[0-9]{3}$";
                    _patterns["Kurzzeit03"] = @"^[A-ZÄÖÜ]{1,3}-03[0-9]{4}$";
                    _patterns["Kurzzeit04"] = @"^[A-ZÄÖÜ]{1,3}-04[0-9]{4}$";
                    _patterns["Kurzzeit05"] = @"^[A-ZÄÖÜ]{1,3}-05[0-9]{4}$";
                    _patterns["Kurzzeit06"] = @"^[A-ZÄÖÜ]{1,3}-06[0-9]{4}$";
                    _patterns["OldtimerRot"] = @"^[A-ZÄÖÜ]{1,3}-07[0-9]{3}$";
                    // Österreich (XX 123 AB)
                    _patterns["Austria"] = @"^[A-ZÄÖÜ]{1,2} [0-9]{1,4} [A-ZÄÖÜ]{1,2}$";
                    // Schweiz (ZH 12345)
                    _patterns["Switzerland"] = @"^[A-ZÄÖÜ]{2} [0-9]{1,6}$";
                    // Frankreich (AB-123-CD)
                    _patterns["France"] = @"^[A-ZÄÖÜ]{2}-[0-9]{3}-[A-ZÄÖÜ]{2}$";
                    // Italien (AB 123 CD)
                    _patterns["Italy"] = @"^[A-ZÄÖÜ]{2} [0-9]{3} [A-ZÄÖÜ]{2}$";
                    // Spanien (1234 ABC)
                    _patterns["Spain"] = @"^[0-9]{4} [A-ZÄÖÜ]{3}$";
                    // Niederlande (AB-12-CD)
                    _patterns["Netherlands"] = @"^[A-ZÄÖÜ]{2}-[0-9]{2}-[A-ZÄÖÜ]{2}$";
                    // Belgien (1-ABC-123)
                    _patterns["Belgium"] = @"^[0-9]-[A-ZÄÖÜ]{3}-[0-9]{3}$";
                    // Polen (WX 12345)
                    _patterns["Poland"] = @"^[A-ZÄÖÜ]{2} [0-9]{1,5}$";
                    // Tschechien (1AB 1234)
                    _patterns["Czech"] = @"^[0-9][A-ZÄÖÜ]{2} [0-9]{4}$";
                    // Ungarn (ABC-123)
                    _patterns["Hungary"] = @"^[A-ZÄÖÜ]{3}-[0-9]{3}$";
                    // Schweden (ABC 123)
                    _patterns["Sweden"] = @"^[A-ZÄÖÜ]{3} [0-9]{3}$";
                    // Norwegen (AB 12345)
                    _patterns["Norway"] = @"^[A-ZÄÖÜ]{2} [0-9]{5}$";
                    // Dänemark (AB 12 345)
                    _patterns["Denmark"] = @"^[A-ZÄÖÜ]{2} [0-9]{2} [0-9]{3}$";
                    // Finnland (ABC-123)
                    _patterns["Finland"] = @"^[A-ZÄÖÜ]{3}-[0-9]{3}$";
                    // Großbritannien (AB12 CDE)
                    _patterns["UK"] = @"^[A-ZÄÖÜ]{2}[0-9]{2} [A-ZÄÖÜ]{3}$";
                    // Irland (123-D-4567)
                    _patterns["Ireland"] = @"^[0-9]{1,3}-[A-ZÄÖÜ]-[0-9]{1,4}$";
                    // Portugal (12-AB-34)
                    _patterns["Portugal"] = @"^[0-9]{2}-[A-ZÄÖÜ]{2}-[0-9]{2}$";
                    // Griechenland (ABX-1234)
                    _patterns["Greece"] = @"^[A-ZÄÖÜ]{3}-[0-9]{4}$";
                    // Türkei (34 AB 1234)
                    _patterns["Turkey"] = @"^[0-9]{2} [A-ZÄÖÜ]{1,2} [0-9]{2,4}$";
                }
                return _patterns;
            }
        }
        private static List<string> _sonderstatus;
        public static List<string> Sonderstatus
        {
            get
            {
                if (_sonderstatus == null)
                {
                    _sonderstatus.Add("Diplomat");
                    _sonderstatus.Add("DiplomatVar1");
                    _sonderstatus.Add("DiplomatVar2");
                    _sonderstatus.Add("DiplomatVar3");
                    _sonderstatus.Add("DiplomatAlias");
                    _sonderstatus.Add("DiplomatHistoric");
                    _sonderstatus.Add("ConsularCC");
                }
                return _sonderstatus;
            }
        }


        // Standardkennzeichen BRD
        // Beispiel: B-MA1234
        // Polizei NRW
        // Beispiel: NRW-9-ABCD
        // Diplomatenkennzeichen
        // Beispiel: 0-123 45
        // Diplomatisches Korps – Variante 1
        // Beispiel: 0 B 17-1
        // Diplomatenkennzeichen Alias
        // Beispiel: 0 17-37A
        // Diplomaten Historic
        // Beispiel: 0 45-12H
        // Konsularische Fahrzeuge CC
        // Beispiel: F-9123
        // Bundeswehr
        // Beispiel: Y-123456
        // Bundeswehr Elektro
        // Beispiel: Y-12345E
        // Bundeswehr Test
        // Beispiel: Y-98765
        // NATO
        // Beispiel: X-4321
        // THW
        // Beispiel: THW-1234
        // Bundesbehörde
        // Beispiel: BD-123
        // Bundespolizei alt
        // Beispiel: BG-456
        // Zoll
        // Beispiel: Z-789
        // Saison
        // Beispiel: M-AB1234 04/10
        // Wechsel
        // Beispiel: B-XY1234W
        // Kurzzeit allgemein
        // Beispiel: K-1234
        // Händler Rot
        // Beispiel: S-06123
        // Oldtimer
        // Beispiel: HH-AB123H
        // Prüfung Rot
        // Beispiel: F-05123
        // Kurzzeit 03
        // Beispiel: B-03123
        // Kurzzeit 04
        // Beispiel: B-04123
        // Oldtimer Rot
        // Beispiel: M-07123
        // Österreich
        // Beispiel: W 123 AB
        // Schweiz
        // Beispiel: ZH 12345
        // Frankreich
        // Beispiel: AB-123-CD
        // Italien
        // Beispiel: RM 456 CD
        // Spanien
        // Beispiel: 1234 ABC
        // Niederlande
        // Beispiel: AB-12-CD
        // Belgien
        // Beispiel: 1-ABC-123
        // Polen
        // Beispiel: WX 12345
        // Tschechien
        // Beispiel: 1AB 1234
        // Ungarn
        // Beispiel: ABC-123
        // Schweden
        // Beispiel: XYZ 789
        // Norwegen
        // Beispiel: AB 12345
        // Dänemark
        // Beispiel: DK 12 345
        // Finnland
        // Beispiel: FGH-456
        // Großbritannien
        // Beispiel: AB12 CDE
        // Irland
        // Beispiel: 123-D-4567
        // Portugal
        // Beispiel: 12-AB-34
        // Griechenland
        // Beispiel: ATH-1234
        // Türkei
        // Beispiel: 34 AB 1234
        public PatternEngine()
        {

        }

        public bool HatSonderstatus(string patternKey)
        {
            if (string.IsNullOrEmpty(patternKey)) return false;

            // Keys, die Sonderrechte haben (Diplomaten, Konsulate)
            return Sonderstatus.Contains(patternKey);
        }
        public bool Validate(string plate, out string matchedPattern)
        {
            matchedPattern = GetMatch(plate);
            return matchedPattern != null;
        }
        public string GetMatch(string plate)
        {
            plate = plate.Trim().ToUpper();

            foreach (var kv in Patterns)
            {
                if (Regex.IsMatch(plate, kv.Value))
                {
                    return kv.Key;
                }
            }

            return null;
        }
        public bool IsMatch(string plate)
        {
            return GetMatch(plate) != null;
        }


        // ====== Korrektur numernschild ==================================================================
        // =============================================
        // NEU: Confusion-Map (einfach hier einfügen)
        // =============================================
        string[] _confusionMap =
        {
            "0OÖÜUQ", "I1L", "Z2", "S5", "B83KRJ", "6G", "T7", "AH4", "P9"
        };

        public class CorrectionData
        {
            public string RawText { get; set; }
            public string PatternKey { get; set; }
            public string CorrectedText { get; set; }
            public double Evaluation { get; set; }
            public CorrectionData(string rawText, string patternKey, string correctedText, double evaluation = 0)
            {
                RawText = rawText;
                PatternKey = patternKey;
                CorrectedText = correctedText;
                Evaluation = evaluation;
            }
            public string DisplayMember => ToString();
            public override string ToString()
            {
                return $"{CorrectedText} ({PatternKey})";
            }
        }

        // =============================================
        // HAUPTFUNKTION (einfach hier einfügen)
        // =============================================
        public List<CorrectionData> TryFixAllPatterns(string rawText)
        {
            List<CorrectionData> result = new List<CorrectionData>();
            if (string.IsNullOrWhiteSpace(rawText))
            {
                result.Add(new CorrectionData(rawText, null, null, 0));
                // return (null, null);
            }
            else
            {
                while (rawText.Contains("  "))
                    rawText = rawText.Replace("  ", " ");
                while (rawText.Contains("--"))
                    rawText = rawText.Replace("--", "-");

                // 1. Original prüfen
                string test = GetMatch("RPL-4-7680");

                bool i=Regex.IsMatch("RPL-4-7680", @"^(BWL|BY|B|BBL|HB|HH|MVL|NRW|RPL|SAL|DD|LSA|SH|EF-LP|BP)-[0-9]{1,4}(-[A-ZÄÖÜ0-9]{1,4})?$");
                string originalKey = GetMatch(rawText);
                if (originalKey != null)
                {
                    result.Add(new CorrectionData(rawText, originalKey, rawText, 1));
                    // return (originalKey, rawText);
                }

                // 2. Alle möglichen Korrekturen generieren
                var variants = GetAllPossibleCorrections(rawText);

                // 3. Erste Variante nehmen, die einem Pattern entspricht
                foreach (var variant in variants)
                {
                    string key = GetMatch(variant);
                    if (key != null)
                    {
                        // result.Add(new CorrectionData(rawText, key, variant, 1));
                        if (!result.Any(r => r.CorrectedText == variant))
                        {
                            result.Add(new CorrectionData(rawText, key, variant, 1));
                        }
                    }
                    // return (key, variant);
                }
            }
            if (result.Count == 0)
                result.Add(new CorrectionData(rawText, null, null, 0));
            return result;
        }


        private List<string> GetAllPossibleCorrections(string rawText)
        {
            var results = new List<string>();
            var chars = rawText.ToCharArray();
            Generate(chars, 0, results);
            return results;
        }

        private void Generate(char[] chars, int index, List<string> results)
        {
            if (index == chars.Length)
            {
                results.Add(new string(chars));
                return;
            }
            char original = chars[index];
            bool foundGroup = false;
            foreach (var group in _confusionMap)
            {
                if (group.Contains(original))
                {
                    foundGroup = true;

                    foreach (char variant in group)
                    {
                        chars[index] = variant;
                        Generate(chars, index + 1, results);
                    }

                    break;
                }
            }

            // Wenn kein Mapping → Original weitergeben
            if (!foundGroup)
            {
                chars[index] = original;
                Generate(chars, index + 1, results);
            }
        }
    }
}
