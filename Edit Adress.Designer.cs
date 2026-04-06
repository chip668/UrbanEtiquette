
namespace Anzeige
{
    partial class Edit_Adress
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            CPLZ = new System.Windows.Forms.TextBox();
            CPLZLbl = new System.Windows.Forms.Label();
            COrtLbl = new System.Windows.Forms.Label();
            COrt = new System.Windows.Forms.TextBox();
            CMailLbl = new System.Windows.Forms.Label();
            CMail = new System.Windows.Forms.TextBox();
            CWebLbl = new System.Windows.Forms.Label();
            CWeb = new System.Windows.Forms.TextBox();
            CGoogleLbl = new System.Windows.Forms.Label();
            CGoogle = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            CScript = new System.Windows.Forms.TextBox();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // CPLZ
            // 
            CPLZ.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CPLZ.Location = new System.Drawing.Point(78, 4);
            CPLZ.Margin = new System.Windows.Forms.Padding(2);
            CPLZ.Name = "CPLZ";
            CPLZ.Size = new System.Drawing.Size(575, 29);
            CPLZ.TabIndex = 3;
            CPLZ.TextChanged += TextChanged;
            // 
            // CPLZLbl
            // 
            CPLZLbl.AutoSize = true;
            CPLZLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CPLZLbl.Location = new System.Drawing.Point(2, 4);
            CPLZLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            CPLZLbl.Name = "CPLZLbl";
            CPLZLbl.Size = new System.Drawing.Size(36, 21);
            CPLZLbl.TabIndex = 2;
            CPLZLbl.Text = "PLZ";
            CPLZLbl.Resize += CPLZLbl_Resize;
            // 
            // COrtLbl
            // 
            COrtLbl.AutoSize = true;
            COrtLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            COrtLbl.Location = new System.Drawing.Point(2, 41);
            COrtLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            COrtLbl.Name = "COrtLbl";
            COrtLbl.Size = new System.Drawing.Size(33, 21);
            COrtLbl.TabIndex = 2;
            COrtLbl.Text = "Ort";
            // 
            // COrt
            // 
            COrt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            COrt.Location = new System.Drawing.Point(78, 41);
            COrt.Margin = new System.Windows.Forms.Padding(2);
            COrt.Name = "COrt";
            COrt.Size = new System.Drawing.Size(575, 29);
            COrt.TabIndex = 3;
            COrt.TextChanged += TextChanged;
            // 
            // CMailLbl
            // 
            CMailLbl.AutoSize = true;
            CMailLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CMailLbl.Location = new System.Drawing.Point(2, 77);
            CMailLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            CMailLbl.Name = "CMailLbl";
            CMailLbl.Size = new System.Drawing.Size(54, 21);
            CMailLbl.TabIndex = 2;
            CMailLbl.Text = "e-Mail";
            // 
            // CMail
            // 
            CMail.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CMail.Location = new System.Drawing.Point(78, 77);
            CMail.Margin = new System.Windows.Forms.Padding(2);
            CMail.Name = "CMail";
            CMail.Size = new System.Drawing.Size(575, 29);
            CMail.TabIndex = 3;
            CMail.TextChanged += TextChanged;
            // 
            // CWebLbl
            // 
            CWebLbl.AutoSize = true;
            CWebLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CWebLbl.Location = new System.Drawing.Point(2, 114);
            CWebLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            CWebLbl.Name = "CWebLbl";
            CWebLbl.Size = new System.Drawing.Size(73, 21);
            CWebLbl.TabIndex = 2;
            CWebLbl.Text = "Webseite";
            // 
            // CWeb
            // 
            CWeb.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CWeb.Location = new System.Drawing.Point(78, 114);
            CWeb.Margin = new System.Windows.Forms.Padding(2);
            CWeb.Name = "CWeb";
            CWeb.Size = new System.Drawing.Size(575, 29);
            CWeb.TabIndex = 3;
            CWeb.TextChanged += TextChanged;
            // 
            // CGoogleLbl
            // 
            CGoogleLbl.AutoSize = true;
            CGoogleLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CGoogleLbl.Location = new System.Drawing.Point(2, 150);
            CGoogleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            CGoogleLbl.Name = "CGoogleLbl";
            CGoogleLbl.Size = new System.Drawing.Size(62, 21);
            CGoogleLbl.TabIndex = 2;
            CGoogleLbl.Text = "Mängel";
            // 
            // CGoogle
            // 
            CGoogle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CGoogle.Location = new System.Drawing.Point(78, 150);
            CGoogle.Margin = new System.Windows.Forms.Padding(2);
            CGoogle.Name = "CGoogle";
            CGoogle.Size = new System.Drawing.Size(575, 29);
            CGoogle.TabIndex = 3;
            CGoogle.TextChanged += CGoogle_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(2, 195);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(50, 21);
            label1.TabIndex = 2;
            label1.Text = "Script";
            // 
            // CScript
            // 
            CScript.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CScript.Location = new System.Drawing.Point(78, 189);
            CScript.Margin = new System.Windows.Forms.Padding(2);
            CScript.Name = "CScript";
            CScript.Size = new System.Drawing.Size(446, 29);
            CScript.TabIndex = 3;
            CScript.TextChanged += CScript_TextChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "\"Scriptdatei (*.fsc)|*.fsc|Alle Dateien (*.*)|*.*\"";
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(520, 187);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(47, 31);
            button1.TabIndex = 4;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button2.Location = new System.Drawing.Point(606, 187);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(47, 31);
            button2.TabIndex = 4;
            button2.Text = "🏃";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            button3.Location = new System.Drawing.Point(564, 187);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(45, 31);
            button3.TabIndex = 4;
            button3.Text = "✎";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Edit_Adress
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(CScript);
            Controls.Add(label1);
            Controls.Add(CGoogle);
            Controls.Add(CGoogleLbl);
            Controls.Add(CWeb);
            Controls.Add(CWebLbl);
            Controls.Add(CMail);
            Controls.Add(CMailLbl);
            Controls.Add(COrt);
            Controls.Add(COrtLbl);
            Controls.Add(CPLZ);
            Controls.Add(CPLZLbl);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "Edit_Adress";
            Size = new System.Drawing.Size(658, 221);
            Load += Edit_Adress_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CPLZ;
        private System.Windows.Forms.Label CPLZLbl;
        private System.Windows.Forms.Label COrtLbl;
        private System.Windows.Forms.TextBox COrt;
        private System.Windows.Forms.Label CMailLbl;
        private System.Windows.Forms.TextBox CMail;
        private System.Windows.Forms.Label CWebLbl;
        private System.Windows.Forms.TextBox CWeb;
        private System.Windows.Forms.Label CGoogleLbl;
        private System.Windows.Forms.TextBox CGoogle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CScript;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}
