using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class PixelatedArea
    {
        public Rectangle Area { get; set; }
        public Bitmap Bitmap { get; set; }
        public String Filename { get; set; }
        public int Raster { get; set; }
        public PixelatedArea(Rectangle area, Bitmap pixelatedBitmap, String filename, int raster = 20)
        {
            Area = area;
            Bitmap = pixelatedBitmap;
            Filename = filename;
            Raster = raster;
        }
        /// <summary>
        /// verpöixelt eine Region im angegebenen Bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="region"></param>
        private void PixelOutRegion()
        {
            Area.Intersect(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height));
            for (int y = Area.Top; y < Area.Bottom; y += Raster)
            {
                for (int x = Area.Left; x < Area.Right; x += Raster)
                {
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    int n = Raster * Raster;
                    Color c;
                    for (int zx = 0; zx < Raster; zx++)
                    {
                        for (int zy = 0; zy < Raster; zy++)
                        {
                            c = Bitmap.GetPixel(x + zx, y + zy);
                            r += c.R;
                            g += c.G;
                            b += c.B;
                        }
                    }
                    c = Color.FromArgb(r / n, g / n, b / n);
                    for (int zx = 0; zx < Raster; zx++)
                    {
                        for (int zy = 0; zy < Raster; zy++)
                        {
                            Bitmap.SetPixel(x + zx, y + zy, c);
                        }
                    }
                }
            }
        }
    }
}
