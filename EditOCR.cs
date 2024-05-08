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
    public partial class EditOCR : Form
    {
        public OCRLib ocrlib = null;
        private const string DefaultReferenzOrdner = "ReferenzBilder";
        public Bitmap Buchstabe
        {
            get { return (Bitmap)CCharakter.BackgroundImage; }
            set { CCharakter.BackgroundImage = value; }
        }
        public Bitmap Referenz
        {
            get { return (Bitmap)CReferenz.BackgroundImage; }
            set { CReferenz.BackgroundImage = value; }
        }
        public String _BuchstabenString;
        public String BuchstabenString
        {
            get 
            { 
                return _BuchstabenString; 
            }
            set 
            {
                _BuchstabenString = value;
                CText.Text = value; 
            }
        }
        public String BuchstabenStringEdit
        {
            get { return CText.Text; }
            set 
            { 
                CText.Text = value;
                _BuchstabenString = value;
            }
        }
        public EditOCR()
        {
            InitializeComponent();
        }

        private void COK_Click(object sender, EventArgs e)
        {

        }

        private void button_Click(object sender, EventArgs e)
        {
            CText.Text = ((Control)sender).Text;
        }

        private void CText_VisibleChanged(object sender, EventArgs e)
        {
            CText.Focus();
        }

        private void CText_TextChanged(object sender, EventArgs e)
        {
            if (ocrlib.referenzBitmapsInfo.ContainsKey(CText.Text))
            {
                Referenz = ocrlib.referenzBitmapsInfo[CText.Text].bmp;
            }
        }
    }
}
