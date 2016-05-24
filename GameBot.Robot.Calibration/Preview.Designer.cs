namespace GameBot.Robot.Calibration
{
    partial class Preview
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
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxRight)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ImageBoxLeft);
            this.flowLayoutPanel1.Controls.Add(this.ImageBoxRight);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(389, 277);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // imageBox1
            // 
            this.ImageBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.ImageBoxLeft.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBoxLeft.Name = "imageBox1";
            this.ImageBoxLeft.Size = new System.Drawing.Size(156, 127);
            this.ImageBoxLeft.TabIndex = 2;
            this.ImageBoxLeft.TabStop = false;
            // 
            // imageBox2
            // 
            this.ImageBoxRight.Location = new System.Drawing.Point(156, 0);
            this.ImageBoxRight.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBoxRight.Name = "imageBox2";
            this.ImageBoxRight.Size = new System.Drawing.Size(212, 127);
            this.ImageBoxRight.TabIndex = 2;
            this.ImageBoxRight.TabStop = false;
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 277);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Preview";
            this.Text = "GameBot.Robot.Calibration";
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxRight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Emgu.CV.UI.ImageBox ImageBoxLeft;
        private Emgu.CV.UI.ImageBox ImageBoxRight;
    }
}

