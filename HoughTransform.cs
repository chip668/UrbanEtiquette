using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class HoughTransform
    {
        public string AmtlichesKennzeichen
        {
            get
            {
                String result = "";
                if (vd != null)
                {
                    result = vd.Kennzeichen.Replace ("█", "")
                        .Replace("☻", "☺")
                        .Replace("☺☺", "☺")
                        .Replace("☺", "-")
                        .Replace("▐", "").Trim();
                }
                return result;
            }
        }
        public VDurchschuss vd = null;
        public static OCRLib ocrlib = new OCRLib();
        public List<(double theta, double rho)> DetectLines(Bitmap image, int threshold =100)
        {
            // Bild in Graustufen konvertieren
            Bitmap grayImage = ConvertToGrayscale(image);

            // Hough-Akkumulator initialisieren
            int maxRho = (int)Math.Sqrt(image.Width * image.Width + image.Height * image.Height);
            int thetaBins = 180;
            int[,] accumulator = new int[thetaBins, 2 * maxRho + 1];

            // Hough-Transformation durchführen
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = grayImage.GetPixel(x, y);
                    if (pixel.R < 128) // Linienkandidat
                    {
                        for (int theta = 0; theta < 180; theta++)
                        {
                            double rho = x * Math.Cos(theta * Math.PI / 180) + y * Math.Sin(theta * Math.PI / 180);
                            int rhoIndex = (int)Math.Round(rho) + maxRho;
                            accumulator[theta, rhoIndex]++;
                        }
                    }
                }
            }

            // Linien aus dem Akkumulator extrahieren
            List<(double theta, double rho)> lines = new List<(double theta, double rho)>();
            for (int theta = 0; theta < thetaBins; theta++)
            {
                for (int rho = 0; rho < 2 * maxRho + 1; rho++)
                {
                    if (accumulator[theta, rho] >= threshold)
                    {
                        double thetaValue = theta * Math.PI / 180;
                        double rhoValue = rho - maxRho;
                        lines.Add((thetaValue, rhoValue));
                    }
                }
            }

            return lines;
        }
        private Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                    grayImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            return grayImage;
        }
        private Bitmap ConvertToBlackAndWhite_old(Bitmap image)
        {
            Bitmap bwImage = new Bitmap(image.Width, image.Height);

            int threshold = 128; // Schwellenwert für Schwarzweißkonvertierung

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B); // Grauwert berechnen
                    Color newPixel = (grayValue < threshold) ? Color.Black : Color.White; // Überprüfen, ob der Grauwert unter dem Schwellenwert liegt
                    bwImage.SetPixel(x, y, newPixel);
                }
            }
            return bwImage;
        }
        private Bitmap ConvertToBlackAndWhite_old2(Bitmap image)
        {
            Bitmap bwImage = new Bitmap(image.Width, image.Height);
            double whitePercentage = 0;

            // Berechne den Schwellenwert basierend auf dem prozentualen Anteil von weißen und schwarzen Flächen
            int totalPixels = image.Width * image.Height;
            int whiteThreshold = (int)(totalPixels * whitePercentage);

            int whiteCount = 0;

            // Zähle die Anzahl der weißen Pixel im Bild
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B); // Grauwert berechnen
                    if (grayValue > 128) // Wenn der Grauwert über dem Schwellenwert liegt
                        whiteCount++;
                }
            }

            // Wenn der prozentuale Anteil der weißen Fläche größer als der gewünschte prozentuale Anteil ist, invertiere die Farben
            bool invertColors = whiteCount > whiteThreshold;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B); // Grauwert berechnen
                    Color newPixel = (grayValue > 128) ? Color.White : Color.Black; // Überprüfen, ob der Grauwert über dem Schwellenwert liegt
                    if (invertColors)
                        newPixel = (newPixel == Color.White) ? Color.Black : Color.White; // Farben invertieren, falls erforderlich
                    bwImage.SetPixel(x, y, newPixel);
                }
            }

            return bwImage;
        }
        private Bitmap ConvertToBlackAndWhite(Bitmap image)
        {
            Bitmap bwImage = new Bitmap(image.Width, image.Height);
            // Berechne den Schwellenwert basierend auf dem gewünschten Prozentsatz des weißen Bereichs
            int threshold = CalculateThreshold(image, 0.22); // Beispiel: 50% Weißanteil
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B); // Grauwert berechnen
                    Color newPixel = (grayValue > threshold) ? Color.White : Color.Black; // Überprüfen, ob der Grauwert über dem Schwellenwert liegt
                    bwImage.SetPixel(x, y, newPixel);
                }
            }
            return bwImage;
        }
        private int CalculateThreshold(Bitmap image, double whitePercentage)
        {
            int totalPixels = image.Width * image.Height;
            int whiteThreshold = (int)(totalPixels * whitePercentage);

            int whiteCount = 0;

            // Zähle die Anzahl der weißen Pixel im Bild
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B); // Grauwert berechnen
                    if (grayValue > 128) // Wenn der Grauwert über dem Schwellenwert liegt
                        whiteCount++;
                }
            }

            return (whiteCount > whiteThreshold) ? 128 : 0; // Wenn der gewünschte Prozentsatz überschritten wird, wird der Schwellenwert auf 128 gesetzt, sonst auf 0.
        }
        public double CalculateBrightness(Bitmap bitmap)
        {
            double totalBrightness = 0;
            int pixelCount = 0;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    double brightness = GetBrightness(pixelColor);
                    totalBrightness += brightness;
                    pixelCount++;
                }
            }

            // Calculate average brightness
            double averageBrightness = totalBrightness / pixelCount;
            return averageBrightness;
        }
        private static double GetBrightness(Color color)
        {
            // Brightness can be calculated using different formulas. 
            // Here, I'm using a simple method that averages the color channels.
            return (color.R + color.G + color.B) / (3.0 * 255.0);
        }
        private Bitmap AdjustContrast(Bitmap image, float contrast)
        {
            Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

            // Kontrastberechnungsfaktor
            float factor = (100.0f + contrast) / 100.0f;
            factor *= factor;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // Anpassung der RGB-Werte
                    float red = pixel.R / 255.0f;
                    float green = pixel.G / 255.0f;
                    float blue = pixel.B / 255.0f;

                    red = (((red - 0.5f) * factor) + 0.5f) * 255.0f;
                    green = (((green - 0.5f) * factor) + 0.5f) * 255.0f;
                    blue = (((blue - 0.5f) * factor) + 0.5f) * 255.0f;

                    // Begrenzung der RGB-Werte auf den Bereich von 0 bis 255
                    red = Math.Min(Math.Max(red, 0), 255);
                    green = Math.Min(Math.Max(green, 0), 255);
                    blue = Math.Min(Math.Max(blue, 0), 255);

                    adjustedImage.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue));
                }
            }

            return adjustedImage;
        }
        private Bitmap AdjustBrightnessAndContrast(Bitmap image, float brightness, float contrast)
        {
            Bitmap adjustedImage = new Bitmap(image.Width, image.Height);

            // Helligkeits- und Kontrastberechnungsfaktoren
            float brightnessFactor = (brightness + 1f);
            float contrastFactor = (1f + contrast);
            contrastFactor *= contrastFactor;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // Anpassung der Helligkeit
                    float red = pixel.R * brightnessFactor;
                    float green = pixel.G * brightnessFactor;
                    float blue = pixel.B * brightnessFactor;

                    // Anpassung des Kontrasts
                    red = (((red / 255.0f) - 0.5f) * contrastFactor + 0.5f) * 255.0f;
                    green = (((green / 255.0f) - 0.5f) * contrastFactor + 0.5f) * 255.0f;
                    blue = (((blue / 255.0f) - 0.5f) * contrastFactor + 0.5f) * 255.0f;

                    // Begrenzung der RGB-Werte auf den Bereich von 0 bis 255
                    red = Math.Min(Math.Max(red, 0), 255);
                    green = Math.Min(Math.Max(green, 0), 255);
                    blue = Math.Min(Math.Max(blue, 0), 255);

                    adjustedImage.SetPixel(x, y, Color.FromArgb((int)red, (int)green, (int)blue));
                }
            }

            return adjustedImage;
        }
        public double BerechneDurchschnittlichenWinkel(Bitmap image, int threshold = 100)
        {
            return BerechneDurchschnittlichenWinkel(DetectLines(image, threshold));
        }
        public double BerechneDurchschnittlichenWinkel(List<(double theta, double rho)> linien)
        {
            if (linien.Count == 0)
                return 0.0;

            double summeSinTheta = 0.0;
            double summeCosTheta = 0.0;

            foreach (var (theta, rho) in linien)
            {
                summeSinTheta += Math.Sin(theta);
                summeCosTheta += Math.Cos(theta);
            }

            double durchschnittsSinTheta = summeSinTheta / linien.Count;
            double durchschnittsCosTheta = summeCosTheta / linien.Count;

            double durchschnittlicherWinkel = Math.Atan2(durchschnittsSinTheta, durchschnittsCosTheta);

            // Umwandlung in Grad
            durchschnittlicherWinkel = durchschnittlicherWinkel * (180 / Math.PI);

            return durchschnittlicherWinkel;
        }
        public Bitmap DrehenUmWinkel(Bitmap image, Boolean withEdit)
        {
            Bitmap bw;
            double winkel = BerechneDurchschnittlichenWinkel(image)-90;
            double b = CalculateBrightness(image);
            bw = AdjustBrightnessAndContrast(image, (float)(1 - b) - 0.4f, 0f);
            // bw = AdjustBrightnessAndContrast(image, 1f, 0f);
            // bw = AdjustContrast(image, 1f);
            bw = ConvertToBlackAndWhite(bw);
            // Bild drehen
            Bitmap rotatedImage = new Bitmap(bw.Width, bw.Height);
            Graphics g = Graphics.FromImage(rotatedImage);
            g.TranslateTransform(bw.Width / 2, bw.Height / 2);
            g.RotateTransform((float)winkel);
            g.TranslateTransform(-bw.Width / 2, -bw.Height / 2);
            g.DrawImage(bw, new Point(0, 0));
            Rectangle rcl = FindInnerRectangle(rotatedImage);
            Bitmap segment = Durchschuss.CropBitmap(rotatedImage, rcl);
            vd = new VDurchschuss(segment, withEdit);
            using (Pen pen = new Pen(Color.Blue))
            {
                g.DrawRectangle(pen, rcl);
            }
            // return rotatedImage;
            // return segment;
            return vd.bmp;
        }
        public Rectangle FindInnerRectangle(Bitmap bitmap)
        {
            int minx = bitmap.Width;
            int maxx = 0;
            int miny = bitmap.Height;
            int maxy = 0;
            int firstbd = 0;

            if (File.Exists(@"C:\Temp\test.bmp"))
                File.Delete(@"C:\Temp\test.bmp");
            bitmap.Save(@"C:\Temp\test.bmp");

            // Read down
            Rectangle result = new Rectangle(0, 0, 0, 0);
            for (int y =0; y < bitmap.Height/2; y++)
            {
                Durchschuss d = new Durchschuss(bitmap, y);
                if (d.isDurchschuss)
                {
                    minx = Math.Min(minx, d.wleft);
                    maxx = Math.Max(maxx, d.wright);
                    miny = Math.Min(miny, y);
                    if (d.isDurchschuss && y <= d.Height / 2)
                    {
                        miny = Math.Max(miny, y);
                    }
                }
                if (d.isBDurchschuss && y < d.Height / 2)
                {
                    miny = y;
                }
            }
            // Read up
            for (int y = bitmap.Height-1; y >= bitmap.Height/2; y--)
            {
                Durchschuss d = new Durchschuss(bitmap, y);
                if (d.isDurchschuss)
                {
                    minx = Math.Min(minx, d.wleft);
                    maxx = Math.Max(maxx, d.wright);
                    maxy = Math.Min(maxy, y);
                    if (d.isDurchschuss && y >= d.Height / 2)
                    {
                        maxy = Math.Max(miny, y);
                    }
                }
                if (d.isBDurchschuss && y > d.Height / 2)
                {
                    maxy = y;  
                }
            }
            return new Rectangle(minx, miny, maxx-minx, maxy-miny);
        }
        public class Durchschuss
        {
            public Bitmap bmp { get; set; }
            public int Y { get; set; }
            /// <summary>
            /// Scharze Linie
            /// </summary>
            public Boolean isBDurchschuss
            {
                get { return (bcount >= bmp.Width / 2); }
            }
            /// <summary>
            /// weiße Line
            /// </summary>
            public Boolean isDurchschuss
            {
                get { return (wcount >= bmp.Width / 2); }
            }
            /// <summary>
            /// komplett weißer Durchschuss
            /// </summary>
            public Boolean isFullDurchschuss
            {
                get { return ((wleft == 0) && (wright == bmp.Width-1)); }
            }
            public Durchschuss(Bitmap bmp, int y)
            {
                this.bmp = bmp;
                Y = y;
                for (int x=0; x<bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, Y);
                    Boolean wc = isWhite(c);
                    if (wcount< bmp.Width/2)
                    {
                        if (!wc)
                        {
                            wleft++;
                            wcount = 0;
                        }
                        else
                        {
                            wcount++;
                        }
                    }
                    else
                    {
                        if (!wc)
                        {
                            wright = x;
                            break;
                        }
                        else
                        {
                            wright++;
                        }
                    }
                    wc = isBlack(c);
                    if (wc)
                        bcount++;
                    else
                        bcount=0;
                }
            }

            public Durchschuss()
            {
            }

            public int Width 
            { 
                get 
                {
                    return (bmp == null ? 0 : bmp.Width);
                } 
            }
            public int Height
            {
                get
                {
                    return (bmp == null ? 0 : bmp.Height);
                }
            }
            public int wleft { get; set; } = 0;
            public int wcount { get; set; } = 0;
            public int bcount { get; set; } = 0;
            public int wright { get; set; } = 0;
            public Boolean isWhite(Color c)
            {
                return EQColor(c, Color.White);
            }
            public Boolean isWhite(Point p)
            {
                return EQColor(p, Color.White);
            }
            public Boolean isWhite(int x, int y)
            {
                return EQColor(x, y , Color.White);
            }
            public Boolean isBlack(Color c)
            {
                return EQColor(c, Color.Black);
            }
            public Boolean isBlack(Point p)
            {
                return EQColor(p, Color.Black);
            }
            public Boolean isBlack(int x, int y)
            {
                return EQColor(x, y, Color.Black);
            }
            public Boolean EQColor(Color c, Color rf)
            {
                return ((c.R == rf.R) && (c.G == rf.G) && (c.B == rf.B));
            }
            public Boolean EQColor(Point p, Color rf)
            {
                return EQColor(bmp.GetPixel(p.X, p.Y), rf);
            }
            public Boolean EQColor(int x, int y, Color rf)
            {
                return EQColor(new Point (x,y), rf);
            }
            public static Bitmap CropBitmap(Bitmap source, Rectangle rect)
            {
                // Überprüfen, ob das Rechteck innerhalb der Grenzen des Bitmaps liegt
                if (rect.X < 0 || rect.Y < 0 || rect.Right > source.Width || rect.Bottom > source.Height)
                {
                    throw new ArgumentException("Das angegebene Rechteck liegt außerhalb der Grenzen des Bitmaps.");
                }

                // Erstellen eines Ziel-Bitmap mit der Größe des Rechtecks
                Bitmap croppedBitmap = new Bitmap(rect.Width, rect.Height);

                // Erstellen eines Grafikobjekts für das Ziel-Bitmap
                using (Graphics g = Graphics.FromImage(croppedBitmap))
                {
                    // Kopieren des ausgeschnittenen Bereichs aus dem Quell-Bitmap in das Ziel-Bitmap
                    g.DrawImage(source, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                }

                return croppedBitmap;
            }
        }
        public class VDurchschuss : Durchschuss
        {
            public String Kennzeichen = "";
            public List<Rectangle> BuchstabenRechtecke = new List<Rectangle>();
            public VDurchschuss(Bitmap bmp, Boolean withEdit) : base()
            {
                this.bmp = bmp;

                int x0 = 0;
                int x1 = 0;
                bool inBuchstabe = false;

                // Durchlaufen der Spalten (x-Werte)
                for (int x = 0; x < Width; x++)
                {
                    bool istDurchschuss = true; // Annahme: Die Spalte ist ein Durchschuss, bis zum Beweis des Gegenteils

                    // Durchlaufen der Zeilen (y-Werte) in der aktuellen Spalte
                    for (int y = 0; y < Height; y++)
                    {
                        if (!isWhite(x, y))
                        {
                            istDurchschuss = false; // Kein Durchschuss in dieser Spalte
                            break;
                        }
                    }

                    if (istDurchschuss)
                    {
                        // Wenn wir bereits in einem Buchstaben sind, schließe ihn ab
                        if (inBuchstabe)
                        {
                            BuchstabenRechtecke.Add(new Rectangle(x0, 0, x1 - x0 + 1, Height));
                            inBuchstabe = false;
                        }
                    }
                    else
                    {
                        // Wenn wir noch nicht in einem Buchstaben sind, beginne einen neuen Buchstaben
                        if (!inBuchstabe)
                        {
                            x0 = x; // Startpunkt des neuen Buchstabens
                            inBuchstabe = true;
                        }

                        x1 = x; // Aktualisieren des Endpunkts des aktuellen Buchstabens
                    }
                }
                // Falls wir am Ende des Bitmaps noch in einem Buchstaben sind, füge ihn hinzu
                if (inBuchstabe)
                {
                    BuchstabenRechtecke.Add(new Rectangle(x0, 0, x1 - x0 + 1, Height));
                }

                if (BuchstabenRechtecke.Count<3)
                {
                    // Kennzeichen <Ort> <Buchstaben> <Zahl> nmuss mindestens 3 Segmente haben.
                    //  Kennzeichen nicht erkannt
                    withEdit = false;
                }
                else
                {
                    // Zeichnen der Buchstaben-Rechtecke in das Bitmap
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Kennzeichen = "";
                        for (int i = 0; i < BuchstabenRechtecke.Count; i++)
                        {
                            Rectangle rect = BuchstabenRechtecke[i];
                            Bitmap b = Charbitmap(bmp, rect);
                            String temp = ocrlib.RecognizeCharacter(b);
                            if (withEdit)
                            {
                                temp = ocrlib.EditCharakter(b, temp);
                            }
                            Kennzeichen += temp;
                            using (Pen pen = new Pen(Color.Green))
                            {
                                g.DrawRectangle(pen, rect);
                            }
                        }
                    }
                }
            }
            public Bitmap Charbitmap(Bitmap original, Rectangle rect)
            {
                
                // Lade das ursprüngliche Bitmap
                Bitmap originalBitmap = CropBitmap(bmp, rect);

                int newWidth = 20;
                int newHeight = 30;

                Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);
                using (Graphics graphics = Graphics.FromImage(scaledBitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
                }
                return scaledBitmap;
            }
        }
    }
}
