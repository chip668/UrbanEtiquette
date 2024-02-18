using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class ConfigEditorForm : Form
    {
        // private string configFile = "config.txt";
        private Dictionary<string, string> configValues;
        private string _Configfile;
        public String Configfile
        {
            get
            {
                return _Configfile;
            }
            set
            {
                _Configfile = value;
                CreateForm();
            }

        }
        public ConfigEditorForm()
        {
            InitializeComponent();
            Configfile = "config.txt";
        }

        private void CreateForm()
        {
            String[] lines = File.ReadAllLines(Configfile);
            this.splitContainer1.Panel1.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Clear();
            int offset = 0;
            String name = "";
            TextBox tbx = null;
            Label lbl = null;
            int width = 0;
            foreach (String ln in lines)
            {
                if (ln.Length > 4 && ln.Substring(0, 2) == "<z" && ln.Substring(ln.Length - 1, 1) == ">")
                {
                    name = (ln.Substring(2, 1).ToUpper()) + (ln.Substring(3, ln.Length - 4));
                    tbx = new TextBox();
                    lbl = new Label();
                    tbx.Name = name;
                    lbl.Name = "lbl" + name;
                    lbl.Text = name;
                    tbx.Top = offset;
                    lbl.Top = offset;
                    width = Math.Max(width, lbl.Width);
                    this.splitContainer1.SplitterDistance = width + 15;
                    tbx.Width = this.splitContainer1.Panel2.Width;
                    this.splitContainer1.Panel1.Controls.Add(lbl);
                    this.splitContainer1.Panel2.Controls.Add(tbx);
                    offset += (tbx.Height + 3);
                }
                else
                {
                    if (tbx != null)
                    {
                        tbx.Text = ln;
                    }
                }
            }

            Button btnSave = new Button();
            btnSave.Name = "btnSpeichern";
            btnSave.Text = "Speichern";
            btnSave.Width = 100;
            btnSave.Height = 33;
            btnSave.Top = offset - 10;
            btnSave.Left = 0;
            btnSave.Click += btnSave_Click;  // Add event handler if needed
            this.splitContainer1.Panel2.Controls.Add(btnSave);

            Button btnCancel = new Button();
            btnCancel.Name = "btnAbbrechen";
            btnCancel.Text = "Abbrechen";
            btnCancel.Width = 100;
            btnCancel.Height = 33;
            btnCancel.Top = offset - 10;
            btnCancel.Left = btnSave.Width+10;  // Adjust the spacing between buttons
            btnCancel.Click += btnCancel_Click;  // Add event handler if needed
            this.splitContainer1.Panel2.Controls.Add(btnCancel);

            this.Height = btnCancel.Top + btnCancel.Height + 100;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<String> lines = new List<String>();
            foreach (Control i in this.splitContainer1.Panel2.Controls)
            {
                if (i.Name != "btnAbbrechen" && i.Name != "btnSpeichern")
                {
                    lines.Add("<z" + i.Name.ToLower() + ">");
                    lines.Add(i.Text);
                }
            }
            if (File.Exists(Configfile))
                File.Delete(Configfile);
            File.WriteAllLines(Configfile, lines);
            Application.Restart();
            Environment.Exit(0); // Optional: Schließen Sie den aktuellen Prozess
        }

        private void ConfigEditorForm_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
