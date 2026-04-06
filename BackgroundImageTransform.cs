using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ColorPicker
{
    public class BackgroundImageTransform
    {
        public struct ColorContainer
        {
            public Color Color;
            public string Name;
            public Color ClassColor;
            public ColorContainer(Color c, string n, Color classcolor) { Color = c; Name = n; ClassColor = classcolor; }
            public override string ToString()
            {
                return $"{Name} ({Color.R},{Color.G},{Color.B})";
            }
        }

        public Color PathColor { get; set; } = Color.Red; // <- Hier das Attribut für die Pfadfarbe
        Bitmap _bmp;
        public Bitmap Bmp
        {
            get { return _bmp; }
            set { _bmp = value; }
        }
        // ---------------- FullBmp mit Pfad ----------------
        public Bitmap FullBmp
        {
            get
            {
                Bitmap result = null; // EIN EXITPOINT
                try
                {
                    if (Bmp != null)
                    {
                        // Kopie des Originalbildes
                        result = new Bitmap(Bmp);

                        // Pfad einzeichnen, falls vorhanden
                        if (contour != null && contour.Count > 0)
                        {
                            using (Graphics g = Graphics.FromImage(result))
                            using (Pen pen = new Pen(PathColor, 1)) // <- Pfadfarbe aus Attribut
                            {
                                for (int i = 0; i < contour.Count - 1; i++)
                                    g.DrawLine(pen, contour[i], contour[i + 1]);

                                // Linie zurück zum Startpunkt schließen
                                g.DrawLine(pen, contour[contour.Count - 1], contour[0]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Erstellen von FullBmp mit Pfad: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return result; // EIN EXITPOINT
            }
        }
        ImageLayout Layout;
        private Size _controlSize;

        public Size ControlSize
        {
            get
            {
                return _controlSize;
            }
            set
            {
                try
                {
                    // Validierung: Eine Fläche kleiner oder gleich 0 macht für Skalierung keinen Sinn
                    if (value.Width > 0 && value.Height > 0)
                    {
                        _controlSize = value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Setzen der Control-Größe: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        List<Point> contour = new List<Point>(); // EIN EXITPOINT
        public BackgroundImageTransform(Control ctl)
        {
            ControlSize = ctl.Size;
            Bmp = (Bitmap)ctl.BackgroundImage;
            Layout = ctl.BackgroundImageLayout;
        }
        public BackgroundImageTransform(Form frm)
        {
            ControlSize = frm.Size;
            Bmp = (Bitmap)frm.BackgroundImage;
            Layout = frm.BackgroundImageLayout;
        }
        public BackgroundImageTransform(Bitmap bmp, ImageLayout layout, Size sz)
        {
            Bmp = (Bitmap)bmp;
            ControlSize = sz;
            Layout = layout;
        }
        public BackgroundImageTransform(String file, ImageLayout layout)
        {
            Bmp = (Bitmap)Bitmap.FromFile(file);
            ControlSize = Bmp.Size;
            Layout = layout;
        }

        public Point Control2Image(Point pnt)
        {
            Point result = pnt;
            try
            {
                if (Bmp != null && ControlSize.Width > 0 && ControlSize.Height > 0)
                {
                    // Wir nutzen double für die Zwischenrechnung, um Rundungsfehler zu vermeiden
                    double cw = ControlSize.Width;
                    double ch = ControlSize.Height;
                    double iw = Bmp.Width;
                    double ih = Bmp.Height;

                    switch (Layout)
                    {
                        case ImageLayout.Center: // OK 
                            result.X = (int)(pnt.X - (cw - iw) / 2.0);
                            result.Y = (int)(pnt.Y - (ch - ih) / 2.0);
                            break;

                        case ImageLayout.Stretch: 
                            // Verhältnis erst berechnen, dann multiplizieren
                            result.X = (int)(pnt.X * (iw / cw));
                            result.Y = (int)(pnt.Y * (ih / ch));
                            break;

                        case ImageLayout.Zoom:
                            // Das Bild wird beim Zoom immer proportional eingepasst und zentriert
                            double scale = Math.Min(cw / iw, ch / ih);

                            // Offsets berechnen: Wo fängt das gezoomte Bild im Control an?
                            double offsetX = (cw - (iw * scale)) / 2.0;
                            double offsetY = (ch - (ih * scale)) / 2.0;

                            result.X = (int)((pnt.X - offsetX) / scale);
                            result.Y = (int)((pnt.Y - offsetY) / scale);
                            break;

                        case ImageLayout.Tile:
                        case ImageLayout.None:
                            result.X = pnt.X % (int)iw;
                            result.Y = pnt.Y % (int)ih;
                            if (result.X < 0) result.X += (int)iw;
                            if (result.Y < 0) result.Y += (int)ih;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler in Control2Image: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result; // EINZIGER EXITPOINT
        }
        public Point Image2Control(Point pnt)
        {
            if (Bmp == null) return pnt;

            int cw = ControlSize.Width;
            int ch = ControlSize.Height;
            int iw = Bmp.Width;
            int ih = Bmp.Height;

            Point result = new Point(pnt.X, pnt.Y);

            switch (Layout)
            {
                case ImageLayout.Center:
                    result.X += (cw - iw) / 2;
                    result.Y += (ch - ih) / 2;
                    break;

                case ImageLayout.Stretch:
                    result.X = pnt.X * cw / iw;
                    result.Y = pnt.Y * ch / ih;
                    break;

                case ImageLayout.Tile:
                    result.X %= iw;
                    result.Y %= ih;
                    if (result.X < 0) result.X += iw;
                    if (result.Y < 0) result.Y += ih;
                    break;

                case ImageLayout.Zoom:
                    float scale = Math.Min((float)cw / iw, (float)ch / ih);
                    int dispWidth = (int)(iw * scale);
                    int dispHeight = (int)(ih * scale);
                    int offsetX = (cw - dispWidth) / 2;
                    int offsetY = (ch - dispHeight) / 2;
                    result.X = (int)(pnt.X * scale + offsetX);
                    result.Y = (int)(pnt.Y * scale + offsetY);
                    break;

                default:
                    break;
            }

            return result;
        }
        public MouseEventArgs Control2Image(MouseEventArgs e)
        {
            MouseEventArgs result = null; // EIN EXITPOINT
            try
            {
                if (Bmp != null)
                {
                    Point pnt = new Point(e.X, e.Y);

                    int cw = ControlSize.Width;
                    int ch = ControlSize.Height;
                    int iw = Bmp.Width;
                    int ih = Bmp.Height;

                    switch (Layout)
                    {
                        case ImageLayout.Center:
                            pnt = new Point(pnt.X - (cw - iw) / 2, pnt.Y - (ch - ih) / 2);
                            break;

                        case ImageLayout.Stretch:
                            pnt = new Point(pnt.X * iw / cw, pnt.Y * ih / ch);
                            break;

                        case ImageLayout.Tile:
                            int x = pnt.X % iw;
                            int y = pnt.Y % ih;
                            if (x < 0) x += iw;
                            if (y < 0) y += ih;
                            pnt = new Point(x, y);
                            break;

                        case ImageLayout.Zoom:
                            float scale = Math.Min((float)cw / iw, (float)ch / ih);
                            int dispWidth = (int)(iw * scale);
                            int dispHeight = (int)(ih * scale);
                            int offsetX = (cw - dispWidth) / 2;
                            int offsetY = (ch - dispHeight) / 2;
                            pnt = new Point((int)((pnt.X - offsetX) / scale), (int)((pnt.Y - offsetY) / scale));
                            break;
                    }

                    result = new MouseEventArgs(e.Button, e.Clicks, pnt.X, pnt.Y, e.Delta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler in Control2Image: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result; // EIN EINZIGER EXITPOINT
        }
        public MouseEventArgs Image2Control(MouseEventArgs e)
        {
            MouseEventArgs result = null; // EIN EXITPOINT
            try
            {
                if (Bmp != null)
                {
                    Point pnt = new Point(e.X, e.Y);

                    int cw = ControlSize.Width;
                    int ch = ControlSize.Height;
                    int iw = Bmp.Width;
                    int ih = Bmp.Height;

                    switch (Layout)
                    {
                        case ImageLayout.Center:
                            pnt = new Point(pnt.X + (cw - iw) / 2, pnt.Y + (ch - ih) / 2);
                            break;

                        case ImageLayout.Stretch:
                            pnt = new Point(pnt.X * cw / iw, pnt.Y * ch / ih);
                            break;

                        case ImageLayout.Tile:
                            int x = pnt.X % iw;
                            int y = pnt.Y % ih;
                            if (x < 0) x += iw;
                            if (y < 0) y += ih;
                            pnt = new Point(x, y);
                            break;

                        case ImageLayout.Zoom:
                            float scale = Math.Min((float)cw / iw, (float)ch / ih);
                            int dispWidth = (int)(iw * scale);
                            int dispHeight = (int)(ih * scale);
                            int offsetX = (cw - dispWidth) / 2;
                            int offsetY = (ch - dispHeight) / 2;
                            pnt = new Point((int)(pnt.X * scale + offsetX), (int)(pnt.Y * scale + offsetY));
                            break;
                    }

                    result = new MouseEventArgs(e.Button, e.Clicks, pnt.X, pnt.Y, e.Delta);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler in Image2Control: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result; // EIN EINZIGER EXITPOINT
        }
        public void Add(Point pnt)
        {
            contour.Add(pnt);
        }
        public void FindArea(Point start, int deltaRGB)
        {
            try
            {
                contour = new List<Point>();

                if (Bmp == null)
                {
                    goto End; // Sprung zum einzigen Exitpoint
                }

                int width = Bmp.Width;
                int height = Bmp.Height;
                Color startColor = Bmp.GetPixel(start.X, start.Y);

                // Uhrzeigersinn Richtungen (Moore-Neighbor)
                int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
                int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

                // 1. Finde den tatsächlichen Startpunkt am Rand (nach links gehen bis Farbgrenze)
                Point current = start;
                while (current.X > 0 && ColorDistance(startColor, Bmp.GetPixel(current.X - 1, current.Y)) <= deltaRGB)
                {
                    current.X--;
                }

                Point firstPoint = current;
                Point backtrackPoint = new Point(current.X - 1, current.Y);
                int enterDir = 0;

                // 2. Kontur verfolgen
                do
                {
                    contour.Add(current);
                    bool foundNext = false;

                    // Wir untersuchen die 8 Nachbarn im Uhrzeigersinn
                    for (int i = 0; i < 8; i++)
                    {
                        int checkDir = (enterDir + i) % 8;
                        int nx = current.X + dx[checkDir];
                        int ny = current.Y + dy[checkDir];

                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        {
                            if (ColorDistance(startColor, Bmp.GetPixel(nx, ny)) <= deltaRGB)
                            {
                                current = new Point(nx, ny);
                                // Die neue Eintrittsrichtung ist die Richtung, aus der wir kamen (Rückwärts-Index)
                                enterDir = (checkDir + 5) % 8;
                                foundNext = true;
                                break;
                            }
                        }
                    }

                    if (!foundNext || (current == firstPoint && contour.Count > 1))
                        break;

                } while (contour.Count < 10000); // Sicherheits-Limit gegen Endlosschleifen
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler in FindArea: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        End:
            return; // DER EINZIGE EXITPOINT
        }
        public Color GetAverageAreaColor(Point start, int deltaRGB)
        {
            FindArea(start, deltaRGB);
            Color averageColor = Color.Transparent;
            try
            {
                // Fallunterscheidung: Nur ausführen, wenn Pfad und Bild existieren
                if (Bmp != null && contour != null && contour.Count >= 3)
                {
                    long totalR = 0, totalG = 0, totalB = 0;
                    int pixelCount = 0;

                    using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        path.AddPolygon(contour.ToArray());

                        using (Region region = new Region(path))
                        {
                            // Wir holen uns die exakten Scan-Rechtecke, die den Pfad füllen
                            RectangleF[] scans = region.GetRegionScans(new System.Drawing.Drawing2D.Matrix());

                            foreach (RectangleF scanRect in scans)
                            {
                                Rectangle rect = Rectangle.Round(scanRect);

                                // Nur innerhalb der Bitmap-Grenzen arbeiten
                                int startX = Math.Max(rect.Left, 0);
                                int startY = Math.Max(rect.Top, 0);
                                int endX = Math.Min(rect.Right, Bmp.Width);
                                int endY = Math.Min(rect.Bottom, Bmp.Height);

                                for (int y = startY; y < endY; y++)
                                {
                                    for (int x = startX; x < endX; x++)
                                    {
                                        Color c = Bmp.GetPixel(x, y);
                                        totalR += c.R;
                                        totalG += c.G;
                                        totalB += c.B;
                                        pixelCount++;
                                    }
                                }
                            }
                        }
                    }

                    // Durchschnitt berechnen
                    if (pixelCount > 0)
                    {
                        averageColor = Color.FromArgb(
                            (int)(totalR / pixelCount),
                            (int)(totalG / pixelCount),
                            (int)(totalB / pixelCount)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Farbmittelung: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return averageColor; // DER EINZIGE EXITPOINT
        }
        public Bitmap FillAreaWithAverageColor(Point start, int deltaRGB)
        {
            Bitmap resultBmp = null;
            try
            {
                // Fallunterscheidung: Nur starten, wenn ein Quellbild existiert
                if (Bmp != null)
                {
                    // 1. Kopie der Bitmap erstellen
                    resultBmp = new Bitmap(Bmp);

                    // 2. Durchschnittsfarbe ermitteln (nutzt deine FindArea Logik intern)
                    Color avgColor = GetAverageAreaColor(start, deltaRGB);

                    // 3. Wenn eine gültige Kontur gefunden wurde, Fläche in der Kopie füllen
                    if (contour != null && contour.Count >= 3 && avgColor != Color.Transparent)
                    {
                        using (Graphics g = Graphics.FromImage(resultBmp))
                        {
                            // Glätten aktivieren für saubere Kanten beim Füllen
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            using (Brush fillBrush = new SolidBrush(avgColor))
                            {
                                // Wir füllen das Polygon basierend auf der gefundenen Kontur
                                g.FillPolygon(fillBrush, contour.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Füllen der Fläche: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Im Fehlerfall sicherstellen, dass wir keine halbfertige Bitmap zurückgeben
                if (resultBmp != null)
                {
                    resultBmp.Dispose();
                    resultBmp = null;
                }
            }

            return resultBmp; // DER EINZIGE EXITPOINT
        }
        private float ColorDistance(Color c1, Color c2)
        {
            float dr = c1.R - c2.R;
            float dg = c1.G - c2.G;
            float db = c1.B - c2.B;
            return (float)Math.Sqrt(dr * dr + dg * dg + db * db);
        }

        // Definition der Container-Struktur
        static int d = 10;

        // Die statische Bibliothek (nur einmal im Speicher)
        private static readonly ColorContainer[] ColorLibrary = new[]
        {
            
            new ColorContainer(Color.Gold, "sonstiges", Color.Gold),               // Sonstiges

            new ColorContainer(Color.FromArgb(255, 255, 255), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255, 255, 255 - d), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255, 255 - d, 255), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255 - d, 255, 255), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255 - d, 255 - d, 255), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255, 255 - d, 255 - d), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255 - d, 255, 255 - d), "Weiss", Color.White), // 000
            new ColorContainer(Color.FromArgb(255 - d, 255 - d, 255 - d), "Weiss", Color.White), // 000

            new ColorContainer(Color.Aqua, "sonstiges", Color.Aqua),
            new ColorContainer(Color.Purple, "Lila", Color.Purple),
            new ColorContainer(Color.Teal, "Blaugrün", Color.Teal),
            new ColorContainer(Color.Brown, "Braun", Color.Brown),
            new ColorContainer(Color.FromArgb(165, 117, 84), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(255, 138, 102), "Rot", Color.Red),
            new ColorContainer(Color.FromArgb(104, 109, 169), "Blau", Color.Navy),
            new ColorContainer(Color.FromArgb(236, 80, 102), "Rot", Color.Red),
            new ColorContainer(Color.FromArgb(194, 38, 62), "Rot", Color.Red),
            new ColorContainer(Color.FromArgb(255, 220, 220), "Rot", Color.Red),
            new ColorContainer(Color.FromArgb(182, 170, 192), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(141, 111, 121), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(227, 221, 231), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(124, 87, 78), "Silber/Grau", Color.Silver),
            new ColorContainer(Color.FromArgb(181, 150, 129), "Silber/Grau", Color.Silver),
            new ColorContainer(Color.FromArgb(72, 64, 103), "Blau", Color.Navy),
            new ColorContainer(Color.FromArgb(255, 217, 145), "Weiss", Color.White),


            /*
            // --- GRAU- & SILBER-TÖNE (Metall, Asphalt, Schatten) ---
            new ColorContainer(Color.Silver, "Silber/Grau"),
            new ColorContainer(Color.FromArgb(169, 169, 169), "Silber/Grau"),     // Dark Gray
            new ColorContainer(Color.FromArgb(128, 128, 128), "Silber/Grau"),     // Medium Gray
            new ColorContainer(Color.FromArgb(105, 105, 105), "Silber/Grau"),     // Dim Gray (Asphalt)
            new ColorContainer(Color.FromArgb(181, 150, 129), "Silber/Grau"),     // Deine Messung (warmes Grau)
            new ColorContainer(Color.FromArgb(35, 41, 41), "Silber/Grau"),     // Deine Messung (warmes Grau)
            new ColorContainer(Color.FromArgb(70, 77, 85), "Silber/Grau"),     // Deine Messung (warmes Grau)
            new ColorContainer(Color.FromArgb(129, 152, 166), "Silber/Grau"),     // Deine Messung (warmes Grau)
            new ColorContainer(Color.FromArgb(79, 86, 54), "Silber/Grau"),     // Deine Messung (warmes Grau)
            */


            // Die 8 Eckpunkte um den grauen Kern (128)
            new ColorContainer(Color.FromArgb(128, 128, 128), "Silber/Grau", Color.Silver), // 000
            new ColorContainer(Color.FromArgb(128 - d, 128 - d, 128 - d), "Silber/Grau", Color.Silver), // 000
            new ColorContainer(Color.FromArgb(128 - d, 128 - d, 128 + d), "Silber/Grau", Color.Silver), // 001
            new ColorContainer(Color.FromArgb(128 - d, 128 + d, 128 - d), "Silber/Grau", Color.Silver), // 010
            new ColorContainer(Color.FromArgb(128 - d, 128 + d, 128 + d), "Silber/Grau", Color.Silver), // 011
            new ColorContainer(Color.FromArgb(128 + d, 128 - d, 128 - d), "Silber/Grau", Color.Silver), // 100
            new ColorContainer(Color.FromArgb(128 + d, 128 - d, 128 + d), "Silber/Grau", Color.Silver), // 101
            new ColorContainer(Color.FromArgb(128 + d, 128 + d, 128 - d), "Silber/Grau", Color.Silver), // 110
            new ColorContainer(Color.FromArgb(128 + d, 128 + d, 128 + d), "Silber/Grau", Color.Silver), // 111

            // Die 8 Eckpunkte um den grauen Kern (192)
            new ColorContainer(Color.FromArgb(192, 192, 192), "Silber/Grau", Color.Silver), // 000
            new ColorContainer(Color.FromArgb(192 - d, 192 - d, 192 - d), "Silber/Grau", Color.Silver), // 000
            new ColorContainer(Color.FromArgb(192 - d, 192 - d, 192 + d), "Silber/Grau", Color.Silver), // 001
            new ColorContainer(Color.FromArgb(192 - d, 192 + d, 192 - d), "Silber/Grau", Color.Silver), // 010
            new ColorContainer(Color.FromArgb(192 - d, 192 + d, 192 + d), "Silber/Grau", Color.Silver), // 011
            new ColorContainer(Color.FromArgb(192 + d, 192 - d, 192 - d), "Silber/Grau", Color.Silver), // 100
            new ColorContainer(Color.FromArgb(192 + d, 192 - d, 192 + d), "Silber/Grau", Color.Silver), // 101
            new ColorContainer(Color.FromArgb(192 + d, 192 + d, 192 - d), "Silber/Grau", Color.Silver), // 110
            new ColorContainer(Color.FromArgb(192 + d, 192 + d, 192 + d), "Silber/Grau", Color.Silver), // 111

            // Die 8 Eckpunkte um den grauen Kern (64)
            new ColorContainer(Color.FromArgb(64, 64, 64), "Silber/Grau", Color.Silver), // 000
            new ColorContainer(Color.FromArgb(64 - d, 64 - d, 64 - d), "Silber/Grau", Color.Silver), // 000
            new ColorContainer(Color.FromArgb(64 - d, 64 - d, 64 + d), "Silber/Grau", Color.Silver), // 001
            new ColorContainer(Color.FromArgb(64 - d, 64 + d, 64 - d), "Silber/Grau", Color.Silver), // 010
            new ColorContainer(Color.FromArgb(64 - d, 64 + d, 64 + d), "Silber/Grau", Color.Silver), // 011
            new ColorContainer(Color.FromArgb(64 + d, 64 - d, 64 - d), "Silber/Grau", Color.Silver), // 100
            new ColorContainer(Color.FromArgb(64 + d, 64 - d, 64 + d), "Silber/Grau", Color.Silver), // 101
            new ColorContainer(Color.FromArgb(64 + d, 64 + d, 64 - d), "Silber/Grau", Color.Silver), // 110
            new ColorContainer(Color.FromArgb(64 + d, 64 + d, 64 + d), "Silber/Grau", Color.Silver), // 111

            /*
            // --- Schwarz (Tiefe Schatten) ---
            new ColorContainer(Color.Black, "Schwarz"),
            new ColorContainer(Color.FromArgb(40, 40, 40), "Schwarz"),     // Off-Black (Kunststoff)
            new ColorContainer(Color.FromArgb(30, 32, 35), "Schwarz"),     // Anthrazit
            */

            // --- Schwarz-KORRIDOR ---
            new ColorContainer(Color.FromArgb(0, 0, 0), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0, 0, 0 + d), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0, 0 + d, 0), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0 + d, 0, 0), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0 + d, 0 + d, 0), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0, 0 + d, 0 + d), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0 + d, 0, 0 + d), "Schwarz", Color.Black),
            new ColorContainer(Color.FromArgb(0 + d, 0 + d, 0 + d), "Schwarz", Color.Black),


            // --- ROT-TÖNE (Inkl. Weinrot und helles Licht-Rot) ---
            new ColorContainer(Color.Red, "Rot", Color.Red),
            new ColorContainer(Color.FromArgb(139, 0, 0), "Rot", Color.Red),          // Dunkelrot
            new ColorContainer(Color.FromArgb(255, 99, 71), "Rot", Color.Red),        // Tomatenrot (hell)
            new ColorContainer(Color.FromArgb(111, 38, 19), "Rot", Color.Red),        // Deine Messung (Erd-Rot)
            new ColorContainer(Color.FromArgb(194, 38, 62), "Rot", Color.Red),        // Deine Messung (Kirschrot)
            new ColorContainer(Color.FromArgb(157, 99, 123), "Rot", Color.Red),        // 
            new ColorContainer(Color.FromArgb(166, 86, 87), "Rot", Color.Red),        // 
            new ColorContainer(Color.FromArgb(144, 43, 57), "Rot", Color.Red),        // 
            new ColorContainer(Color.FromArgb(152, 56, 70), "Rot", Color.Red),        // 
            new ColorContainer(Color.FromArgb(149, 48, 64), "Rot", Color.Red),        // 

            // --- BLAU-TÖNE (Himmel, Wasser, Jeans) ---
            new ColorContainer(Color.Blue, "Blau", Color.Navy),
            new ColorContainer(Color.FromArgb(0, 0, 139), "Blau", Color.Navy),         // Dark Blue
            new ColorContainer(Color.FromArgb(70, 130, 180), "Blau", Color.Navy),      // Stahlblau (Jeans/Schatten)
            new ColorContainer(Color.FromArgb(72, 64, 103), "Blau", Color.Navy),       // Deine Messung
            new ColorContainer(Color.FromArgb(12, 66, 102), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(10, 64, 100), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(9, 56, 108), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(21, 59, 121), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(6, 57, 138), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(35, 72, 116), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(42, 59, 79), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(125, 152, 182), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(16, 36, 71), "Blau", Color.Navy), // Blue
            new ColorContainer(Color.FromArgb(135, 206, 235), "Blau", Color.Navy), // Sky Blue
            new ColorContainer(Color.FromArgb(0, 162, 252), "Blau", Color.Navy),

            // --- GRÜN-TÖNE (Laub, Gras, Moos) ---
            new ColorContainer(Color.Green, "Grün", Color.Green),
            new ColorContainer(Color.FromArgb(45, 172, 80), "Grün", Color.Green),
            new ColorContainer(Color.FromArgb(0, 100, 0), "Grün", Color.Green),               // Dark Green
            new ColorContainer(Color.FromArgb(107, 142, 35), "Grün", Color.Green),      // Olivgrün (sehr wichtig für Fotos!)
            new ColorContainer(Color.FromArgb(144, 238, 144), "Grün", Color.Green), // Light Green
            new ColorContainer(Color.FromArgb(45, 172, 80), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(70, 87, 45), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(66, 96, 8), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(67, 85, 37), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(70, 85, 42), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(70, 86, 39), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(60, 255, 254), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(193, 255, 255), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(73, 104, 98), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(77, 110, 99), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(123, 142, 120), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(129, 149, 124), "Grün", Color.Green),  // Deine Messung
            new ColorContainer(Color.FromArgb(117, 137, 112), "Grün", Color.Green),  // Deine Messung

            // --- GELB- & ORANGE-TÖNE (Sonne, Herbstlaub) ---
            new ColorContainer(Color.Yellow, "Gelb", Color.Yellow),
            new ColorContainer(Color.FromArgb(255, 215, 0), "Gelb", Color.Yellow),       // Gold
            new ColorContainer(Color.FromArgb(188, 143, 76), "Gelb", Color.Yellow),    // gelb
            new ColorContainer(Color.FromArgb(188, 137, 71), "Gelb", Color.Yellow),    // gelb
            new ColorContainer(Color.FromArgb(179, 146, 103), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(168, 133, 77), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(141, 101, 39), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(158, 110, 36), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(163, 126, 73), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(213, 181, 142), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(226, 192, 131), "Gelb", Color.Yellow),     // gelb
            new ColorContainer(Color.FromArgb(213, 180, 137), "Gelb", Color.Yellow),     // gelb

            new ColorContainer(Color.Orange, "Orange", Color.FromArgb(255, 128, 0)),
            new ColorContainer(Color.FromArgb(255, 140, 0), "Orange", Color.FromArgb(255, 128, 0)),     // Dunkelorange
            new ColorContainer(Color.FromArgb(188, 109, 78), "Orange", Color.FromArgb(255, 128, 0)),     // Dunkelorange
            new ColorContainer(Color.FromArgb(189, 112, 82), "Orange", Color.FromArgb(255, 128, 0)),     // Dunkelorange
            new ColorContainer(Color.FromArgb(206, 181, 108), "Orange", Color.FromArgb(255, 128, 0)),     // Dunkelorange
            new ColorContainer(Color.FromArgb(164, 95, 54), "Orange", Color.FromArgb(255, 128, 0)),     // Dunkelorange


            // --- BRAUN-TÖNE (Holz, Erde, Leder) ---
            new ColorContainer(Color.Brown, "Braun", Color.Maroon),
            new ColorContainer(Color.FromArgb(139, 69, 19), "Braun", Color.Maroon), 
            new ColorContainer(Color.FromArgb(101, 67, 33), "Braun", Color.Maroon), 
            new ColorContainer(Color.FromArgb(210, 180, 140), "Braun", Color.Maroon), 
            new ColorContainer(Color.FromArgb(141, 85, 36), "Braun", Color.Maroon),       // Dunkle Haut
            new ColorContainer(Color.FromArgb(111, 38, 19), "Rot", Color.Red),

            // --- VIOLETT- & TÜRKIS-TÖNE ---
            new ColorContainer(Color.Purple, "Lila", Color.FromArgb(192, 0, 192)),
            new ColorContainer(Color.FromArgb(128, 0, 128), "Lila", Color.FromArgb(192, 0, 192)),
            new ColorContainer(Color.Teal, "Blaugrün", Color.FromArgb(0, 90, 140)),

            // --- HAUTTÖNE / FLESH (Wichtig für Porträts, oft als Beige/Braun wahrgenommen) ---
            new ColorContainer(Color.FromArgb(255, 224, 189), "Weiss", Color.White), 
            new ColorContainer(Color.FromArgb(255, 205, 148), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(174, 157, 147), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(169, 159, 135), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(172, 153, 136), "Weiss", Color.White),
            new ColorContainer(Color.FromArgb(174, 151, 133), "Weiss", Color.White),



            new ColorContainer(Color.Gold, "sonstiges", Color.Gold)
            
        };
        
        public string MatchAverageColorName(Point start, int deltaRGB)
        {
            return MatchAverageColorName(GetAverageAreaColor(start, deltaRGB));
        }


        public ColorContainer MatchAverageColor(Color averageColor)
        {
            ColorContainer bestMatch = ColorLibrary[0];
            try
            {
                if (averageColor != Color.Transparent)
                {
                    double shortestDistance = double.MaxValue;

                    // Zugriff auf die statische Library
                    for (int i = 0; i < ColorLibrary.Length; i++)
                    {
                        // Euklidischer Abstand (ohne Math.Pow für bessere Performance)
                        int rDiff = averageColor.R - ColorLibrary[i].Color.R;
                        int gDiff = averageColor.G - ColorLibrary[i].Color.G;
                        int bDiff = averageColor.B - ColorLibrary[i].Color.B;

                        double distance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);

                        if (distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            bestMatch = ColorLibrary[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Farbabgleich: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bestMatch; // DER EINZIGE EXITPOINT
        }
        public string MatchAverageColorName(Color averageColor)
        {

            return MatchAverageColor(averageColor).Name; // DER EINZIGE EXITPOINT
        }
    }
}