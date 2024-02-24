
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
            this.CCar2 = new System.Windows.Forms.PictureBox();
            this.CExpand = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CCar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CBike)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CCar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CExpand)).BeginInit();
            this.SuspendLayout();
            // 
            // CCar
            // 
            this.CCar.BackColor = System.Drawing.Color.Transparent;
            this.CCar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CCar.BackgroundImage")));
            this.CCar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CCar.Location = new System.Drawing.Point(0, 60);
            this.CCar.Name = "CCar";
            this.CCar.Size = new System.Drawing.Size(210, 426);
            this.CCar.TabIndex = 0;
            this.CCar.TabStop = false;
            this.CCar.Click += new System.EventHandler(this.Abstandsmeter_Click);
            // 
            // CBike
            // 
            this.CBike.BackColor = System.Drawing.Color.Transparent;
            this.CBike.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CBike.BackgroundImage")));
            this.CBike.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CBike.Location = new System.Drawing.Point(400, 60);
            this.CBike.Name = "CBike";
            this.CBike.Size = new System.Drawing.Size(80, 195);
            this.CBike.TabIndex = 0;
            this.CBike.TabStop = false;
            this.CBike.Click += new System.EventHandler(this.Abstandsmeter_Click);
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
            // CCar2
            // 
            this.CCar2.BackColor = System.Drawing.Color.Transparent;
            this.CCar2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CCar2.BackgroundImage")));
            this.CCar2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CCar2.Location = new System.Drawing.Point(580, 62);
            this.CCar2.Name = "CCar2";
            this.CCar2.Size = new System.Drawing.Size(210, 426);
            this.CCar2.TabIndex = 0;
            this.CCar2.TabStop = false;
            // 
            // CExpand
            // 
            this.CExpand.BackColor = System.Drawing.Color.Transparent;
            this.CExpand.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CExpand.BackgroundImage")));
            this.CExpand.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CExpand.Location = new System.Drawing.Point(0, 0);
            this.CExpand.Name = "CExpand";
            this.CExpand.Size = new System.Drawing.Size(21, 42);
            this.CExpand.TabIndex = 1;
            this.CExpand.TabStop = false;
            this.CExpand.Visible = false;
            this.CExpand.Click += new System.EventHandler(this.CExpand_Click);
            // 
            // Abstandsmeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.CExpand);
            this.Controls.Add(this.CTemplate);
            this.Controls.Add(this.CBike);
            this.Controls.Add(this.CCar2);
            this.Controls.Add(this.CCar);
            this.DoubleBuffered = true;
            this.Name = "Abstandsmeter";
            this.Size = new System.Drawing.Size(582, 490);
            this.Load += new System.EventHandler(this.Abstandsmeter_Load);
            this.Click += new System.EventHandler(this.Abstandsmeter_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Abstandsmeter_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.CCar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CBike)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CCar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CExpand)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox CCar;
        private System.Windows.Forms.PictureBox CBike;
        private System.Windows.Forms.PictureBox CTemplate;
        private System.Windows.Forms.PictureBox CCar2;
        private System.Windows.Forms.PictureBox CExpand;
    }
}
