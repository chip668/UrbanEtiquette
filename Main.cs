using IronOcr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;




namespace Anzeige
{
    public partial class Main : Form
    {
        KeyEventArgs ed = null;
        /// <summary>
        /// Korrekturliste kennzheichenerkennung
        /// </summary>
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
        private Color c2;        // Abstandsmessung 
        Double refwidth = 30;
        float scaleFactor = 3.0f; // Vergrößerungsfaktor
        // private String currentfilename;
        private Bitmap _loadedImage;
        private Bitmap loadedImage
        {
            get { return ScaleImage(_loadedImage, (float)CScaleMess.Value / 100); }
            set { _loadedImage = value; }
        }
        private Bitmap lineImage;
        Point stop;
        Point downhelp;
        Point pleft;
        Point pright;
        Point paug;
        Point pref1;
        Point pref2;
        Point dist1;
        Point dist2;
        Pen penFlucht;
        Pen penRef;
        Pen penDist;
        Pen penHelp;
        enum Mode
        {
            LEFT,
            RIGHT,
            AUGPUNKT,
            REF1,
            REF2,
            DIST1,
            DIST2
        }
        Mode mousemode;
        // Falschparker 
        /// <summary>
        /// Enthält Ladungsfähige Anschift
        /// </summary>
        String configfile = "config.txt";
        /// <summary>
        /// Bußgeldklasse berechnet das vorraussichtliche Bußgeld
        /// </summary>
        Bussgeld verstossbussgeld = new Bussgeld();
        // Liste der möglichen Bußgelder bei definierten Verstößen
        Dictionary<String, Bussgeld> bussgelder = new Dictionary<String, Bussgeld>(); 
        // Texterkennung kennzeichen. Funktioniert nur seltnen
        IronTesseract ocrreader = new IronTesseract();
        // Liste der Orte
        Dictionary<String, Ort> Orte = new Dictionary<String, Ort>();
        // Korrekturfen bei der Kennzeichenerkennung
        String plaque = "-aeoSs2B8O0DQCU";
        String numbers = "0123456789";
        String upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ";
        String lower = "abcdefghijklmnopqrstuvwxyzäöüß";
        String spaces = " \t";
        String newline = "\r\n";
        String ortssuche = "https://www.google.com/maps/place/<strasse>+<hn>,+<plz>+<ort>"; // Google Suche Ort und PLZ
        // Hilfsliste Farben. todo : Ähnlichkeit wird selten erkannt
        List<Color> refcolor = new List<Color>();
        // Cursorstapel um schnell den alten Cursor wieder herzustellen
        Stack<Cursor> cstack = new Stack<Cursor>();
        /// <summary>
        /// ShellExecute Methode https://learn.microsoft.com/de-de/windows/win32/api/shellapi/nf-shellapi-shellexecutea
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lpOperation"></param>
        /// <param name="lpFile"></param>
        /// <param name="lpParameters"></param>
        /// <param name="lpDirectory"></param>
        /// <param name="nShowCmd"></param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        Bitmap original; // aktuelles Fotos
        Bitmap ausschnitt; // Kennzeichen
        Point start; // Startpunkt bei der Kennzeichenauswahl
        Rectangle Bildausschnitt; // clientkoordinaten
        Rectangle bmpAusschnitt;  // bitmapkoordinaten
        String ausschnittTemp = "";
        WebBrowser oabrowser; // Browser mit dem Externe Seiten dargestellt werden
        int x0 = 0;
        int y0 = 0;
        int w = 0;
        int h = 0;
        int cntpixel = 0;
        // todo : Segmentierung Kennzeichen nicht aktiv
        private PictureBox _selectedRef;
        public  PictureBox selectedRef
        {
            get { return _selectedRef; }
            set 
            { 
                _selectedRef = value;
                _selectedRef.Parent.Refresh();
            }
        }
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
        public string TemplatePath
        {
            get { return CTemplateFiles.Text; }
            set { CTemplateFiles.Text = value; }
        }
        public string Template
        {
            get
            {
                return File.ReadAllText(TemplatePath);
            }
            set
            {
                File.WriteAllText(TemplatePath, value);
            }
        }
        public List<String> _Files = new List<String>();
        /// <summary>
        /// Liste4 der Dateien
        /// </summary>
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
        /// <summary>
        /// Originaldateien, da die Filesliste modifiziert werden kann, werden die Originale für die Rückstellung benötigt
        /// </summary>
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
        /// <summary>
        /// Verstoßdatum
        /// </summary>
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
        /// <summary>
        /// Zeit des Verstoßes (von)
        /// </summary>
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
        /// <summary>
        /// Verstoßzeit bis
        /// </summary>
        public string ZeitBis
        {
            get
            {

                return CZeitBis.Text;
            }
            set
            {
                CZeitBis.Text = value;
            }
        }
        /// <summary>
        /// Ort des Verstoßes
        /// </summary>
        public string Ort
        {
            get
            {
                String[] items = COrt.Text.Split(';');
                if (items.Length > 1)
                {
                    if (PLZ == "")
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
        /// <summary>
        /// PLZ zum Verstoß
        /// </summary>
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
        /// <summary>
        /// URL des zuständigen Ordnungsamtes
        /// </summary>
        public string URL { get; private set; }
        /// <summary>
        /// Mail des Zuständigen Ordnungsamtes
        /// </summary>
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
        /// <summary>
        /// Strasse an der es den Verstoß gab
        /// </summary>
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
        /// <summary>
        /// Haus an dem der Verstoß stattfand
        /// </summary>
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
        /// <summary>
        ///  Kennzeichen  des Fahrzeuges das den Verstoß geangen hat.
        /// </summary>
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
        /// <summary>
        /// Automarke des Verstoßfahrzeuges
        /// </summary>
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
        /// <summary>
        /// Farbe des Verstoßfahrzeuges
        /// </summary>
        public string Farbe
        {
            get; set;
        }
        /// <summary>
        /// Liste der Verstöße String mit \r\n getrennt
        /// </summary>
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
        /// <summary>
        /// Freitext, wird mit dem Verstoß verbunden
        /// </summary>
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
        /// <summary>
        /// Ladungsfähiger Name
        /// </summary>
        public string ZName { get; private set; }
        /// <summary>
        /// Ladungsfähiger Vorname
        /// </summary>
        public string ZVorname { get; private set; }
        /// <summary>
        /// Ladungsfähiger Ort
        /// </summary>
        public string ZOrt { get; private set; }
        /// <summary>
        ///  Ladungsfähige PLZ
        /// </summary>
        public string ZPLZ { get; private set; }
        /// <summary>
        /// Ladungsfähige Strasse
        /// </summary>
        public string ZStrasse { get; private set; }
        /// <summary>
        /// Ladungsfähige Hausnummer
        /// </summary>
        public string ZHausnummer { get; private set; }
        /// <summary>
        /// Zielpfad Datenablage für eventuelle Gerichtsverhandlung
        /// </summary>
        public string ZZielpfad { get; private set; }
        /// <summary>
        /// Email des Ladungsfähigen Zeugen
        /// </summary>
        public string ZEMail { get; private set; }
        /// <summary>
        /// Telefon des Ladungsfähigen Zeugen 
        /// </summary>
        public string ZPhone { get; private set; }
        /// <summary>
        /// URL um eine GPS Location zu suchen
        /// </summary>
        public string GPSLocation { get; private set; }
        /// <summary>
        /// Vollständige Nachzricht an die Ordnungsbehörde
        /// </summary>
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
                result = result.Replace("<zeitbis>", (Zeit != ZeitBis) ? " bis " + ZeitBis : "");                
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
                result = result.Replace("<zbluetooth>", PDFFilename);
                result = result.Replace("<zielpfad>", PDFFilename);
                result = result.Replace("<zvorname>", PDFFilename);
                result = result.Replace("<zsmtpserver>", PDFFilename);
                result = result.Replace("<zsmtpport>", PDFFilename);
                result = result.Replace("<zsendermail>", PDFFilename);
                result = result.Replace("<zsubject>", PDFFilename);
                result = result.Replace("<zpassword>", PDFFilename);

                return result;
            }
        }
        /// <summary>
        /// Sollen die Pfade in die Meldung übertragen werden (defautl true)
        /// </summary>
        public Boolean AddPath
        {
            get { return CAddPath.Checked; }
        }
        /// <summary>
        /// Sollen die Dateien in die Meldung übertragen werden (defautl true)
        /// </summary>
        public Boolean AddFile
        {
            get { return CAddFile.Checked; }
        }
        /// <summary>
        /// Alle Initialisierungen
        /// </summary>
        public string InitValues
        {
            get
            {
                return
                "Datum=" + Datum +
                "\nZeit=" + Zeit +
                ((Zeit!=ZeitBis)? (" Bis" + ZeitBis):"") +
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
                            case "ZeitBis":
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
        /// <summary>
        /// Einlesen aller initialisierungsdfaten aus config.txt und data.txt
        /// </summary>
        /// <param name="verstossonly"></param>
        public void Init(Boolean verstossonly = false)
        {
            String key = "";
            verstossbussgeld = new Bussgeld();
            List<string> allLines = new List<string>();
            if (!File.Exists (configfile))
            {
                string configfileept = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ept");
                MessageBox.Show("Sie müssen zunächst ihre ladungsfähige Anschrift eingeben damit sie als Zeuge des Vorfalls geladen werden können. Bitte beachten : Anzeigen sind nicht anonym möglich. " + configfileept);
                File.Move(configfileept, configfile);
                ShellExecute(IntPtr.Zero, "open", configfile, "", "", 5);
                Environment.Exit(0);
            }
            allLines.AddRange(File.ReadAllLines(configfile));
            allLines.AddRange(File.ReadAllLines("Data.txt"));
            string[] lines = allLines.ToArray();
            CDataList.Items.Clear();
            CDataList.Items.AddRange(File.ReadAllLines("Data.txt"));

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
                        if (items.Length > 3)
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
                                    if (items.Length > 1)
                                    {
                                        v = new Bussgeld(Convert.ToDouble(items[1]), Convert.ToDouble(items[2]), Convert.ToDouble(items[3]));
                                    }
                                    else
                                    {
                                        v = new Bussgeld(false, false, false);
                                        break;
                                    }
                                    break;
                            }
                            v.parken = (items[0].ToLower().Contains("parken"));
                            v.halten = (items[0].ToLower().Contains("halten"));
                            bussgelder.Add(items[0], v);
                        }
                    }
                    else if (key == "<bluetooth>")
                    {
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
                    else if (key == "<zemail>")
                    {
                        ZEMail = s;
                    }
                    else if (key == "<zphone>")
                    {
                        ZPhone = s;
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
            CAnzeigeText.Text = Message;
            Bildausschnitt = CSave.ClientRectangle;
            oabrowser = new WebBrowser();
            oabrowser.Dock = DockStyle.Fill;
            oabrowser.Navigate("https://www.google.com/");
            oabrowser.ScriptErrorsSuppressed = true;
            PDFFilename = "";
            Farbe = Color.Gold.ToString();
        }
        public string PDFFilename { get; private set; }
        public bool UseLogo { get; private set; }

        AboutBox1 aboutdlg = null;
        /// <summary>
        /// Erzeugt aus einer Vorlage (htmp mit Platzhalter) eine PDF Datei 
        /// mit den aktuellen Daten
        /// </summary>
        /// <param name="template">Vorlage(ohne .htm)</param>
        /// <returns>Temporärpfad auf die PDF Datei</returns>
        private String CreatePDFFile(String template = null)
        {
            PdfHelper pdfHelper = new PdfHelper();
            pdfHelper.nameVorname = $"{ZName}, {ZVorname}";
            pdfHelper.anschrift = $"{ZPLZ} {ZOrt} {ZStrasse} {ZHausnummer}";
            pdfHelper.telefon = $"{ZPhone}";
            pdfHelper.email = $"{ZEMail}";
            pdfHelper.tatdatum = Datum;
            pdfHelper.tatzeitVon = Zeit;
            pdfHelper.tatzeitBis = ZeitBis;
            pdfHelper.tatort = $"{PLZ}  {Ort}";
            pdfHelper.kennzeichen = Kennzeichen;
            pdfHelper.ort = $"{Ort}";
            pdfHelper.fahrzeugtyp = this.Marke;
            pdfHelper.tatvorwurf = this.Verstoss.Replace("\r\n", "<br>");
            pdfHelper.files = pdfHelper.files = CFiles.Items.Cast<string>().ToArray();
            if (template != null)
                pdfHelper.template = template;
            PDFFilename = pdfHelper.ErstelleLEVPDF();
            Clipboard.SetText(PDFFilename);
            return PDFFilename;
        }
        /// <summary>
        /// Startroutine der Anwendung
        /// </summary>
        public Main()
        {
            aboutdlg = new AboutBox1();
            aboutdlg.Show();
            aboutdlg.Refresh();
            Thread.Sleep(2000);
            InitializeComponent();
            CKennzeichen.GotFocus += CKennzeichen_GotFocus;
            CKennzeichen.LostFocus += CKennzeichen_LostFocus;

            Init();
            String[] lines = File.ReadAllLines("ort.txt");

            foreach (String ln in lines)
            {
                Ort o = new Ort(ln);
                if (o.OrtCode != "Ort")
                    Orte.Add(o.OrtCode, o);
            }
            refcolor.Clear();
            foreach (Control i in panel1.Controls)
            {
                refcolor.Add(i.BackColor);
            }
        }
        /// <summary>
        /// Wählt aus der Bildeliste das aktuelle Bild und 
        /// setzt alle Attribute für das ausgewählte bild 
        /// Wenn Geordaten vorhandn sind werden diese auf 
        /// google maps angezeigt. Hier kann dann  ort und 
        /// Strasse kopiert werden.
        /// </summary>
        /// <param name="fileName"></param>
        private void SelectFile(string fileName)
        {
            PhotoMetadataExtractor data = new PhotoMetadataExtractor(fileName);
            Datum = data.Date;
            Zeit = data.Time;
            try
            {
                original = (Bitmap)Bitmap.FromFile(fileName);
                ausschnitt = original;
                // ausschnitt = ScaleImage(original, CSave.Width, CSave.Height);
                // CFoto.Image = Bitmap.FromFile(fileName);
                CSave.BackgroundImage = ausschnitt;
                // currentfilename = fileName;
                if (!data.Valid)
                {
                    GPSLocation = data.GoogleMapsURL;
                    if (GPSLocation!=null)
                        ShellExecute(IntPtr.Zero, "open", GPSLocation, "", "", 5);
                    else
                        ShellExecute(IntPtr.Zero, "open", ortssuche, "", "", 5);
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
            catch
            {
            }
        }
        /// <summary>
        /// Erzeugt einen vollständigen Pfad
        /// </summary>
        /// <param name="path"></param>
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
        /// <summary>
        /// Plausibilitätsprüfung der Daten
        /// </summary>
        /// <returns>true wenn alle Daten korrekt sidn sonst false</returns>
        private Boolean pruefeDaten()
        {
            if (cntpixel == 0) { if (MessageBox.Show("Müssen noch unbeteiligte verpixelt werden?", "DSGVO", MessageBoxButtons.YesNo) == DialogResult.Yes) { return false; } }
            if (Files == "") { MessageBox.Show("Bitte Foto wählen"); return false; }
            if (Datum == "") { MessageBox.Show("Datum des Vorfalls"); return false; }
            if (Zeit == "") { MessageBox.Show("Zeit des Vorfalls"); return false; }
            if (Ort == "") { MessageBox.Show("In welchem Ort hat der Verstoß statt gefunden"); return false; }
            if (PLZ == "") { MessageBox.Show("Wie lautet die PLZ"); return false; }
            if (Strasse == "") { MessageBox.Show("Auf welcher Strasse hat der Verstoß statt gefunden"); return false; }
            if (HN == "") { MessageBox.Show("An welcher Hausnummer hat der Verstoß statt gefunden"); return false; }
            if (Kennzeichen == "") { MessageBox.Show("Wie lautet das Kennzeichen"); return false; }
            if (Marke == "") { MessageBox.Show("Welche Automarke hatte das Fahrzeug"); return false; }
            if (Farbe == Color.Gold.ToString()) { MessageBox.Show("Welche Farbe hatte das Fahrzeug"); return false; }
            if ((Verstoss == "") && (FreeText == "")) { MessageBox.Show("Welcher Verstoß"); return false; }
            if (Mail == "") { MessageBox.Show("Wohin soll ich die Mail senden"); return false; }
            if (ZName == "") { MessageBox.Show("Wie lautet dein Name"); return false; }
            if (ZVorname == "") { MessageBox.Show("Wie lautet dein Vorname"); return false; }
            if (ZOrt == "") { MessageBox.Show("An welchem Ort wohnst du"); return false; }
            if (ZPLZ == "") { MessageBox.Show("Wie lautet die PLZ deines Wohnortes"); return false; }
            if (ZStrasse == "") { MessageBox.Show("Auf welcher Strasse wohnst du"); return false; }
            if (ZHausnummer == "") { MessageBox.Show("Wie lautet die Hausnummer deiner Wohnung"); return false; }
            if (Message == "") { MessageBox.Show("Text benötigt"); return false; }
            if (AreColorsEqual(SystemColors.Control, panel1.BackColor)) { MessageBox.Show("Bitte Farbe auswählen."); return false; }
            return true;
        }
        /// <summary>
        /// todo : Prüft ob zwei Farben ähnlich sind. Nicht zuverlässig
        /// </summary>
        /// <param name="color1">Farbe 1</param>
        /// <param name="color2">Farbe 2</param>
        /// <returns></returns>
        static bool AreColorsEqual(Color color1, Color color2)
        {
            return color1.A == color2.A &&
                   color1.R == color2.R &&
                   color1.G == color2.G &&
                   color1.B == color2.B;
        }
        /// <summary>
        /// todo : Berechnet den Unterschied zwischen zwei Farben. unzuverlässig
        /// </summary>
        /// <param name="a">Farbe 1</param>
        /// <param name="b">Farbe 2</param>
        /// <returns></returns>
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
        /// <summary>
        /// Named Farbe in Text RGB(rot,grün,blau)
        /// </summary>
        /// <param name="c">Farbe</param>
        /// <returns>RGB(c.R,c.G,cf.B)</returns>
        private static String ToRGB(System.Drawing.Color c)
    => $"RGB({c.R},{c.G},{c.B})";
        /// <summary>
        /// schwärtz eine Region im angegebenen Bitmap
        /// </summary>
        /// <param name="bitmap">Bild in dem geschwärzt wird</param>
        /// <param name="region">rergion die geschwärzt werden soll</param>
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
        /// <summary>
        /// verpöixelt eine Region im angegebenen Bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="region"></param>
        private void PixelOutRegion(Bitmap bitmap, Rectangle region)
        {
            int raster = (int)CRaster.Value;

            // Stelle sicher, dass die Region innerhalb der Bitmap-Grenzen liegt
            region.Intersect(new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            // Verpixeln der Region
            for (int y = region.Top; y < region.Bottom; y += raster)
            {
                for (int x = region.Left; x < region.Right; x += raster)
                {
                    int r = 0;
                    int g = 0;
                    int b = 0;
                    int n = raster * raster;
                    Color c;
                    for (int zx = 0; zx < raster; zx++)
                    {
                        for (int zy = 0; zy < raster; zy++)
                        {
                            c = bitmap.GetPixel(x + zx, y + zy);
                            r += c.R;
                            g += c.G;
                            b += c.B;
                        }
                    }
                    c = Color.FromArgb(r / n, g / n, b / n);
                    for (int zx = 0; zx < raster; zx++)
                    {
                        for (int zy = 0; zy < raster; zy++)
                        {
                            bitmap.SetPixel(x + zx, y + zy, c);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Transformation zwischen Bilddarstellung background zoom und bitmap original
        /// </summary>
        /// <param name="bildausschnitt"></param>
        /// <param name="clientControl"></param>
        /// <param name="ausschnitt"></param>
        /// <returns></returns>
        private Rectangle Transform(Rectangle bildausschnitt, Control clientControl, Rectangle ausschnitt)
        {
            return Transform(Bildausschnitt, clientControl.ClientRectangle, ausschnitt.Size);
        }
        /// <summary>
        /// Transformation zwischen Bilddarstellung background zoom und bitmap original
        /// </summary>
        /// <param name="bildausschnitt"></param>
        /// <param name="clientRectangle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Rectangle Transform(Rectangle bildausschnitt, Rectangle clientRectangle, Size size)
        {
            Double faktor = 1;
            Rectangle result = clientRectangle;
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
            h = (int)(size.Height / faktor);
            w = (int)(size.Width / faktor);
            bmpAusschnitt = new Rectangle(
                (int)((Bildausschnitt.X - x0) * faktor),
                (int)((Bildausschnitt.Y - y0) * faktor),
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
        /// <summary>
        /// Transformation zwischen Bilddarstellung background zoom und bitmap original
        /// </summary>
        /// <param name="point"></param>
        /// <param name="clientRectangle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
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
        /// <summary>
        /// unused
        /// </summary>
        /// <param name="fileName"></param>
        private void SelectFile_new(string fileName)
        {
            PhotoMetadataExtractor data = new PhotoMetadataExtractor(fileName);
            Datum = data.Date;
            Zeit = data.Time;

            try
            {
                original = (Bitmap)Bitmap.FromFile(fileName);

                // Scale the image to fit in CSave

                CSave.BackgroundImage = ausschnitt;

                // Restliche Logik bleibt unverändert...

            }
            catch
            {
                // Fehlerbehandlung
            }
        }
        private Bitmap ScaleImage(Bitmap image, int maxWidth, int maxHeight)
        {
            // Berechnen Sie das Seitenverhältnis des Bildes
            float aspectRatio = (float)image.Width / image.Height;

            // Berechnen Sie die neuen Abmessungen basierend auf dem Seitenverhältnis und den maximalen Dimensionen
            int newWidth = maxWidth;
            int newHeight = (int)(newWidth / aspectRatio);

            if (newHeight > maxHeight)
            {
                // Wenn die Höhe nach der Skalierung größer als die maximale Höhe ist, passen Sie die Höhe entsprechend an
                newHeight = maxHeight;
                newWidth = (int)(newHeight * aspectRatio);
            }

            // Erstellen Sie ein neues Bitmap-Objekt mit den skalierten Abmessungen
            Bitmap scaledImage = new Bitmap(newWidth, newHeight);

            // Erstellen Sie eine Grafik vom skalierten Bild
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                // Setzen Sie die Interpolation auf hohe Qualität für eine bessere Skalierung
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Zeichnen Sie das skalierte Bild
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return scaledImage;
        }
        /// <summary>
        /// Versucht OCR ERkennung zu korregieren
        /// </summary>
        /// <param name="original"></param>
        /// <returns>koprregierter String</returns>
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
        /// <summary>
        /// OCR des Kennzeichens
        /// </summary>
        /// <param name="image">Bildausschnitt mit Kennzeichen</param>
        /// <returns>Erkannter Text</returns>
        public string ReadTextFromBitmap(Bitmap image)
        {
            if (image != null)
            {
                String original = ocrreader.Read(image).Text;
                return FixOCRText(original);
            }
            return "";
        }
        /// <summary>
        /// Bild/Bitmap in ein Schwarzweiß bitmap verwandeln (nicht grau)
        /// threshold gibt dabe den Grenzwert an.
        /// </summary>
        /// <param name="originalBitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public Bitmap ConvertToBlackAndWhite(Bitmap originalBitmap, int threshold)
        {
            Bitmap bwBitmap = null;
            if (originalBitmap != null)
            {
                bwBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);

                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        Color originalColor = originalBitmap.GetPixel(x, y);

                        // Durchschnitt der RGB-Werte berechnen
                        int averageColor = (originalColor.R + originalColor.G + originalColor.B) / 3;

                        // Schwellenwert für die Konvertierung festlegen (z.B., 128 für einen einfachen Mittelwert)
                        // int threshold = 128;

                        // Schwarz oder Weiß basierend auf dem Schwellenwert setzen
                        Color bwColor = (averageColor > threshold) ? Color.White : Color.Black;

                        // Pixel im Schwarzweiß-Bitmap setzen
                        bwBitmap.SetPixel(x, y, bwColor);
                    }
                }

            }
            return bwBitmap;
        }
        /// <summary>
        /// Eerzeugt eine Kopie eines Bildes und skaliert das bild um es dann abzuspeichern
        /// </summary>
        /// <param name="src">Quelldatei</param>
        /// <param name="dst">Ziel</param>
        /// <param name="faktor">Skalierung</param>
        public void ScaledCopy(String src, String dst, double faktor)
        {
            ScaledSave((Bitmap)Image.FromFile(src), dst, faktor);
        }
        /// <summary>
        /// Eerzeugt eine Kopie eines Bildes und skaliert das bild um es dann abzuspeichern
        /// </summary>
        /// <param name="original">Original Bitmap</param>
        /// <param name="dst">Ziel</param>
        /// <param name="faktor">Skalierung</param>
        public void ScaledSave(Bitmap original, String dst, double faktor)
        {
            // Bitmap original = (Bitmap)Image.FromFile(src);
            Bitmap resized = new Bitmap(original, new Size((int)(original.Width * faktor), (int)(original.Height * faktor)));
            resized.Save(dst);
        }
        /// <summary>
        /// Verstoß aus der Verstossliste entfernen
        /// </summary>
        /// <param name="verstoss"></param>
        private void RemoveVerstoss(String verstoss)
        {
            CVerstoss.Items.Remove(verstoss.Trim());
            verstossbussgeld = new Bussgeld();
            foreach (String i in CVerstoss.Items)
                AddBussgeld(i);
            bussgeldrechner1.bussgeld = verstossbussgeld;
        }
        /// <summary>
        /// Busgeldbeiträge summieren
        /// </summary>
        /// <param name="verstoss"></param>
        private void AddBussgeld(String verstoss)
        {
            try
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
            catch { }
        }
        /// <summary>
        /// Bußgeldbeträge summieren und Anzeige aktualisieren
        /// </summary>
        /// <param name="verstoss"></param>
        private void AddVerstoss(String verstoss)
        {
            CVerstoss.Items.Add(verstoss.Trim());
            AddBussgeld(verstoss);
            bussgeldrechner1.bussgeld = verstossbussgeld;
        }
        /// <summary>
        /// Tooltip setzen
        /// </summary>
        /// <param name="ctl"></param>
        void setSelectedLineTip(Control ctl)
        {
            toolTip1.SetToolTip(ctl, ctl.Text);
        }
        /// <summary>
        /// Ausschnitt aus einem bitmap ausschneiden und als Bitmap zurück geben
        /// </summary>
        /// <param name="sourceBitmap">Originalbild aus dem ausgeschnitten werden soll</param>
        /// <param name="cropRectangle">Ausschnittsrechteck</param>
        /// <returns>Ausschit Bitmap</returns>
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
        /// <summary>
        /// Ausschnitt aus einem bitmap ausschneiden und als Bitmap zurück geben
        /// </summary>
        /// <param name="sourceBitmap">Originalbild aus dem ausgeschnitten werden soll</param>
        /// <param name="cropRectangle">Ausschnittsrechteck</param>
        /// <returns>Ausschit Bitmap</returns>
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
                return new Bitmap (0, 0);
            }
        }
        /// <summary>
        /// Kopiert ein Bild aus einer Datei und schreibt das Ergebnis wieder in eine Datei
        /// </summary>
        /// <param name="sourceFilePath">Quelldatei</param>
        /// <param name="destinationFilePath">Zieldatei</param>
        public void ScaleAndSaveImage(string sourceFilePath, string destinationFilePath)
        {
            ScaleAndSaveImage(sourceFilePath, destinationFilePath, 0.5);
        }
        /// <summary>
        /// Kopiert ein Bild aus einer Datei, sklaiert sie und schreibt das Ergebnis wieder in eine Datei
        /// </summary>
        /// <param name="sourceFilePath">Quelldatei</param>
        /// <param name="destinationFilePath">Zieldatei</param>
        /// <param name="scale">Skalierungsfaktor</param>
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
        /// <summary>
        /// Schreibt ein Bild in eine trmporäre Datei (Kennzeichen)
        /// </summary>
        /// <returns>dateiname</returns>
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
        /*        
         *      Point pleft;
                Point pright;
                Point paug;
                Point pref1;
                Point pref2;
                Pen penFlucht;
                Pen penRef;
        */
        public void DrawLines()
        {
            if (loadedImage != null)
            {
                lineImage = (Bitmap)loadedImage.Clone();
                Graphics graphics = Graphics.FromImage(lineImage);
                {
                    graphics.DrawLine(penFlucht, pleft, paug);
                    graphics.DrawLine(penFlucht, pright, paug);
                    graphics.DrawLine(penRef, pref1, pref2);
                    graphics.DrawLine(penDist, dist1, dist2);
                    // graphics.DrawLine(penHelp, stop, new Point(stop.X, 0));
                    graphics.DrawLine(penHelp, downhelp, new Point(stop.X, 0));
                    Font largerFont = new Font(this.Font.FontFamily, this.Font.Size * scaleFactor, this.Font.Style);
                    graphics.DrawString("Distanz: " + Distance.Text, largerFont, new SolidBrush(Color.White), new Point(20, 20));

                    // Gespiegelte Linie zeichnen
                    try
                    {
                        float f1 = (float)Convert.ToDecimal(RealDistance.Text);
                        float f2 = (float)Convert.ToDecimal(Distance.Text);
                        if (f2 != 0)
                        {
                            float lamba = 1 - (f1 / f2);
                            graphics.DrawLine(penDist, paug.X, paug.Y, dist2.X - lamba * (dist2.X - dist1.X), dist1.Y);
                            graphics.DrawLine(penDist, paug.X, paug.Y, dist2.X + (lamba) * (dist2.X - dist1.X), dist1.Y);
                        }
                    }
                    catch { }
                }
                pictureBox.Image = lineImage;
            }
        }



        // ====== Eventhandling ==============================
        /// <summary>
        /// Initialisierungen der Anwendung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            smallToolbox1.AddButtons(new string[] { "🗎", "🖼", "📋" }, new string[] { "Neue Anzeige", "Bilder laden", "Adresse einfügen" });
            smallToolbox2.AddButtons(new string[] { "💾", "📁", "🎥", "📁", "🖼" }, new string[] { "Speichern der aktuellen Anzeige", "Anzeige laden", "Video auswählen", "Ordner öffnen", "Bild aus der Zwischenablage einfügen" });
            smallToolbox3.AddButtons(new string[] { "⌖", "👷", "⚙", "🗎", "🛈" }, new string[] { "Adresse mbei Google anzeigen", "Assistent", "Einstellung", "Textvorlage bearbeiten", "Hilfe" });
            smallToolbox4.AddButtons(new string[] { "💾", "📁" }, new string[] { "Anzeigeart speichern", "Anzeigeart laden" });
            smallToolbox5.AddButtons(new string[] { "⇊", "🠗", "↑", "⇈" }, new string[] { "Alles Verstöße anzeigen", "Verstoß anzeigen", "Verstoß nicht anzeigen", "Keine Verstöße anzeigen" });
            
            smallToolbox2.OpenMode = false;
            smallToolbox3.OpenMode = false;
            smallToolbox5.OpenMode = false;


            UseLogo = true;
            listBoxDevices_SelectedIndexChanged(sender, e);
            string[] templates = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.tpl").ToArray();
            CTemplateFiles.Items.Clear();

            foreach (String i in templates)
                CTemplateFiles.Items.Add(new FileInfo(i));
            CTemplateFiles.SelectedIndex = 0;
            splitContainer1.SplitterDistance = 440;

            // Initialisieren Sie Ihre Formularkomponenten hier
            left.Checked = true;
            SetImage(global::Anzeige.Properties.Resources.eng);
            selectedRef = pictureBox4;
            // selectedRef.Parent.Paint += Parent_Paint;
            aboutdlg.Hide();
            // edit_Adress1.Dock = DockStyle.Fill;
            // edit_Line1.Dock = DockStyle.Fill;
        }
        /// <summary>
        /// Farbe auswählen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Bestimmt die Daten zum zugehörigen Ordnungsamt
        /// </summary>
        /// <param name="ort"></param>
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
        /// <summary>
        /// Alle Verstöße hinzu fügen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            if (items.Length>1)
            {
                selectOrt(items[1]);
                CAnzeigeText.Text = Message;
            }
        }
        private void CMarke_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSelectedLineTip((Control)sender);
            if (UseLogo)
            {
                try
                {
                    CLogo.BackgroundImage = Bitmap.FromFile(CMarke.Text + ".jpg");
                    CAnzeigeText.Text = Message;
                }
                catch
                {
                    Clipboard.SetText(CMarke.Text + ".jpg");
                    ShellExecute(IntPtr.Zero, "open", $"https://www.google.com/search?q={CMarke.Text}+logo+auto", "", "", 5);
                }
            }
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        private void CFoto_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                start = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (ausschnitt != null)
                {
                    Point p = Transform(e.Location, CSave.ClientRectangle, ausschnitt.Size);
                    Bitmap b = (Bitmap)CSave.BackgroundImage;
                    Color c = b.GetPixel(p.X, p.Y);
                    Color c2 = Color.Gold;
                    panel1.BackColor = c;
                    c2 = ColorHelper.FindClosestColor(refcolor, c);
                    //panel1.BackColor = c2;

                    /*
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
                    */
                }
                else
                {

                }
            }
        }
        private void CFoto_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ausschnitt != null)
                {
                    try
                    {
                        Point p = Transform(e.Location, CSave.ClientRectangle, ausschnitt.Size);
                        Bitmap b = (Bitmap)CSave.BackgroundImage;
                        if (b!=null)
                        {
                            Point start1 = new Point(Math.Min(start.X, e.X), Math.Min(start.Y, e.Y));
                            Point ende1 = new Point(Math.Max(start.X, e.X), Math.Max(start.Y, e.Y));

                            Color c = b.GetPixel(p.X, p.Y);
                            this.Text = "Wegeheld 2 |" + ToRGB(c) + " | " + ToRGB(panel1.BackColor);
                            Bildausschnitt = new Rectangle(start1, new Size(ende1.X - start1.X, ende1.Y - start1.Y));
                        }
                    }
                    catch { }
                }
            }
        }
        private void CFoto_MouseUp_Old(object sender, MouseEventArgs e)
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
                            // COCRPicture.BackgroundImage = ConvertToBlackAndWhite(tempausschnitt);
                            ScaledSave(tempausschnitt, ausschnittTemp, 3);

                            if (!CPixeln.Checked)
                            {
                                // Hier wird eine Kopie der Originaldatei erstellt
                                Bitmap kopie = new Bitmap(ausschnitt);
                                // Den ausgeschnittenen Bereich in der Kopie schwarz färben
                                BlackOutRegion(kopie, bmpAusschnitt);

                                // Den Dateipfad für die geschwärzte Kopie festlegen
                                // string geschwaerzteKopiePfad = Path.GetTempFileName().Replace(".tmp", ".jpg");
                                CreateDirectoryIfNotExists(ZZielpfad + "public\\");
                                string geschwaerzteKopiePfad = ZZielpfad + "public\\" + Guid.NewGuid().ToString() + ".jpg"; ;
                                // Die geschwärzte Kopie speichern
                                kopie.Save(geschwaerzteKopiePfad, ImageFormat.Jpeg);
                                // Jetzt haben Sie die geschwärzte Kopie in der Variable 'geschwaerzteKopiePfad'
                            }
                            else
                            {
                                PixelOutRegion(ausschnitt, bmpAusschnitt);
                                //original
                                CSave.BackgroundImage = ausschnitt;
                                CSave.Refresh();
                                Bitmap tempausschnitt2 = CropRectangleFromBitmap(ausschnitt, bmpAusschnitt); ;
                                CAusschnitt.BackgroundImage = tempausschnitt2;
                                CAusschnitt.Show();
                                // original.Save(currentfilename);
                            }
                        }
                        Bitmap bmp = (Bitmap)CAusschnitt.BackgroundImage;
                        if (bmp!=null)
                        {
                            if (bmp.Width * bmp.Height < 400000)
                            {
                                for (int th = 0; th < 256; th += 16)
                                {
                                    bmp = ConvertToBlackAndWhite((Bitmap)CAusschnitt.BackgroundImage, (int)th);
                                    String text = ReadTextFromBitmap((Bitmap)COCRPicture.BackgroundImage);
                                    String[] s = text.Split(' ');
                                    if (s.Length == 3 && CKennzeichen.Text == "")
                                    {
                                        CKennzeichen.Text = text;
                                        COCRPicture.BackgroundImage = bmp;
                                    }
                                    COCRPicture.Refresh();
                                }
                            }
                        }
                        if (CKennzeichen.Text == "" && !CPixeln.Checked)
                        {
                            CKennzeichen.Text = ReadTextFromBitmap(tempausschnitt);
                        }
                        CAusschnitt.Refresh();
                    }
                    else
                    {
                    }
                }
                catch { }
            }
        }
        private void CFoto_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    cstack.Push(this.Cursor);
                    this.Cursor = Cursors.WaitCursor;
                    if (ausschnitt != null)
                    {

                        Rectangle rcl = Transform(Bildausschnitt, CSave.ClientRectangle, ausschnitt.Size);
                        Bitmap tempausschnitt = CropRectangleFromBitmap(ausschnitt, bmpAusschnitt);
                        CAusschnitt.BackgroundImage = tempausschnitt;
                        CAusschnitt.Show();
                        ausschnittTemp = (AddPath) ? Path.GetTempFileName().Replace(".tmp", ".jpg") : null;
                        if (tempausschnitt != null)
                        {
                            ScaledSave(tempausschnitt, ausschnittTemp, 3);

                            if (!CPixeln.Checked)
                            {
                                Bitmap kopie = new Bitmap(ausschnitt);
                                BlackOutRegion(kopie, bmpAusschnitt);

                                // Den Dateipfad für die geschwärzte Kopie festlegen
                                string geschwaerzteKopiePfad = ZZielpfad + "public\\" + Guid.NewGuid().ToString() + ".jpg";

                                // Graphics-Objekt für das Zeichnen auf der Kopie erstellen
                                using (Graphics g = Graphics.FromImage(kopie))
                                {
                                    // Schriftart und Pinsel für den Text festlegen
                                    Font font = new Font("Arial", 24);
                                    SolidBrush brush = new SolidBrush(Color.White);

                                    if (bussgeldrechner1.bussgeld != null)
                                    {
                                        Bussgeld bussgeld = bussgeldrechner1.bussgeld;
                                        int y0 = kopie.Height - 280;
                                        g.DrawString($"Parken: {(bussgeld.parken ? "ja" : "nein")}", font, brush, 10, y0); y0 += 40;
                                        g.DrawString($"Halten: {(bussgeld.halten ? "ja" : "nein")}", font, brush, 10, y0); y0 += 40;
                                        g.DrawString($"Mit Behinderung: {(bussgeld.mitbehinderung ? "ja" : "nein")}", font, brush, 10, y0); y0 += 40;
                                        g.DrawString($"Mit Gefährdung: {(bussgeld.mitgefaerdung ? "ja" : "nein")}", font, brush, 10, y0); y0 += 40;
                                        g.DrawString($"Verdopplung: {(bussgeld.faktor == 2 ? "ja" : "nein")}", font, brush, 10, y0); y0 += 40;
                                        g.DrawString($"Bußgeld: {bussgeld.Betrag:C2}", font, brush, 10, y0); y0 += 40;
                                    }
                                }

                                // Die geschwärzte Kopie speichern
                                kopie.Save(geschwaerzteKopiePfad, ImageFormat.Jpeg);

                                // Kopie freigeben und zerstören
                                kopie.Dispose();
                            }
                            else
                            {
                                PixelOutRegion(ausschnitt, bmpAusschnitt);
                                CSave.BackgroundImage = ausschnitt;
                                CSave.Refresh();
                                Bitmap tempausschnitt2 = CropRectangleFromBitmap(ausschnitt, bmpAusschnitt);
                                CAusschnitt.BackgroundImage = tempausschnitt2;
                                CAusschnitt.Show();
                            }
                        }
                        Bitmap bmp = (Bitmap)CAusschnitt.BackgroundImage;
                        if (bmp != null && bmp.Width * bmp.Height < 400000)
                        {
                            for (int th = 0; th < 256; th += 16)
                            {
                                bmp = ConvertToBlackAndWhite((Bitmap)CAusschnitt.BackgroundImage, (int)th);
                                String text = ReadTextFromBitmap((Bitmap)COCRPicture.BackgroundImage);
                                String[] s = text.Split(' ');
                                if (s.Length == 3 && CKennzeichen.Text == "")
                                {
                                    CKennzeichen.Text = text;
                                    COCRPicture.BackgroundImage = bmp;
                                }
                                COCRPicture.Refresh();
                            }
                        }
                        if (CKennzeichen.Text == "" && !CPixeln.Checked)
                        {
                            CKennzeichen.Text = ReadTextFromBitmap(tempausschnitt);
                        }
                        CAusschnitt.Refresh();
                    }
                    else
                    {
                    }
                }
                catch { }
                cstack.Pop();
            }
        }
        private void CFotoAnzeige_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rcl = new Rectangle(x0, y0, w, h);

            e.Graphics.DrawRectangle(new Pen(Color.Green), rcl);
            e.Graphics.DrawRectangle(new Pen(Color.Red), Bildausschnitt);

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
        private void CFiles_DoubleClick(object sender, EventArgs e)
        {
            SelectFile(CFiles.Text);
        }
        private void CKennzeichen_TextChanged(object sender, EventArgs e)
        {
            if (COrt.Text == "")
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
            if (ausschnitt != null)
            {
                Rectangle rcl = Transform(Bildausschnitt, CSave.ClientRectangle, ausschnitt.Size);
            }
            CAusschnitt.Height = CSave.Height / 2;
            edit_Line1.Location = new Point(0, 0);
            edit_Line1.Size = CSave.Size;
            edit_Adress1.Location = new Point(0, 0);
            edit_Adress1.Size = CSave.Size;

        }
        private void CFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSelectedLineTip((Control)sender);
            ausschnittTemp = "";
            cntpixel = 0;
            CAusschnitt.Hide();
        }
        private void CTabPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            CStadtPate.Controls.Clear();
            CTabPageOA.Controls.Clear();
            CWeglide.Controls.Clear();
            CGMaps.Controls.Clear();
            if (URL != null)
            {
                if (CTabPages.SelectedTab == CTabPageOA)
                {
                    if (URL.Length < 1)
                    {

                    }
                    else if (URL == "{pdf}")
                    {
                        CreatePDF.Checked = false;
                        CreatePDF.Checked = true;

                        if (PDFFilename != "")
                        {
                            ShellExecute(IntPtr.Zero, "open", PDFFilename, "", "", 5);
                        }
                    }
                    else if (URL.Substring(0, 1) == "@")
                    {
                        ShellExecute(IntPtr.Zero, "open", URL.Substring(1), "", "", 5);
                    }
                    else
                    {
                        oabrowser.Navigate(URL);
                        CTabPageOA.Controls.Add(oabrowser);
                    }
                }
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
            else if (CTabPages.SelectedTab == CPolice)
            {
                oabrowser.Navigate("https://www.google.com/search?q=polizei+<ort>".Replace("<ort>", Ort));
                CPolice.Controls.Add(oabrowser);
            }
            else if (CTabPages.SelectedTab == CGMaps)
            {
                if (GPSLocation == "")
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
            return isNumeric(c) || isAlpha(c);
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
        public String TestOutput(String text)
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
                        if (isNumeric(m) && numbers.Length < 4)
                        {
                            numbers = m.ToString() + numbers;
                        }
                        else
                        {
                            if (!isSpace(m)) // nur mit einem Buchstaben zu 3
                            {
                                m = MakeNumberToChar(original, i);
                                if (isAlpha(m))
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
                            else if (isAlpha(m) && (letters.Length < 3))
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
                            i = 0;
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
            if (letters.Length > 0)
            {
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
        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void CZeit_TextChanged(object sender, EventArgs e)
        {
            String[] items = CZeit.Text.Split(':');
            if (CZeitBis.Text == "")
            {
                CZeitBis.Text = CZeit.Text;
            }
        }
        private void CreatePDF_CheckedChanged(object sender, EventArgs e)
        {
            if (CreatePDF.Checked)
            {
                CreatePDFFile();
            }
            else
                PDFFilename = null;
        }
        private void CAddPath_CheckedChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void CAddFile_CheckedChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
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
                    CLoadPic_Click(this, new EventArgs());
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
        private void CTemplateFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            CTemplateFiles_SelectedValueChanged(sender, e);
        }
        private void CTemplateFiles_SelectedValueChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void CAnzeigeText_TextChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void CZeitBis_TextChanged(object sender, EventArgs e)
        {
            CAnzeigeText.Text = Message;
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            edit_Adress1.Hide();
            edit_Line1.Hide();
            if (tabControl1.SelectedTab == CTAbstand)
            {
                // ShellExecute(IntPtr.Zero, "open", @"C:\Users\schip\source\repos\BildMessen\BildMessen\bin\Debug\netcoreapp3.1\BildMessen.exe", "", "", 5);
                splitContainer1.SplitterDistance = 160;
                pictureBox.Visible = true;
                pictureBox.Dock = DockStyle.Fill;
                pictureBox.BackgroundImageLayout = ImageLayout.None;
            }
            else
            {
                splitContainer1.SplitterDistance = 440;
                pictureBox.Visible = false;
            }
        }
        private void openButton_Click(object sender, EventArgs e)
        {
            // Öffnen Sie den OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Bild auswählen";
                openFileDialog.Filter = "Bilddateien (*.jpg, *.png)|*.jpg;*.png";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadImage(openFileDialog.FileName);
                }
            }
        }
        private void SetImage (Bitmap bmp)
        {
            // global::Anzeige.Properties.Resources.eng
            loadedImage = bmp;

            pleft = new Point(0, loadedImage.Height);
            pright = new Point(loadedImage.Width, loadedImage.Height);
            paug = new Point(loadedImage.Width / 2, 0); ;
            pref1 = new Point(loadedImage.Width * 3 / 5, loadedImage.Height * 3 / 4);
            pref2 = new Point(loadedImage.Width * 4 / 5, loadedImage.Height * 3 / 4);
            dist1 = new Point(loadedImage.Width * 4 / 9, loadedImage.Height * 4 / 5);
            dist2 = new Point(loadedImage.Width * 5 / 9, loadedImage.Height * 4 / 5);

            penFlucht = new Pen(Color.Yellow, 2.0f);
            penRef = new Pen(Color.Blue, 2.0f);
            penDist = new Pen(Color.Red, 2.0f);
            penHelp = new Pen(Color.Green, 2.0f);

            // Zeigen Sie das geladene Bild auf dem PictureBox-Steuerelement an
            // pictureBox.Image = loadedImage;
            DrawLines();
        }
        private void LoadImage(String filename)
        {
            Bitmap temp = new Bitmap(filename);
            temp = ScaleImage(temp, pictureBox.Width, pictureBox.Height);

            SetImage(temp);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Bilddateien (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Kopie der Bitmap erstellen
                    Bitmap copiedImage = new Bitmap(lineImage);

                    // Abstand unten links einfügen
                    using (Graphics g = Graphics.FromImage(copiedImage))
                    {
                        // Hier wird der Abstand unter Verwendung von RealDistance.Text eingefügt.
                        float distance;
                        if (float.TryParse(RealDistance.Text, out distance))
                        {
                            // Setzen Sie hier die gewünschten Werte für X und Y ein.
                            float x = 10; // Beispiel: 10 Pixel von links
                            float y = copiedImage.Height - distance - 50; // Beispiel: 10 Pixel von unten abzüglich des Abstands

                            // Text zeichnen
                            Font largerFont = new Font(this.Font.FontFamily, this.Font.Size * scaleFactor, this.Font.Style);
                            g.DrawString("Abstand: " + RealDistance.Text, largerFont, new SolidBrush(Color.White), x, y);

                            // vertical lines 
                            Pen greenPen = new Pen(Color.Green, 1);
                            g.DrawLine(greenPen, dist1, new Point(dist1.X, 0));
                            g.DrawLine(greenPen, dist2, new Point(dist2.X, 0));
                        }
                    }
                    // Bitmap speichern
                    copiedImage.Save(saveFileDialog.FileName);
                }
            }
        }
        private void Insert_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                try
                {
                    Image clipboardImage = Clipboard.GetImage();
                    if (clipboardImage != null)
                    {
                        string tempFilePath = Path.GetTempFileName();
                        clipboardImage.Save(tempFilePath);
                        LoadImage(tempFilePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ein Fehler ist aufgetreten: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Die Zwischenablage enthält kein Bild.");
            }
        }
        private void ownwidth_TextChanged(object sender, EventArgs e)
        {
            refwidth = Convert.ToDouble(textBox1.Text);
            textrefresh();
        }
        private void textrefresh()
        {
            textBox1.Text = refwidth.ToString();
            CalculateAndDisplayDistance();
            // Distance.Text = Math.Round(refwidth * (double)numericUpDown1.Value, 2).ToString();
        }
        private void CalculateAndDisplayDistance()
        {
            // Messen Sie die Entfernung zwischen dist1 und paug
            double distanceDist1ToAug = MeasureDistance(dist1, paug);

            // Messen Sie die Entfernung zwischen ref1 und paug
            double distanceRef1ToAug = MeasureDistance(pref1, paug);

            // Berechnen Sie den Vergrößerungsfaktor basierend auf dem Strahlensatz
            double scaleFactor = distanceRef1ToAug / distanceDist1ToAug;

            // Messen Sie die Entfernung zwischen dist1 und dist2
            double distanceDist1ToDist2 = MeasureDistance(dist1, dist2);

            // Messen Sie die Entfernung zwischen ref1 und ref2
            double distanceRef1ToRef2 = MeasureDistance(pref1, pref2);

            // Berechnen Sie die relative Entfernung unter Berücksichtigung des Strahlensatzes
            double relativeDistance = distanceDist1ToDist2 * scaleFactor * Convert.ToDouble(textBox1.Text) / distanceRef1ToRef2;

            // Aktualisieren Sie das Textfeld mit dem berechneten Ergebnis
            Distance.Text = $"{relativeDistance:F2}"; // Anpassen Sie die Formatierung nach Bedarf
            RealDistance.Text = (relativeDistance - Convert.ToDouble(ownwidth.Text) / 2).ToString("F2");
        }
        private double MeasureDistance(Point point1, Point point2)
        {
            // Berechnen Sie die Entfernung zwischen point1 und point2
            int deltaX = point2.X - point1.X;
            int deltaY = point2.Y - point1.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return distance;
        }
        private void left_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.LEFT;
        }
        private void right_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.RIGHT;
        }
        private void Augpunkt_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.AUGPUNKT;
        }
        private void CRef1_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.REF1;
        }
        private void CRef2_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.REF2;
        }
        private void CDist1_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.DIST1;
        }
        private void CDist2_CheckedChanged(object sender, EventArgs e)
        {
            mousemode = Mode.DIST2;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            refwidth = 11.25;
            selectedRef = pictureBox1;
            textrefresh();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            refwidth = 22.5;
            selectedRef = pictureBox2;
            textrefresh();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            refwidth = 30;
            selectedRef = pictureBox3;
            textrefresh();
        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            refwidth = 12;
            selectedRef = pictureBox4;
            textrefresh();
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            refwidth = 50;
            selectedRef = pictureBox6;
            textrefresh();
        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            refwidth = 73;
            selectedRef = pictureBox5;
            textrefresh();
        }
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            refwidth = 220;
            selectedRef = pictureBox7;
            textrefresh();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            textrefresh();
        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            start = new Point(e.Location.X, e.Location.Y);
            DrawLines();
        }
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            stop = new Point(e.Location.X, e.Location.Y);
            if (loadedImage != null)
                downhelp = new Point(e.Location.X, loadedImage.Height);
            DrawLines();
        }
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch (mousemode)
            {
                case Mode.LEFT:
                    pleft = stop;
                    right.Checked = true;
                    pright = new Point(pright.X, pleft.Y);
                    break;
                case Mode.RIGHT:
                    pright = stop;
                    Augpunkt.Checked = true;
                    break;
                case Mode.AUGPUNKT:
                    paug = stop;
                    break;
                case Mode.REF1:
                    pref1 = stop;
                    mousemode = Mode.REF2;
                    CRef2.Checked = true;
                    pref2 = new Point(pref2.X, pref1.Y);
                    break;
                case Mode.REF2:
                    pref2 = stop;
                    break;
                case Mode.DIST1:
                    dist1 = stop;
                    CDist2.Checked = true;
                    dist2 = new Point(paug.X, dist1.Y);
                    break;
                case Mode.DIST2:
                    dist2 = stop;
                    CalculateAndDisplayDistance();
                    break;
            }
            textrefresh();
            DrawLines();
        }
        private Bitmap ScaleImage(Bitmap originalImage, float scaleFactor)
        {
            int newWidth = (int)(originalImage.Width * scaleFactor);
            int newHeight = (int)(originalImage.Height * scaleFactor);

            Bitmap scaledImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return scaledImage;
        }
        private void CScaleMess_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox.BackgroundImage = loadedImage;
        }
        private Bitmap CreateSegmentBitmap(Bitmap sourceBitmap, int top, int bottom, int left, int right)
        {
            int segmentWidth = right - left + 1;
            int segmentHeight = bottom - top + 1;

            Rectangle segmentRect = new Rectangle(left, top, segmentWidth, segmentHeight);
            Bitmap segmentBitmap = sourceBitmap.Clone(segmentRect, sourceBitmap.PixelFormat);

            return segmentBitmap;
        }
        private void COCR_Click(object sender, EventArgs e)
        {
        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            refwidth = Convert.ToDouble(textBox1.Text);
            textrefresh();
        }
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            refwidth = 50;
            selectedRef = pictureBox8;
            textrefresh();
        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            refwidth = 100;
            selectedRef = pictureBox9;
            textrefresh();
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1 && !e.Shift && !e.Control && !e.Alt)
            {
                CHelp_Click(sender, new EventArgs());
            }
            else
            {
                ed = e;
            }
        }
        private void Main_KeyPress(object sender, KeyPressEventArgs e)
        {
           switch ((int)e.KeyChar)
            {
                case 22:
                    if (tabControl1.SelectedTab == CTAbstand)
                    {
                        Insert_Click(sender, new EventArgs());
                    }
                    else
                    {
                        CClipImage_Click(sender, new EventArgs());
                    }
                    break;

                case 3:
                        if (CFiles.SelectedItem != null)
                        {
                            Clipboard.SetText(CFiles.SelectedItem.ToString());
                        }

                    break;

                case 9:
                    if (CFiles.SelectedItem != null)
                    {
                        Clipboard.SetImage(CSave.BackgroundImage);
                    }
                    break;
            }
        }
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            refwidth = 12;
            selectedRef = pictureBox11;
            textrefresh();
        }
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            refwidth = 50;
            selectedRef = pictureBox10;
            textrefresh();
        }
        private void CTAnzeige_Click(object sender, EventArgs e)
        {

        }
        private void CNextAnzeige_Click(object sender, EventArgs e)
        {
            CNew_Click(sender, e);
        }
        private void CDataList_Click(object sender, EventArgs e)
        {
        }
        private void CDataList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = CDataList.SelectedIndex;
            int pos = idx - 1;
            while ((pos > -1) && (((String)CDataList.Items[pos])).Substring(0, 1) != "<")
                pos--;
            if (pos >-1)
            {
                String s = (String)CDataList.Items[pos];
                String s0 = (String)CDataList.Items[idx];
                if (s0.Substring(0, 1) != "<")
                {
                    switch (s)
                    {
                        case "<ort>;<mail>":
                            edit_Line1.Visible = false;
                            edit_Adress1.Line = s0;
                            edit_Adress1.Dock = DockStyle.Fill;
                            edit_Adress1.Visible = true;
                            break;

                        default:
                            edit_Adress1.Visible = false;
                            edit_Line1.Caption = s;
                            edit_Line1.Text = s0;
                            edit_Line1.Dock = DockStyle.Fill;
                            edit_Line1.Visible = true;
                            break;
                    }
                }
            }
        }
        private void edit_Adress1_Changed(object sender, EventArgs e)
        {
            if (CDataList.SelectedIndex != -1)
            {
               CDataList.Items[CDataList.SelectedIndex] = edit_Adress1.Line;
            }
        }
        private void edit_Line1_Changed(object sender, EventArgs e)
        {
            if (CDataList.SelectedIndex != -1)
            {
                CDataList.Items[CDataList.SelectedIndex] = edit_Line1.Text;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists("Data.txt") || MessageBox.Show("Die Datei existiert bereits. Möchten Sie sie überschreiben?", "Bestätigung", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (File.Exists("Data.bak"))
                    File.Delete("Data.bak");
                if (File.Exists("Data.txt"))
                    File.Move("Data.txt", "Data.bak");
                // CDataList.Items.Cast<object>().ToList().ForEach(item => File.WriteAllText("ComboBoxData.txt", item.ToString() + Environment.NewLine));
                File.AppendAllText("Data.txt", string.Join(Environment.NewLine, CDataList.Items.Cast<object>().Select(item => item.ToString())));

                Application.Restart();
                Environment.Exit(0); // Optional: Schließen Sie den aktuellen Prozess
            }
        }
        private void CSchaden_Click(object sender, EventArgs e)
        {

            if (COrt.SelectedItem!=null)
            {
                String[] items = ((String)COrt.SelectedItem).Split(';');
                if (items.Length > 4)
                {
                    PdfHelper pdfHelper = new PdfHelper();
                    CreatePDFFile("Mangel");
                    // PDFFilename = pdfHelper.ErstelleLEVPDF();
                    ShellExecute(IntPtr.Zero, "open", items[4], "", "", 5);
                    Clipboard.SetText(PDFFilename);
                    ShellExecute(IntPtr.Zero, "open", PDFFilename, "", "", 5);
                }
            }
        }
        private void CKennzeichen_GotFocus(object sender, EventArgs e)
        {
            CPixeln.Checked = false;
        }
        private void CKennzeichen_LostFocus(object sender, EventArgs e)
        {
            CPixeln.Checked = true;
        }
        private void smallToolbox1_ClickTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
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
                        CZeitBis.Text = "";
                        CAusschnitt.Hide();
                        CKennzeichen.Text = "";
                        CLogo.BackgroundImage = null;
                        CFreeText.Text = "";
                        panel1.BackColor = Color.Gold;
                        cntpixel = 0;
                        CPixeln.Checked = true;
                        Init();
                    }
                    break;
                case 1:
                    {
                        CNew_Click(sender, new SmallToolbox.ClickToolEventArgs(0, "user", ""));
                        CreateDirectoryIfNotExists(ZZielpfad + "Download");
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
                    break;
                case 2:
                    {
                        // Oberbilker Allee 98, 40227 Düsseldorf
                        String[] items = Clipboard.GetText().Split(',');

                        if (items.Length == 2)
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
                    break;
            }
        }
        private void smallToolbox2_ClickTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
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
                            ScaleAndSaveImage(Bild, fullfilename, 0.5);
                            filelist.Add(fullfilename);
                        }
                        if (File.Exists(fullpath + "\\Anzeige.WH2"))
                        {
                            File.Delete(fullpath + "\\Anzeige.WH2");
                        }
                        File.WriteAllText(fullpath + "\\Anzeige.WH2", InitValues);

                    }
                    break;
                case 1:
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
                    break;
                case 2:
                    {
                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Filter = "Video Files|*.mp4;*.avi;*.mkv|All Files|*.*";
                        dlg.Title = "Select a Video File";
                        dlg.InitialDirectory = ZZielpfad + "Download";
                        CreateDirectoryIfNotExists(ZZielpfad + "Download");
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            ShellExecute(IntPtr.Zero, "open", dlg.FileName, "", "", 5);
                        }

                    }
                    break;
                case 3:
                    {
                        // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
                        ShellExecute(IntPtr.Zero, "open", FullPath, "", "", 5);
                    }
                    break;
                case 4:
                    {
                        String s = SaveClipboardImageAsTemporaryFile();
                        if (s != null)
                        {
                            SelectFile(s);
                            CFiles.Items.Add(s);
                            // _Files.Add(s);
                            // CFiles.Items.Add(s);
                            // CSave.BackgroundImage = Bitmap.FromFile(s);
                        }
                        CAnzeigeText.Text = Message;
                        this.Refresh();
                    }
                    break;
            }
        }
        private void smallToolbox3_ClickTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
                    {
                        String url = ortssuche;
                        url = url.Replace("<strasse>", Strasse);
                        url = url.Replace("<hn>", HN);
                        url = url.Replace("<plz>", PLZ);
                        url = url.Replace("<ort>", Ort);
                        // ortssuche
                        ShellExecute(IntPtr.Zero, "open", url, "", "", 5);
                    }
                    break;
                case 1:
                    {
                        assistent dlg = new assistent();
                        dlg.masterform = this;
                        this.StartPosition = FormStartPosition.Manual;
                        this.Location = Screen.AllScreens[0].WorkingArea.Location;
                        this.Width = Screen.AllScreens[0].WorkingArea.Width - dlg.Width;

                        // Hier setzen wir die Position des zweiten Fensters auf der rechten Seite des Desktops.
                        dlg.StartPosition = FormStartPosition.Manual;
                        dlg.Location = new Point(this.Left + this.Width, this.Top);
                        dlg.Show();
                    }
                    break;
                case 2:
                    {
                        // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
                        ConfigEditorForm dlg = new ConfigEditorForm();
                        dlg.ShowDialog();
                    }
                    break;
                case 3:
                    {
                        // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
                        EditTemplate dlg = new EditTemplate();
                        dlg.Template = Template;
                        dlg.ShowDialog();
                        Template = dlg.Template;
                        Application.Restart();
                        Environment.Exit(0); // Optional: Schließen Sie den aktuellen Prozess
                    }
                    break;
                case 4:
                    {
                        ShellExecute(IntPtr.Zero, "open", "hilfe.pdf", "", "", 5);
                    }
                    break;
            }
        }
        private void smallToolbox4_ClickTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
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
                    break;
                case 1:
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
                                CVerstoss.Items.Clear();
                                Init();

                                foreach (string line in lines)
                                {
                                    AddVerstoss(line);
                                    CVerstossaus.Items.Remove(line);
                                }
                            }
                        }
                    }
                    break;
            }
        }
        private void smallToolbox5_ClickTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
                    {
                        CVerstossaus.Items.Clear();
                        foreach (String i in CVerstoss.Items)
                        {
                            AddVerstoss(i);
                        }
                        CVerstoss.Items.Clear();
                        CAnzeigeText.Text = Message;
                    }
                    break;
                case 1:
                    {
                        if (CVerstoss.SelectedItem != null)
                        {

                            CVerstossaus.Items.Add(CVerstoss.SelectedItem);
                            RemoveVerstoss((String)CVerstoss.SelectedItem);
                        }
                        CAnzeigeText.Text = Message;
                    }
                    break;
                case 2:
                    {
                        if (CVerstossaus.SelectedItem != null)
                        {
                            AddVerstoss((String)CVerstossaus.SelectedItem);
                            CVerstossaus.Items.Remove(CVerstossaus.SelectedItem);
                        }
                        CAnzeigeText.Text = Message;
                    }
                    break;
                case 3:
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
                    break;
            }
        }
        private void smallToolbox_EnterTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            SmallToolbox s = (SmallToolbox)sender;
            toolTip1.SetToolTip(e.s, e.toolTipText);
            toolTip1.Show(e.toolTipText, s);
            toolTip1.ShowAlways = true; 
        }
        private void smallToolbox_LeaveTool(object sender, SmallToolbox.ClickToolEventArgs e)
        {
            SmallToolbox s = (SmallToolbox)sender;
            toolTip1.SetToolTip(e.s, "");
            toolTip1.Hide(s);
            toolTip1.ShowAlways = false;
        }


        /// Obsolet
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
            CZeitBis.Text = "";
            CAusschnitt.Hide();
            CKennzeichen.Text = "";
            CLogo.BackgroundImage = null;
            CFreeText.Text = "";
            panel1.BackColor = Color.Gold;
            cntpixel = 0;
            CPixeln.Checked = true;
            Init();
        }
        /// <summary>
        /// lade Bilder für eine Anzeige
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CLoadPic_Click(object sender, EventArgs e)
        {
            CNew_Click(sender, e);
            CreateDirectoryIfNotExists(ZZielpfad + "Download");
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
        private void CClip_Click(object sender, EventArgs e)
        {
            // Oberbilker Allee 98, 40227 Düsseldorf
            String[] items = Clipboard.GetText().Split(',');

            if (items.Length == 2)
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
                ScaleAndSaveImage(Bild, fullfilename, 0.5);
                filelist.Add(fullfilename);
            }
            if (File.Exists(fullpath + "\\Anzeige.WH2"))
            {
                File.Delete(fullpath + "\\Anzeige.WH2");
            }
            File.WriteAllText(fullpath + "\\Anzeige.WH2", InitValues);
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
        private void CLupe_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Video Files|*.mp4;*.avi;*.mkv|All Files|*.*";
            dlg.Title = "Select a Video File";
            dlg.InitialDirectory = ZZielpfad + "Download";
            CreateDirectoryIfNotExists(ZZielpfad + "Download");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ShellExecute(IntPtr.Zero, "open", dlg.FileName, "", "", 5);
            }
        }
        private void CDirOpen_Click(object sender, EventArgs e)
        {
            // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
            ShellExecute(IntPtr.Zero, "open", FullPath, "", "", 5);
        }
        private void CClipImage_Click(object sender, EventArgs e)
        {
            String s = SaveClipboardImageAsTemporaryFile();
            if (s != null)
            {
                SelectFile(s);
                CFiles.Items.Add(s);
                // _Files.Add(s);
                // CFiles.Items.Add(s);
                // CSave.BackgroundImage = Bitmap.FromFile(s);
            }
            CAnzeigeText.Text = Message;
            this.Refresh();
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
        private void button3_Click(object sender, EventArgs e)
        {
            assistent dlg = new assistent();
            dlg.masterform = this;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Screen.AllScreens[0].WorkingArea.Location;
            this.Width = Screen.AllScreens[0].WorkingArea.Width - dlg.Width;

            // Hier setzen wir die Position des zweiten Fensters auf der rechten Seite des Desktops.
            dlg.StartPosition = FormStartPosition.Manual;
            dlg.Location = new Point(this.Left + this.Width, this.Top);
            dlg.Show();
        }
        private void CSettings_Click(object sender, EventArgs e)
        {
            // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
            ConfigEditorForm dlg = new ConfigEditorForm();

            dlg.ShowDialog();
        }
        private void CText_Click(object sender, EventArgs e)
        {
            // ShellExecute(IntPtr.Zero, "open", Directory.GetCurrentDirectory(), "", "", 5);
            EditTemplate dlg = new EditTemplate();
            dlg.Template = Template;
            dlg.ShowDialog();
            Template = dlg.Template;
            Application.Restart();
            Environment.Exit(0); // Optional: Schließen Sie den aktuellen Prozess
        }
        private void CHelp_Click(object sender, EventArgs e)
        {
            ShellExecute(IntPtr.Zero, "open", "hilfe.pdf", "", "", 5);
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
                    CVerstoss.Items.Clear();
                    Init();

                    foreach (string line in lines)
                    {
                        AddVerstoss(line);
                        CVerstossaus.Items.Remove(line);
                    }
                }
            }
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
        private void CToo_Click(object sender, EventArgs e)
        {
            if (CVerstossaus.SelectedItem != null)
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

        private void panel_ClickBack(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Gold;
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
        }
    }
}
