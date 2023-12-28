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
    public partial class ConfigEditorForm : Form
    {
        private string configFile = "Data2.txt";
        private Dictionary<string, string> configValues;

        public ConfigEditorForm()
        {
            InitializeComponent();
            configValues = ReadConfigFile(configFile);
            DisplayConfigValues();
        }

        private void DisplayConfigValues()
        {
            // Zeige die aktuellen Konfigurationswerte in den TextBoxen an
            textBoxName.Text = GetValue("zname");
            textBoxVorname.Text = GetValue("zvorname");
            textBoxStrasse.Text = GetValue("zstrasse");
            textBoxHausnummer.Text = GetValue("zhausnummer");
            textBoxPLZ.Text = GetValue("zplz");
            textBoxOrt.Text = GetValue("zort");
            textBoxZielpfad.Text = GetValue("zielpfad");
            textBoxOrtMail.Text = GetValue("ort_mail");
        }

        private Dictionary<string, string> ReadConfigFile(string filePath)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i += 2)
                {
                    string key = lines[i].Trim();
                    string value = lines[i + 1].Trim();
                    values[key] = value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Lesen der Konfigurationsdatei: {ex.Message}");
            }

            return values;
        }

        private void SaveConfigFile()
        {
            try
            {
                // Aktualisiere die Werte aus den TextBoxen
                SetValue("zname", textBoxName.Text);
                SetValue("zvorname", textBoxVorname.Text);
                SetValue("zstrasse", textBoxStrasse.Text);
                SetValue("zhausnummer", textBoxHausnummer.Text);
                SetValue("zplz", textBoxPLZ.Text);
                SetValue("zort", textBoxOrt.Text);
                SetValue("zielpfad", textBoxZielpfad.Text);
                SetValue("ort_mail", textBoxOrtMail.Text);

                // Schreibe die aktualisierten Werte zurück in die Datei
                File.WriteAllLines(configFile, new List<string>(configValues.Keys));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfigurationsdatei: {ex.Message}");
            }
        }

        private string GetValue(string key)
        {
            return configValues.ContainsKey(key) ? configValues[key] : string.Empty;
        }

        private void SetValue(string key, string value)
        {
            if (configValues.ContainsKey(key))
            {
                configValues[key] = value;
            }
            else
            {
                configValues.Add(key, value);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveConfigFile();
            MessageBox.Show("Konfiguration gespeichert!");
        }
    }
}
