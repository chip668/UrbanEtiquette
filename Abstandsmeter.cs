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
                if (CCar.Left + CCar.Width > CBike.Left)
                {
                    CBike.BackgroundImage = CTemplate.BackgroundImage;
                }
                else
                {
                    CBike.BackgroundImage = CTemplate.Image;
                }
                CCar2.Left = 480 + value.Abstand2;
                if (CCar2.Left < 480)
                {
                    CBike.BackgroundImage = CTemplate.BackgroundImage;
                }
                else
                {
                    CBike.BackgroundImage = CTemplate.Image;
                }
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
            if (CCar.Left - CCar.Width < 0)
            {
                // Der Abstandsmesser-Rahmen
                int frameWidth = 200;
                int frameHeight = 20;
                // Position des Rahmens
                int frameX = CCar.Left - frameWidth;
                int frameY = (CCar.Top + CCar.Bottom) / 2 - frameHeight / 2;
                // Überprüfen, ob der Rahmen innerhalb des sichtbaren Bereichs liegt
                if (frameX > 0)
                {
                    // Pfeil zeichnen
                    Point[] arrowPoints = new Point[]
                    {
                        new Point(frameX, frameY + frameHeight / 2),
                        new Point(frameX + 10, frameY),
                        new Point(frameX + 10, frameY + frameHeight),
                    };
                    e.Graphics.FillPolygon(Brushes.Black, arrowPoints);
                    // Rahmen zeichnen
                    e.Graphics.DrawRectangle(Pens.Black, frameX, frameY, frameWidth, frameHeight);
                }
            }
        }
    }
}
