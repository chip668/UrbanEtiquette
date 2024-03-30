using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class Colortraining : Form
    {
        public const String  colorfile = "clolormatching.clr";
        public String backupcolorfile
        {
            get { return colorfile.Replace(".clr", ".bak"); }
        }

        public Colortraining()
        {
            InitializeComponent();
            try
            {
                ColorClassifier.LoadFromFile(colorfile);
            }
            catch
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(backupcolorfile))
                    File.Delete(backupcolorfile);
                if (File.Exists(colorfile))
                    File.Move(colorfile, backupcolorfile);

                ColorClassifier.SaveToFile(colorfile);
            }
            catch { }
        }


        private void panel_Click(object sender, EventArgs e)
        {
            CReference.BackColor = ((Panel)sender).BackColor;
        }

        Point pnt = new Point(0,0);
        private void CReferenzColor_MouseDown(object sender, MouseEventArgs e)
        {
            pnt = e.Location;
        }

        private void CReferenzColor_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)CReferenzColor.BackgroundImage;
            if (bmp != null)
            {
                Bitmap scaledBitmap = Tools.ResizeBitmap(bmp, CReferenzColor.Width, CReferenzColor.Height);
                CSelected.BackColor = scaledBitmap.GetPixel(pnt.X, pnt.Y);
                if (CTrainColors.Checked)
                {
                    ColorClassifier.Train (CReference.BackColor, CSelected.BackColor);
                }
                else
                {
                    CReference.BackColor = ColorClassifier.Classify(CSelected.BackColor);
                }
            }
        }

        private void Colortraining_Load(object sender, EventArgs e)
        {

        }
    }
}

