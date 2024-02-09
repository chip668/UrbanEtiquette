﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class Tools
    {
        static void InitializeUmlautMap(Dictionary<string, string> umlautToAscii, Dictionary<string, string> asciiToUmlaut)
        {
            // Umwandlungen in beide Richtungen
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ä", "ae");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ö", "oe");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ü", "ue");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ß", "ss");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "é", "e");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "á", "a");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "í", "i");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ó", "o");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ú", "u");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "è", "e");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ì", "i");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ò", "o");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ù", "u");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "å", "a");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ş", "s");
            AddUmlautMapping(umlautToAscii, asciiToUmlaut, "ğ", "g");
            // Füge hier weitere Umlaute/Diärese und ihre ASCII-Varianten hinzu
        }
        static void AddUmlautMapping(Dictionary<string, string> umlautToAscii, Dictionary<string, string> asciiToUmlaut, string umlaut, string ascii)
        {
            umlautToAscii.Add(umlaut, ascii);
            asciiToUmlaut.Add(ascii, umlaut);
        }
        static Dictionary<string, string> umlautToAscii = null;
        static Dictionary<string, string> asciiToUmlaut = null;
        public static string ConvertUmlauts(string input, Dictionary<string, string> umlautMap)
        {
            if (umlautToAscii == null)
            {
                umlautToAscii = new Dictionary<String, String>();
                asciiToUmlaut = new Dictionary<String, String>();
                InitializeUmlautMap(umlautToAscii, asciiToUmlaut);
            }
            StringBuilder result = new StringBuilder(input);

            foreach (var entry in umlautMap)
            {
                result.Replace(entry.Key, entry.Value);
            }

            return result.ToString();
        }

        static void InitializeSonderzeichenMap(Dictionary<string, string> sonderzeichenToAscii, Dictionary<string, string> asciiToSonderzeichen)
        {
            // Umwandlungen in beide Richtungen
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "©", "(C)");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ª", "^a");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "®", "(R)");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "²", "^2");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "³", "^3");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "¼", "1/4");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "½", "1/2");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "¾", "3/4");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "Ø", "(/)");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ʣ", "dz");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ʰ", "^h");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ʲ", "^j");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ʳ", "^r");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ʷ", "^w");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ʸ", "^y");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͣ", "^a");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͤ", "^e");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͥ", "^i");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͦ", "^o");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͧ", "^u");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͨ", "^c");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͩ", "^d");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͪ", "^h");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͫ", "^m");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͬ", "^r");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͭ", "^t");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͮ", "^v");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "ͯ", "^x");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁰", "^0");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁴", "^4");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁵", "^5");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁶", "^6");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁷", "^7");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁸", "^8");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⁹", "^9");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "₨", "Rs");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "℅", "C/O");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "№", "No.");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "℗", "(P)");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "™", "TM");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "Ω", "Omega");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "℮", "exp");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅍", "A/S");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅓", "1/3");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅔", "2/3");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅛", "1/8");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅜", "3/8");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅝", "5/8");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "⅞", "7/8");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∑", "Sum");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∏", "Prod");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∆", "Delta");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "√", "Squareroot");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∞", "Infinity");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∩", "Intersection");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "≈", "aehnlich");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "≠", "!=");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "≤", "<=");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "≥", ">=");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∧", "&");
            AddSonderzeichenMapping(sonderzeichenToAscii, asciiToSonderzeichen, "∨", "|");
        }
        static void AddSonderzeichenMapping(Dictionary<string, string> sonderzeichenToAscii, Dictionary<string, string> asciiToSonderzeichen, string sonderzeichen, string ascii)
        {
            sonderzeichenToAscii.Add(sonderzeichen, ascii);
            asciiToSonderzeichen.Add(ascii, sonderzeichen);
        }
        static Dictionary<string, string> sonderzeichenToAscii = null;
        static Dictionary<string, string> asciiToSonderzeichen = null;
        public static string ConvertSonderzeichens(string input, Dictionary<string, string> sonderzeichenMap)
        {
            if (sonderzeichenToAscii == null)
            {
                sonderzeichenToAscii = new Dictionary<String, String>();
                asciiToSonderzeichen = new Dictionary<String, String>();
                InitializeSonderzeichenMap(sonderzeichenToAscii, asciiToSonderzeichen);
            }
            StringBuilder result = new StringBuilder(input);

            foreach (var entry in sonderzeichenMap)
            {
                result.Replace(entry.Key, entry.Value);
            }

            return result.ToString();
        }
    }
}