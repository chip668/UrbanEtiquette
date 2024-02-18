
namespace Anzeige
{
    partial class Bussgeldrechner
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
            this.label1 = new System.Windows.Forms.Label();
            this.C1 = new System.Windows.Forms.TextBox();
            this.CVT = new System.Windows.Forms.CheckBox();
            this.VW = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CGes = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CPT = new System.Windows.Forms.TextBox();
            this.CBH = new System.Windows.Forms.CheckBox();
            this.CBG = new System.Windows.Forms.CheckBox();
            this.CPA = new System.Windows.Forms.CheckBox();
            this.C2 = new System.Windows.Forms.TextBox();
            this.C3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bußgeld :";
            // 
            // C1
            // 
            this.C1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            this.C1.Enabled = false;
            this.C1.Location = new System.Drawing.Point(3, 62);
            this.C1.Name = "C1";
            this.C1.Size = new System.Drawing.Size(59, 29);
            this.C1.TabIndex = 1;
            // 
            // CVT
            // 
            this.CVT.AutoSize = true;
            this.CVT.Enabled = false;
            this.CVT.Location = new System.Drawing.Point(0, 116);
            this.CVT.Name = "CVT";
            this.CVT.Size = new System.Drawing.Size(80, 25);
            this.CVT.TabIndex = 2;
            this.CVT.Text = "Vorsatz";
            this.CVT.UseVisualStyleBackColor = true;
            // 
            // VW
            // 
            this.VW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            this.VW.Enabled = false;
            this.VW.Location = new System.Drawing.Point(83, 163);
            this.VW.Name = "VW";
            this.VW.Size = new System.Drawing.Size(111, 29);
            this.VW.TabIndex = 1;
            this.VW.Text = "28,50";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(3, 206);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Gesamt :";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // CGes
            // 
            this.CGes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            this.CGes.Enabled = false;
            this.CGes.Location = new System.Drawing.Point(83, 198);
            this.CGes.Name = "CGes";
            this.CGes.Size = new System.Drawing.Size(111, 29);
            this.CGes.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(2, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Punkte :";
            this.label4.Click += new System.EventHandler(this.label3_Click);
            // 
            // CPT
            // 
            this.CPT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            this.CPT.Enabled = false;
            this.CPT.Location = new System.Drawing.Point(5, 163);
            this.CPT.Name = "CPT";
            this.CPT.Size = new System.Drawing.Size(59, 29);
            this.CPT.TabIndex = 1;
            // 
            // CBH
            // 
            this.CBH.AutoSize = true;
            this.CBH.Enabled = false;
            this.CBH.Location = new System.Drawing.Point(83, 93);
            this.CBH.Name = "CBH";
            this.CBH.Size = new System.Drawing.Size(115, 25);
            this.CBH.TabIndex = 2;
            this.CBH.Text = "m. Behinder.";
            this.CBH.UseVisualStyleBackColor = true;
            // 
            // CBG
            // 
            this.CBG.AutoSize = true;
            this.CBG.Enabled = false;
            this.CBG.Location = new System.Drawing.Point(83, 116);
            this.CBG.Name = "CBG";
            this.CBG.Size = new System.Drawing.Size(109, 25);
            this.CBG.TabIndex = 2;
            this.CBG.Text = "m. Gefährd.";
            this.CBG.UseVisualStyleBackColor = true;
            // 
            // CPA
            // 
            this.CPA.AutoSize = true;
            this.CPA.Enabled = false;
            this.CPA.Location = new System.Drawing.Point(0, 94);
            this.CPA.Name = "CPA";
            this.CPA.Size = new System.Drawing.Size(77, 25);
            this.CPA.TabIndex = 2;
            this.CPA.Text = "parken";
            this.CPA.UseVisualStyleBackColor = true;
            // 
            // C2
            // 
            this.C2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            this.C2.Enabled = false;
            this.C2.Location = new System.Drawing.Point(68, 62);
            this.C2.Name = "C2";
            this.C2.Size = new System.Drawing.Size(59, 29);
            this.C2.TabIndex = 1;
            // 
            // C3
            // 
            this.C3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(216)))), ((int)(((byte)(255)))));
            this.C3.Enabled = false;
            this.C3.Location = new System.Drawing.Point(133, 62);
            this.C3.Name = "C3";
            this.C3.Size = new System.Drawing.Size(59, 29);
            this.C3.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(83, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Bearbeitung:";
            this.label2.Click += new System.EventHandler(this.label3_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(9, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Einfach";
            this.label5.Click += new System.EventHandler(this.label3_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(58, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Behinderung";
            this.label6.Click += new System.EventHandler(this.label3_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(128, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Gefährdung";
            this.label7.Click += new System.EventHandler(this.label3_Click);
            // 
            // Bussgeldrechner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.CBH);
            this.Controls.Add(this.CBG);
            this.Controls.Add(this.CVT);
            this.Controls.Add(this.CPA);
            this.Controls.Add(this.CPT);
            this.Controls.Add(this.CGes);
            this.Controls.Add(this.VW);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.C3);
            this.Controls.Add(this.C2);
            this.Controls.Add(this.C1);
            this.Controls.Add(this.label1);
            this.Name = "Bussgeldrechner";
            this.Size = new System.Drawing.Size(195, 230);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox C1;
        private System.Windows.Forms.CheckBox CVT;
        private System.Windows.Forms.TextBox VW;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox CGes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox CPT;
        private System.Windows.Forms.CheckBox CBH;
        private System.Windows.Forms.CheckBox CBG;
        private System.Windows.Forms.CheckBox CPA;
        private System.Windows.Forms.TextBox C2;
        private System.Windows.Forms.TextBox C3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}
