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

    public partial class SmallToolbox : UserControl
    {
        public class ClickToolEventArgs : EventArgs
        {
            public int ButtonIndex { get; }
            public string ButtonText { get; }
            public string toolTipText { get; }
            public Button s{ get; set; }

            public ClickToolEventArgs(int buttonIndex, string buttonText, string toolTipText)
            {
                this.ButtonIndex = buttonIndex;
                this.ButtonText = buttonText;
                this.toolTipText = toolTipText;
                this.s = null;
            }
        }
        List<String> tooltip = new List<String>();
        public event EventHandler<ClickToolEventArgs> ClickTool;
        public event EventHandler<ClickToolEventArgs> EnterTool;
        public event EventHandler<ClickToolEventArgs> LeaveTool;

        int n { get; set; }
        int _width = 20;
        int width 
        {
            get { return _width; }
            set 
            { 
                _width = value;
                if (!_OpenMode)
                    this.Width = width;
            }
        }

        private Boolean _OpenMode = true;
        public Boolean OpenMode
        {
            get { return _OpenMode;  }
            set 
            { 
                _OpenMode = value;
                if (!_OpenMode)
                {
                    this.BackgroundImage = imageList1.Images[0];
                    this.Width = 20;
                }
                else
                {
                    this.BackgroundImage = imageList1.Images[1];
                    this.Width = width;
                }
            }
        }
        public SmallToolbox()
        {
            InitializeComponent();
            Controls.Clear();
            n = 0;
            this.Width = 20;
            OpenMode = true;
        }
        private void SmallToolbox_Load(object sender, EventArgs e)
        {
        }
        private void SmallToolbox_Click(object sender, EventArgs e)
        {
            OpenMode = !OpenMode;
        }
        public void AddButtons(String []buttons, String[] tooltips)
        {
            foreach (String bt in buttons)
            {
                this.tooltip.Add(tooltips[n]);
                AddButton(bt);
            }


            OpenMode = true;
        }
        public void AddButton(String button)
        {
            Button bt = new Button();
            bt.Text = button.ToString();
            bt.Tag = n++;
            bt.Font = new Font("Arial Unicode MS", 12); // Hier Schriftart anpassen
            this.Controls.Add(bt);
            bt.Location = new Point(width, 0);
            bt.Size = new Size(33, 40);
            width = bt.Left + bt.Width;
            bt.Click += CTButton_Click;
            bt.MouseEnter += CTButton_MouseEnter;
            bt.MouseLeave += CTButton_MouseLeave;
        }

        protected virtual void OnClickTool(ClickToolEventArgs e)
        {
            ClickTool?.Invoke(this, e);
        }
        protected virtual void OnEnterTool(ClickToolEventArgs e)
        {
            EnterTool?.Invoke(this, e);
        }
        protected virtual void OnLeaveTool(ClickToolEventArgs e)
        {
            LeaveTool?.Invoke(this, e);
        }

        
        private void CTButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int buttonIndex = (int)clickedButton.Tag;
                string buttonText = clickedButton.Text;

                // Hier wird das ClickTool-Ereignis ausgelöst
                OnClickTool(new ClickToolEventArgs(buttonIndex, buttonText, ""));
            }
        }
        private void CTButton_MouseEnter(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int n = (int)clickedButton.Tag; 
            if (clickedButton != null)
            {
                int buttonIndex = (int)clickedButton.Tag;
                string buttonText = clickedButton.Text;
                string tooltipText = tooltip[n];

                // Hier wird das ClickTool-Ereignis ausgelöst
                SmallToolbox.ClickToolEventArgs ex = new ClickToolEventArgs(buttonIndex, buttonText, tooltipText);
                ex.s = clickedButton;
                OnEnterTool(ex);
            }
        }
        private void CTButton_MouseLeave(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int n = (int)clickedButton.Tag;
            if (clickedButton != null)
            {
                int buttonIndex = (int)clickedButton.Tag;
                string buttonText = clickedButton.Text;
                string tooltipText = tooltip[n];

                // Hier wird das ClickTool-Ereignis ausgelöst
                SmallToolbox.ClickToolEventArgs ex = new ClickToolEventArgs(buttonIndex, buttonText, tooltipText);
                ex.s = clickedButton;
                OnLeaveTool(ex);
            }
        }
    }
}
