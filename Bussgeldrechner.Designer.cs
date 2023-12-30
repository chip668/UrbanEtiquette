
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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bußgeld :";
            // 
            // C1
            // 
            this.C1.Enabled = false;
            this.C1.Location = new System.Drawing.Point(3, 31);
            this.C1.Name = "C1";
            this.C1.Size = new System.Drawing.Size(59, 29);
            this.C1.TabIndex = 1;
            // 
            // CVT
            // 
            this.CVT.AutoSize = true;
            this.CVT.Enabled = false;
            this.CVT.Location = new System.Drawing.Point(0, 82);
            this.CVT.Name = "CVT";
            this.CVT.Size = new System.Drawing.Size(80, 25);
            this.CVT.TabIndex = 2;
            this.CVT.Text = "Vorsatz";
            this.CVT.UseVisualStyleBackColor = true;
            // 
            // VW
            // 
            this.VW.Enabled = false;
            this.VW.Location = new System.Drawing.Point(83, 129);
            this.VW.Name = "VW";
            this.VW.Size = new System.Drawing.Size(111, 29);
            this.VW.TabIndex = 1;
            this.VW.Text = "28,50";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Gesamt :";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // CGes
            // 
            this.CGes.Enabled = false;
            this.CGes.Location = new System.Drawing.Point(83, 164);
            this.CGes.Name = "CGes";
            this.CGes.Size = new System.Drawing.Size(111, 29);
            this.CGes.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 21);
            this.label4.TabIndex = 0;
            this.label4.Text = "Punkte :";
            this.label4.Click += new System.EventHandler(this.label3_Click);
            // 
            // CPT
            // 
            this.CPT.Enabled = false;
            this.CPT.Location = new System.Drawing.Point(5, 129);
            this.CPT.Name = "CPT";
            this.CPT.Size = new System.Drawing.Size(59, 29);
            this.CPT.TabIndex = 1;
            // 
            // CBH
            // 
            this.CBH.AutoSize = true;
            this.CBH.Enabled = false;
            this.CBH.Location = new System.Drawing.Point(83, 59);
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
            this.CBG.Location = new System.Drawing.Point(83, 82);
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
            this.CPA.Location = new System.Drawing.Point(0, 60);
            this.CPA.Name = "CPA";
            this.CPA.Size = new System.Drawing.Size(77, 25);
            this.CPA.TabIndex = 2;
            this.CPA.Text = "parken";
            this.CPA.UseVisualStyleBackColor = true;
            // 
            // C2
            // 
            this.C2.Enabled = false;
            this.C2.Location = new System.Drawing.Point(68, 31);
            this.C2.Name = "C2";
            this.C2.Size = new System.Drawing.Size(59, 29);
            this.C2.TabIndex = 1;
            // 
            // C3
            // 
            this.C3.Enabled = false;
            this.C3.Location = new System.Drawing.Point(133, 31);
            this.C3.Name = "C3";
            this.C3.Size = new System.Drawing.Size(59, 29);
            this.C3.TabIndex = 1;
            // 
            // Bussgeldrechner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CBH);
            this.Controls.Add(this.CBG);
            this.Controls.Add(this.CVT);
            this.Controls.Add(this.CPA);
            this.Controls.Add(this.CPT);
            this.Controls.Add(this.CGes);
            this.Controls.Add(this.VW);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.C3);
            this.Controls.Add(this.C2);
            this.Controls.Add(this.C1);
            this.Controls.Add(this.label1);
            this.Name = "Bussgeldrechner";
            this.Size = new System.Drawing.Size(195, 198);
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
    }
}
