
namespace Anzeige
{
    partial class Colortraining
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Colortraining));
            this.button1 = new System.Windows.Forms.Button();
            this.CReferenzColor = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.CReference = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CSelected = new System.Windows.Forms.Panel();
            this.CTrainColors = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.CReferenzColor)).BeginInit();
            this.CReference.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(1065, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CReferenzColor
            // 
            this.CReferenzColor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CReferenzColor.BackgroundImage")));
            this.CReferenzColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CReferenzColor.Location = new System.Drawing.Point(12, 54);
            this.CReferenzColor.Name = "CReferenzColor";
            this.CReferenzColor.Size = new System.Drawing.Size(1307, 856);
            this.CReferenzColor.TabIndex = 15;
            this.CReferenzColor.TabStop = false;
            this.CReferenzColor.Click += new System.EventHandler(this.CReferenzColor_Click);
            this.CReferenzColor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CReferenzColor_MouseDown);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(1195, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(124, 35);
            this.button2.TabIndex = 0;
            this.button2.Text = "Abbrechen";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button1_Click);
            // 
            // CReference
            // 
            this.CReference.BackColor = System.Drawing.Color.Gold;
            this.CReference.Controls.Add(this.panel14);
            this.CReference.Controls.Add(this.panel12);
            this.CReference.Controls.Add(this.panel11);
            this.CReference.Controls.Add(this.panel10);
            this.CReference.Controls.Add(this.panel9);
            this.CReference.Controls.Add(this.panel8);
            this.CReference.Controls.Add(this.panel7);
            this.CReference.Controls.Add(this.panel6);
            this.CReference.Controls.Add(this.panel5);
            this.CReference.Controls.Add(this.panel4);
            this.CReference.Controls.Add(this.panel3);
            this.CReference.Controls.Add(this.panel2);
            this.CReference.Location = new System.Drawing.Point(12, 13);
            this.CReference.Name = "CReference";
            this.CReference.Size = new System.Drawing.Size(646, 35);
            this.CReference.TabIndex = 16;
            this.CReference.TabStop = true;
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.Color.Yellow;
            this.panel14.Location = new System.Drawing.Point(236, 1);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(28, 32);
            this.panel14.TabIndex = 2;
            this.panel14.Tag = "Gelb";
            this.panel14.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.Aqua;
            this.panel12.Location = new System.Drawing.Point(324, 1);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(28, 32);
            this.panel12.TabIndex = 0;
            this.panel12.Tag = "Sonstige";
            this.panel12.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel11.Location = new System.Drawing.Point(295, 1);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(28, 32);
            this.panel11.TabIndex = 0;
            this.panel11.Tag = "Orange";
            this.panel11.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.panel10.Location = new System.Drawing.Point(266, 1);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(28, 32);
            this.panel10.TabIndex = 0;
            this.panel10.Tag = "Lila";
            this.panel10.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.Green;
            this.panel9.Location = new System.Drawing.Point(207, 1);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(28, 32);
            this.panel9.TabIndex = 0;
            this.panel9.Tag = "Grün";
            this.panel9.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel8.Location = new System.Drawing.Point(178, 1);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(28, 32);
            this.panel8.TabIndex = 0;
            this.panel8.Tag = "Gelb";
            this.panel8.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Red;
            this.panel7.Location = new System.Drawing.Point(149, 1);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(28, 32);
            this.panel7.TabIndex = 0;
            this.panel7.Tag = "Rot";
            this.panel7.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Maroon;
            this.panel6.Location = new System.Drawing.Point(120, 1);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(28, 32);
            this.panel6.TabIndex = 0;
            this.panel6.Tag = "Braun";
            this.panel6.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Navy;
            this.panel5.Location = new System.Drawing.Point(91, 1);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(28, 32);
            this.panel5.TabIndex = 0;
            this.panel5.Tag = "Blau";
            this.panel5.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Location = new System.Drawing.Point(62, 1);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(28, 32);
            this.panel4.TabIndex = 0;
            this.panel4.Tag = "Weiss";
            this.panel4.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Silver;
            this.panel3.Location = new System.Drawing.Point(33, 1);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(28, 32);
            this.panel3.TabIndex = 0;
            this.panel3.Tag = "Silber/Grau";
            this.panel3.Click += new System.EventHandler(this.panel_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(5, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(28, 32);
            this.panel2.TabIndex = 0;
            this.panel2.Tag = "Schwarz";
            this.panel2.Click += new System.EventHandler(this.panel_Click);
            // 
            // CSelected
            // 
            this.CSelected.Location = new System.Drawing.Point(664, 14);
            this.CSelected.Name = "CSelected";
            this.CSelected.Size = new System.Drawing.Size(285, 34);
            this.CSelected.TabIndex = 17;
            // 
            // CTrainColors
            // 
            this.CTrainColors.AutoSize = true;
            this.CTrainColors.Checked = true;
            this.CTrainColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CTrainColors.Location = new System.Drawing.Point(955, 19);
            this.CTrainColors.Name = "CTrainColors";
            this.CTrainColors.Size = new System.Drawing.Size(85, 25);
            this.CTrainColors.TabIndex = 18;
            this.CTrainColors.Text = "Training";
            this.CTrainColors.UseVisualStyleBackColor = true;
            // 
            // Colortraining
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1328, 912);
            this.Controls.Add(this.CTrainColors);
            this.Controls.Add(this.CSelected);
            this.Controls.Add(this.CReference);
            this.Controls.Add(this.CReferenzColor);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Colortraining";
            this.Text = "Colortraining";
            this.Load += new System.EventHandler(this.Colortraining_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CReferenzColor)).EndInit();
            this.CReference.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.PictureBox CReferenzColor;
        private System.Windows.Forms.Panel CReference;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel CSelected;
        private System.Windows.Forms.CheckBox CTrainColors;
    }
}