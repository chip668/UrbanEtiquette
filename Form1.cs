﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IronOcr;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;

namespace Anzeige
{
    public partial class Form1 : Form
    {
        String configfile= "config.txt";
        ErrorLogger debug = null;
        Bussgeld verstossbussgeld = new Bussgeld();
        Dictionary<String, Bussgeld> bussgelder = new Dictionary<String, Bussgeld>();
        IronTesseract ocrreader = new IronTesseract();
        Dictionary<String,Ort>Orte = new Dictionary<String, Ort>();
        String plaque = "-aeoSs2B8O0DQCU";
        String numbers = "0123456789";
        String upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ";
        String lower = "abcdefghijklmnopqrstuvwxyzäöüß";
        String spaces = " \t";
        String newline = "\r\n";
        String ortssuche = "https://www.google.com/maps/place/<strasse>+<hn>,+<plz>+<ort>";
        List<Color> refcolor = new List<Color>(); 
        Stack<Cursor> cstack = new Stack<Cursor>();
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        Bitmap original;
        Bitmap ausschnitt;
        Point start;
        Rectangle Bildausschnitt; // clientkoordinaten
        Rectangle bmpAusschnitt;  // bitmapkoordinaten
        String ausschnittTemp="";
        WebBrowser oabrowser;
        int x0 = 0;
        int y0 = 0;
        int w = 0;
        int h = 0;
        private String FullPath
        {
            get
            {
                CreateDirectoryIfNotExists(ZZielpfad);
                CreateDirectoryIfNotExists(ZZielpfad + Ort);
                String fullpath = ZZielpfad + Ort + "\\" + this.Kennzeichen + "\\" + DateTime.Now.ToString("yyyyMMdd"); ;
                CreateDirectoryIfNotExists(fullpath);
                return fullpath;
            }
        }
        public string Template { get; private set; }
        public List<String> _Files = new List<String>();
        public string Files
        {
            get
            {
                String fullpath = ZZielpfad + Ort;
                fullpath = fullpath + "\\" + this.Kennzeichen + "\\" + DateTime.Now.ToString("yyyyMMdd");
                CreateDirectoryIfNotExists(fullpath);
                String result = "";
                if (AddPath)
                {
                    foreach (String i in CFiles.Items)
                    {
                        FileInfo fi = new FileInfo(i);
                        String fullfilename = fullpath + "\\" + fi.Name;
                        result += "\r\n" + fullfilename;
                    }
                }
                return result;
            }
            set
            {
                CFiles.Items.Clear();
                string[] fileLines = value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fileValue in fileLines)
                {
                    CFiles.Items.Add(fileValue.Trim());
                }
            }
        }
        public string FilesOriginal
        {
            get
            {
                String result = "";

                foreach (String i in CFiles.Items)
                {
                    result += "\r\n" + i;
                }
                return result;
            }
            set
            {
                CFiles.Items.Clear();
                string[] fileLines = value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fileValue in fileLines)
                {
                    CFiles.Items.Add(fileValue.Trim());
                }
            }
        }
        public string Datum
        {
            get
            {
                return CDatum.Text;
            }
            set
            {
                CDatum.Text = value;
            }
        }
        public string Zeit
        {
            get
            {
                return CZeit.Text;
            }
            set
            {
                CZeit.Text = value;
            }
        }
        public string Ort
        {
            get
            {
                String[] items = COrt.Text.Split(';');
                if (items.Length > 1)
                {
                    if (PLZ =="")
                    {
                        PLZ = items[0];
                    }
                    Mail = items[2];
                    return items[1];
                }
                return "";
            }
            set
            {
                COrt.Text = value;
            }

        }
        public string PLZ
        {
            get
            {
                return CPLZ.Text;
            }
            set
            {
                CPLZ.Text = value;
            }
        }
        public string URL { get; private set; }
        public string Mail
        {
            get
            {
                return CMail.Text;
            }
            set
            {
                CMail.Text = value;
            }
        }
        public string Strasse
        {
            get
            {
                return CStrasse.Text;
            }
            set
            {
                CStrasse.Text = value;
            }
        }
        public string HN
        {
            get
            {
                return CHN.Text;
            }
            set
            {
                CHN.Text = value;
            }
        }
        public string Kennzeichen
        {
            get
            {
                return CKennzeichen.Text;
            }
            set
            {
                CKennzeichen.Text = value;
            }
        }
        public string Marke
        {
            get
            {
                return CMarke.Text;
            }
            set
            {
                CMarke.Text = value;
            }
        }
        public string Farbe
        {
            get; set;
        }
        public string Verstoss
        {
            get
            {
                String result = "";

                foreach (String i in CVerstoss.Items)
                {
                    result += "\r\n" + i;
                }
                return result;
            }
            set
            {
                CVerstoss.Items.Clear();
                string[] verstossLines = value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string verstossValue in verstossLines)
                {
                    AddVerstoss(verstossValue);
                }
            }

        }
        public string FreeText
        {
            get
            {
                return CFreeText.Text;
            }
            set
            {
                CFreeText.Text = value;
            }
        }
        public string ZName { get; private set; }
        public string ZVorname { get; private set; }
        public string ZOrt { get; private set; }
        public string ZPLZ { get; private set; }
        public string ZStrasse { get; private set; }
        public string ZHausnummer { get; private set; }
        public string ZZielpfad { get; private set; }
        public string GPSLocation { get; private set; }
        public string Message 
        { 
            get 
            {
                String result = Template;
                result = result.Replace("<mail>", Mail);
                result = result.Replace("<verstoss>", Verstoss);
                result = result.Replace("<freetext>", FreeText);
                result = result.Replace("<datum>", Datum);
                result = result.Replace("<zeit>", Zeit);
                result = result.Replace("<strasse>", Strasse);
                result = result.Replace("<hausnummer>", HN);
                result = result.Replace("<plz>", PLZ);
                result = result.Replace("<ort>", Ort);
                result = result.Replace("<marke>", Marke);
                result = result.Replace("<farbe>", Farbe);
                result = result.Replace("<kennzeichen>", Kennzeichen);
                result = result.Replace("<zname>", ZName);
                result = result.Replace("<zvorrname>", ZVorname);
                result = result.Replace("<zstrasse>", ZStrasse);
                result = result.Replace("<zhausnummer>", ZHausnummer);
                result = result.Replace("<zplz>", ZPLZ);
                result = result.Replace("<zort>", ZOrt);
                result = result.Replace("<files>", Files);
                result = result.Replace("<kennzeichenbild>", ausschnittTemp);
                result = result.Replace("<pdffile>", PDFFilename);

                return result;
            }
        }
        public Boolean AddPath
        {
            get { return CAddPath.Checked; }
        }
        public Boolean AddFile
        {
            get { return CAddFile.Checked; }
        }
        public string InitValues
        {
            get
            {
                return
                "Datum=" + Datum +
                "\nZeit=" + Zeit +
                "\nOrt=" + Ort +
                "\nPLZ=" + PLZ +
                "\nStrasse=" + Strasse +
                "\nHausnummer=" + HN +
                "\nKennzeichen=" + Kennzeichen +
                "\nMarke=" + Marke +
                "\nFarbe=" + Farbe +
                "\nVerstoss=" + Verstoss +
                "\nMail=" + Mail +
                "\nName Zeuge=" + ZName +
                "\nVorname Zeuge=" + ZVorname +
                "\nOrt Zeuge=" + ZOrt +
                "\nOPLZ Zeuge=" + ZPLZ +
                "\nStrasse Zeuge=" + ZStrasse +
                "\nHausnummer Zeuge=" + ZHausnummer +
                // "\nMessage=" + Message +
                "\nFiles=" + Files;
            }
            set
            {
                string[] lines = value.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new char[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        string propertyName = parts[0].Trim();
                        string propertyValue = parts[1].Trim();

                        switch (propertyName)
                        {
                            case "Datum":
                                Datum = propertyValue;
                                break;
                            case "Zeit":
                                Zeit = propertyValue;
                                break;
                            case "Ort":
                                Ort = propertyValue;
                                break;
                            case "PLZ":
                                PLZ = propertyValue;
                                break;
                            case "Strasse":
                                Strasse = propertyValue;
                                break;
                            case "Hausnummer":
                                HN = propertyValue;
                                break;
                            case "Kennzeichen":
                                Kennzeichen = propertyValue;
                                break;
                            case "Marke":
                                Marke = propertyValue;
                                break;
                            case "Farbe":
                                Farbe = propertyValue;
                                break;
                            case "Verstoss":
                                Verstoss = propertyValue;
                                break;
                            case "Mail":
                                Mail = propertyValue;
                                break;
                            case "Name Zeuge":
                                ZName = propertyValue;
                                break;
                            case "Vorname Zeuge":
                                ZVorname = propertyValue;
                                break;
                            case "Ort Zeuge":
                                ZOrt = propertyValue;
                                break;
                            case "OPLZ Zeuge":
                                ZPLZ = propertyValue;
                                break;
                            case "Strasse Zeuge":
                                ZStrasse = propertyValue;
                                break;
                            case "Hausnummer Zeuge":
                                ZHausnummer = propertyValue;
                                break;
                            // case "Message":
                            //     Message = propertyValue;
                            //     break;
                            case "Files":
                                Files = propertyValue;
                                break;
                            default:
                                // Optional: Behandlung für unbekannte Eigenschaften
                                break;
                        }
                    }
                }
            }
        }
        public void Init(Boolean verstossonly = false)
        {
            String key = "";
            verstossbussgeld = new Bussgeld();
            List<string> allLines = new List<string>();
            allLines.AddRange(File.ReadAllLines(configfile));
            allLines.AddRange(File.ReadAllLines("Data.txt"));
            string[] lines = allLines.ToArray();

            // String[] lines = File.ReadAllLines("Data.txt");

            if (!verstossonly) COrt.Items.Clear();
            CVerstossaus.Items.Clear();
            if (!verstossonly) CMarke.Items.Clear();
            bussgelder.Clear();
            foreach (String s in lines)
            {
                if (!verstossonly)
                {
                    if (s[0] == '<')
                    {
                        key = s;
                    }
                    else if (key == "<ort>;<mail>")
                    {
                        COrt.Items.Add(s);
                    }
                    else if (key == "<verstoss>")
                    {
                        String[] items = s.Split('|');
                        CVerstossaus.Items.Add(items[0]);
                        if (items.Length>3)
                        {
                            Bussgeld v;

                            switch (items[1])
                            {
                                case "parken":
                                    v = new Bussgeld(true, false, false);
                                    break;
                                case "behinderung":
                                    v = new Bussgeld(false, true, false);
                                    break;
                                case "gefährdung":
                                    v = new Bussgeld(false, false, true);
                                    break;
                                case "x2":
                                    v = new Bussgeld(2);
                                    break;
                                default:
                                    v = new Bussgeld(Convert.ToDouble(items[1]), Convert.ToDouble(items[2]), Convert.ToDouble(items[3]));
                                    break;
                            }
                            v.parken = (items[0].ToLower().Contains("parken"));
                            v.halten = (items[0].ToLower().Contains("halten"));
                            bussgelder.Add(items[0], v);
                        }
                    }
                    else if (key == "<bluetooth>")
                    {
                        listBoxDevices.Text = s;
                    }
                    else if (key == "<marke>")
                    {
                        CMarke.Items.Add(s);
                    }
                    else if (key == "<zname>")
                    {
                        ZName = s;
                    }
                    else if (key == "<zvorname>")
                    {
                        ZVorname = s;
                    }
                    else if (key == "<zstrasse>")
                    {
                        ZStrasse = s;
                    }
                    else if (key == "<zhausnummer>")
                    {
                        ZHausnummer = s;
                    }
                    else if (key == "<zplz>")
                    {
                        ZPLZ = s;
                    }
                    else if (key == "<zort>")
                    {
                        ZOrt = s;
                    }
                    else if (key == "<zielpfad>")
                    {
                        ZZielpfad = s;
                    }
                    else
                    {
                    }
                }
                else if (key == "<verstoss>")
                {
                    CVerstossaus.Items.Add(s);
                }

            }
            Template = File.ReadAllText("mail.txt");
            CAnzeigeText.Text = Message;
            Bildausschnitt = CSave.ClientRectangle;
            oabrowser = new WebBrowser();
            oabrowser.Dock = DockStyle.Fill;
            oabrowser.Navigate("https://www.google.com/");
            oabrowser.ScriptErrorsSuppressed = true;
            PDFFilename = "";
            Farbe = Color.Gold.ToString();
        }
        public string PDFFilename{ get; private set; }
        public Form1()
        {
            InitializeComponent();
            debug = ErrorLoggerMsgBox.Create();
            debug.Add(new ErrorLoggerFile());
            debug.Add(new ErrorLoggerNetMsg());

            Init();
            String[] lines = File.ReadAllLines("ort.txt");

            foreach (String ln in lines)
            {
                Ort o = new Ort(ln);
                if (o.OrtCode!="Ort")
                    Orte.Add(o.OrtCode, o);
            }
            refcolor.Clear();
            foreach (Control i in panel1.Controls)
            {
                refcolor.Add(i.BackColor);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CNew_Click(sender, e);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = ZZielpfad + "Download";
            // ShellExecute(IntPtr.Zero, "open", openFileDialog.InitialDirectory, "", "", 5);
            // Filter für verschiedene Bilddateitypen festlegen
            openFileDialog.Filter = "JPEG-Bilder (*.jpg, *.jpeg)|*.jpg;*.jpeg|" +
                                    "PNG-Bilder (*.png)|*.png|" +
                                    "GIF-Bilder (*.gif)|*.gif|" +
                                    "BMP-Bilder (*.bmp)|*.bmp|" +
                                    "TIFF-Bilder (*.tiff)|*.tiff|" +
                                    "TIFF-Bilder (*.tif)|*.tif|" +
                                    "Alle Dateien (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CFiles.Items.Clear();
                foreach (string fileName in openFileDialog.FileNames)
                {
                    SelectFile(fileName);
                    CFiles.Items.Add(fileName);
                }
            }
            CAnzeigeText.Text = Message;
        }
        private void SelectFile(string fileName)
        {
            PhotoMetadataExtractor data = new PhotoMetadataExtractor(fileName);
            Datum = data.Date;
            Zeit = data.Time;
            try
            {
                original = (Bitmap)Bitmap.FromFile(fileName);
                ausschnitt = original;
                // CFoto.Image = Bitmap.FromFile(fileName);
                CSave.BackgroundImage = ausschnitt;
                if (!data.Valid)
                {
                    GPSLocation = data.GoogleMapsURL;
                    ShellExecute(IntPtr.Zero, "open", GPSLocation, "", "", 5);
                }
                else
                {
                    Strasse = data.Street;
                    HN = data.HouseNumber;
                    PLZ = data.PostalCode;
                    Ort = data.City;
                    String cliptext = data.Street + " " + data.HouseNumber + "," + data.PostalCode + " " + data.City;
                    Clipboard.SetText(cliptext);
                    CClip_Click(this, new EventArgs());
                }
            }
            catch { }
       }
        private void Form1_Load(object sender, EventArgs e)
        {
            listBoxDevices_SelectedIndexChanged(sender, e);
        }
        private void panel_Click(object sender, EventArgs e)
        {
            Panel psender = ((Panel)sender);
            Farbe = (String)(psender.Tag);
            CAnzeigeText.Text = Message;
            panel1.BackColor = psender.BackColor;
            // panel1.BackColor = Color.Gold;
            CAnzeigeText.Text = Message;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel5.BorderStyle = BorderStyle.FixedSingle;
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel7.BorderStyle = BorderStyle.FixedSingle;
            panel8.BorderStyle = BorderStyle.FixedSingle;
            panel9.BorderStyle = BorderStyle.FixedSingle;
            panel10.BorderStyle = BorderStyle.FixedSingle;
            panel11.BorderStyle = BorderStyle.FixedSingle;
            panel12.BorderStyle = BorderStyle.FixedSingle;
            psender.BorderStyle = BorderStyle.Fixed3D;
            panel1.BorderStyle = BorderStyle.Fixed3D;
        }
        private void CBackall_Click(object sender, EventArgs e)
        {
            CVerstossaus.Items.Clear();
            foreach (String i in CVerstoss.Items)
            {
                AddVerstoss(i);
            }
            CVerstoss.Items.Clear();
            CAnzeigeText.Text = Message;
        }
        private void CBack_Click(object sender, EventArgs e)
        {
            if (CVerstoss.SelectedItem != null)
            {

                CVerstossaus.Items.Add(CVerstoss.SelectedItem);
                RemoveVerstoss((String)CVerstoss.SelectedItem);
            }
            CAnzeigeText.Text = Message;
        }
        private void RemoveVerstoss(String verstoss)
        {
            CVerstoss.Items.Remove(verstoss.Trim());
            verstossbussgeld = new Bussgeld();
            foreach (String i in CVerstoss.Items)
                AddBussgeld(i);
            bussgeldrechner1.bussgeld = verstossbussgeld;
        }
        private void AddBussgeld (String verstoss)
        {
            Bussgeld v = bussgelder[verstoss];
            if (v.verstoss != 0) verstossbussgeld.verstoss = Math.Min(v.verstoss, verstossbussgeld.verstoss);
            if (v.behinderung != 0) verstossbussgeld.behinderung = Math.Min(v.behinderung, verstossbussgeld.behinderung);
            if (v.gefaerdung != 0) verstossbussgeld.gefaerdung = Math.Min(v.gefaerdung, verstossbussgeld.gefaerdung);
            if (v.parken) verstossbussgeld.parken = true;
            if (v.faktor > 1) verstossbussgeld.faktor = v.faktor;
            if (v.mitbehinderung) verstossbussgeld.mitbehinderung = v.mitbehinderung;
            if (v.mitgefaerdung) verstossbussgeld.mitgefaerdung = v.mitgefaerdung;
            if (v.p1 > 0) verstossbussgeld.p1 = v.p1;
            if (v.p2 > 0) verstossbussgeld.p2 = v.p2;
            if (v.p3 > 0) verstossbussgeld.p3 = v.p3;
        }
        private void AddVerstoss(String verstoss)
        {
            CVerstoss.Items.Add(verstoss.Trim());
            AddBussgeld(verstoss);
            bussgeldrechner1.bussgeld = verstossbussgeld;
        }
        private void CToo_Click(object sender, EventArgs e)
        {
            if (CVerstossaus.SelectedItem!=null)
            {
                AddVerstoss((String)CVerstossaus.SelectedItem);
                CVerstossaus.Items.Remove(CVerstossaus.SelectedItem);
            }
            CAnzeigeText.Text = Message;
        }
        private void Ctoall_Click(object sender, EventArgs e)
        {
            CVerstoss.Items.Clear();
            foreach (String i in CVerstossaus.Items)
            {
                // CVerstoss.Items.Add(i);
                AddVerstoss(i);
            }
            CVerstossaus.Items.Clear();
            CAnzeigeText.Text = Message;
        }
        void setSelectedLineTip (Control ctl)
        {
            toolTip1.SetToolTip(ctl, ctl.Text);
        }
        private void CVerstossaus_DoubleClick(object sender, EventArgs e)
        {
            CToo_Click(sender, e);
        }
        private void CVerstoss_DoubleClick(object sender, EventArgs e)
        {
            CBack_Click(sender, e);
        }
        private void CVerstossaus_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSelectedLineTip((Control)sender);
        }
        private void CVerstoss_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSelectedLineTip((Control)sender);
        }
        private void CMail_TextChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void CPLZ_TextChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void COrt_SelectedIndexChanged(object sender, EventArgs e)
        {
            String[] items = COrt.Text.Split(';');
            selectOrt(items[1]);
            CAnzeigeText.Text = Message;
        }
        private void CMarke_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSelectedLineTip((Control)sender);
            try
            {
                CLogo.BackgroundImage = Bitmap.FromFile(CMarke.Text + ".jpg");
                CAnzeigeText.Text = Message;
            }
            catch { }
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }
        }
        private Boolean pruefeDaten()
        {
            if (Files =="") { MessageBox.Show("Bitte Foto wählen"); return false; }
            if (Datum =="") { MessageBox.Show("Datum des Vorfalls"); return false; }
            if (Zeit =="") { MessageBox.Show("Zeit des Vorfalls"); return false; }
            if (Ort =="") { MessageBox.Show("In welchem Ort hat der Verstoß statt gefunden"); return false; }
            if (PLZ =="") { MessageBox.Show("Wie lautet die PLZ"); return false; }
            if (Strasse =="") { MessageBox.Show("Auf welcher Strasse hat der Verstoß statt gefunden"); return false; }
            if (HN =="") { MessageBox.Show("An welcher Hausnummer hat der Verstoß statt gefunden"); return false; }
            if (Kennzeichen =="") { MessageBox.Show("Wie lautet das Kennzeichen"); return false; }
            if (Marke =="") { MessageBox.Show("Welche Automarke hatte das Fahrzeug"); return false; }
            if (Farbe == Color.Gold.ToString()) { MessageBox.Show("Welche Farbe hatte das Fahrzeug"); return false; }
            if ((Verstoss =="") && (FreeText=="")) { MessageBox.Show("Welcher Verstoß"); return false; }
            if (Mail =="") { MessageBox.Show("Wohin soll ich die Mail senden"); return false; }
            if (ZName =="") { MessageBox.Show("Wie lautet dein Name"); return false; }
            if (ZVorname =="") { MessageBox.Show("Wie lautet dein Vorname"); return false; }
            if (ZOrt =="") { MessageBox.Show("An welchem Ort wohnst du"); return false; }
            if (ZPLZ =="") { MessageBox.Show("Wie lautet die PLZ deines Wohnortes"); return false; }
            if (ZStrasse =="") { MessageBox.Show("Auf welcher Strasse wohnst du"); return false; }
            if (ZHausnummer =="") { MessageBox.Show("Wie lautet die Hausnummer deiner Wohnung"); return false; }
            if (Message == "") { MessageBox.Show("Text benötigt"); return false; }
            if (AreColorsEqual(SystemColors.Control, panel1.BackColor)) { MessageBox.Show("Bitte Farbe auswählen."); return false; }
            return true;
        }
        static bool AreColorsEqual(Color color1, Color color2)
        {
            return color1.A == color2.A &&
                   color1.R == color2.R &&
                   color1.G == color2.G &&
                   color1.B == color2.B;
        }
        public static Bitmap CropBitmap(Bitmap sourceBitmap, Rectangle cropRectangle)
        {
            if (sourceBitmap == null)
                throw new ArgumentNullException(nameof(sourceBitmap));

            if (cropRectangle.Left < 0 || cropRectangle.Top < 0 ||
                cropRectangle.Right > sourceBitmap.Width || cropRectangle.Bottom > sourceBitmap.Height)
                throw new ArgumentException("The crop rectangle is outside the bounds of the source bitmap.");

            // Erstelle ein neues Bitmap mit den Abmessungen des Ausschnitts
            Bitmap croppedBitmap = new Bitmap(cropRectangle.Width, cropRectangle.Height);

            // Erstelle einen Graphics-Objekt, um den Ausschnitt zu zeichnen
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                // Definiere den Ausschnitt im Zielbitmap
                Rectangle destinationRect = new Rectangle(0, 0, cropRectangle.Width, cropRectangle.Height);

                // Kopiere den Ausschnitt vom Quellbitmap in das Zielbitmap
                g.DrawImage(sourceBitmap, destinationRect, cropRectangle, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }
        public double ColorDistance(Color a, Color b)
        {
            double brightnessA = a.GetBrightness();
            double brightnessB = b.GetBrightness();

            double normalizedR_A = a.R / brightnessA;
            double normalizedG_A = a.G / brightnessA;
            double normalizedB_A = a.B / brightnessA;

            double normalizedR_B = b.R / brightnessB;
            double normalizedG_B = b.G / brightnessB;
            double normalizedB_B = b.B / brightnessB;

            double diffR = Math.Abs(normalizedR_A - normalizedR_B);
            double diffG = Math.Abs(normalizedG_A - normalizedG_B);
            double diffB = Math.Abs(normalizedB_A - normalizedB_B);

            return diffR + diffG + diffB;
        }
        private void CFoto_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                start = e.Location;
            } else if (e.Button == MouseButtons.Right)
            {
                Point p = Transform(e.Location, CSave.ClientRectangle, ausschnitt.Size);
                Bitmap b = (Bitmap)CSave.BackgroundImage;
                Color c = b.GetPixel(p.X, p.Y);
                panel1.BackColor = c;
                Color clickcolor = c;
                c = refcolor[0];
                double mxdiff = double.MaxValue;
                foreach (Color i in refcolor)
                {
                    double d = ColorDistance(i, panel1.BackColor);
                    if (d < mxdiff)
                    {
                        mxdiff = d;
                        panel1.BackColor = i;
                    }
                        
                }
            }
        }
        private static String ToRGB(System.Drawing.Color c)
    => $"RGB({c.R},{c.G},{c.B})";
        private void CFoto_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ausschnitt!=null)
                {
                    try
                    {
                        Point p = Transform(e.Location, CSave.ClientRectangle, ausschnitt.Size);
                        Bitmap b = (Bitmap)CSave.BackgroundImage;
                        Color c = b.GetPixel(p.X, p.Y);
                        this.Text = "Wegeheld 2 |" + ToRGB(c) + " | " + ToRGB(panel1.BackColor);
                        Bildausschnitt = new Rectangle(start, new Size(e.X - start.X, e.Y - start.Y));
                    }
                    catch { }
                }
            }
        }
        private void BlackOutRegion(Bitmap bitmap, Rectangle region)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(brush, region);
                }
            }
        }
        private void CFoto_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    if (ausschnitt != null)
                    {
                        Rectangle rcl = Transform(Bildausschnitt, CSave.ClientRectangle, ausschnitt.Size);
                        Bitmap tempausschnitt = CropRectangleFromBitmap(ausschnitt, bmpAusschnitt); ;
                        CAusschnitt.BackgroundImage = tempausschnitt;
                        CAusschnitt.Show();
                        
                        ausschnittTemp = (AddPath) ? Path.GetTempFileName().Replace(".tmp", ".jpg") : null;
                        if (tempausschnitt != null)
                        {
                            ScaledSave(tempausschnitt, ausschnittTemp, 3);
                            // tempausschnitt.Save(ausschnittTemp, ImageFormat.Jpeg);
                            // Clipboard.SetText(ausschnittTemp);

                            // Hier wird eine Kopie der Originaldatei erstellt
                            Bitmap kopie = new Bitmap(ausschnitt);
                            // Den ausgeschnittenen Bereich in der Kopie schwarz färben
                            BlackOutRegion(kopie, bmpAusschnitt);
                            // Den Dateipfad für die geschwärzte Kopie festlegen
                            // string geschwaerzteKopiePfad = Path.GetTempFileName().Replace(".tmp", ".jpg");
                            string geschwaerzteKopiePfad = ZZielpfad + "public\\" + Guid.NewGuid().ToString() + ".jpg"; ;
                            
                            // Die geschwärzte Kopie speichern
                            kopie.Save(geschwaerzteKopiePfad, ImageFormat.Jpeg);
                            // Jetzt haben Sie die geschwärzte Kopie in der Variable 'geschwaerzteKopiePfad'
                        }
                        if (CKennzeichen.Text == "")
                        {
                            CKennzeichen.Text = ReadTextFromBitmap(tempausschnitt);
                        }
                        CAusschnitt.Refresh();
                    }
                }
                catch { }
            }
        }
        private Rectangle Transform(Rectangle bildausschnitt, Control clientControl, Rectangle ausschnitt)
        {
            return Transform(Bildausschnitt, clientControl.ClientRectangle, ausschnitt.Size);
        }
        private Rectangle Transform(Rectangle bildausschnitt, Rectangle clientRectangle, Size size)
        {
            Double faktor = 1;
            Rectangle result = clientRectangle;
            if ((Double)clientRectangle.Width/ size.Width < (Double)clientRectangle.Height / size.Height)
            {
                faktor = (Double)size.Width / clientRectangle.Width;
                x0 = 0;
                y0 = (clientRectangle.Height - (int)(size.Height / faktor))/2;
            }
            else
            {
                faktor = (Double)size.Height / clientRectangle.Height;
                x0 =( clientRectangle.Width - (int)(size.Width / faktor))/2;
                y0 = 0;
            }
            h = (int)(size.Height / faktor);
            w = (int)(size.Width / faktor);
            bmpAusschnitt = new Rectangle(
                (int)((Bildausschnitt.X-x0) * faktor), 
                (int)((Bildausschnitt.Y-y0) * faktor), 
                (int)(Bildausschnitt.Width * faktor), 
                (int)(Bildausschnitt.Height * faktor));

            // Rectangle Bildausschnitt; // clientkoordinaten
            // Rectangle bmpAusschnitt;  // bitmapkoordinaten
            // ausschnitt = CropRectangleFromBitmap(ausschnitt, bmpAusschnitt);
            DrawRectangleAndCopyToClipboard(ausschnitt, bmpAusschnitt);
            CSave.Refresh();

            // Rectangle Bildausschnitt; // clientkoordinaten
            // Rectangle bmpAusschnitt;  // bitmapkoordinaten
            // DrawRectangleAndCopyToClipboard(ausschnitt, bmpAusschnitt);
            return result;
        }
        private Point Transform(Point point, Rectangle clientRectangle, Size size)
        {
            Double faktor = 1;
            Point result = point;
            int x0 = 0;
            int y0 = 0;

            if ((Double)clientRectangle.Width / size.Width < (Double)clientRectangle.Height / size.Height)
            {
                faktor = (Double)size.Width / clientRectangle.Width;
                x0 = 0;
                y0 = (clientRectangle.Height - (int)(size.Height / faktor)) / 2;
            }
            else
            {
                faktor = (Double)size.Height / clientRectangle.Height;
                x0 = (clientRectangle.Width - (int)(size.Width / faktor)) / 2;
                y0 = 0;
            }

            int x = (int)((point.X - x0) * faktor);
            int y = (int)((point.Y - y0) * faktor);

            return new Point(x, y);
        }
        private void CFotoAnzeige_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rcl = new Rectangle(x0, y0, w, h);

            e.Graphics.DrawRectangle(new Pen(Color.Green), rcl);
            e.Graphics.DrawRectangle(new Pen(Color.Red), Bildausschnitt);

        }
        public Bitmap CropRectangleFromBitmap(Bitmap sourceBitmap, Rectangle rectangle)
        {
            try
            {
                if (sourceBitmap == null)
                {
                    throw new ArgumentNullException(nameof(sourceBitmap), "Quell-Bitmap darf nicht null sein.");
                }
                Bitmap croppedBitmap = new Bitmap(rectangle.Width, rectangle.Height);
                using (Graphics graphics = Graphics.FromImage(croppedBitmap))
                {
                    graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, rectangle.Width, rectangle.Height), rectangle, GraphicsUnit.Pixel);
                }
                return croppedBitmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void DrawRectangleAndCopyToClipboard(Bitmap sourceBitmap, Rectangle rectangle)
        {
            try
            {
                if (sourceBitmap == null)
                {
                    throw new ArgumentNullException(nameof(sourceBitmap), "Quell-Bitmap darf nicht null sein.");
                }
                using (Bitmap copiedBitmap = (Bitmap)sourceBitmap.Clone())
                {
                    using (Graphics graphics = Graphics.FromImage(copiedBitmap))
                    {
                        using (Pen pen = new Pen(Color.Red, 3))
                        {
                            graphics.DrawRectangle(pen, rectangle);
                        }
                    }
                    string tempFilePath = Path.GetTempFileName();
                    copiedBitmap.Save(tempFilePath, ImageFormat.Png);
                    // Clipboard.SetText(tempFilePath);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public Bitmap CropRectangleFromBitmap_x(Bitmap sourceBitmap, Rectangle sourceRectangle)
        {
            // Überprüfen, ob das übergebene Rechteck innerhalb der Bitmap-Grenzen liegt
            if (!IsRectangleInsideBitmap(sourceBitmap, sourceRectangle))
            {
                throw new ArgumentException("Das Rechteck liegt außerhalb der Bitmap-Grenzen.");
            }

            // Erstellen Sie einen Bitmap-Ausschnitt des übergebenen Rechtecks
            Bitmap croppedBitmap = new Bitmap(sourceRectangle.Width, sourceRectangle.Height);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(sourceBitmap, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height), sourceRectangle, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }
        private static bool IsRectangleInsideBitmap(Bitmap bitmap, Rectangle rect)
        {
            return rect.X >= 0 && rect.Y >= 0 && rect.Right <= bitmap.Width && rect.Bottom <= bitmap.Height;
        }
        private void selectOrt(String ort) 
        {
            {
                String[] items = ort.Split('(');
                ort = items[0].Trim();                
            }
            foreach (String i in COrt.Items)
            {
                if (i.Contains(ort))
                {
                    COrt.SelectedValue = i;
                    COrt.Text = i;
                    String[] items = i.Split(';');

                    if (CPLZ.Text != "")
                    {
                        CPLZ.Text = items[0];
                    }
                    CMail.Text = items[2];
                    URL = items[3];
                    return;
                }
            }
        }
        private void CClip_Click(object sender, EventArgs e)
        {
            // Oberbilker Allee 98, 40227 Düsseldorf
            String [] items = Clipboard.GetText().Split(',');

            if (items.Length==2)
            {
                String plz = items[1].Substring(0, 6);
                String ort = items[1].Substring(7);
                selectOrt(ort);
                string fullAddress = items[0];
                string street = string.Empty;
                string houseNumber = string.Empty;



                // Suche nach dem letzten Leerzeichen, um die Hausnummer zu trennen
                int lastSpaceIndex = fullAddress.LastIndexOf(' ');
                if (lastSpaceIndex != -1 && lastSpaceIndex < fullAddress.Length - 1)
                {
                    street = fullAddress.Substring(0, lastSpaceIndex).Trim();
                    houseNumber = fullAddress.Substring(lastSpaceIndex + 1).Trim();
                }
                else
                {
                    // Wenn kein Leerzeichen gefunden wurde, verwenden wir die gesamte Eingabe als Straße
                    street = fullAddress.Trim();
                }
                CPLZ.Text = plz;
                CStrasse.Text = street;
                CHN.Text = houseNumber;
                CMail.Text = Mail;
                // CTabPages.SelectedTab = CTabPageOA;
            }
        }
        private void CFiles_DoubleClick(object sender, EventArgs e)
        {
            SelectFile(CFiles.Text);
        }
        private void CNew_Click(object sender, EventArgs e)
        {
            ausschnittTemp = "";
            CStrasse.Text = "";
            CHN.Text = "";
            CMarke.Text = "";
            CVerstoss.Items.Clear();
            CFiles.Items.Clear();
            panel_Click(panel4, e);
            CSave.BackgroundImage = null;
            CDatum.Text = "";
            CZeit.Text = "";
            CAusschnitt.Hide();
            CKennzeichen.Text = "";
            CLogo.BackgroundImage = null;
            CFreeText.Text = "";
            panel1.BackColor = Color.Gold;
            Init();
        }
        private void CKennzeichen_TextChanged(object sender, EventArgs e)
        {
            if (COrt.Text =="")
            {
                String ortsname;
                CAnzeigeText.Text = Message;
                String[] items = CKennzeichen.Text.Split(' ');
                ortsname = FindOrtName(items[0]);
                selectOrt(ortsname);
                // String[] items = COrt.Text.Split(';');
                // CAnzeigeText.Text = Message;
            }
        }
        private void CStrasse_TextChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void CHN_TextChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        public static void SendeEmailMitAnhaengen(string empfaenger, string betreff, string text, List<String> anhangDateiPfade)
        {
            SendeEmailMitAnhaengen(empfaenger, betreff, text, anhangDateiPfade.ToArray());
        }
/*
        public static void SendEmailWithAttachments(string recipient, string subject, string body, params string[] attachmentPaths)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("your-email@example.com");
                    mailMessage.To.Add(recipient);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    foreach (string attachmentPath in attachmentPaths)
                    {
                        if (System.IO.File.Exists(attachmentPath))
                        {
                            Attachment attachment = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                            mailMessage.Attachments.Add(attachment);
                        }
                    }

                    using (SmtpClient smtpClient = new SmtpClient("your-smtp-server.com"))
                    {
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new NetworkCredential("your-username", "your-password");
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
*/
        public static void SendeEmailMitAnhaengen(string empfaenger, string betreff, string text, params string[] anhangDateiPfade)
        {
            try
            {
                string mailtoUri = $"mailto:{empfaenger}?subject={Uri.EscapeDataString(betreff)}&body={Uri.EscapeDataString(text)}";

                if (anhangDateiPfade != null && anhangDateiPfade.Length > 0)
                {
                    
                    foreach (string anhangDateiPfad in anhangDateiPfade)
                    {
                        String att = anhangDateiPfad.Replace("//", "\\");
                        att = att.Replace("\\\\", "\\");
                        if (System.IO.File.Exists(att))
                        {
                            mailtoUri += $"&attachment=\"{att}\"";
                        }
                    }
                }

                ShellExecute(IntPtr.Zero, "open", mailtoUri, null, null, 1);
            }
            catch (Exception ex)
            {
            }
        }
        private void CFotoAnzeige_Click(object sender, EventArgs e)
        {
            
        }
        private void CFotoAnzeige_Resize(object sender, EventArgs e)
        {
            if (ausschnitt!=null)
            {
                Rectangle rcl = Transform(Bildausschnitt, CSave.ClientRectangle, ausschnitt.Size);
            }
            CAusschnitt.Height = CSave.Height / 2;
        }
        private void CFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSelectedLineTip((Control)sender);
            ausschnittTemp = "";
            CAusschnitt.Hide();
        }
        private void CTabPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            CStadtPate.Controls.Clear();
            CTabPageOA.Controls.Clear();
            CWeglide.Controls.Clear();
            CGMaps.Controls.Clear();
            if (CTabPages.SelectedTab == CTabPageOA)
            {
                oabrowser.Navigate(URL);
                CTabPageOA.Controls.Add(oabrowser);
            }
            else if (CTabPages.SelectedTab == CStadtPate)
            {
                oabrowser.Navigate("https://stadtpate.de/<ort>/OWI".Replace("<ort>", Ort));
                CStadtPate.Controls.Add(oabrowser);
            }
            else if (CTabPages.SelectedTab == CWeglide)
            {
                oabrowser.Navigate("https://weg-li.de".Replace("<ort>", Ort));
                CWeglide.Controls.Add(oabrowser);
            }
            else if (CTabPages.SelectedTab == CGMaps)
            {
                if (GPSLocation=="")
                {
                    GPSLocation = "https://www.google.de/maps";
                }
                // oabrowser.Navigate(GPSLocation);
                // CGMaps.Controls.Add(oabrowser);
                ShellExecute(IntPtr.Zero, "open", GPSLocation, "", "", 5);
            }
            else if (CTabPages.SelectedTab == CTest)
            {
                // TestOCRFIX();
            }
            else if (CTabPages.SelectedTab == CAbout)
            {
                AboutBox1 dlg = new AboutBox1();
                dlg.ShowDialog();
            }
        }
        private void CAusschnitt_Click(object sender, EventArgs e)
        {
            CAusschnitt.Hide();

        }
        public Boolean isAlNum(char c)
        {
            return isNumeric(c)|| isAlpha(c);
        }
        public Boolean isAlpha(char c)
        {
            return isContent(c, upper);
        }
        public Boolean isLowerAlpha(char c)
        {
            return isContent(c, lower);
        }       
        public Boolean isNumeric(char c)
        {
            return isContent(c, numbers);
        }
        public Boolean isSpace(char c)
        {
            return isContent(c, spaces);
        }
        public Boolean isNewline(char c)
        {
            return isContent(c, newline);
        }
        public Boolean isPlaque(char c)
        {
            return isContent(c, plaque);
        }
        public Boolean isContent(char c, String cont)
        {
            return cont.Contains(c);
        }
        public String ReplaceCharAtIndex(string originalString, int position, char newCharacter)
        {
            if (position >= 0 && position < originalString.Length)
            {
                char[] charArray = originalString.ToCharArray();
                charArray[position] = newCharacter;
                return new string(charArray);
            }
            else
            {
                throw new ArgumentOutOfRangeException("position", "Ungültige Position.");
            }
        }
        char MakeNumberToChar(String original, int i)
        {
            char result = original[i];
            // "O" (Buchstabe) und "0" (Zahl)
            // "B"(Buchstabe) und "8"(Zahl)
            // "l"(kleines L) und "1"(Zahl eins)
            // "I"(großes i) und "1"(Zahl eins)
            // "Z"(Buchstabe) und "2"(Zahl)
            // "S"(Buchstabe) und "5"(Zahl)
            if (result == '0')
            {
                result = 'D';
            }
            else if (result == '8')
            {
                result = 'B';
            }
            else if (result == '1')
            {
                result = 'L';
            }
            else if (result == '2')
            {
                result = 'Z';
            }
            else if (result == '5')
            {
                result = 'S';
            }
            return result;
        }
        // "B D-B 2265 §"
        // "rr — el\r\nDB 2265 9"
        // "eS\r\n\r\nDB 2265"
        // "Se\r\ni 0-8 2265 J"
        // "——\r\nDB 2265"
        // "ao\r\ni DB 2265 §"
        // "a — il\r\n5 D-B 2265"
        // "a\r\nf DB 2265"
        // "————\r\nA DB 2265"
        public String TestOutput (String text)
        {
            return (text + " => \t:" + FixOCRText(text)).Replace("\r", "\\r").Replace("\n", "\\n") + "\r\n";
        }
        public void TestOCRFIX()
        {
            String test = "";
            // 
            test += TestOutput("ee\r\nB D2B 2265");
            test += TestOutput("eS\r\n\r\nDB 2265");
            test += TestOutput("——\r\nDB 2265");
            test += TestOutput("ao\r\ni DB 2265 §");
            test += TestOutput("a — il\r\n5 D-B 2265");
            test += TestOutput("a\r\nf DB 2265");
            test += TestOutput("————\r\nA DB 2265");
            test += TestOutput("Se\r\ni 0-8 2265 J");
            test += TestOutput("B D-B 2265 §");
            test += TestOutput("rr — el\r\nDB 2265 9");
            CTestText.Text = test;
        }
        public string FixOCRText2(string original)
        {
            // Ort extrahieren (der Ort besteht aus Großbuchstaben und Leerzeichen)
            Match ortMatch = Regex.Match(original, @"\b[A-Z]+\b");
            string ort = ortMatch.Success ? ortMatch.Value : "";

            // Buchstaben extrahieren (nur Großbuchstaben, bis zu 3 Buchstaben)
            Match buchstabenMatch = Regex.Match(original, @"\b[A-Z]{1,3}\b");
            string buchstaben = buchstabenMatch.Success ? buchstabenMatch.Value : "";

            // Zahlen extrahieren (nur bis zu 4 Ziffern)
            Match zahlenMatch = Regex.Match(original, @"\b\d{1,4}\b");
            string zahlen = zahlenMatch.Success ? zahlenMatch.Value : "";

            // Wenn Ort und Buchstaben gefunden wurden, wird das Kennzeichen als erkannt betrachtet
            if (!string.IsNullOrEmpty(ort) && !string.IsNullOrEmpty(buchstaben) && zahlen.Length > 0)
            {
                return ort + " " + buchstaben + " " + zahlen;
            }
            else
            {
                // Ort im Buchstaben-Teil suchen
                string gefundenerOrt = FindOrt(buchstaben);
                buchstaben = buchstaben.Replace(gefundenerOrt, "").Trim();
                return gefundenerOrt + " " + buchstaben + " " + zahlen;
            }
        }
        public string FixOCRText(String original)
        {
            String result = "";
            String numbers = "";
            String letters = "";
            String ort = "";
            int i = original.Length - 1;
            int Status = 1;
            char m;
            int j = 0; // lastblock

            while (i >= 0)
            {
                m = original[i];
                switch (Status)
                {
                    case 1:
                        if (isNumeric(m))
                        {
                            Status = 2;
                            numbers = m.ToString() + numbers;
                        }
                        break;

                    case 2:
                        j = i;
                        if (isNumeric(m) && numbers.Length<4)
                        {
                            numbers = m.ToString() + numbers;
                        }
                        else
                        {
                            if (!isSpace(m)) // nur mit einem Buchstaben zu 3
                            {
                                m = MakeNumberToChar(original, i);
                                if (isAlpha (m))
                                {
                                    letters = m.ToString() + letters;
                                    Status = 3;
                                }
                            }
                            else
                            {
                                Status = 3;
                                j = i;
                            }
                        }
                        break;

                    case 3:
                        {
                            m = MakeNumberToChar(original, i);
                            if (isNumeric(m))
                            {
                                Status = 2;
                                numbers = m.ToString() + numbers;
                                i = j;
                                numbers = "";
                                letters = "";
                            }
                            else if (isAlpha(m) && (letters.Length<3))
                            {
                                letters = m.ToString() + letters;
                            }
                            else if (isSpace(m) || isPlaque(m) || (i == 0))
                            {
                                Status = 4;
                            } 
                            else 
                            {
                                Status = 4;
                            }
                        }
                    break;

                    case 4:
                        {
                            m = MakeNumberToChar(original, i);
                            if (isAlpha(m))
                            {
                                ort = m.ToString() + ort;
                            }
                            else
                            {
                                Status = 5;
                            }
                        }
                        break;


                    case 5:
                        {
                            i=0;
                        }
                        break;
                } 
                i--;
            }

            // String o2 = FindOrt(ort);
            if (ort.Length == 0 || ort != FindOrt(ort))
            {
                ort = FindOrt(letters);
                letters = letters.Substring(ort.Length);
            }
            else
            {

            }
            return (ort + " " + letters + " " + numbers).Trim();
            // return original;
        }
        private Ort FindOrtObjekt(string letters)
        {
            Ort result = null;
            String key = "";
            int i = 1;
            while (result == null)
            {
                key = letters.Substring(0, i);
                try
                {
                    result = Orte[key];
                }
                catch (Exception e)
                {
                }
            }

            if (result.OrtCode.Length == 0)
            {
                // hier die orte Suchen die nicht namentlich zu finden sind sondern
                // mit ähnlichen Buchstaben zu binden sind.
            }
            return result;
        }
        private string FindOrt(string letters)
        {
            String result = "";
            Ort o = FindOrtObjekt(letters);
            if (o != null)
                result = o.OrtCode;
            return result;
        }
        private string FindOrtName(string letters)
        {
            String result = "";
            Ort o = FindOrtObjekt(letters);
            if (o != null)
                result = o.Name;
            return result;
        }
        Dictionary<char, List<char>> similarLetters = new Dictionary<char, List<char>>()
        {
            { 'A', new List<char>(){ 'A', 'H', 'K', 'Ä', '4' } },
            { 'B', new List<char>(){ 'B', 'E', 'F', '3', '8' } },
            { 'C', new List<char>(){ 'C', 'G', 'O', 'Q', 'Ö', '0', 'D', 'Ü', 'U', '6', '9' } },
            { 'I', new List<char>(){ 'I', 'J', 'L', 'T', '1', '7' } },
            { 'M', new List<char>(){ 'M', 'N', 'W' } },
            { 'P', new List<char>(){ 'P', 'R' } },
            { 'S', new List<char>(){ 'S', '5' } }
        };
        private string FindOrtX(string letters)
        {
            String result = "";
            String key = "";
            int i = 1;
            result = FindOrt(letters);
            if (result != "")
                return result;
            while (result == "")
            {
                key = letters.Substring(0, i);
                try
                {
                    result = Orte[key].OrtCode;
                }
                catch (Exception e)
                {
                }
                i++;
            }

            if (result.Length == 0)
            {
                // Teste alle möglichen Kombinationen ähnlicher Buchstaben
                List<char> possibleLetters = new List<char>();
                foreach (char letter in key)
                {
                    if (similarLetters.ContainsKey(letter))
                    {
                        possibleLetters.AddRange(similarLetters[letter]);
                    }
                    else
                    {
                        possibleLetters.Add(letter);
                    }
                }

                foreach (var permutation in GetAllPermutations(possibleLetters))
                {
                    string permKey = new string(permutation.ToArray());
                    try
                    {
                        result = Orte[permKey].OrtCode;
                        break;
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return result;
        }
        // Funktion, um alle Permutationen einer Liste von Buchstaben zu erhalten
        private List<List<char>> GetAllPermutations(List<char> letters)
        {
            List<List<char>> result = new List<List<char>>();
            if (letters.Count == 1)
            {
                result.Add(new List<char> { letters[0] });
                return result;
            }

            foreach (char letter in letters)
            {
                List<char> remainingLetters = new List<char>(letters);
                remainingLetters.Remove(letter);
                List<List<char>> permutations = GetAllPermutations(remainingLetters);
                foreach (var permutation in permutations)
                {
                    permutation.Insert(0, letter);
                    result.Add(permutation);
                }
            }
            return result;
        }
        public string FixOCRText_old(String original)
        {
            String result = "";
            String numbers = "";
            String letters = "";
            String ort = "";
            int i = original.Length - 1;
            int Status = 0;
            while (i >= 0)
            {
                switch (Status)
                {
                    case 0:
                        if (isNumeric(original[i]))
                        {
                            Status++;
                            numbers = original[i].ToString() + numbers;
                        }
                        break;

                    case 1:
                        if (isNumeric(original[i]) && numbers.Length < 4)
                        {
                            numbers = original[i].ToString() + numbers;
                        }
                        else if (isSpace(original[i]))
                        {
                            Status++;
                        }
                        else if (isAlpha(original[i]))
                        {
                            numbers = original[i].ToString() + numbers;
                            Status++;
                        }
                        break;

                    case 2:
                        {
                            // original = MakeNumberToChar(original, i);
                            if (isNumeric(original[i]))
                            {
                                result = original[i].ToString();
                                Status = 1;
                            }
                            else if (isSpace(original[i]) || isPlaque(original[i]))
                            {
                                result = " " + result;
                                Status = 4;
                            }
                            else if (isAlpha(original[i]))
                            {
                                letters = original[i].ToString() + letters;
                            }
                            else if (isNewline(original[i]) || i == 0)
                            {
                                i = 0;
                            }
                        }
                        break;

                    case 3:
                        if (isSpace(original[i]) || isPlaque(original[i]))
                        {
                            result = " " + result;
                            Status = 3;
                        }
                        else if (isNewline(original[i]) || i == 0)
                        {
                            i = 0;
                        }
                        else if (isNumeric(original[i]))
                        {
                            Status++;
                            ort = original[i].ToString() + ort;
                        }
                        break;
                }
                i--;
            }
            // return ort + " " + letters + " " + numbers;
            return original;
        }
        public string ReadTextFromBitmap(Bitmap image)
        {
            String original = ocrreader.Read(image).Text;
            return FixOCRText(original);
        }
        public Bitmap ConvertToBlackAndWhite(Bitmap originalBitmap)
        {
            Bitmap bwBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);
            for (int x = 0; x < originalBitmap.Width; x++)
            {
                for (int y = 0; y < originalBitmap.Height; y++)
                {
                    Color originalColor = originalBitmap.GetPixel(x, y);
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                    Color newColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    bwBitmap.SetPixel(x, y, newColor);
                }
            }
            return bwBitmap;
        }
        private void CLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WH2-Dateien|*.WH2|Alle Dateien|*.*";
            openFileDialog.InitialDirectory = ZZielpfad;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string selectedFilePath = openFileDialog.FileName;

                    InitValues = File.ReadAllText(selectedFilePath);
                }
                catch (Exception ex)
                {
                }
            }
        }
        private void CSpeichern_Click(object sender, EventArgs e)
        {
            // CTabPages.SelectedIndex = 2;
            String fullpath = ZZielpfad + Ort;
            CreateDirectoryIfNotExists(fullpath);
            fullpath = fullpath + "\\" + this.Kennzeichen;
            CreateDirectoryIfNotExists(fullpath);
            List<String> filelist = new List<String>();
            foreach (String Bild in CFiles.Items)
            {
                FileInfo fi = new FileInfo(Bild);
                String fullfilename = fullpath + "\\" + fi.Name;
                if (File.Exists(fullfilename))
                    File.Delete(fullfilename);
                // File.Copy(Bild, fullfilename);
                ScaleAndSaveImage  (Bild, fullfilename, 0.5);
                filelist.Add(fullfilename);
            }
            if (File.Exists(fullpath + "\\Anzeige.WH2"))
            {
                File.Delete(fullpath + "\\Anzeige.WH2");
            }
            File.WriteAllText(fullpath + "\\Anzeige.WH2", InitValues);
        }
        public void ScaledCopy(String src, String dst, double faktor)
        {
            ScaledSave((Bitmap)Image.FromFile(src), dst, faktor);
        }
        public void ScaledSave(Bitmap original, String dst, double faktor)
        {
            // Bitmap original = (Bitmap)Image.FromFile(src);
            Bitmap resized = new Bitmap(original, new Size((int)(original.Width * faktor), (int)(original.Height * faktor)));
            resized.Save(dst);
        }
        private void CAnzeige_Click(object sender, EventArgs e)
        {
            if (pruefeDaten())
            {
                try
                {
                    Bitmap tempausschnitt = (Bitmap)CAusschnitt.BackgroundImage;

                    List<String> filelist = new List<String>();
                    _Files.Clear();
                    foreach (String Bild in CFiles.Items)
                    {
                        FileInfo fi = new FileInfo(Bild);
                        String fullfilename = FullPath + "\\" + fi.Name;
                        if (File.Exists(fullfilename))
                        {
                            File.Delete(fullfilename);
                        }
                        // File.Copy(Bild, fullfilename);
                        ScaleAndSaveImage(Bild, fullfilename);
                        filelist.Add(fullfilename);
                        _Files.Add(fullfilename);
                    }
                    if (File.Exists(FullPath + "\\Anzeige.txt"))
                        File.Delete(FullPath + "\\Anzeige.txt");
                    File.WriteAllText(FullPath + "\\Anzeige.txt", Message);
                    if (tempausschnitt != null)
                    {
                        if (File.Exists(FullPath + "\\Kenneichen.jpg"))
                            File.Delete(FullPath + "\\Kenneichen.jpg");
                        tempausschnitt.Save(FullPath + "\\Kenneichen.jpg", ImageFormat.Jpeg);
                        SendeEmailMitAnhaengen(Mail, "Anzeige einer Verkehrsordnungswiedrigkeit", Message, filelist);
                    }
                }
                catch { }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
        private void cBlueTooth_Click(object sender, EventArgs e)
        {

        }
        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void listBoxDevices_SelectedValueChanged(object sender, EventArgs e)
        {
            listBoxDevices_SelectedIndexChanged(sender, e);
        }
        private void COrt_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    selectOrt(COrt.Text);
                    break;
            }
        }
        private void CSave_MouseEnter(object sender, EventArgs e)
        {
            cstack.Push(CSave.Cursor);
            CSave.Cursor = Cursors.Cross;
        }
        private void CSave_MouseLeave(object sender, EventArgs e)
        {
            CSave.Cursor = cstack.Pop();
        }
        // public string Ort
        // public string PLZ
        // public string Strasse
        // public string HN
        private void cOrtSuche_Click(object sender, EventArgs e)
        {
            String url = ortssuche;
            url = url.Replace("<strasse>", Strasse);
            url = url.Replace("<hn>", HN);
            url = url.Replace("<plz>", PLZ);
            url = url.Replace("<ort>", Ort);
            // ortssuche
            ShellExecute(IntPtr.Zero, "open", url, "", "", 5);
        }
        public void ScaleAndSaveImage(string sourceFilePath, string destinationFilePath)
        {
            ScaleAndSaveImage(sourceFilePath, destinationFilePath, 0.5);
        }
        public void ScaleAndSaveImage(string sourceFilePath, string destinationFilePath, double scale)
        {
            using (Bitmap sourceBitmap = new Bitmap(sourceFilePath))
            {
                // Berechne die neuen Dimensionen nach der Skalierung
                int newWidth = (int)(sourceBitmap.Width * scale);
                int newHeight = (int)(sourceBitmap.Height * scale);

                // Erstelle ein neues Bitmap mit den neuen Dimensionen
                using (Bitmap scaledBitmap = new Bitmap(newWidth, newHeight))
                {
                    // Erstelle einen Grafikobjekt zum Zeichnen im neuen Bitmap
                    using (Graphics graphics = Graphics.FromImage(scaledBitmap))
                    {
                        // Einstellungen für das resampling (Bildabtastung)
                        graphics.SmoothingMode = SmoothingMode.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.Low;

                        // Skaliere das Bild auf das neue Bitmap
                        graphics.DrawImage(sourceBitmap, 0, 0, newWidth, newHeight);
                    }

                    // Speichere das skalierte Bitmap im Zielverzeichnis
                    scaledBitmap.Save(destinationFilePath, ImageFormat.Jpeg);
                }
            }
        }
        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void CZeit_TextChanged(object sender, EventArgs e)
        {
            String[] items = CZeit.Text.Split(':');

            if (items.Length == 3)
            {
                timePicker1.Hour = Convert.ToInt32(items[0]);
                timePicker1.Minute = Convert.ToInt32(items[1]);
                timePicker1.Second = Convert.ToInt32(items[2]);
            }
        }
        private void CreatePDF_CheckedChanged(object sender, EventArgs e)
        {
            if (CreatePDF.Checked)
            {
                PdfHelper pdfHelper = new PdfHelper();
                pdfHelper.nameVorname = $"{ZName}, {ZVorname}";
                pdfHelper.anschrift = $"{ZPLZ} {ZOrt} {ZStrasse} {ZHausnummer}";
                pdfHelper.telefon = "";
                pdfHelper.email = "";
                pdfHelper.tatdatum = Datum;
                pdfHelper.tatzeitVon = Zeit;
                pdfHelper.tatzeitBis = Zeit;
                pdfHelper.tatort = $"{PLZ}  {Ort}";
                pdfHelper.kennzeichen = Kennzeichen;
                pdfHelper.fahrzeugtyp = this.Marke;
                pdfHelper.tatvorwurf = this.Verstoss;
                PDFFilename = pdfHelper.ErstelleLEVPDF();
                Clipboard.SetText(PDFFilename);
            }
            else
                PDFFilename = null;
        }
        private void CLupe_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Video Files|*.mp4;*.avi;*.mkv|All Files|*.*";
            dlg.Title = "Select a Video File";
            dlg.InitialDirectory = ZZielpfad + "Download";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ShellExecute(IntPtr.Zero, "open", dlg.FileName, "", "", 5);
            }
        }
        private void CAddPath_CheckedChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void CAddFile_CheckedChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        static string SaveClipboardImageAsTemporaryFile()
        {
            // Überprüfen, ob die Zwischenablage ein Bild enthält
            if (Clipboard.ContainsImage())
            {
                // Bild aus der Zwischenablage abrufen
                Image clipboardImage = Clipboard.GetImage();

                // Temporären Dateipfad erstellen
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"clipboard_image_{Guid.NewGuid()}.png");

                // Bild als PNG-Datei speichern
                clipboardImage.Save(tempFilePath, System.Drawing.Imaging.ImageFormat.Png);

                // Ressourcen freigeben
                clipboardImage.Dispose();

                return tempFilePath;
            }
            else
            {
                return null;
            }
        }
        private void CClipImage_Click(object sender, EventArgs e)
        {
            String s = SaveClipboardImageAsTemporaryFile();
            if (s!=null)
            {
                _Files.Add(s);
                CFiles.Items.Add(s);
                CSave.BackgroundImage = Bitmap.FromFile(s);
            }
            CAnzeigeText.Text = Message;
            this.Refresh();
        }
        private void CDirOpen_Click(object sender, EventArgs e)
        {
            // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
            ConfigEditorForm dlg = new ConfigEditorForm();

            dlg.ShowDialog();
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void CFiles_DragDrop(object sender, DragEventArgs e)
        {
            //target control will accept data here 
            Panel destination = (Panel)sender;
            destination.BackgroundImage = (Bitmap)e.Data.GetData(typeof(Bitmap));
        }
        private void CFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Bitmap)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void CFiles_MouseDown(object sender, MouseEventArgs e)
        {
            // Panel source = (Panel)sender;
            // DoDragDrop(source.BackgroundImage, DragDropEffects.Copy);
        }
        private void CDatum_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(CDatum.Text);
                DateTime t = Convert.ToDateTime(CZeit.Text);
                DateTime kombiniertesDateTime = new DateTime(dt.Year, dt.Month, dt.Day, t.Hour, t.Minute, t.Second);
                CDTMEdit.Value = kombiniertesDateTime;
            }
            catch { }
            CDTMEdit.Show();

        }
        private void CDatum_TextChanged(object sender, EventArgs e)
        {
        }
        private void CDTMEdit_ValueChanged(object sender, EventArgs e)
        {
            DateTime dateTimeValue = CDTMEdit.Value;
            CDatum.Text = dateTimeValue.ToString("dd MMM yyyy");
            CZeit.Text = dateTimeValue.ToString("HH:mm");
            CDTMEdit.Hide();
        }
        private void CSaveVerstoss_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            // Legen Sie die Eigenschaften des Dialogs fest
            dlg.Filter = "Textdatei|*.txt|Alle Dateien|*.*";
            dlg.Title = "Textdatei speichern";
            dlg.DefaultExt = "txt";
            dlg.InitialDirectory = Environment.CurrentDirectory + "\\Default";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dlg.FileName))
                {
                    File.WriteAllLines(dlg.FileName, CVerstoss.Items.Cast<object>().Select(item => item.ToString()));
                }
            }
        }
        private void CLoadVerstoss_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Textdatei|*.txt|Alle Dateien|*.*";
            dlg.Title = "Textdatei öffnen";
            dlg.DefaultExt = "txt";
            dlg.InitialDirectory = Environment.CurrentDirectory + "\\Default";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dlg.FileName))
                {
                    string[] lines = File.ReadAllLines(dlg.FileName);
                    Init();

                    foreach (string line in lines)
                    {
                        AddVerstoss(line);
                        CVerstossaus.Items.Remove(line);
                    }
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            assistent dlg = new assistent();
            dlg.masterform = this;
            dlg.Show();
        }
        public void MasterFormExecute(String message)
        {
            switch (message)
            {
                case "Neu":
                    CNew_Click(this, new EventArgs());
                    break;

                case "Beenden":
                    break;

                case "Foto":
                    button1_Click(this, new EventArgs());
                    break;

                case "Video":
                    CLupe_Click(this, new EventArgs());
                    break;

                case "Marke":
                    CClip_Click(this, new EventArgs());
                    CMarke.Focus();
                    this.BringToFront();
                    break;

                case "Verstoss":
                    CVerstossaus.Focus();
                    this.BringToFront();
                    break;

                case "Auswahl":
                    break;

                case "Vorlage":
                    CLoadVerstoss_Click(this, new EventArgs());
                    break;

                case "Farbe":
                    break;

                case "Kennzeichen":
                    CKennzeichen.Focus();
                    this.BringToFront();
                    break;

                case "Anzeigen":
                    CAnzeige_Click(this, new EventArgs());
                    break;
            }
        }

        private void CText_Click(object sender, EventArgs e)
        {
            // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
            EditTemplate dlg = new EditTemplate();

            dlg.ShowDialog();
        }
    }
}
