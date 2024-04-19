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
        Stack<Cursor> cstack = new Stack<Cursor>();
        public const String  colorfile = "clolormatching.clr";
        public String backupcolorfile
        {
            get { return colorfile.Replace(".clr", ".bak"); }
        }
        private Bitmap _Original;
        public Bitmap Original
        {
            get { return _Original; }
            set 
            { 
                _Original = value;
                CReferenzColor.BackgroundImage = _Original;
            }
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
            CMode.SelectedIndex = 0;
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
        private void panel_Click2(object sender, EventArgs e)
        {
            CSelected.BackColor = ((Panel)sender).BackColor;
        }

        private void Colortraining_Load(object sender, EventArgs e)
        {

        }

        private void CFullPicture_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)CReferenzColor.BackgroundImage;
            if (bmp != null)
            {
                // Bitmap scaledBitmap = Tools.ResizeBitmap(bmp, CReferenzColor.Width, CReferenzColor.Height);
                Bitmap scaledBitmap = new Bitmap(bmp);

                for (int y = 0; y < scaledBitmap.Height; y++)
                {
                    for (int x=0; x< scaledBitmap.Width; x++)
                    {
                        Color c = scaledBitmap.GetPixel(x, y);
                        Color cto = ColorClassifier.Classify(c);
                        scaledBitmap.SetPixel(x, y, cto);
                        // scaledBitmap.SetPixel(x, y, Color.Red);
                        CReferenzColor.BackgroundImage = scaledBitmap;
                    }
                    CReferenzColor.Refresh();
                }
            }
            /*
            start = new Point(0, 0);
            stop = new Point(Original.Width, Original.Height);
            MouseButtons bt = MouseButtons.Right;
            MouseEventArgs e2 = new MouseEventArgs(bt, 1, stop.X, stop.Y, 0);
            CReferenzColor_MouseUp(sender, e2);
            */
        }

        private void CKennzeichen_CheckedChanged(object sender, EventArgs e)
        {
            ColorClassifier.kennzeichen = CKennzeichen.Checked;
            if (CKennzeichen.Checked)
            {
                CTrainColors.Checked = false;
            }
        }

        Point start = new Point(0, 0);
        Point stop = new Point(0, 0);
        private void CReferenzColor_MouseDown(object sender, MouseEventArgs e)
        {
            start = e.Location;
        }

        public void DrawRectangleOnBitmap(Bitmap bitmap, Point start, Point stop)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Berechne die Breite und Höhe des Rechtecks
                int width = stop.X - start.X;
                int height = stop.Y - start.Y;

                // Zeichne das Rechteck auf die Bitmap
                g.DrawRectangle(Pens.Red, start.X, start.Y, width, height);
            }
        }

        Bitmap marker;
        private void CReferenzColor_MouseMove(object sender, MouseEventArgs e)
        {
            bt = e.Button;
            stop = e.Location;
            if (bt == MouseButtons.Right)
            {
                marker = new Bitmap(Original);
                // double dx = (double)marker.Width / CReferenzColor.Width;
                // double dy = (double)marker.Height / CReferenzColor.Height;
                // Point start1 = new Point((int)(start.X * dx), (int)(start.Y * dy));
                // Point stop1 = new Point((int)(stop.X * dx), (int)(stop.Y * dy));
                Point start1 = new Point(start.X, start.Y);
                Point stop1 = new Point(stop.X, stop.Y);
                KoordReorg(ref start1, ref stop1, marker);
                DrawRectangleOnBitmap(marker, start1, stop1);
                CReferenzColor.BackgroundImage = marker;
            }
        }


        public static void LimitPoints(ref Point start, ref Point stop, Bitmap bmp)
        {
            start.X = LimitKoord(start.X, bmp.Width);
            start.Y = LimitKoord(start.Y, bmp.Height);
            stop.X = LimitKoord(stop.X, bmp.Width);
            stop.Y = LimitKoord(stop.Y, bmp.Height);
        }

        public static int LimitKoord(int x, int size)
        {
            return Math.Max(0, Math.Min(x, size - 1));
        }

        public void KoordReorg (ref Point start, ref Point stop, Bitmap bmp)
        {
            int h;
            double dx = (double)bmp.Width / CReferenzColor.Width;
            double dy = (double)bmp.Height / CReferenzColor.Height;
            // double dx = 1;
            // double dy = 1;
            start = new Point((int)(start.X * dx), (int)(start.Y * dy));
            stop = new Point((int)(stop.X * dx), (int)(stop.Y * dy));

            if (start.X > stop.X)
            {
                h = start.X;
                start.X = stop.X;
                stop.X = h;
            }
            if (start.Y > stop.Y)
            {
                h = start.Y;
                start.Y = stop.Y;
                stop.Y = h;
            }

        }

        MouseButtons bt = MouseButtons.None;
        private void CReferenzColor_MouseUp(object sender, MouseEventArgs e)
        {
            cstack.Push(this.Cursor);
            stop = e.Location;
            bt = e.Button;
            if (Original == null)
                return;

            Bitmap bmp = new Bitmap(Original);
            CReferenzColor.BackgroundImage = bmp;

            if (bt == MouseButtons.Left)
            {
                start = stop;
                if (bmp != null)
                {
                    Bitmap scaledBitmap = Tools.ResizeBitmap(bmp, CReferenzColor.Width, CReferenzColor.Height);
                    KoordReorg(ref start, ref stop, scaledBitmap);
                    LimitPoints(ref start, ref stop, scaledBitmap);
                    CSelected.BackColor = scaledBitmap.GetPixel(stop.X, stop.Y);
                    if (CTrainColors.Checked)
                    {
                        ColorClassifier.Train(CReference.BackColor, CSelected.BackColor);
                    }
                    else
                    {
                        CReference.BackColor = ColorClassifier.Classify(CSelected.BackColor);
                    }
                }
            }
            else if (bt == MouseButtons.Right)
            {
                KoordReorg(ref start, ref stop, bmp);
                if (bmp != null)
                {
                    Color cr1 = CReference.BackColor;
                    Color cr2 = CSelected.BackColor;
                    KoordReorg(ref start, ref stop, bmp);
                    LimitPoints(ref start, ref stop, bmp);
                    for (int y = start.Y; y < stop.Y; y++)
                    {
                        for (int x = start.X; x < stop.X; x++)
                        {
                            Color c = bmp.GetPixel(x, y);
                            Color cto = ColorClassifier.Classify(c);

                            switch (CMode.Text)
                            {
                                case "ersetzen":
                                    if (cr2 == cto)
                                    {
                                        // ColorClassifier.Train(CReference.BackColor, CSelected.BackColor);
                                        ColorClassifier.Train(cr1, c);
                                    }
                                    break;
                                case "zeigen":
                                    {
                                        bmp.SetPixel(x, y, cto);
                                        // scaledBitmap.SetPixel(x, y, Color.Red);
                                        CReferenzColor.BackgroundImage = bmp;
                                        CReferenzColor.Refresh();
                                    }
                                    break;
                            }

                        }
                    }
                }
            }
            cstack.Pop();
        }

        private void CReset_Click(object sender, EventArgs e)
        {
            ColorClassifier.LoadFromFile();
        }

        private void COriginal_Click(object sender, EventArgs e)
        {
            CReferenzColor.BackgroundImage = Original;
        }

        private void CColorMapping_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                var listBox = sender as ListBox;
                var pair = (KeyValuePair<Color, Color>)listBox.Items[e.Index];

                int itemWidth = e.Bounds.Width / 2;

                // Rechtecke für die Farben zeichnen
                Rectangle rectKey = new Rectangle(e.Bounds.Left, e.Bounds.Top, itemWidth, e.Bounds.Height);
                Rectangle rectValue = new Rectangle(e.Bounds.Left + itemWidth, e.Bounds.Top, itemWidth, e.Bounds.Height);

                // Zeichnen des Hintergrunds (bei Auswahl rotes Rechteck um das gesamte Element)
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    using (Pen redPen = new Pen(Color.Red, 4))
                    {
                        e.Graphics.DrawRectangle(redPen, e.Bounds);
                    }
                }
                else
                {
                    e.DrawBackground();
                }

                using (SolidBrush brushKey = new SolidBrush(pair.Key))
                {
                    e.Graphics.FillRectangle(brushKey, rectKey);
                }

                using (SolidBrush brushValue = new SolidBrush(pair.Value))
                {
                    e.Graphics.FillRectangle(brushValue, rectValue);
                }

                // Text darstellen
                using (Font boldFont = new Font(listBox.Font, FontStyle.Bold))
                {
                    using (SolidBrush textBrush = new SolidBrush(Tools.InvertColor(pair.Key)))
                    {
                        e.Graphics.DrawString(pair.Key.Name, boldFont, textBrush, rectKey.X + 2, e.Bounds.Top + 3);
                    }
                    using (SolidBrush textBrush = new SolidBrush(Tools.InvertColor(pair.Value)))
                    {
                        e.Graphics.DrawString(pair.Value.Name, boldFont, textBrush, rectValue.X + 2, e.Bounds.Top + 3);
                    }
                }

                e.DrawFocusRectangle();
            }
        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CColorMapping.Items.Clear();
            foreach (var pair in ColorClassifier.weights)
            {
                CColorMapping.Items.Add(pair);
            }
        }

        private void CColorMapping_Click(object sender, EventArgs e)
        {
            if (CColorMapping.SelectedIndex >= 0)
            {
                var listBox = sender as ListBox;
                var pair = (KeyValuePair<Color, Color>)listBox.Items[CColorMapping.SelectedIndex];

            }

        }

        private void CColorMapping_DoubleClick(object sender, EventArgs e)
        {
            if (CColorMapping.SelectedIndex >= 0)
            {
                var listBox = sender as ListBox;
                var pair = (KeyValuePair<Color, Color>)listBox.Items[CColorMapping.SelectedIndex];
                CColorMapping.Items.Remove(pair);
                ColorClassifier.weights.Remove(pair.Key);
            }
        }

        private void CColorMapping_SelectedIndexChanged(object sender, EventArgs e)
        {
            CColorMapping.Refresh();
            if (CColorMapping.SelectedIndex >= 0)
            {
                var listBox = sender as ListBox;
                var pair = (KeyValuePair<Color, Color>)listBox.Items[CColorMapping.SelectedIndex];
                CKey1.BackColor = pair.Key;
                CKey1.ForeColor = Tools.InvertColor(pair.Key);
                CKey1.Text = pair.Key.Name;
                CValue1.BackColor = pair.Value;
                CValue1.ForeColor = Tools.InvertColor(pair.Value);
                CValue1.Text = pair.Value.Name;
            }
        }
    }
}

