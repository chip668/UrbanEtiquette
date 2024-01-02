using System;
using System.Windows.Forms;

namespace Anzeige
{
    public partial class TimePicker : UserControl
    {
        public event EventHandler TimeChanged;
        public DateTime Value
        {
            get
            {
                DateTime dtm = DateTime.Now;
                return new DateTime(dtm.Year, dtm.Month, dtm.Day, Hour, Minute, Second);
            }
            set
            {
                DateTime dtm = value;
                Hour = dtm.Hour;
                Minute = dtm.Minute;
                Second = dtm.Second;
            }


        }
        public int Hour
        {
            get
            {
                return (int)numericUpDown1.Value;
            }
            set
            {
                numericUpDown1.Value = value;
            }
        }
        public int Minute
        {
            get
            {
                return (int)numericUpDown2.Value;
            }
            set
            {
                numericUpDown2.Value = value;
            }
        }
        public int Second
        {
            get
            {
                return (int)numericUpDown3.Value;
            }
            set
            {
                numericUpDown3.Value = value;
            }
        }
        public Boolean Short
        {
            get
            {
                return !numericUpDown3.Visible;
            }
            set
            {
                numericUpDown3.Visible = !value;
                this.Width = this.Width;
            }
        }

        public TimePicker()
        {
            InitializeComponent();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value == numericUpDown3.Maximum)
            {
                numericUpDown2.Value++;
            }
            else if (numericUpDown3.Value == numericUpDown3.Minimum)
            {
                numericUpDown2.Value--;
            }
            OnTimeChanged(); // Hier wird das Ereignis ausgelöst
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value == numericUpDown2.Maximum)
            {
                numericUpDown1.Value++;
            }
            else if (numericUpDown2.Value == numericUpDown2.Minimum)
            {
                numericUpDown1.Value--;
            }
            OnTimeChanged(); // Hier wird das Ereignis ausgelöst
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value == numericUpDown2.Maximum)
            {
                // numericUpDown1.Value++;
            }
            else if (numericUpDown2.Value == numericUpDown2.Minimum)
            {
                // numericUpDown1.Value--;
            }
            OnTimeChanged(); // Hier wird das Ereignis ausgelöst
        }
        private void UserControl1_Resize(object sender, EventArgs e)
        {
            if (numericUpDown3.Visible)
            {
                this.Width = numericUpDown3.Left + numericUpDown3.Width;
                this.Height = numericUpDown3.Top + numericUpDown3.Height;
            }
            else
            {
                this.Width = numericUpDown2.Left + numericUpDown2.Width;
                this.Height = numericUpDown2.Top + numericUpDown2.Height;
            }
        }
        private void UserControl1_Load(object sender, EventArgs e)
        {
            Value = DateTime.Now;
        }
        protected virtual void OnTimeChanged()
        {
            TimeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
