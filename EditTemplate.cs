using System;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class EditTemplate : Form
    {
        public String Template { get; set; }
        public EditTemplate()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            textBox1.Text = Template;
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
            Template = textBox1.Text;
            this.Hide();
        }

        private void CAbbrechen_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
