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
    public partial class EditTemplate : Form
    {
        public EditTemplate()
        {
            InitializeComponent();
        }
        
        private void Form3_Load(object sender, EventArgs e)
        {
            textBox1.Text = ((Main)this.Parent).Template;
        }

        private void CTake_Click(object sender, EventArgs e)
        {
            string insertText = comboBox1.Text;

            int selectionStart = textBox1.SelectionStart;
            int selectionLength = textBox1.SelectionLength;

            // Überprüfen, ob Text markiert ist
            if (selectionLength > 0)
            {
                // Markierter Text ersetzen
                textBox1.Text = textBox1.Text.Remove(selectionStart, selectionLength);
                textBox1.Text = textBox1.Text.Insert(selectionStart, insertText);
                textBox1.SelectionStart = selectionStart + insertText.Length;
            }
            else
            {
                // Kein markierter Text, füge den Text an der aktuellen Cursorposition ein
                textBox1.Text = textBox1.Text.Insert(selectionStart, insertText);
                textBox1.SelectionStart = selectionStart + insertText.Length;
            }
        }

        private void CSave_Click(object sender, EventArgs e)
        {
            ((Main)this.Parent).Template = textBox1.Text;
            Application.Restart();
            Environment.Exit(0); // Optional: Schließen Sie den aktuellen Prozess
        }

        private void CAbbrechen_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
