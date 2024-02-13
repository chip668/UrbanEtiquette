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
        public event EventHandler Changed;

        protected virtual void OnChanged(EventArgs e)
        {
            Changed?.Invoke(this, e);
        }

        public String Caption
        {
            get { return CCaption.Text; }
            set { CCaption.Text = value; }
        }
        public String Text
        {
            get { return CText.Text; }
            set 
            { 
                CText.Text = value;
                OnChanged(EventArgs.Empty);
            }
        }
        public Edit_Line()
        {
            InitializeComponent();
        }

        private void CText_TextChanged(object sender, EventArgs e)
        {
            // Text = CText.Text;
        }
    }
}
