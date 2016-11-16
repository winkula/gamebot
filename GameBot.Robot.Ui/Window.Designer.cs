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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.ImageBoxOriginal = new Emgu.CV.UI.ImageBox();
            this.ImageBoxProcessed = new Emgu.CV.UI.ImageBox();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxProcessed)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBoxOriginal
            // 
            this.ImageBoxOriginal.Location = new System.Drawing.Point(0, 0);
            this.ImageBoxOriginal.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBoxOriginal.Name = "ImageBoxOriginal";
            this.ImageBoxOriginal.Size = new System.Drawing.Size(391, 276);
            this.ImageBoxOriginal.TabIndex = 2;
            this.ImageBoxOriginal.TabStop = false;
            // 
            // ImageBoxProcessed
            // 
            this.ImageBoxProcessed.Location = new System.Drawing.Point(283, 0);
            this.ImageBoxProcessed.Margin = new System.Windows.Forms.Padding(0);
            this.ImageBoxProcessed.Name = "ImageBoxProcessed";
            this.ImageBoxProcessed.Size = new System.Drawing.Size(108, 80);
            this.ImageBoxProcessed.TabIndex = 3;
            this.ImageBoxProcessed.TabStop = false;
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Interval = 50;
            this.Timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 277);
            this.Controls.Add(this.ImageBoxProcessed);
            this.Controls.Add(this.ImageBoxOriginal);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Window";
            this.Text = "GameBot.Robot.Ui";
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBoxProcessed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox ImageBoxOriginal;
        private Emgu.CV.UI.ImageBox ImageBoxProcessed;
        private System.Windows.Forms.Timer Timer;
    }
}

