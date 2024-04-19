using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class Numberplate
    {
        public Bitmap scaledBitmap { get; set; }

        public Numberplate (Bitmap bmp)
        {
            if (bmp != null)
            {
                scaledBitmap = Tools.ResizeBitmap(bmp, bmp.Width, bmp.Height);

                for (int y = 0; y < scaledBitmap.Height; y+=1)
                {
                    for (int x = 0; x < scaledBitmap.Width; x+=1)
                    {
                        Color c = scaledBitmap.GetPixel(x, y);
                        Color cto = ColorClassifier.Classify(c);
                        if (cto == ColorClassifier.plateblue)
                        {
                            cto = cto;
                            scaledBitmap.SetPixel(x, y, Color.Red);
                        }
                        scaledBitmap.SetPixel(x, y, cto);
                        // scaledBitmap.SetPixel(x, y, Color.Red);
                        // CReferenzColor.BackgroundImage = scaledBitmap;
                    }
                    // CReferenzColor.Refresh();
                }
            }

        }
    }
}
