using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Anzeige
{
    /// <summary>
    /// Hilfsklasse zur Berechnung ähnlicher Farben.
    /// </summary>
    public static class ColorHelper
    {
        public static Color FindClosestColor(List<Color> refcolor, Color clickcolor)
        {
            Color closestColor = refcolor[0];
            double minDiff = CalculateColorDifferenceCIELAB(refcolor[0], clickcolor);

            foreach (Color color in refcolor)
            {
                double diff = CalculateColorDifferenceCIELAB(color, clickcolor);

                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestColor = color;
                }
            }

            return closestColor;
        }

        private static double CalculateColorDifferenceCIELAB(Color color1, Color color2)
        {
            double[] lab1 = RGBtoLAB(color1.R, color1.G, color1.B);
            double[] lab2 = RGBtoLAB(color2.R, color2.G, color2.B);

            double diffL = lab1[0] - lab2[0];
            double diffA = lab1[1] - lab2[1];
            double diffB = lab1[2] - lab2[2];

            return Math.Sqrt(diffL * diffL + diffA * diffA + diffB * diffB);
        }

        private static double[] RGBtoLAB(int r, int g, int b)
        {
            double[] result = new double[3];

            double[] xyz = RGBtoXYZ(r, g, b);

            result[0] = 116.0 * F(xyz[1] / 100.0) - 16.0;
            result[1] = (xyz[0] / 95.047 > 0.008856) ? CubicRoot(xyz[0] / 95.047) : (903.3 * xyz[0] / 100.0 + 16.0) / 116.0;
            result[2] = (xyz[2] / 108.883 > 0.008856) ? CubicRoot(xyz[2] / 108.883) : (903.3 * xyz[2] / 100.0 + 16.0) / 116.0;

            result[1] *= 100.0;
            result[2] *= 100.0;

            return result;
        }

        private static double[] RGBtoXYZ(int r, int g, int b)
        {
            double[] result = new double[3];

            double varR = (r / 255.0);
            double varG = (g / 255.0);
            double varB = (b / 255.0);

            varR = (varR > 0.04045) ? Math.Pow((varR + 0.055) / 1.055, 2.4) : varR / 12.92;
            varG = (varG > 0.04045) ? Math.Pow((varG + 0.055) / 1.055, 2.4) : varG / 12.92;
            varB = (varB > 0.04045) ? Math.Pow((varB + 0.055) / 1.055, 2.4) : varB / 12.92;

            varR *= 100.0;
            varG *= 100.0;
            varB *= 100.0;

            result[0] = varR * 0.4124564 + varG * 0.3575761 + varB * 0.1804375;
            result[1] = varR * 0.2126729 + varG * 0.7151522 + varB * 0.0721750;
            result[2] = varR * 0.0193339 + varG * 0.1191920 + varB * 0.9503041;

            return result;
        }

        private static double F(double value)
        {
            return (value > 0.008856) ? Math.Pow(value, 0.3333) : ((value * 903.3) + 16.0) / 116.0;
        }

        private static double CubicRoot(double value)
        {
            return Math.Pow(value, 0.3333);
        }
    }
}
