using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anzeige
{
    public class OCRLib
    {
        public class CharakterInfo
        {
            public CharakterInfo(String filename)
            {
                Load(filename);
            }
            public CharakterInfo(Bitmap bmp, String key, int count=0)
            {
                this.bmp = bmp;
                this.key = key;
                this.count = count;
            }
            public const string DefaultReferenzOrdner = "ReferenzBilder";
            public Bitmap bmp=null;
            public int count=0;
            public String key="";
            public String Filename
            {
                get 
                {
                    return Path.Combine(
                        DefaultReferenzOrdner, 
                        "Ref_"+ key + "_" + count.ToString() + "_" + ".bmp");
                }
            }
            public void Load(String filename)
            {
                String[] items = filename.Split("_");
                if (items.Length == 4)
                {
                    key = items[1];
                    count = Convert.ToInt32(items[2]);
                    bmp = (Bitmap)Bitmap.FromFile(filename);
                }
            }
            public void Createdir()
            {
                // Überprüfen, ob der angegebene Ordner existiert
                if (!Directory.Exists(DefaultReferenzOrdner))
                {
                    Directory.CreateDirectory(DefaultReferenzOrdner);
                }
            }
            public double CalculateSimilarity(Bitmap b)
            {
                double similarity = 0.0;

                // Überprüfen der Dimensionen der Bitmaps, um sicherzustellen, dass sie gleich sind
                if (bmp.Width != b.Width || bmp.Height != b.Height)
                {
                    // Wenn die Dimensionen nicht übereinstimmen, skalieren Sie das externe Bitmap b auf die Größe dieser Bitmap
                    b = new Bitmap(b, bmp.Width, bmp.Height);
                }

                // Iterieren durch jedes Pixel in den beiden Bitmaps und berechnen die Ähnlichkeit
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        // Extrahieren der Farbwerte der beiden Bitmaps
                        Color color1 = this.bmp.GetPixel(x, y);
                        Color color2 = b.GetPixel(x, y);

                        // Berechnen der Differenz der Farbwerte
                        int deltaR = color1.R - color2.R;
                        int deltaG = color1.G - color2.G;
                        int deltaB = color1.B - color2.B;

                        // Berechnen der quadratischen Differenz und Summierung
                        similarity += deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
                    }
                }
                // Normalisieren der Ähnlichkeit, indem sie durch die Anzahl der Pixel geteilt wird
                similarity /= (bmp.Width * bmp.Height);

                return similarity;
            }
            /// <summary>
            /// Erstelle eine neue Refernez aus der vorhandenen und der neuen
            /// </summary>
            /// <param name="newBitmap"></param>
            public void IncorporateNewBitmap(Bitmap newBitmap)
            {
                if (bmp==null)
                {
                    bmp = newBitmap;
                }
                else
                {
                    double proportion = 1.0 / (count + 1);
                    Bitmap combinedBitmap = new Bitmap(bmp.Width, bmp.Height);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            // Berechnung des neuen Pixelwerts basierend auf dem Anteil
                            Color currentColor = bmp.GetPixel(x, y);
                            Color newColor = newBitmap.GetPixel(x, y);
                            int combinedR = (int)(proportion * newColor.R + (1 - proportion) * currentColor.R);
                            int combinedG = (int)(proportion * newColor.G + (1 - proportion) * currentColor.G);
                            int combinedB = (int)(proportion * newColor.B + (1 - proportion) * currentColor.B);
                            combinedBitmap.SetPixel(x, y, Color.FromArgb(combinedR, combinedG, combinedB));
                        }
                    }
                    bmp = combinedBitmap;
                }
                count++;
            }
            public void MixWithBitmapAndUpdateCounter(Bitmap bitmap)
            {
                String oldfile = Filename;
                if (bitmap.Width != bmp.Width || bitmap.Height != bmp.Height)
                {
                    throw new ArgumentException("Die Dimensionen der Bitmaps müssen übereinstimmen.");
                }

                double weightBitmap = 1.0 / (count + 1);
                double weightReference = 1 - weightBitmap;

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color bitmapColor = bitmap.GetPixel(x, y);
                        Color referenceColor = bmp.GetPixel(x, y);

                        int mixedR = (int)(weightBitmap * bitmapColor.R + weightReference * referenceColor.R);
                        int mixedG = (int)(weightBitmap * bitmapColor.G + weightReference * referenceColor.G);
                        int mixedB = (int)(weightBitmap * bitmapColor.B + weightReference * referenceColor.B);

                        bmp.SetPixel(x, y, Color.FromArgb(mixedR, mixedG, mixedB));
                    }
                }
                count++;
                try
                {
                    bitmap.Save(Filename);
                    File.Delete(oldfile);
                }
                catch { }
            }
        }
        public EditOCR editOCR = new EditOCR();
        public Dictionary<string, CharakterInfo> referenzBitmapsInfo = new Dictionary<string, CharakterInfo>();
        // private Dictionary<string, Bitmap> referenzBitmaps = new Dictionary<string, Bitmap>();
        // private Dictionary<string, int> referenzCounter = new Dictionary<string, int>();
        public OCRLib()
        {
            // Laden der Referenz-Bitmaps aus dem Standardordner
            LoadReferenzBitmaps();
            editOCR = new EditOCR();
            editOCR.ocrlib = this;
        }
        public void SaveReferenzBitmaps()
        {
            foreach (var kvp in referenzBitmapsInfo)
            {
                try
                {
                    kvp.Value.bmp.Save(kvp.Value.Filename);
                }
                catch { }
            }
        }
        private void LoadReferenzBitmaps()
        {
            // Überprüfen, ob der angegebene Ordner existiert
            if (Directory.Exists(CharakterInfo.DefaultReferenzOrdner))
            {
                // Laden aller Bitmap-Dateien im angegebenen Ordner
                string[] dateien = Directory.GetFiles(CharakterInfo.DefaultReferenzOrdner, "*.bmp");
                foreach (string datei in dateien)
                {
                    String[] items = datei.Split('_');
                    string schlüssel = Path.GetFileNameWithoutExtension(datei);

                    // Laden der Bitmap und Hinzufügen zum Dictionary
                    Bitmap obitmap = new Bitmap(datei);
                    Bitmap bitmap = new Bitmap(obitmap);
                    obitmap.Dispose();
                    CharakterInfo nc = new CharakterInfo(bitmap, items[1], Convert.ToInt32(items[2]));
                    if (!referenzBitmapsInfo.ContainsKey(nc.key))
                        referenzBitmapsInfo.Add(nc.key, nc);
                    else
                    {
                        if (nc.count > referenzBitmapsInfo[nc.key].count)
                        {
                            referenzBitmapsInfo[nc.key] = nc;
                        }
                    }
                }
            }
        }
        public String EditCharakter(Bitmap charBitmap, String bestMatchChar = "¿")
        {
            editOCR.Buchstabe = charBitmap;
            editOCR.BuchstabenStringEdit = bestMatchChar;
            try
            {
                editOCR.Referenz = referenzBitmapsInfo[bestMatchChar].bmp;
            } catch { }
            if (editOCR.ShowDialog() == DialogResult.OK)
            {
                bestMatchChar = editOCR.BuchstabenStringEdit;
                CharakterInfo nc = new CharakterInfo(charBitmap, bestMatchChar);
                if (!referenzBitmapsInfo.ContainsKey(nc.key))
                {
                    referenzBitmapsInfo.Add(nc.key, nc);
                    nc.Createdir();
                }
            }
            if (!editOCR.BuchstabenStringEdit.Equals(editOCR.BuchstabenString))
            {
                referenzBitmapsInfo[editOCR.BuchstabenStringEdit].MixWithBitmapAndUpdateCounter(charBitmap);
            }
            return editOCR.BuchstabenStringEdit;
        }
        public String RecognizeCharacter(Bitmap charBitmap)
        {
            double bestMatchScore = double.MaxValue;
            String bestMatchChar = "¿"; // Defaultwert, falls kein Übereinstimmung gefunden wird
            foreach (var kvp in referenzBitmapsInfo)
            {
                string referenceChar = kvp.Key;
                CharakterInfo referenceBitmap = kvp.Value;
                double matchScore = referenceBitmap.CalculateSimilarity(charBitmap);
                if (matchScore < bestMatchScore)
                {
                    bestMatchScore = matchScore;
                    bestMatchChar = kvp.Value.key;
                }
            }
            if (bestMatchChar == "¿")
            {
                bestMatchChar = EditCharakter(charBitmap, bestMatchChar);
            }
            return bestMatchChar;
        }
        private double CalculateMatchScore(Bitmap charBitmap, Bitmap referenceBitmap)
        {
            // Hier implementieren Sie den Algorithmus zur Berechnung der Übereinstimmungsbewertung 
            // zwischen dem aktuellen Buchstaben-Bitmap und dem Referenz-Bitmap.
            // Ein möglicher Ansatz wäre die Verwendung von Bildverarbeitungstechniken wie 
            // Vergleich der Pixelwerte, Vergleich der Histogramme, strukturelles Ähnlichkeitsmaß usw.
            // Beispiel: 
            // Hier wird einfach die Anzahl der übereinstimmenden Pixel gezählt und zurückgegeben.
            int matchCount = 0;
            for (int x = 0; x < charBitmap.Width; x++)
            {
                for (int y = 0; y < charBitmap.Height; y++)
                {
                    if (charBitmap.GetPixel(x, y) == referenceBitmap.GetPixel(x, y))
                    {
                        matchCount++;
                    }
                }
            }
            // Normalisierung der Übereinstimmungsbewertung, indem die Anzahl der übereinstimmenden 
            // Pixel durch die Gesamtzahl der Pixel im Bitmap geteilt wird.
            return (double)matchCount / (charBitmap.Width * charBitmap.Height);
        }


    }
}
