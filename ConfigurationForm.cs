using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anzeige
{
    public class ConfigurationForm : Form
    {
        private EmailSender emailSender;
        private TextBox smtpServerTextBox;
        private TextBox smtpPortTextBox;
        private TextBox smtpUsernameTextBox;
        private TextBox smtpPasswordTextBox;
        private Button saveButton;

        public ConfigurationForm(EmailSender emailSender)
        {
            this.emailSender = emailSender;

            InitializeComponents();
            LoadConfiguration();
        }

        private void InitializeComponents()
        {
            // Initialisiere alle Steuerelemente und Layouts für das Formular
            // ...

            // Beispiel:
            smtpServerTextBox = new TextBox();
            smtpServerTextBox.Location = new Point(10, 10);
            this.Controls.Add(smtpServerTextBox);

            smtpPortTextBox = new TextBox();
            smtpPortTextBox.Location = new Point(10, 40);
            this.Controls.Add(smtpPortTextBox);

            smtpUsernameTextBox = new TextBox();
            smtpUsernameTextBox.Location = new Point(10, 70);
            this.Controls.Add(smtpUsernameTextBox);

            smtpPasswordTextBox = new TextBox();
            smtpPasswordTextBox.Location = new Point(10, 100);
            this.Controls.Add(smtpPasswordTextBox);

            saveButton = new Button();
            saveButton.Location = new Point(10, 130);
            saveButton.Text = "Save";
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            // Weitere Steuerelemente entsprechend hinzufügen...
        }

        private void LoadConfiguration()
        {
            // Setze die Textboxen mit den geladenen Werten
            smtpServerTextBox.Text = emailSender.smtpServer;
            smtpPortTextBox.Text = emailSender.smtpPort.ToString();
            smtpUsernameTextBox.Text = emailSender.smtpUsername;
            smtpPasswordTextBox.Text = emailSender.smtpPassword;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Aktualisiere die EmailSender-Klasse mit den neuen Werten
            emailSender.UpdateConfiguration(
                smtpServerTextBox.Text,
                int.TryParse(smtpPortTextBox.Text, out int port) ? port : 0,
                smtpUsernameTextBox.Text,
                smtpPasswordTextBox.Text
            );

            // Prüfe, ob die Daten gültig sind
            if (emailSender.IsValidConfiguration())
            {
                // Schließe das Formular und setze den DialogResult auf OK
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Ungültige Konfigurationsdaten. Die Daten wurden nicht gespeichert.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
