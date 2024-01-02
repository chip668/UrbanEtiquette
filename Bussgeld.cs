using System;

namespace Anzeige
{
    public class Bussgeld
    {
        public int verstoss;
        public int p1;
        public int behinderung;
        public int p2;
        public int gefaerdung;
        public int p3;
        public int faktor;
        public Boolean parken { get; set; }
        public Boolean mitbehinderung { get; set; }
        public Boolean mitgefaerdung { get; set; }
        public Boolean halten
        {
            get { return !parken; }
            set { parken = !value; }
        }

        public Bussgeld()
        {
            this.faktor = 1;
            this.parken = false;
            this.mitbehinderung = false;
            this.mitgefaerdung = false;
            this.verstoss = int.MaxValue;
            this.behinderung = int.MaxValue;
            this.gefaerdung = int.MaxValue;
        }
        public Bussgeld(int faktor)
        {
            this.faktor = faktor;
        }
        public Bussgeld(bool parken, bool behinderung, bool gefaerdung)
        {
            this.parken = parken;
            this.mitbehinderung = behinderung;
            this.mitgefaerdung = gefaerdung;
        }
        public Bussgeld(Double verstoss, Double behinderung, Double gefaerdung)
        {
            this.verstoss = (int)verstoss;
            this.behinderung = (int)behinderung;
            this.gefaerdung = (int)gefaerdung;
            this.p1 = (int)((verstoss - (int)verstoss) * 10);
            this.p2 = (int)((behinderung - (int)behinderung) * 10 + 0.1);
            this.p3 = (int)((gefaerdung - (int)gefaerdung) * 10 + 0.1);
            this.faktor = 1;
            this.parken = false;
        }
    }
}
