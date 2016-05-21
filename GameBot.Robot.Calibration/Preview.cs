using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using GameBot.Robot.Configuration;
using GameBot.Robot.Quantizers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace GameBot.Robot.Calibration
{
    public partial class Preview : Form
    {
        private Capture capture = default(Capture);

        public Preview()
        {
            InitializeComponent();
            imageBox1.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            imageBox2.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            splitContainer1.SplitterWidth = 1;
            splitContainer1.Panel1MinSize = 640;
            splitContainer1.Panel2MinSize = 166;
            splitContainer1.Width = 1000;
            splitContainer1.Height = 480;

           // MinimumSize = new System.Drawing.Size(1000, 480);
           // MaximumSize = new System.Drawing.Size(1000, 580);
            ClientSize = new System.Drawing.Size(1000, 480);

            // Width = 1000;
            //Height = 480;
            AutoSize = true;     

            Load += Loaded;
            imageBox1.MouseClick += MouseClickEvent;
        }

        private void MouseClickEvent(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(e.X + " -- " + e.Y);
        }

        private void Loaded(object sender, EventArgs e)
        {
            Init();
            
            var t = new Thread(Loop);
            t.IsBackground = true;
            t.Start();
        }
        
        public void Init()
        {
            capture = new Capture(1);
            CheckForIllegalCrossThreadCalls = false;
            //capture.Start();
        }

        public void Loop()
        {
            while (true)
            {
                /*
                var src = new Image<Bgr, byte>(480, 320);
                capture.Grab();
                capture.Retrieve(src, 0);
                CvInvoke.CvtColor(src, dst, ColorConversion.Rgb2Gray);
                */
                //var test = new Image<Gray, byte>(@"C:\Users\Winkler\Pictures\Saved Pictures\IMG_20160115_164732 (2).jpg");
                var src = capture.QueryFrame();
                var dst = new Image<Gray, byte>(src.Width, src.Height);
                CvInvoke.CvtColor(src, dst, ColorConversion.Rgb2Gray);

                var quantizer = new Quantizer(new Config());
                var screenshot = quantizer.Quantize(dst, new TimeSpan());

                SetImage(src, quantizer.ImageOutput);
            }
        }

        public void SetImage(IImage image1, IImage image2)
        {
            try
            {
                imageBox1.Image = image1;
                imageBox2.Image = image2;
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }
    }
}
