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
    public partial class Edit_Adress : UserControl
    {
        WebBrowser w = null;
        public String Line
        {
            get { return CPLZ.Text + ";" + COrt.Text + ";" + CMail.Text + ";" + CWeb.Text + ";" + CGoogle.Text; }
            set 
            {
                String[] items = value.Split(';');
                if (items.Length > 0)
                    CPLZ.Text = items[0];

                if (items.Length > 1)
                    COrt.Text = items[1];

                if (items.Length > 2)
                    CMail.Text = items[2];

                if (items.Length > 3)
                    CWeb.Text = items[3];

                if (items.Length > 4)
                {
                    CGoogle.Text = items[4];
                    CreateBrowser();
                    w.Navigate(CGoogle.Text);
                }
            }
        }
        public Edit_Adress()
        {
            InitializeComponent();
        }

        private void Edit_Adress_Load(object sender, EventArgs e)
        {
        }

        private void CPLZLbl_Resize(object sender, EventArgs e)
        {
            CPLZ.Location = new Point(150, 0);
            CPLZ.Width = CPLZLbl.Width - CPLZ.Left;
            COrt.Location = new Point(150, CPLZ.Top + CPLZ.Height);
            COrt.Width = CPLZLbl.Width - COrt.Left;
            CMail.Location = new Point(150, COrt.Top + COrt.Height);
            CMail.Width = CPLZLbl.Width - CMail.Left;
            CWeb.Location = new Point(150, CMail.Top + CMail.Height);
            CWeb.Width = CPLZLbl.Width - CWeb.Left;
            CGoogle.Location = new Point(150, CWeb.Top + CWeb.Height);
            CGoogle.Width = CPLZLbl.Width - CGoogle.Left;


            CPLZLbl.Top = CPLZ.Top;
            COrtLbl.Top = COrt.Top;
            CMailLbl.Top = CMail.Top;
            CWebLbl.Top = CWeb.Top;
            CGoogleLbl.Top = CGoogle.Top;

            CPLZLbl.Width = 150;
            COrtLbl.Width = 150;
            CMailLbl.Width = 150;
            CWebLbl.Width = 150;
            CGoogleLbl.Width = 150;

            CreateBrowser();
            w.Navigate(CGoogle.Text);
        }
        private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // Der Code, der ausgeführt wird, wenn die URL sich ändert
            CGoogle.Text = w.Url.ToString();
        }

        private void CGoogle_TextChanged(object sender, EventArgs e)
        {
            if (w!=null)
                w.Navigate(CGoogle.Text);
        }
        private void CreateBrowser()
        {
            if (w == null)
            {
                w = new WebBrowser();
                this.Controls.Add(w);
                w.Navigated += WebBrowser_Navigated;
                w.ScriptErrorsSuppressed = true;
                w.Left = 0;
                w.Top = CGoogle.Top + CGoogle.Height;
                w.Width = this.Width;
                w.Height = this.Height - w.Top;
                w.Visible = true;
            }
        }
    }
}
