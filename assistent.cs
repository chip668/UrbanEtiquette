using System;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class assistent : Form
    {
        public Main masterform;
        public String[,] command =
        {
            { "Neu", "Haben Sie eine Videoaufzeichung oder ein Foto", "Beenden", "Foto", "Video"},
            { "Foto", "Bitte kopieren Sie die Adresse aus google maps (in  die Zwischenablage).", "Marke", "", ""},
            { "Video", "Machen Sie ein Bildschirmfoto von dem Verstoss", "Marke", "", ""},
            { "Marke", "Wählen Sie die Automarke aus.", "Verstoss", "", ""},
            { "Verstoss", "Wählen Sie alle Verstöße aus. Oder wählen sie einen Vorgegeben Verstoß.", "Auswahl", "Vorlage", ""},
            { "Auswahl", "Wählen Sie bitte Farbe.", "Farbe", "", ""},
            { "Vorlage", "Wählen Sie bitte Farbe.", "Farbe", "", ""},
            { "Farbe", "Wählen Sie bitte Farbe.", "Kennzeichen", "", ""},
            { "Kennzeichen", "Verstoß anzeigen.", "Anzeigen", "", ""},
            { "Anzeigen", "Assistent schliessen", "Neu", "Beenden", ""},
        };
        public assistent()
        {
            InitializeComponent();
            SetForm(command, 0);
        }
        public void SetForm(String[,] cmd, int idx)
        {
            if ((0 <= cmd.Length / 5) && (idx < cmd.Length / 5))
            {
                this.Text = cmd[idx, 0];
                CDisplay.Text = cmd[idx, 1];
                CBT1.Text = cmd[idx, 2];
                CBT1.Visible = (CBT1.Text != "");
                CBT2.Text = cmd[idx, 3];
                CBT2.Visible = (CBT2.Text != "");
                CBT3.Text = cmd[idx, 4];
                CBT3.Visible = (CBT3.Text != "");
            }
        }
        public void SetNext(String text)
        {
            if (text == "Beenden")
                this.Close();

            int next = FindIndex(text);
            SetForm(command, next);
        }

        public int FindIndex(String text)
        {
            int result = 0;
            for (int i = 0; i < command.Length / 5; i++)
            {
                if (text == command[i, 0])
                    result = i;
            }
            return result;
        }

        private void CBT_Click(object sender, EventArgs e)
        {
            String command = ((Control)sender).Text;
            masterform.MasterFormExecute(command);
            SetNext(command);
        }

        private void assistent_Load(object sender, EventArgs e)
        {

        }
    }
}
