﻿using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Text;
using System.IO;

namespace Anzeige
{
    /// <summary>
    /// Hilfsklasse um aus einer HTML Vorlage und zugehörigen Daten neine PDF Datei zu erzeugen
    /// </summary>
    public class PdfHelper
    {
        public string anrede = "Herr";
        public string nameVorname;
        public string anschrift;
        public string telefon;
        public string email;
        public string tatdatum;
        public string tatzeitVon;
        public string tatzeitBis;
        public string tatort;
        public string kennzeichen;
        public string fahrzeugtyp;
        public string tatvorwurf;
        private string _ort;
        public string ort
        {
            get { return _ort; }
            set 
            { 
                _ort = value;
                template = ort;
            }
        }
        public string template;
        public string[] files = null;
        public string htmlCode
        {
            get
            {
                if (files != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        // string imageHtml = $"<a href=\"{fi.Name}\" target=\"_blank\"><img src=\"{fi.Name}\" alt=\"{fi.FullName}\"></a>";
                        string imageHtml = $"            <img src=\"{fi.FullName}\" alt=\"{fi.Name}\">";
                        sb.AppendLine(imageHtml);
                    }
                    return sb.ToString();
                }

                // Falls keine Dateien vorhanden sind
                return "Keine Dateien vorhanden.";
            }
        }

        /// <summary>
        /// ERzeuigt die PDF Datei
        /// </summary>
        /// <returns></returns>
        public string ErstellePDF()
        {
            // Erstellen Sie einen temporären Dateinamen für die PDF-Datei
            string tempPdfFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");

            // Erstellen Sie ein Dokument und öffnen Sie die Datei zum Schreiben
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(tempPdfFileName, FileMode.Create));
            doc.Open();

            // Fügen Sie Texte zur PDF-Datei hinzu
            FügeTextZurPDFHinzu(doc, "Dies ist der erste Text.");
            FügeTextZurPDFHinzu(doc, "Hier ist ein weiterer Text.");

            // Schließen Sie das Dokument und speichern Sie die PDF-Datei
            doc.Close();

            return tempPdfFileName;
        }
        private string ErsetzePlatzhalter(string htmlContent)
        {
            htmlContent = htmlContent.Replace("{anrede}", anrede)
                                     .Replace("{nameVorname}", nameVorname)
                                     .Replace("{anschrift}", anschrift)
                                     .Replace("{telefon}", telefon)
                                     .Replace("{email}", email)
                                     .Replace("{tatdatum}", tatdatum)
                                     .Replace("{tatzeitVon}", tatzeitVon)
                                     .Replace("{tatzeitBis}", tatzeitBis)
                                     .Replace("{tatort}", tatort)
                                     .Replace("{kennzeichen}", kennzeichen)
                                     .Replace("{fahrzeugtyp}", fahrzeugtyp)
                                     .Replace("{tatvorwurf}", tatvorwurf)
                                     .Replace("{ort}", ort)
                                     .Replace("{bilder}", htmlCode)
                                     ;

            return htmlContent;
        }

        public string ErstelleLEVPDF()
        {
            // Erstellen Sie einen temporären Dateinamen für die PDF-Datei
            string tempPdfFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");

            // Erstellen Sie ein Dokument und öffnen Sie die Datei zum Schreiben
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(tempPdfFileName, FileMode.Create));
            doc.Open();

            // Definieren Sie den fettgedruckten Text
            Font fettSchrift = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);

            // Ihr Text mit HTML-Tags für die Fettformatierung
            string text = $@"
            <b>Stadt Leverkusen</b><div style='text-align: right;'><b>Anzeige einer Verkehrsordnungswidrigkeit</b><br></div>
            < br>
            <b>Fachbereich Ordnung und Straßenverkehr</b><br>
            Zentrale Bußgeldstelle<br>
            Miselohestraße 4<br>
            51379 Leverkusen<br>
            Per Mail: 362-02@stadt.leverkusen.de<br>
            <br>
            <b>Angaben zur Anzeigenerstatterin oder zum Anzeigenerstatter</b><br>
            Anrede: <I><U>{anrede}</U></I><br>
            Name, Vorname:  <I><U>{nameVorname}</U></I><br>
            Anschrift:  <I><U>{anschrift}</U></I><br>
            Für Rückfragen:<br>  
            Telefon: <I><U>{telefon}</U></I><br>
            E-Mail:  <I><U>{email}</U></I><br>
            <br>
            <b>Angaben zur Verkehrsordnungswidrigkeit</b><br>
            Tatdatum:  <I><U>{tatdatum}</U></I><br>
            Tatzeit von  <I><U>{tatzeitVon}</U></I> Uhr bis  <I><U>{tatzeitBis}</U></I> Uhr<br>
            Tatort:  <I><U>{tatort}</U></I><br>
            <br>
            <b>Angaben zum Fahrzeug</b><br>
            Kennzeichen:  <I><U>{kennzeichen}</U></I><br>
            Fabrikat/Marke Fahrzeugtyp:  <I><U>{fahrzeugtyp}</U></I><br>
            <br>
            <b>Tatvorwurf/Art des Parkverstoßes (Bitte schildern Sie die Situation)</b><br>
            <I><U>{tatvorwurf}</U></I><br>
            <br>
            Zur weiteren Bearbeitung werden aussagekräftige Fotos benötigt, die den Verkehrsverstoß sowie
            das Kennzeichen, den Fahrzeugtyp und ggf. die Beschilderung erkennen lassen. Bei Parkverstößen
            auf Schwerbehindertenparkplätzen werden zusätzlich aussagekräftige Fotos von der Auslagefläche
            des betroffenen Fahrzeuges benötigt.<br>
            <br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>
            <div style='text-align: center;'>- 2 -</div>
            Durch die Anzeige erklärt sich die Anzeigenerstatterin/ der Anzeigenerstatter bereit als Zeugin/
            Zeuge für die angezeigte Ordnungswidrigkeit zur Verfügung zu stehen, so dass Sie in einem
            gegebenenfalls zu erlassenden Bußgeldbescheid als Zeuge oder Zeugin namentlich unter Angabe
            der ladungsfähigen Anschrift benannt werden und Sie davon ausgehen müssen, dass Sie bei einer
            eventuellen Hauptverhandlung vor dem Amtsgericht aussagen müssen.<br><br>
            Außerdem werden Sie hiermit darüber informiert, dass im Verfahren Ihr Name und Ihre Wohnsitzgemeinde
            gegenüber dem Fahrzeugführer oder Fahrzeughalter bzw. dem Betroffenen oder der
            Betroffenen angegeben werden. Eine Anonymität der Anzeigenerstatterin oder des Anzeigenerstatters
            ist nicht möglich. Anonyme Anzeigen werden nicht bearbeitet.<br><br>
            Ihre Anzeige wird an die Zentrale Bußgeldstelle beim Fachbereich Ordnung und Straßenverkehr
            weitergeleitet. Bitte haben Sie Verständnis dafür, dass in der Regel keine Antworten oder Aussagen
            zum Sachstand der Bearbeitung erfolgen können.<br><br>
            Das Formular wird ohne Unterschrift versendet.<br>
            ________________________________________________________________________________________________________<br>
            <b>nur vom Fachbereich Ordnung und Straßenverkehr auszufüllen:</b><br>
            Aktenzeichen: _____________________<br>
            Tatkennziffer:_____________________ <div style='text-align: right;'>Betrag EUR:<br>_____________________<br></div>";
            String filename = template + ".htm";
            if (File.Exists(filename))
            {
                text = File.ReadAllText(filename);
                text = ErsetzePlatzhalter(text);
            }

            // Verwenden Sie HTML-Parser, um den formatierten Text in das PDF-Dokument einzufügen
            using (var sr = new StringReader(text))
            {
                var parsedHtmlElements = HTMLWorker.ParseToList(sr, null);
                foreach (var htmlElement in parsedHtmlElements)
                {
                    doc.Add(htmlElement as IElement);
                }
            }

            // Schließen Sie das Dokument und speichern Sie die PDF-Datei
            doc.Close();

            return tempPdfFileName;
        }
        /// <summary>
        /// SWetzt Text
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="text"></param>
        private void FügeTextZurPDFHinzu(Document doc, string text)
        {
            Paragraph paragraph = new Paragraph(text, FontFactory.GetFont(FontFactory.HELVETICA, 12));
            doc.Add(paragraph);
        }
        /// <summary>
        /// Setzt Text Fett
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        private void FügeFettTextZurPDFHinzu(Document doc, string text, Font font)
        {
            Paragraph paragraph = new Paragraph();
            Chunk chunk = new Chunk(text, font);
            paragraph.Add(chunk);
            doc.Add(paragraph);
        }
    }
}