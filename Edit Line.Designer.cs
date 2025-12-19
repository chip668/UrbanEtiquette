
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
            CCaption = new System.Windows.Forms.Label();
            CText = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // CCaption
            // 
            CCaption.AutoSize = true;
            CCaption.Dock = System.Windows.Forms.DockStyle.Top;
            CCaption.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            CCaption.Location = new System.Drawing.Point(0, 0);
            CCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            CCaption.Name = "CCaption";
            CCaption.Size = new System.Drawing.Size(117, 40);
            CCaption.TabIndex = 0;
            CCaption.Text = "Caption";
            // 
            // CText
            // 
            CText.Dock = System.Windows.Forms.DockStyle.Fill;
            CText.Location = new System.Drawing.Point(0, 40);
            CText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            CText.Name = "CText";
            CText.Size = new System.Drawing.Size(666, 23);
            CText.TabIndex = 1;
            CText.TextChanged += CText_TextChanged;
            // 
            // Edit_Line
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(CText);
            Controls.Add(CCaption);
            Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            Name = "Edit_Line";
            Size = new System.Drawing.Size(666, 290);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label CCaption;
        private System.Windows.Forms.TextBox CText;
    }
}
