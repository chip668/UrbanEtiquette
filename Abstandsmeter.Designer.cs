
namespace Anzeige
{
    partial class Abstandsmeter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Abstandsmeter));
            this.CCar = new System.Windows.Forms.PictureBox();
            this.CBike = new System.Windows.Forms.PictureBox();
            this.CTemplate = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CCar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CBike)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CTemplate)).BeginInit();
            this.SuspendLayout();
            // 
            // CCar
            // 
            this.CCar.BackColor = System.Drawing.Color.Transparent;
            this.CCar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CCar.BackgroundImage")));
            this.CCar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CCar.Location = new System.Drawing.Point(0, 3);
            this.CCar.Name = "CCar";
            this.CCar.Size = new System.Drawing.Size(210, 426);
            this.CCar.TabIndex = 0;
            this.CCar.TabStop = false;
            // 
            // CBike
            // 
            this.CBike.BackColor = System.Drawing.Color.Transparent;
            this.CBike.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CBike.BackgroundImage")));
            this.CBike.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CBike.Location = new System.Drawing.Point(396, 69);
            this.CBike.Name = "CBike";
            this.CBike.Size = new System.Drawing.Size(80, 195);
            this.CBike.TabIndex = 0;
            this.CBike.TabStop = false;
            // 
            // CTemplate
            // 
            this.CTemplate.BackColor = System.Drawing.Color.Transparent;
            this.CTemplate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CTemplate.BackgroundImage")));
            this.CTemplate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CTemplate.Image = ((System.Drawing.Image)(resources.GetObject("CTemplate.Image")));
            this.CTemplate.Location = new System.Drawing.Point(396, 292);
            this.CTemplate.Name = "CTemplate";
            this.CTemplate.Size = new System.Drawing.Size(80, 195);
            this.CTemplate.TabIndex = 0;
            this.CTemplate.TabStop = false;
            this.CTemplate.Visible = false;
            // 
            // Abstandsmeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.CTemplate);
            this.Controls.Add(this.CBike);
            this.Controls.Add(this.CCar);
            this.DoubleBuffered = true;
            this.Name = "Abstandsmeter";
            this.Size = new System.Drawing.Size(479, 490);
            this.Load += new System.EventHandler(this.Abstandsmeter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CCar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CBike)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CTemplate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox CCar;
        private System.Windows.Forms.PictureBox CBike;
        private System.Windows.Forms.PictureBox CTemplate;
    }
}
