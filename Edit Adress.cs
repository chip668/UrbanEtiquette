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
using static QRCoder.PayloadGenerator;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Anzeige
{
    public partial class Edit_Adress : UserControl
    {
        public event EventHandler Changed;
        protected virtual void OnChanged(EventArgs e)
        {
            Changed?.Invoke(this, e);
        }
        WebBrowser w = null;
        public Boolean changed = false;
        public int Index = -1;
        static bool initializing = false;
        public String Line
        {
            get
            {
                changed = false;
                return CPLZ.Text + ";" + COrt.Text + ";" + CMail.Text + ";" + CWeb.Text + ";" + CGoogle.Text + ((CScript.Text != "") ? ";" + CScript.Text : "");
            }
            set
            {
                String[] items = value.Split(';');
                String prev = Line;

                if (!initializing)
                {
                    initializing = true;
                    if (items.Length > 0)
                        CPLZ.Text = items[0];

                    if (items.Length > 1)
                        COrt.Text = items[1];

                    if (items.Length > 2)
                        CMail.Text = items[2];

                    if (items.Length > 3)
                        CWeb.Text = items[3];

                    if (items.Length > 4)
                    {
                        CGoogle.Text = items[4];
                        CreateBrowser();
                        try
                        {
                            w.Navigate(CGoogle.Text);
                        }
                        catch { }
                    }
                    CScript.Text = (items.Length > 5) ? items[5] : "";
                    OnChanged(EventArgs.Empty);
                    initializing = false;
                    changed = true;
                }
            }
        }
        public Edit_Adress()
        {
            InitializeComponent();
        }
        private void Edit_Adress_Load(object sender, EventArgs e)
        {
        }


        private void CPLZLbl_Resize(object sender, EventArgs e)
        {
            CPLZ.Location = new Point(150, 0);
            CPLZ.Width = CPLZLbl.Width - CPLZ.Left;
            COrt.Location = new Point(150, CPLZ.Top + CPLZ.Height);
            COrt.Width = CPLZLbl.Width - COrt.Left;
            CMail.Location = new Point(150, COrt.Top + COrt.Height);
            CMail.Width = CPLZLbl.Width - CMail.Left;
            CWeb.Location = new Point(150, CMail.Top + CMail.Height);
            CWeb.Width = CPLZLbl.Width - CWeb.Left;
            CGoogle.Location = new Point(150, CWeb.Top + CWeb.Height);
            CGoogle.Width = CPLZLbl.Width - CGoogle.Left;
            CScript.Location = new Point(150, CGoogle.Top + CGoogle.Height);
            CScript.Width = CPLZLbl.Width - CScript.Left;


            CPLZLbl.Top = CPLZ.Top;
            COrtLbl.Top = COrt.Top;
            CMailLbl.Top = CMail.Top;
            CWebLbl.Top = CWeb.Top;
            CGoogleLbl.Top = CGoogle.Top;

            CPLZLbl.Width = 150;
            COrtLbl.Width = 150;
            CMailLbl.Width = 150;
            CWebLbl.Width = 150;
            CGoogleLbl.Width = 150;

            CreateBrowser();
            w.Navigate(CGoogle.Text);
        }
        private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // Der Code, der ausgeführt wird, wenn die URL sich ändert
            CGoogle.Text = w.Url.ToString();
        }
        private void CGoogle_TextChanged(object sender, EventArgs e)
        {
            if (w != null)
                w.Navigate(CGoogle.Text);
            TextChanged(sender, e);
        }
        private void CreateBrowser()
        {
            if (w == null)
            {
                w = new WebBrowser();
                this.Controls.Add(w);
                w.Navigated += WebBrowser_Navigated;
                w.ScriptErrorsSuppressed = true;
                w.Left = 0;
                w.Top = CScript.Top + CScript.Height;
                w.Width = this.Width;
                w.Height = this.Height - w.Top;
                w.Visible = true;
            }
        }
        private void TextChanged(object sender, EventArgs e)
        {
            if (!initializing)
                Line = Line;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            FileInfo fi = new FileInfo(openFileDialog1.FileName);
            CScript.Text = fi.Name;
        }
        private void CScript_TextChanged(object sender, EventArgs e)
        {
            if (w != null)
                w.Navigate(CScript.Text);
            TextChanged(sender, e);

        }
        private void smallToolbox1_ClickTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
                    button1_Click(sender, e);
                    break;
                case 1:
                    button3_Click(sender, e);
                    break;
                case 2:
                    button2_Click(sender, e);
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "MacroTest");
            Directory.CreateDirectory(tempDir);
            string bitmappath = Path.Combine(tempDir, "Bild.png");
            string bitmappathjpg = Path.Combine(tempDir, "Bild.jpg");
            Bitmap bmp = new Bitmap(500, 500);
            Bitmap kennzeichenbmp = new Bitmap(500, 500);
            bmp.Save(bitmappath);
            bmp.Save(bitmappathjpg);
            string ort = COrt.Text; // z. B. "Bonn"
            string template =
            $@"name ""{ort}""
                url ""https://....{ort.ToLower()}.de/""
                start
                sleep 3000
                ";
            string script = template; // z. B. "Bonn"
            DateTime dtm = DateTime.Now;


            if (System.IO.File.Exists(CScript.Text))
                script = System.IO.File.ReadAllText(CScript.Text);

            string result = script;
            result = result.Replace("<mail>", "test@mail.com");
            result = result.Replace("<verstoss>", "TEST_VERSTOSS");
            result = result.Replace("<freetext>", "TEST_FREETEXT");

            // string formatted1 = dtm.ToString("dd.MM.yyyy HH:mm:ss");
            // string formatted2 = dtm.ToString("yyyy-MM-dd");
            // string formatted3 = dtm.ToString("HH:mm");
            // string datum = dtm.AddDays(-1).ToString("dd.MM.yyyy");     // gestern
            // string zeit = dtm.AddHours(-2).ToString("HH:mm");          // 2 Stunden zurück

            result = result.Replace("<datum>", dtm.AddDays(-1).ToString("dd.MM.yyyy"));
            result = result.Replace("<datumDD>", dtm.AddDays(-1).ToString("dd"));
            result = result.Replace("<datumMM>", dtm.AddDays(-1).ToString("MM"));
            result = result.Replace("<datumYYYY>", dtm.AddDays(-1).ToString("yyyy"));

            result = result.Replace("<zeit>", dtm.AddHours(-2).ToString("HH:mm"));
            result = result.Replace("<zeitbis>", dtm.AddHours(-2).AddMinutes(5).ToString("HH:mm"));
            result = result.Replace("<zeithh>", dtm.AddHours(-2).ToString("HH:mm"));
            result = result.Replace("<zeitmm>", dtm.AddHours(-2).ToString("HH:mm"));
            result = result.Replace("<zeitbishh>", dtm.AddHours(-2).AddMinutes(5).ToString("HH:mm"));
            result = result.Replace("<zeitbismm>", dtm.AddHours(-2).AddMinutes(5).ToString("HH:mm"));

            result = result.Replace("<strasse>", "Gerhart-Hauptmann-Platz");
            result = result.Replace("<hausnummer>", "44");
            result = result.Replace("<plz>", CPLZ.Text);
            result = result.Replace("<ort>", COrt.Text);

            result = result.Replace("<marke>", "Audi");
            result = result.Replace("<farbe>", "blau");
            result = result.Replace("<kennzeichen>", "HT-CD123");

            result = result.Replace("<kennzeicheno>", "HT");
            result = result.Replace("<kennzeichenc>", "CD");
            result = result.Replace("<kennzeichenn>", "123");

            result = result.Replace("<kfztyp>", "PKW");
            result = result.Replace("<kfzcountry>", "DE");

            result = result.Replace("<zname>", "Mustermann");
            result = result.Replace("<zvorname>", "Erich");
            result = result.Replace("<zstrasse>", "Meulenstraße");
            result = result.Replace("<zhausnummer>", "1");
            result = result.Replace("<zplz>", "54313");
            result = result.Replace("<zort>", "Zemmer");
            result = result.Replace("<zemail>", "zeuge@test.de");
            result = result.Replace("<zphone>", "040 428990");

            result = result.Replace("<files>", bitmappath);

            Graphics g = Graphics.FromImage(kennzeichenbmp);
            g.DrawString("HT-CD123", new Font("Arial", 36, FontStyle.Bold), Brushes.Black, new PointF(10, 25));
            kennzeichenbmp.Save(bitmappath, System.Drawing.Imaging.ImageFormat.Png);
            g.Dispose(); result = result.Replace("<kennzeichenbild>", "TEST_KENNZEICHENBILD");
            result = result.Replace("<pdffile>", "TEST_PDF");

            result = result.Replace("<zbluetooth>", "BT_TEST");
            result = result.Replace("<zielpfad>", "TEST_ZIELPFAD");

            result = result.Replace("<zvorname>", "erika");
            result = result.Replace("<zsmtpserver>", "smtp.test.de");
            result = result.Replace("<zsmtpport>", "25");
            result = result.Replace("<zsendermail>", "sender@test.de");
            result = result.Replace("<zsubject>", "Anzeige einer Verkehrsordnungswiedrigkeit");
            result = result.Replace("<zpassword>", "TEST_PASS");

            result = result.Replace("<totalimage>", bitmappath);
            result = result.Replace("<totalimagejpg>", bitmappathjpg);

            int imageCount = 8; // Anzahl der Bilder, kann beliebig angepasst werden
            // Ersetze die Platzhalter <image 0> ... <image n> mit existierenden Testbildern
            for (int i = 0; i < imageCount; i++)
            {
                string path = Path.Combine(tempDir, $"Bild{i}.png");

                // Stelle sicher, dass das Bild existiert
                if (!System.IO.File.Exists(path))
                {
                    using (Bitmap bmpImage = new Bitmap(100, 100))
                    using (Graphics gfx = Graphics.FromImage(bmpImage))
                    {
                        gfx.Clear(Color.LightGray);
                        gfx.DrawString($"Bild{i}", new Font("Arial", 12), Brushes.Black, new PointF(10, 40));
                        bmpImage.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                // Platzhalter ersetzen
                result = result.Replace($"<image {i}>", path);
            }

            BrowserControl bc = new BrowserControl();
            bc.ExecuteScript(result);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(CScript.Text))
                button1_Click(sender, e);
            Tools.ShellExecute(IntPtr.Zero, "open", CScript.Text, "", "", 5);
        }
    }
}
