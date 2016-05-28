namespace GameBot.Robot.Ui
{
    partial class Window
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ImageBoxLeft = new Emgu.CV.UI.ImageBox();
            this.ImageBoxRight = new Emgu.CV.UI.ImageBox();
            this.Textbox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxRight)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ImageBoxLeft);
            this.flowLayoutPanel1.Controls.Add(this.ImageBoxRight);
            this.flowLayoutPanel1.Controls.Add(this.Textbox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(389, 277);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // ImageBoxLeft
            // 
            this.ImageBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.ImageBoxLeft.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBoxLeft.Name = "ImageBoxLeft";
            this.ImageBoxLeft.Size = new System.Drawing.Size(156, 127);
            this.ImageBoxLeft.TabIndex = 2;
            this.ImageBoxLeft.TabStop = false;
            // 
            // ImageBoxRight
            // 
            this.ImageBoxRight.Location = new System.Drawing.Point(156, 0);
            this.ImageBoxRight.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBoxRight.Name = "ImageBoxRight";
            this.ImageBoxRight.Size = new System.Drawing.Size(212, 127);
            this.ImageBoxRight.TabIndex = 2;
            this.ImageBoxRight.TabStop = false;
            // 
            // Textbox
            // 
            this.Textbox.Location = new System.Drawing.Point(0, 127);
            this.Textbox.Margin = new System.Windows.Forms.Padding(0);
            this.Textbox.Multiline = true;
            this.Textbox.Name = "Textbox";
            this.Textbox.ReadOnly = true;
            this.Textbox.Size = new System.Drawing.Size(139, 110);
            this.Textbox.TabIndex = 3;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 277);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.Text = "GameBot.Robot.Ui";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxRight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Emgu.CV.UI.ImageBox ImageBoxLeft;
        private Emgu.CV.UI.ImageBox ImageBoxRight;
        private System.Windows.Forms.TextBox Textbox;
    }
}

