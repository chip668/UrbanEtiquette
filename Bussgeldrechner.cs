using System;
using System.Windows.Forms;

namespace Anzeige
{
    /// <summary>
    /// busgeld Control
    /// </summary>
    public partial class Bussgeldrechner : UserControl
    {
        private Bussgeld _bussgeld;
        public Bussgeld bussgeld
        {
            get
            {
                return _bussgeld;

            }
            set
            {
                _bussgeld = value;
                if (_bussgeld != null)
                {
                    double betrag = _bussgeld.verstoss;
                    C1.Text = _bussgeld.verstoss.ToString();
                    C2.Text = _bussgeld.behinderung.ToString();
                    C3.Text = _bussgeld.gefaerdung.ToString();
                    CPA.Checked = _bussgeld.parken;
                    CPT.Text = _bussgeld.p1.ToString();
                    if (_bussgeld.mitbehinderung)
                    {
                        CBH.Checked = _bussgeld.mitbehinderung;
                        betrag = _bussgeld.behinderung;
                        CPT.Text = _bussgeld.p2.ToString();
                    }
                    if (_bussgeld.mitgefaerdung)
                    {
                        betrag = _bussgeld.gefaerdung;
                        CBG.Checked = _bussgeld.mitgefaerdung;
                        CPT.Text = _bussgeld.p3.ToString();
                    }
                    if (_bussgeld.faktor > 1)
                    {
                        betrag *= _bussgeld.faktor;
                        CVT.Checked = _bussgeld.faktor == 2;
                    }
                    betrag += 28.5;
                    CGes.Text = betrag.ToString();
                }
            }
        }
        public Bussgeldrechner()
        {
            InitializeComponent();
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
