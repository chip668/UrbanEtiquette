
namespace Anzeige
{
    partial class Edit_Line
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
            this.CCaption = new System.Windows.Forms.Label();
            this.CText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CCaption
            // 
            this.CCaption.AutoSize = true;
            this.CCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.CCaption.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.CCaption.Location = new System.Drawing.Point(0, 0);
            this.CCaption.Name = "CCaption";
            this.CCaption.Size = new System.Drawing.Size(117, 40);
            this.CCaption.TabIndex = 0;
            this.CCaption.Text = "Caption";
            // 
            // CText
            // 
            this.CText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CText.Location = new System.Drawing.Point(0, 40);
            this.CText.Name = "CText";
            this.CText.Size = new System.Drawing.Size(856, 29);
            this.CText.TabIndex = 1;
            // 
            // Edit_Line
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CText);
            this.Controls.Add(this.CCaption);
            this.Name = "Edit_Line";
            this.Size = new System.Drawing.Size(856, 406);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CCaption;
        private System.Windows.Forms.TextBox CText;
    }
}
