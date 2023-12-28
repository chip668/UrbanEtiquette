
namespace Anzeige
{
    partial class assistent
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
            this.CBT1 = new System.Windows.Forms.Button();
            this.CDisplay = new System.Windows.Forms.TextBox();
            this.CBT2 = new System.Windows.Forms.Button();
            this.CBT3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // CBT1
            // 
            this.CBT1.Location = new System.Drawing.Point(336, 120);
            this.CBT1.Name = "CBT1";
            this.CBT1.Size = new System.Drawing.Size(93, 38);
            this.CBT1.TabIndex = 1;
            this.CBT1.Text = "Next";
            this.CBT1.UseVisualStyleBackColor = true;
            this.CBT1.Click += new System.EventHandler(this.CBT_Click);
            // 
            // CDisplay
            // 
            this.CDisplay.BackColor = System.Drawing.SystemColors.Control;
            this.CDisplay.Location = new System.Drawing.Point(11, 8);
            this.CDisplay.Multiline = true;
            this.CDisplay.Name = "CDisplay";
            this.CDisplay.Size = new System.Drawing.Size(504, 93);
            this.CDisplay.TabIndex = 2;
            this.CDisplay.Text = "Haben Sie eine Videoaufzeichung oder ein Foto";
            // 
            // CBT2
            // 
            this.CBT2.Location = new System.Drawing.Point(435, 120);
            this.CBT2.Name = "CBT2";
            this.CBT2.Size = new System.Drawing.Size(93, 38);
            this.CBT2.TabIndex = 1;
            this.CBT2.Text = "Foto";
            this.CBT2.UseVisualStyleBackColor = true;
            this.CBT2.Click += new System.EventHandler(this.CBT_Click);
            // 
            // CBT3
            // 
            this.CBT3.Location = new System.Drawing.Point(534, 120);
            this.CBT3.Name = "CBT3";
            this.CBT3.Size = new System.Drawing.Size(93, 38);
            this.CBT3.TabIndex = 1;
            this.CBT3.Text = "Video";
            this.CBT3.UseVisualStyleBackColor = true;
            this.CBT3.Click += new System.EventHandler(this.CBT_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::Anzeige.Properties.Resources.nummer_5;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(521, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(106, 93);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // assistent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 170);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.CDisplay);
            this.Controls.Add(this.CBT3);
            this.Controls.Add(this.CBT2);
            this.Controls.Add(this.CBT1);
            this.Name = "assistent";
            this.Text = "assistent";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CBT1;
        private System.Windows.Forms.TextBox CDisplay;
        private System.Windows.Forms.Button CBT2;
        private System.Windows.Forms.Button CBT3;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}