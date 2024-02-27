using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class Abstandsmeter : UserControl
    {
        private Messwerte.Messwert _CurrentMesswert = new Messwerte.Messwert(150, 100);
        public Messwerte.Messwert CurrentMesswert
        {
            get { return _CurrentMesswert; }
            set 
            { 
                _CurrentMesswert = value;
                CCar.Left = 400 - value.Abstand - CCar.Width;
                CCar2.Left = 480 + value.Abstand2;
                if (CCar.Left + CCar.Width > CBike.Left)
                {
                    CBike.BackgroundImage = CTemplate.BackgroundImage;
                }
                else
                {
                    CBike.BackgroundImage = CTemplate.Image;
                    if (CCar2.Left < 480)
                    {
                        CBike.BackgroundImage = CTemplate.BackgroundImage;
                    }
                    else
                    {
                        CBike.BackgroundImage = CTemplate.Image;
                    }
                }
                this.Refresh();
            }
        }
        public Abstandsmeter()
        {
            InitializeComponent();
        }
        private void Abstandsmeter_Load(object sender, EventArgs e)
        {
            this.Size = new Size(580, 490);
            CExpand.Visible = false;
            if (CurrentMesswert!=null)
                CurrentMesswert.Abstand2 = 200;
        }
        private void Abstandsmeter_Click(object sender, EventArgs e)
        {
            this.Size = CExpand.Size;
            CExpand.Visible = true;
        }
        private void CExpand_Click(object sender, EventArgs e)
        {
            this.Size = new Size(580, 490);
            CExpand.Visible = false;
        }
        private void Abstandsmeter_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (CCar.Left + CCar.Width < 0)
            {
                // Der Abstandsmesser-Rahmen
                int frameWidth = 200;
                int frameHeight = CCar.Height;
                // Position des Rahmens
                int frameX = 0;
                int frameY = CCar.Top;
                // Überprüfen, ob der Rahmen innerhalb des sichtbaren Bereichs liegt
                if (frameX >= 0)
                {
                    // Pfeil zeichnen
                    Point[] arrowPoints = new Point[]
                    {
                        new Point(frameX+frameWidth, frameY),
                        new Point(frameX+frameWidth, frameY + frameHeight),
                        new Point(frameX, frameY + frameHeight/2),
                        new Point(frameX+frameWidth, frameY)
                    };
                    e.Graphics.FillPolygon(Brushes.Blue, arrowPoints);
                    // Rahmen zeichnen
                    e.Graphics.DrawRectangle(Pens.Blue, frameX, frameY, frameWidth, frameHeight);
                }
            }
            Font boldFont = new Font(Font, FontStyle.Bold);
            int lineWidth = 2;
            Pen linePen = new Pen(Color.Black, lineWidth);
            String Distanz = "Distanz = " + CurrentMesswert.Distanz.ToString();
            int Distleft = Math.Max(0, CCar.Left + CCar.Width);
            int Distright = CBike.Left + CBike.Width / 2;
            int DistTop = 0;
            int DistBottom = CCar.Top;

            String Abstand = "Abstand = " + CurrentMesswert.Abstand.ToString();
            int Abstleft = Math.Max(0, CCar.Left + CCar.Width);
            int Abstright = CBike.Left;
            int AbstTop = DistTop + (int)g.MeasureString(Abstand, boldFont).Height;
            int AbstBottom = CCar.Top;

            // Zeichne Linie für Distanz
            g.DrawLine(linePen, Distleft, DistTop, Distright, DistTop);
            g.DrawLine(linePen, Distleft, DistBottom, Distright, DistBottom);

            // Zeichne vertikale Linien für Distanz
            g.DrawLine(linePen, Distleft, DistTop, Distleft, DistBottom);
            g.DrawLine(linePen, Distright, DistTop, Distright, DistBottom);

            // Zeichne Text für Distanz

            // Zeichne Linie für Abstand
            g.DrawLine(linePen, Abstleft, AbstTop, Abstright, AbstTop);
            g.DrawLine(linePen, Abstleft, AbstBottom, Abstright, AbstBottom);

            // Zeichne vertikale Linien für Abstand
            g.DrawLine(linePen, Abstleft, AbstTop, Abstleft, AbstBottom);
            g.DrawLine(linePen, Abstright, AbstTop, Abstright, AbstBottom);
            g.DrawString(Distanz,
                 boldFont,
                 Brushes.Black,
                 new PointF((Distleft + Distright) / 2,
                 // DistTop + (DistBottom - DistTop) / 2),
                 DistTop + (int)g.MeasureString(Distanz, Font).Height / 2),
                 new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            g.DrawString(
                Abstand,
                boldFont,
                Brushes.Black,
                new PointF((Abstleft + Abstright) / 2,
                AbstTop + (AbstBottom - AbstTop) / 2),
                         new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CBike.BackgroundImage = CTemplate.BackgroundImage;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CBike.BackgroundImage = CTemplate.Image;
        }
    }
}
