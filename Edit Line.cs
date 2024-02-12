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
    public partial class Edit_Line : UserControl
    {
        public String Caption
        {
            get { return CCaption.Text; }
            set { CCaption.Text = value; }
        }
        public String Text
        {
            get { return CText.Text; }
            set { CText.Text = value; }
        }
        public Edit_Line()
        {
            InitializeComponent();
        }
    }
}
