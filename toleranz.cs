using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class toleranz
    {
        public Double k { get; set; }
        public Point pleft { get; set; }
        public Point pright { get; set; }
        public double dleft
        {
            get { return Math.Sqrt(Math.Pow(pleft.X - paug.X, 2) + Math.Pow(pleft.Y - paug.Y, 2)); }
        }
        public Point paug { get; set; }
        public Point pref1 { get; set; }
        public Point pref2 { get; set; }
        public double dpref
        {
            get { return Math.Sqrt(Math.Pow(pref2.X - pref1.X, 2) + Math.Pow(pref2.Y - pref1.Y, 2)); }
        }
        public Point dist1 { get; set; }
        public Point dist2 { get; set; }
        public double ddist
        {
            get { return Math.Sqrt(Math.Pow(dist2.X - dist1.X, 2) + Math.Pow(dist2.Y - dist1.Y, 2)) * faktor; }
        }
        public double reftiefe // Vertikale Strecke Augpunkt zur Referenz 
        {
            get { return (pref1.Y + pref2.Y)/2 - paug.Y; }
        }
        public double messtiefe // Vertikale Strecke Augpunkt zur Messung(links) 
        {
            get { return (dist1.Y+ dist2.Y)/2 - paug.Y; }
        }
        public double faktor // Faktor 
        {
            get { return messtiefe / reftiefe; }
        }
        public Double refwidth { get; set; }
        public double bild2real
        {
            get { return refwidth / dpref; }
        }
        public double distanz
        {
            get { return bild2real * ddist; }
        }
        public double T1
        {
            get { return k * dpref / distanz * refwidth; }
        }
        public double T4
        {
            get { return k/dleft; }
        }
        public double T
        {
            get { return T1 * T4; }
        }
        public double Toleranzwert
        {
            get { return distanz * T; }
        }
        public toleranz()
        {
            k = 1.5;
        }
        public double ToleranzInRealkoordinaten
        {
            get
            {
                return T;
            }
        }
        public override string ToString()
        {
            // Formatierung der ToleranzInRealkoordinaten mit einer Nachkommastelle
            string toleranzString = ToleranzInRealkoordinaten.ToString("F1");

            // Rückgabe der formatierten Zeichenfolge
            return $"{toleranzString}";
        }
    }
}
