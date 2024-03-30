using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Anzeige
{
    // Einfaches neuronales Netzwerk für die Klassifizierung von Farben
    [Serializable]
    public class ColorClassifier
    {
        private static Dictionary<Color, Color> _weights = new Dictionary<Color, Color>();
        public static void Train(Color output, Color input)
        {
            if (_weights.Keys.Contains (input))
                _weights.Remove(input);
            _weights.Add(input, output);

        }

        // Klassifiziere eine Farbe
        public static Color Classify(Color color)
        {
            Color result = Color.Gold;
            double min = double.MaxValue;

            foreach (Color c in _weights.Keys)
            {
                double d = CalculateDistance(c, color);
                if (d < min)
                {
                    min = d;
                    result = _weights[c];
                }
            }
            return result;
        }

        // Berechne die euklidische Distanz zwischen zwei Vektoren
        private static double CalculateDistance(Color color1, Color color2)
        {
            double sum = Math.Pow(color1.R - color2.R, 2) +
                         Math.Pow(color1.G - color2.G, 2) +
                         Math.Pow(color1.B - color2.B, 2);

            return Math.Sqrt(sum);
        }
        
        // Speichert die ColorClassifier-Instanz in eine Datei
        public static void SaveToFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, _weights);
                }
                // Console.WriteLine("ColorClassifier wurde erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
               //  Console.WriteLine("Fehler beim Speichern des ColorClassifier: " + ex.Message);
            }
        }
        public static void LoadFromFile()
        {
            LoadFromFile(Colortraining.colorfile);
        }

        // Lädt die ColorClassifier-Instanz aus einer Datei
        public static void LoadFromFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _weights.Clear();
                    _weights = (Dictionary<Color, Color>)formatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, z.B. Ausgabe der Fehlermeldung
            }
        }
    }
}
