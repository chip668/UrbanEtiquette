using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class Abstandsmeter : UserControl
    {
        public int _Abstand;
        public int Abstand
        {
            get { return _Abstand; }
            set 
            {
                _Abstand = value;
                CCar.Left = 400 -  _Abstand - CCar.Width; 
                if (CCar.Left + CCar.Width > CBike.Left)
                {
                    CBike.BackgroundImage = CTemplate.BackgroundImage;
                }
                else
                {
                    CBike.BackgroundImage = CTemplate.Image;
                }
            }
        }
        public int Rechts { get; private set; }
        public int LinksVorne { get; private set; }
        public int LinksHinten { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }
        public String Zeit { get; private set; }
        public string Line
        {
            get => $"{Rechts},{LinksVorne},{LinksHinten},{Longitude},{Latitude},{Zeit}";
            set
            {
                string[] values = value.Split(',');

                if (values.Length == 6)
                {
                    Rechts = Convert.ToInt32(values[0]);
                    LinksVorne = Convert.ToInt32(values[1]);
                    LinksHinten = Convert.ToInt32(values[2]);
                    Longitude = Convert.ToDouble(values[3]);
                    Latitude = Convert.ToDouble(values[4]);
                    Zeit = values[5];
                }
                else
                {
                    throw new ArgumentException("Ungültige Anzahl von Werten im Eingabestring.");
                }
            }
        }

        public Abstandsmeter()
        {
            InitializeComponent();
        }

        private void Abstandsmeter_Load(object sender, EventArgs e)
        {

        }
    }
}
