using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace Anzeige
{
    public class ImageCollageGenerator
    {
        public static void WriteOptimalCollage(List<string> imagePaths, string filename, int cellSize = 640)
        {
            if (File.Exists(filename))
                File.Delete(filename);
            Bitmap image = CreateOptimalCollage(imagePaths, cellSize);
            image.Save(filename);
            image.Save(filename.Replace(".png", ".jpg"));
        }

        public static Bitmap CreateOptimalCollage_new(List<string> imagePaths, int targetCellSize = 640)
        {
            if (imagePaths == null || imagePaths.Count == 0 || imagePaths.Any(p => !File.Exists(p)))
                return new Bitmap(targetCellSize, targetCellSize);

            int n = imagePaths.Count;
            List<Bitmap> images = imagePaths.Select(p => new Bitmap(p)).ToList();
            var aspects = images.Select(img => (double)img.Width / img.Height).ToList();

            // ====================== DEIN TEIL ======================
            int rows = (int)Math.Ceiling(Math.Sqrt(n));
            int cols = rows - 1;
            if (rows * cols < n)
                cols++;

            int free = rows * cols - n;

            int widestIdx = 0;
            int tallestIdx = 0;
            double maxWideRatio = double.MinValue;
            double maxTallRatio = double.MinValue;

            for (int i = 0; i < n; i++)
            {
                double hq = aspects[i];
                double vq = 1.0 / aspects[i];

                if (hq > maxWideRatio)
                {
                    maxWideRatio = hq;
                    widestIdx = i;
                }
                if (vq > maxTallRatio)
                {
                    maxTallRatio = vq;
                    tallestIdx = i;
                }
            }

            bool spanTall = false;
            bool spanWide = false;
            int spanTallIndex = -1;
            int spanWideIndex = -1;

            if (free > 1)
            {
                spanTall = true;
                spanTallIndex = tallestIdx;
                spanWide = true;
                spanWideIndex = widestIdx;
            }
            else if (free == 1)
            {
                if (maxTallRatio > maxWideRatio)
                {
                    spanTall = true;
                    spanTallIndex = tallestIdx;
                }
                else
                {
                    spanWide = true;
                    spanWideIndex = widestIdx;
                }
            }
            // ====================== ENDE DEIN TEIL ======================

            int cellSize = targetCellSize;
            Bitmap collage = new Bitmap(cols * cellSize, rows * cellSize);

            using (Graphics g = Graphics.FromImage(collage))
            {
                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                bool[,] occupied = new bool[rows, cols];

                int placed = 0;

                // ====================== PLATZIERUNG NACH DEINER NEUEN REGEL ======================

                // spanTall: immer oben RECHTS über 2 Zeilen
                if (spanTall && spanTallIndex >= 0 && spanTallIndex < n)
                {
                    int col = cols - 1;                    // ganz rechts
                    Rectangle destRect = new Rectangle(col * cellSize, 0, cellSize, 2 * cellSize);
                    g.DrawImage(images[spanTallIndex], GetCenteredRect(images[spanTallIndex], destRect));

                    occupied[0, col] = true;
                    if (rows > 1) occupied[1, col] = true;
                }

                // spanWide: immer unten RECHTS über 2 Spalten
                if (spanWide && spanWideIndex >= 0 && spanWideIndex < n)
                {
                    int row = rows - 1;                    // letzte Zeile
                    int col = cols - 2;                    // vorletzte + letzte Spalte
                    if (col < 0) col = 0;

                    Rectangle destRect = new Rectangle(col * cellSize, row * cellSize, 2 * cellSize, cellSize);
                    g.DrawImage(images[spanWideIndex], GetCenteredRect(images[spanWideIndex], destRect));

                    occupied[row, col] = true;
                    if (col + 1 < cols) occupied[row, col + 1] = true;
                }

                int r = 0;
                int c = 0;
                List<string> debug = new List<string>();
                while (placed < n)
                {
                    if(!occupied[r, c])
                    {
                        bool done = false;
                        done = ((spanWide && (spanWideIndex == placed)) || (spanTall && (spanTallIndex == placed)));
                        if (!done)
                        {
                            Rectangle destRect = new Rectangle(c * cellSize, r * cellSize, cellSize, cellSize);
                            g.DrawImage(images[placed], GetCenteredRect(images[placed], destRect));
                            occupied[r, c] = true;
                        }
                        placed++;
                    }
                    debug.Add(r.ToString() + " - " + c.ToString());
                    c++;
                    if (c>=cols)
                    {
                        c = 0;
                        r++;
                        if (r >= rows)
                            break;
                    }
                }
            }

            foreach (var img in images) img.Dispose();
            return collage;
        }
        public static Bitmap CreateOptimalCollage(List<string> imagePaths, int targetCellSize = 640)
        {
            if (imagePaths == null || imagePaths.Count == 0 || imagePaths.Any(p => !File.Exists(p)))
                return new Bitmap(targetCellSize, targetCellSize);

            int n = imagePaths.Count;
            List<Bitmap> images = imagePaths.Select(p => new Bitmap(p)).ToList();
            var aspects = images.Select(img => (double)img.Width / img.Height).ToList();

            // ====================== DEIN TEIL ======================
            int rows = (int)Math.Ceiling(Math.Sqrt(n));
            int cols = rows - 1;
            if (rows * cols < n)
                cols++;

            int free = rows * cols - n;

            int widestIdx = 0;
            int tallestIdx = 0;
            double maxWideRatio = double.MinValue;
            double maxTallRatio = double.MinValue;

            for (int i = 0; i < n; i++)
            {
                double hq = aspects[i];
                double vq = 1.0 / aspects[i];

                if (hq > maxWideRatio)
                {
                    maxWideRatio = hq;
                    widestIdx = i;
                }
                if (vq > maxTallRatio)
                {
                    maxTallRatio = vq;
                    tallestIdx = i;
                }
            }

            bool spanTall = false;
            bool spanWide = false;
            int spanTallIndex = -1;
            int spanWideIndex = -1;

            if (free > 1)
            {
                spanTall = true;
                spanTallIndex = tallestIdx;
                spanWide = true;
                spanWideIndex = widestIdx;
            }
            else if (free == 1)
            {
                if (maxTallRatio > maxWideRatio)
                {
                    spanTall = true;
                    spanTallIndex = tallestIdx;
                }
                else
                {
                    spanWide = true;
                    spanWideIndex = widestIdx;
                }
            }
            // ====================== ENDE DEIN TEIL ======================

            int cellSize = targetCellSize;
            Bitmap collage = new Bitmap(cols * cellSize, rows * cellSize);

            using (Graphics g = Graphics.FromImage(collage))
            {
                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                bool[,] occupied = new bool[rows, cols];

                int placed = 0;

                // ====================== PLATZIERUNG NACH DEINER NEUEN REGEL ======================

                // spanTall: immer oben RECHTS über 2 Zeilen
                if (spanTall && spanTallIndex >= 0 && spanTallIndex < n)
                {
                    int col = cols - 1;                    // ganz rechts
                    Rectangle destRect = new Rectangle(col * cellSize, 0, cellSize, 2 * cellSize);
                    g.DrawImage(images[spanTallIndex], GetCenteredRect(images[spanTallIndex], destRect));

                    occupied[0, col] = true;
                    if (rows > 1) occupied[1, col] = true;
                }

                // spanWide: immer unten RECHTS über 2 Spalten
                if (spanWide && spanWideIndex >= 0 && spanWideIndex < n)
                {
                    int row = rows - 1;                    // letzte Zeile
                    int col = cols - 2;                    // vorletzte + letzte Spalte
                    if (col < 0) col = 0;

                    Rectangle destRect = new Rectangle(col * cellSize, row * cellSize, 2 * cellSize, cellSize);
                    g.DrawImage(images[spanWideIndex], GetCenteredRect(images[spanWideIndex], destRect));

                    occupied[row, col] = true;
                    if (col + 1 < cols) occupied[row, col + 1] = true;
                }

                List<string> debug = new List<string>();
                // Restliche normale Bilder 1x1 füllen
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        bool done = false;
                        done = ((spanWide && (spanWideIndex == placed)) || (spanTall && (spanTallIndex == placed)));
                        while (done)
                        {
                            placed++;
                            done = ((spanWide && (spanWideIndex == placed)) || (spanTall && (spanTallIndex == placed)));
                        }
                        if (placed<n && !occupied[r, c])
                        {
                            Rectangle destRect = new Rectangle(c * cellSize, r * cellSize, cellSize, cellSize);
                            g.DrawImage(images[placed], GetCenteredRect(images[placed], destRect));
                            occupied[r, c] = true;
                            placed++;
                        }
                    }
                }
            }

            foreach (var img in images) img.Dispose();
            return collage;
        }
        // Hilfsmethode: Bild proportional skalieren und mittig in die Zelle setzen
        private static Rectangle GetCenteredRect(Bitmap image, Rectangle targetRect)
        {
            double imgAspect = (double)image.Width / image.Height;
            double targetAspect = (double)targetRect.Width / targetRect.Height;

            int drawWidth, drawHeight;
            int offsetX = 0, offsetY = 0;

            if (imgAspect > targetAspect)
            {
                // Bild ist breiter → Höhe anpassen
                drawWidth = targetRect.Width;
                drawHeight = (int)(targetRect.Width / imgAspect);
                offsetY = (targetRect.Height - drawHeight) / 2;
            }
            else
            {
                // Bild ist höher → Breite anpassen
                drawHeight = targetRect.Height;
                drawWidth = (int)(targetRect.Height * imgAspect);
                offsetX = (targetRect.Width - drawWidth) / 2;
            }

            return new Rectangle(
                targetRect.X + offsetX,
                targetRect.Y + offsetY,
                drawWidth,
                drawHeight
            );
        }
    }
}