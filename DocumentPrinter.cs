using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anzeige
{
    public class DocumentPrinter
    {
        Font printFont;
        string template;
        private readonly Dictionary<string, string> keys = new Dictionary<string, string>();

        public DocumentPrinter(string template, string familyname = "Consolas", int fonsize = 10, FontStyle style = FontStyle.Regular)
        {
            printFont = new Font(familyname, fonsize, style);
            this.template = template;
        }
        public DocumentPrinter(string template, Font font)
        {

            printFont = font;
            this.template = template;
        }
        public static DocumentPrinter CreateDocumentPrinterByFile(string templatepath, string familyname = "Consolas", int fonsize = 10, FontStyle style = FontStyle.Regular)
        {
            String template = File.ReadAllText(templatepath);
            return DocumentPrinter.CreateDocumentPrinterByText(template, familyname, fonsize, style);
        }
        public static DocumentPrinter CreateDocumentPrinterByText(string template, string familyname = "Consolas", int fonsize = 10, FontStyle style = FontStyle.Regular)
        {
            return new DocumentPrinter(template, familyname, fonsize, style);
        }

        // -------------------------
        // AddKey
        // -------------------------
        public void AddKey(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            keys[key] = value ?? string.Empty;
        }

        // -------------------------
        // AddKeys
        // -------------------------
        public void AddKeys(Dictionary<string, string> newKeys)
        {
            if (newKeys == null)
                return;

            foreach (var pair in newKeys)
                keys[pair.Key] = pair.Value ?? string.Empty;
        }

        // -------------------------
        // SetTemplate
        // -------------------------
        public void SetTemplate(string newTemplate)
        {
            template = newTemplate ?? string.Empty;
            keys.Clear(); // optional, aber logisch konsistent
        }
        void backup(string filename)
        {
            string backupPath = filename + ".bak";
            if (File.Exists(filename))
            {
                if (File.Exists(backupPath))
                    File.Delete(backupPath);

                File.Move(filename, backupPath);
            }
        }

        // -------------------------
        // PrintTemplate
        // -------------------------
        public void PrintTemplate(string printerName = null)
        {
            string text = template;

            foreach (var pair in keys)
                text = text.Replace("{" + pair.Key + "}", pair.Value);

            /*
            PrintDocument pd = new PrintDocument();

            if (!string.IsNullOrWhiteSpace(printerName))
                pd.PrinterSettings.PrinterName = printerName;

            pd.PrintPage += (sender, e) =>
            {
                e.Graphics.DrawString(result, printFont, Brushes.Black,
                    e.MarginBounds.Left,
                    e.MarginBounds.Top);
            };

            pd.Print();
            */
            

            var doc = new PrintDocument();
            Font printFont = new Font("Consolas", 10);

            int currentChar = 0;

            doc.PrintPage += (s, e) =>
            {
                int charsFitted;
                int linesFilled;

                e.Graphics.MeasureString(
                    text.Substring(currentChar),
                    printFont,
                    e.MarginBounds.Size,
                    StringFormat.GenericTypographic,
                    out charsFitted,
                    out linesFilled);

                e.Graphics.DrawString(
                    text.Substring(currentChar),
                    printFont,
                    Brushes.Black,
                    e.MarginBounds,
                    StringFormat.GenericTypographic);

                currentChar += charsFitted;

                e.HasMorePages = currentChar < text.Length;
            };

            doc.Print();

        }
    }
}
