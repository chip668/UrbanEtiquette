using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Anzeige
{
    public class PassPrintCanvas
    {
        private FarradPass _pass;
        private int _currentPageIndex = 0;
        private List<Image> _images = new List<Image>();
        private List<string> _textPages = new List<string>();

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        public PassPrintCanvas(FarradPass pass)
        {
            _pass = pass ?? throw new ArgumentNullException(nameof(pass));
            LoadImages();
            LoadTextPages();
        }

        private void LoadImages()
        {
            foreach (var file in _pass.FotoDateien)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        var img = Image.FromFile(file);
                        _images.Add(img);
                    }
                    catch
                    {
                        // Ignorieren, geht in ShellExecute fallback
                        ShellExecute(IntPtr.Zero, "print", file, "", "", 5);
                    }
                }
            }
        }

        private void LoadTextPages()
        {
            // Falls die Datei eine Textdatei ist (txt)
            foreach (var file in _pass.FotoDateien)
            {
                if (Path.GetExtension(file).ToLower() == ".txt")
                {
                    try
                    {
                        string text = File.ReadAllText(file);
                        _textPages.Add(text);
                    }
                    catch
                    {
                        ShellExecute(IntPtr.Zero, "print", file, "", "", 5);
                    }
                }
            }
        }

        PrintDocument pd;
        public void Print()
        {
            pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;
            pd.Print();
        }

        private int _currentImageIndex = 0;
        private int _currentTextFileIndex = 0;
        private int _currentTextLineIndex = 0;
        private float _currentY = 20;
        private bool _passPrinted = false;

        private Rectangle GetScaledToPage(Image img, Rectangle bounds, float topMargin)
        {
            float maxWidth = bounds.Width - 40;
            float maxHeight = bounds.Height - topMargin - 20;

            float ratioX = maxWidth / img.Width;
            float ratioY = maxHeight / img.Height;
            float ratio = Math.Min(ratioX, ratioY); // Zoom, kein Stretch

            int width = (int)(img.Width * ratio);
            int height = (int)(img.Height * ratio);

            int x = bounds.Left + (bounds.Width - width) / 2;
            int y = bounds.Top + (int)topMargin;

            return new Rectangle(x, y, width, height);
        }
        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float lineHeight = 20;
            Font fontHeader = new Font("Arial", 14, FontStyle.Bold);
            Font fontRegular = new Font("Arial", 12);

            float y = _currentY;

            // 1. Passdaten drucken, nur wenn noch nicht gedruckt
            if (!_passPrinted)
            {
                // 1. Passdaten drucken
                g.DrawString("Fahrradpass", fontHeader, Brushes.Black, 20, y); y += lineHeight * 2;
                g.DrawString($"Hersteller: {_pass.Hersteller}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Modell: {_pass.Modell}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Farbe: {_pass.Farbe}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Rahmennummer: {_pass.Rahmennummer}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Typ: {_pass.Fahrradtyp}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Rahmengröße: {_pass.Rahmengröße}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Reifengröße: {_pass.Reifengröße}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Kaufpreis: {_pass.Kaufpreis.ToString(CultureInfo.InvariantCulture)} €", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Zeitwert: {_pass.Zeitwert.ToString(CultureInfo.InvariantCulture)} €", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Kaufdatum: {_pass.Kaufdatum:yyyy-MM-dd}", fontRegular, Brushes.Black, 20, y); y += lineHeight * 2;

                // Codierung & Diebstahlinfos
                g.DrawString($"Codiert: {_pass.IstCodiert}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"CodierungsID: {_pass.CodierungsID}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Schlosstyp: {_pass.SchlossTyp}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Schlossmarke: {_pass.SchlossMarke}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                g.DrawString($"Händler: {_pass.Haendler}", fontRegular, Brushes.Black, 20, y); y += lineHeight;

                // Merkmale
                string[] items = _pass.Merkmale.Split("[crlf]");
                if (items.Any())
                {
                    g.DrawString("Merkmale:", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                    foreach (var m in items)
                    {
                        g.DrawString($"    {m}", fontRegular, Brushes.Black, 40, y); y += lineHeight;
                    }
                }

                _passPrinted = true;
            }

            _currentY = y;

            // 2. Bilder drucken
            // === BILDER ===
            // exakt 1 Bild pro Seite
            if (_currentImageIndex < _images.Count)
            {
                var img = _images[_currentImageIndex];

                // Bild IMMER passend skalieren – egal wie groß
                Rectangle target = GetScaledToPage(img, e.MarginBounds, _currentY);

                g.DrawImage(img, target);

                _currentImageIndex++;        // 👈 Fortschritt!
                _currentY = 20;              // neue Seite startet sauber

                e.HasMorePages = true;
                return;
            }

            // 3. Textdateien drucken
            while (_currentTextFileIndex < _textPages.Count)
            {
                var lines = _textPages[_currentTextFileIndex]
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                while (_currentTextLineIndex < lines.Length)
                {
                    if (_currentY + lineHeight > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return; // Zustand bleibt → OK, weil Y sich ändert
                    }

                    g.DrawString(lines[_currentTextLineIndex], fontRegular, Brushes.Black, 20, _currentY);
                    _currentY += lineHeight;
                    _currentTextLineIndex++;
                }

                _currentTextLineIndex = 0;
                _currentTextFileIndex++;
            }

            // Fertig
            e.HasMorePages = false;

            // Reset Status für nächsten Druck
            _currentY = 20;
            _currentImageIndex = 0;
            _currentTextFileIndex = 0;
            _currentTextLineIndex = 0;
            _passPrinted = false;
        }
        private void Pd_EndPrint(object sender, PrintEventArgs e)
        {
            pd.Dispose();
            pd = null;
        }
        private void Pd_PrintPage_old2(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float y = 20;
            float lineHeight = 20;
            Font fontHeader = new Font("Arial", 14, FontStyle.Bold);
            Font fontRegular = new Font("Arial", 12);

            // 1. Passdaten drucken
            g.DrawString("Fahrradpass", fontHeader, Brushes.Black, 20, y);
            y += lineHeight * 2;

            g.DrawString($"Hersteller: {_pass.Hersteller}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Modell: {_pass.Modell}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Farbe: {_pass.Farbe}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Rahmennummer: {_pass.Rahmennummer}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Typ: {_pass.Fahrradtyp}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Rahmengröße: {_pass.Rahmengröße}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Reifengröße: {_pass.Reifengröße}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Kaufpreis: {_pass.Kaufpreis.ToString(CultureInfo.InvariantCulture)} €", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Zeitwert: {_pass.Zeitwert.ToString(CultureInfo.InvariantCulture)} €", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Kaufdatum: {_pass.Kaufdatum:yyyy-MM-dd}", fontRegular, Brushes.Black, 20, y); y += lineHeight * 2;

            // Codierung & Diebstahlinfos
            g.DrawString($"Codiert: {_pass.IstCodiert}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"CodierungsID: {_pass.CodierungsID}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Schlosstyp: {_pass.SchlossTyp}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Schlossmarke: {_pass.SchlossMarke}", fontRegular, Brushes.Black, 20, y); y += lineHeight;
            g.DrawString($"Händler: {_pass.Haendler}", fontRegular, Brushes.Black, 20, y); y += lineHeight;

            // Merkmale
            string[] items = _pass.Merkmale.Split("[crlf]");
            if (items.Any())
            {
                g.DrawString("Merkmale:", fontRegular, Brushes.Black, 20, y); y += lineHeight;
                foreach (var m in items)
                {
                    g.DrawString($"- {m}", fontRegular, Brushes.Black, 40, y); y += lineHeight;
                }
            }

            y += lineHeight;

            // 2. Bilder drucken
            foreach (var img in _images)
            {
                if (y + 300 > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
                Rectangle targetRect = GetScaledRectangle(img, e.MarginBounds.Width - 40, 300, 20, (int)y);
                g.DrawImage(img, targetRect);
                y += targetRect.Height + lineHeight;
            }

            // 3. Textdateien drucken
            foreach (var txt in _textPages)
            {
                var lines = txt.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    if (y + lineHeight > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                    g.DrawString(line, fontRegular, Brushes.Black, 20, y);
                    y += lineHeight;
                }
            }

            e.HasMorePages = false;
        }
        private Rectangle GetScaledRectangle(Image img, int maxWidth, int maxHeight, int x, int y)
        {
            float scale = Math.Min((float)maxWidth / img.Width, (float)maxHeight / img.Height);
            int width = (int)(img.Width * scale);
            int height = (int)(img.Height * scale);
            return new Rectangle(x, y, width, height);
        }
    }
}
