using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Anzeige
{
    // Anzeige obj = new Anzeige(cntpixel, Files, Datum, Zeit, Ort, PLZ,
    //                      Strasse, HN, Kennzeichen, Marke, Farbe,
    //                      Verstoss, FreeText, Mail, ZName, ZVorname,
    //                      ZOrt, ZPLZ, ZStrasse, ZHausnummer, Message);

    class AnzeigeClass
    {
        public int cntpixel { get; set; }
        public string Files { get; set; }
        public string Datum { get; set; }
        public string Zeit { get; set; }
        public string Ort { get; set; }
        public string PLZ { get; set; }
        public string Strasse { get; set; }
        public string HN { get; set; }
        public string Kennzeichen { get; set; }
        public string Marke { get; set; }
        public string Farbe { get; set; } // Stellen Sie sicher, dass die Farbe als Color-Objekt deklariert ist
        public string Verstoss { get; set; }
        public string FreeText { get; set; }
        public string Mail { get; set; }
        public string ZName { get; set; }
        public string ZVorname { get; set; }
        public string ZOrt { get; set; }
        public string ZPLZ { get; set; }
        public string ZStrasse { get; set; }
        public string ZHausnummer { get; set; }
        public string Message { get; set; }
        public Color BackColor { get; set; }
        public void SetAnzeigeClass(int cntpixel, string files, string datum, string zeit, string ort, string plz,
                     string strasse, string hn, string kennzeichen, string marke, String farbe,
                     string verstoss, string freeText, string mail, string zName, string zVorname,
                     string zOrt, string zPLZ, string zStrasse, string zHausnummer, string message)
        {
            this.cntpixel = cntpixel;
            this.Files = files;
            this.Datum = datum;
            this.Zeit = zeit;
            this.Ort = ort;
            this.PLZ = plz;
            this.Strasse = strasse;
            this.HN = hn;
            this.Kennzeichen = kennzeichen;
            this.Marke = marke;
            this.Farbe = farbe;
            this.Verstoss = verstoss;
            this.FreeText = freeText;
            this.Mail = mail;
            this.ZName = zName;
            this.ZVorname = zVorname;
            this.ZOrt = zOrt;
            this.ZPLZ = zPLZ;
            this.ZStrasse = zStrasse;
            this.ZHausnummer = zHausnummer;
            this.Message = message;
        }
        public AnzeigeClass() { }
        public AnzeigeClass (int cntpixel, string files, string datum, string zeit, string ort, string plz,
                     string strasse, string hn, string kennzeichen, string marke, String farbe,
                     string verstoss, string freeText, string mail, string zName, string zVorname,
                     string zOrt, string zPLZ, string zStrasse, string zHausnummer, string message)
        {
            SetAnzeigeClass (cntpixel, files, datum, zeit, ort, plz,
                     strasse, hn, kennzeichen, marke, farbe,
                     verstoss, freeText, mail, zName, zVorname,
                     zOrt, zPLZ, zStrasse, zHausnummer, message);
        }
        // Hier sollte auch die AreColorsEqual-Methode vorhanden sein, um auf panel1.BackColor zuzugreifen
        static bool AreColorsEqual(Color color1, Color color2)
        {
            return color1.A == color2.A &&
                   color1.R == color2.R &&
                   color1.G == color2.G &&
                   color1.B == color2.B;
        }
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
            if (AreColorsEqual(SystemColors.Control, BackColor)) { MessageBox.Show("Bitte Farbe auswählen."); return false; }
            return true;
        }
        public void Speichern(string dateiPfad)
        {
            try
            {
                // Erstellen Sie einen XmlSerializer für die Klasse "Anzeige"
                XmlSerializer serializer = new XmlSerializer(typeof(AnzeigeClass));

                // Öffnen Sie eine Datei zum Schreiben
                using (FileStream stream = new FileStream(dateiPfad, FileMode.Create))
                {
                    // Serialisieren Sie die Instanz der Klasse und schreiben Sie sie in die Datei
                    serializer.Serialize(stream, this);
                }

                MessageBox.Show("Daten erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern der Daten: " + ex.Message);
            }
        }
        // Methode zum Laden der Daten aus einer XML-Datei
        public static AnzeigeClass Laden(string dateiPfad)
        {
            try
            {
                // Erstellen Sie einen XmlSerializer für die Klasse "Anzeige"
                XmlSerializer serializer = new XmlSerializer(typeof(AnzeigeClass));

                // Öffnen Sie eine Datei zum Lesen
                using (FileStream stream = new FileStream(dateiPfad, FileMode.Open))
                {
                    // Deserialisieren Sie die Daten aus der Datei und erstellen Sie eine Instanz der Klasse
                    AnzeigeClass geladeneAnzeige = (AnzeigeClass)serializer.Deserialize(stream);
                    MessageBox.Show("Daten erfolgreich geladen.");
                    return geladeneAnzeige;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Daten: " + ex.Message);
                return null;
            }
        }
    }
}
