using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Anzeige
{
    public class FloodFiller
    {
        private Bitmap bitmap;
        private Color fillColor;
        private Func<Color, Color, bool> fillCriteria;
        private bool[,] filled;

        public FloodFiller(Bitmap bitmap, Color fillColor, Func<Color, Color, bool> fillCriteria)
        {
            this.bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            this.fillColor = fillColor;
            this.fillCriteria = fillCriteria ?? throw new ArgumentNullException(nameof(fillCriteria));
        }

        public Rectangle FloodFill(Point startPoint)
        {
            // Überprüfung der Grenzen des Bitmaps
            if (startPoint.X < 0 || startPoint.Y < 0 || startPoint.X >= bitmap.Width || startPoint.Y >= bitmap.Height)
                throw new ArgumentException("Startpunkt liegt außerhalb der Grenzen des Bitmaps.");

            // Initialisierung der benötigten Datenstrukturen
            filled = new bool[bitmap.Width, bitmap.Height];
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(startPoint);

            // Festlegen der Startfarbe
            Color startColor = bitmap.GetPixel(startPoint.X, startPoint.Y);

            // Flutfüllalgorithmus
            while (queue.Count > 0)
            {
                Point point = queue.Dequeue();
                if (filled[point.X, point.Y] || !fillCriteria(bitmap.GetPixel(point.X, point.Y), fillColor))
                    continue;

                // Füllen des Pixels mit der Füllfarbe
                bitmap.SetPixel(point.X, point.Y, fillColor);
                filled[point.X, point.Y] = true;

                // Überprüfen der Nachbarpixel
                if (point.X > 0)
                    queue.Enqueue(new Point(point.X - 1, point.Y));
                if (point.X < bitmap.Width - 1)
                    queue.Enqueue(new Point(point.X + 1, point.Y));
                if (point.Y > 0)
                    queue.Enqueue(new Point(point.X, point.Y - 1));
                if (point.Y < bitmap.Height - 1)
                    queue.Enqueue(new Point(point.X, point.Y + 1));
            }

            // Bestimmen der äußeren Abmessungen des gefüllten Bereichs
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (filled[x, y])
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }
    }
}
