using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anzeige
{
    public class PatternEngine
    {
        private readonly Dictionary<string, string> patterns = new Dictionary<string, string>();

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
            // Standardkennzeichen BRD
            patterns["Standard"] = @"^[A-Z]{1,3}[- ]?[A-Z]{1,3}[0-9]{1,4}$";
            // Polizei NRW (NRW-9-XXXX)
            patterns["PolizeiNRW"] = @"^NRW-[0-9]{1}-[A-Z]{1,4}$";
            // Diplomatenkennzeichen (0-XXX 123)
            patterns["Diplomat"] = @"^0-[0-9]{3}[ ]?[0-9]{1,3}$";
            // Diplomatisches Korps – Variante 1 (0 Stadtcode–Landescode–Rangnummer, optional Aliasbuchstabe)
            patterns["DiplomatVar1"] = @"^0 ?[A-Z]{1,2} ?[0-9]{1,3}-[0-9]{1,3}[A-Z]?$";
            // Beispiel: 0 17-1 (USA Botschafter), 0 17-37A (Alias nach Diebstahl)
            // Botschaftspersonal – Variante 2 (Stadtkennung B oder BN, Landescode, Rangnummer)
            patterns["DiplomatVar2"] = @"^(B|BN) ?[0-9]{1,3}-[0-9]{1,3}[A-Z]?$";
            // Beispiel: B 17-323 (Bediensteter der US‑Botschaft Berlin)
            // Konsularisches Korps – Variante 3 (Stadtkennung, fünfstelliger Block beginnend mit 9)
            patterns["DiplomatVar3"] = @"^[A-Z]{1,2}-9[0-9]{2,4}$";
            // Beispiel: F-91234 (Konsulat Frankfurt)
            // Alias‑Kennzeichen (gestohlene Kennzeichen mit Zusatzbuchstabe A/B/C…)
            patterns["DiplomatAlias"] = @"^0 ?[0-9]{1,3}-[0-9]{1,3}[A-Z]$";
            // Beispiel: 0 17-37A
            // Historische Diplomatenfahrzeuge (Kennzeichen mit H am Ende)
            patterns["DiplomatHistoric"] = @"^0 ?[0-9]{1,3}-[0-9]{1,3}H$";
            // Beispiel: 0 45-12H (Frankreich, Oldtimer)
            // Konsularische Fahrzeuge mit CC-Zusatzschild (immer 9XXX, kein Landescode)
            patterns["ConsularCC"] = @"^[A-Z]{1,2}-9[0-9]{3,4}$";
            // Beispiel: F-9123 (Konsulat Frankfurt)
            // Bundeswehr-Kennzeichen (Y-XXXXXX, bis zu 6 Ziffern)
            // Sonderfälle: Y-1 für Inspekteure, Y-XXX für US-Fahrzeuge, Y-XXXXXX für reguläre Dienstfahrzeuge
            patterns["Bundeswehr"] = @"^Y-[0-9]{1,6}$";
            // Bundeswehr-Kennzeichen Elektrofahrzeuge (Y-XXXXXXE)
            patterns["BundeswehrE"] = @"^Y-[0-9]{1,6}E$";
            // Bundeswehr-Kennzeichen Erprobung (rote Y-Kennzeichen, Format identisch, Farbe nicht im Regex abbildbar)
            patterns["BundeswehrTest"] = @"^Y-[0-9]{1,6}$";
            // NATO-Kennzeichen (X-XXXX, vierstellige Erkennungsnummer)
            patterns["NATO"] = @"^X-[0-9]{4}$";
            // THW-Kennzeichen (Technisches Hilfswerk)
            // Format: THW-XXXXXX (1 bis 6 Ziffern, keine Ortskennung)
            // Beispiel: THW-1234, THW-987654
            patterns["THW"] = @"^THW-[0-9]{1,6}$";
            patterns["Bundesbehoerde"] = @"^BD-[0-9]{1,4}$";
            patterns["BundespolizeiAlt"] = @"^BG-[0-9]{1,4}$";
            patterns["Zoll"] = @"^Z-[0-9]{1,4}$";
            patterns["Saison"] = @"^[A-Z]{1,3}-[A-Z]{1,2}[0-9]{1,4} [0-9]{2}/[0-9]{2}$";
            patterns["Wechsel"] = @"^[A-Z]{1,3}-[A-Z]{1,2}[0-9]{1,4}W$";
            patterns["Kurzzeit"] = @"^[A-Z]{1,3}-[0-9]{4}$"; // mit 03/04 als Präfix
            patterns["HaendlerRot"] = @"^[A-Z]{1,3}-06[0-9]{3}$";
            patterns["Oldtimer"] = @"^[A-Z]{1,3}-[A-Z]{1,2}[0-9]{1,4}H$";
            patterns["PruefungRot"] = @"^[A-Z]{1,3}-05[0-9]{3}$";
            patterns["Kurzzeit03"] = @"^[A-Z]{1,3}-03[0-9]{3}$";
            patterns["Kurzzeit04"] = @"^[A-Z]{1,3}-04[0-9]{3}$";
            patterns["OldtimerRot"] = @"^[A-Z]{1,3}-07[0-9]{3}$";
            // Österreich (XX 123 AB)
            patterns["Austria"] = @"^[A-Z]{1,2} [0-9]{1,4} [A-Z]{1,2}$";
            // Schweiz (ZH 12345)
            patterns["Switzerland"] = @"^[A-Z]{2} [0-9]{1,6}$";
            // Frankreich (AB-123-CD)
            patterns["France"] = @"^[A-Z]{2}-[0-9]{3}-[A-Z]{2}$";
            // Italien (AB 123 CD)
            patterns["Italy"] = @"^[A-Z]{2} [0-9]{3} [A-Z]{2}$";
            // Spanien (1234 ABC)
            patterns["Spain"] = @"^[0-9]{4} [A-Z]{3}$";
            // Niederlande (AB-12-CD)
            patterns["Netherlands"] = @"^[A-Z]{2}-[0-9]{2}-[A-Z]{2}$";
            // Belgien (1-ABC-123)
            patterns["Belgium"] = @"^[0-9]-[A-Z]{3}-[0-9]{3}$";
            // Polen (WX 12345)
            patterns["Poland"] = @"^[A-Z]{2} [0-9]{1,5}$";
            // Tschechien (1AB 1234)
            patterns["Czech"] = @"^[0-9][A-Z]{2} [0-9]{4}$";
            // Ungarn (ABC-123)
            patterns["Hungary"] = @"^[A-Z]{3}-[0-9]{3}$";
            // Schweden (ABC 123)
            patterns["Sweden"] = @"^[A-Z]{3} [0-9]{3}$";
            // Norwegen (AB 12345)
            patterns["Norway"] = @"^[A-Z]{2} [0-9]{5}$";
            // Dänemark (AB 12 345)
            patterns["Denmark"] = @"^[A-Z]{2} [0-9]{2} [0-9]{3}$";
            // Finnland (ABC-123)
            patterns["Finland"] = @"^[A-Z]{3}-[0-9]{3}$";
            // Großbritannien (AB12 CDE)
            patterns["UK"] = @"^[A-Z]{2}[0-9]{2} [A-Z]{3}$";
            // Irland (123-D-4567)
            patterns["Ireland"] = @"^[0-9]{1,3}-[A-Z]-[0-9]{1,4}$";
            // Portugal (12-AB-34)
            patterns["Portugal"] = @"^[0-9]{2}-[A-Z]{2}-[0-9]{2}$";
            // Griechenland (ABX-1234)
            patterns["Greece"] = @"^[A-Z]{3}-[0-9]{4}$";
            // Türkei (34 AB 1234)
            patterns["Turkey"] = @"^[0-9]{2} [A-Z]{1,2} [0-9]{2,4}$";
        }

        public bool Validate(string plate, out string matchedPattern)
        {
            plate = plate.Trim().ToUpper();
            foreach (var kv in patterns)
            {
                if (Regex.IsMatch(plate, kv.Value))
                {
                    matchedPattern = kv.Key;
                    return true;
                }
            }
            matchedPattern = null;
            return false;
        }
    }
}
