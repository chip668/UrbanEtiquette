
namespace Anzeige
{
    partial class ConfigEditorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxVorname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxStrasse = new System.Windows.Forms.TextBox();
            this.textBoxHausnummer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPLZ = new System.Windows.Forms.TextBox();
            this.textBoxOrt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxZielpfad = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxOrtMail = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(114, 3);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(426, 29);
            this.textBoxName.TabIndex = 1;
            // 
            // textBoxVorname
            // 
            this.textBoxVorname.Location = new System.Drawing.Point(546, 3);
            this.textBoxVorname.Name = "textBoxVorname";
            this.textBoxVorname.Size = new System.Drawing.Size(426, 29);
            this.textBoxVorname.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Strasse HN";
            // 
            // textBoxStrasse
            // 
            this.textBoxStrasse.Location = new System.Drawing.Point(114, 38);
            this.textBoxStrasse.Name = "textBoxStrasse";
            this.textBoxStrasse.Size = new System.Drawing.Size(426, 29);
            this.textBoxStrasse.TabIndex = 1;
            // 
            // textBoxHausnummer
            // 
            this.textBoxHausnummer.Location = new System.Drawing.Point(546, 38);
            this.textBoxHausnummer.Name = "textBoxHausnummer";
            this.textBoxHausnummer.Size = new System.Drawing.Size(426, 29);
            this.textBoxHausnummer.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 21);
            this.label5.TabIndex = 0;
            this.label5.Text = "PLZ Ort";
            // 
            // textBoxPLZ
            // 
            this.textBoxPLZ.Location = new System.Drawing.Point(114, 73);
            this.textBoxPLZ.Name = "textBoxPLZ";
            this.textBoxPLZ.Size = new System.Drawing.Size(426, 29);
            this.textBoxPLZ.TabIndex = 1;
            // 
            // textBoxOrt
            // 
            this.textBoxOrt.Location = new System.Drawing.Point(546, 73);
            this.textBoxOrt.Name = "textBoxOrt";
            this.textBoxOrt.Size = new System.Drawing.Size(426, 29);
            this.textBoxOrt.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 21);
            this.label7.TabIndex = 0;
            this.label7.Text = "Ziel";
            // 
            // textBoxZielpfad
            // 
            this.textBoxZielpfad.Location = new System.Drawing.Point(114, 108);
            this.textBoxZielpfad.Name = "textBoxZielpfad";
            this.textBoxZielpfad.Size = new System.Drawing.Size(858, 29);
            this.textBoxZielpfad.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 21);
            this.label8.TabIndex = 0;
            this.label8.Text = "Absender";
            // 
            // textBoxOrtMail
            // 
            this.textBoxOrtMail.Location = new System.Drawing.Point(114, 143);
            this.textBoxOrtMail.Name = "textBoxOrtMail";
            this.textBoxOrtMail.Size = new System.Drawing.Size(858, 29);
            this.textBoxOrtMail.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(764, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(208, 35);
            this.button1.TabIndex = 2;
            this.button1.Text = "&speichern";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // ConfigEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 236);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxOrtMail);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxZielpfad);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxOrt);
            this.Controls.Add(this.textBoxPLZ);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxHausnummer);
            this.Controls.Add(this.textBoxStrasse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxVorname);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Name = "ConfigEditorForm";
            this.Text = "ConfigEditorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxVorname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxStrasse;
        private System.Windows.Forms.TextBox textBoxHausnummer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPLZ;
        private System.Windows.Forms.TextBox textBoxOrt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxZielpfad;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxOrtMail;
        private System.Windows.Forms.Button button1;
    }
}