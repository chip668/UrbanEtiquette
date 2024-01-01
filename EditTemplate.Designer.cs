
namespace Anzeige
{
    partial class EditTemplate
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
            this.CAbbrechen = new System.Windows.Forms.Button();
            this.CSave = new System.Windows.Forms.Button();
            this.CSaveAs = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.CTake = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CAbbrechen
            // 
            this.CAbbrechen.Location = new System.Drawing.Point(1184, 12);
            this.CAbbrechen.Name = "CAbbrechen";
            this.CAbbrechen.Size = new System.Drawing.Size(115, 33);
            this.CAbbrechen.TabIndex = 0;
            this.CAbbrechen.Text = "Abbrechen";
            this.CAbbrechen.UseVisualStyleBackColor = true;
            this.CAbbrechen.Click += new System.EventHandler(this.CAbbrechen_Click);
            // 
            // CSave
            // 
            this.CSave.Location = new System.Drawing.Point(1184, 51);
            this.CSave.Name = "CSave";
            this.CSave.Size = new System.Drawing.Size(115, 33);
            this.CSave.TabIndex = 0;
            this.CSave.Text = "Speichern";
            this.CSave.UseVisualStyleBackColor = true;
            this.CSave.Click += new System.EventHandler(this.CSave_Click);
            // 
            // CSaveAs
            // 
            this.CSaveAs.Enabled = false;
            this.CSaveAs.Location = new System.Drawing.Point(1184, 90);
            this.CSaveAs.Name = "CSaveAs";
            this.CSaveAs.Size = new System.Drawing.Size(115, 33);
            this.CSaveAs.TabIndex = 0;
            this.CSaveAs.Text = "Speichern als";
            this.CSaveAs.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(10, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1168, 695);
            this.textBox1.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "<mail>",
            "<verstoss>",
            "<freetext>",
            "<datum>",
            "<zeit>",
            "<strasse>",
            "<hausnummer>",
            "<plz>",
            "<ort>",
            "<marke>",
            "<farbe>",
            "<kennzeichen>",
            "<zname>",
            "<zvorrname>",
            "<zstrasse>",
            "<zhausnummer>",
            "<zplz>",
            "<zort>",
            "<files>",
            "<kennzeichenbild>",
            "<pdffile>"});
            this.comboBox1.Location = new System.Drawing.Point(1178, 213);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 29);
            this.comboBox1.TabIndex = 2;
            // 
            // CTake
            // 
            this.CTake.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CTake.Location = new System.Drawing.Point(1178, 248);
            this.CTake.Name = "CTake";
            this.CTake.Size = new System.Drawing.Size(121, 54);
            this.CTake.TabIndex = 3;
            this.CTake.Text = "⇦";
            this.CTake.UseVisualStyleBackColor = true;
            this.CTake.Click += new System.EventHandler(this.CTake_Click);
            // 
            // EditTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 713);
            this.Controls.Add(this.CTake);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.CSaveAs);
            this.Controls.Add(this.CSave);
            this.Controls.Add(this.CAbbrechen);
            this.Name = "EditTemplate";
            this.Text = "Vorlage bearbeiten";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CAbbrechen;
        private System.Windows.Forms.Button CSave;
        private System.Windows.Forms.Button CSaveAs;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button CTake;
    }
}