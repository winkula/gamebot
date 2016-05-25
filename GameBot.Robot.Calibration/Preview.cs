using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using GameBot.Core;
using GameBot.Game.Tetris;
using GameBot.Robot.Configuration;
using GameBot.Robot.Quantizers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

namespace GameBot.Robot.Calibration
{
    public partial class Preview : Form
    {
        private int leftWidth;
        private int leftHeight;
        private int rightWidth;
        private int rightHeight;

        private static IConfig config = new Config();

        private Capture capture = default(Capture);
        private Quantizer quantizer = new Quantizer(config);
        private List<int> keypoints = new List<int>();
        private List<int> keypointsApplied = new List<int>();
        private TetrisExtractor extractor = new TetrisExtractor(config);

        public Preview()
        {
            InitializeComponent();

            capture = new Capture(config.Read<int>("Robot.Camera.Index"));
            CheckForIllegalCrossThreadCalls = false;

            InitDimensions();

            ImageBoxLeft.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            ImageBoxLeft.Width = leftWidth;
            ImageBoxLeft.Height = leftHeight;
            ImageBoxRight.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            ImageBoxRight.Width = rightWidth;
            ImageBoxRight.Height = rightHeight;

            var size = new Size(leftWidth + rightWidth, leftHeight);
            MinimumSize = size;
            ClientSize = size;
            
            AutoSize = true;

            Load += Loaded;
            ImageBoxLeft.MouseClick += MouseClickEvent;

            KeyPreview = true;
            KeyPress += Preview_KeyPress;
        }

        private void InitDimensions()
        {
            leftWidth = capture.Width;
            leftHeight = capture.Height;
            rightWidth = (leftHeight * 160) / 144;
            rightHeight = leftHeight;
        }

        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void Preview_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '+')
            {
                extractor.BlockThreshold = Clamp(extractor.BlockThreshold + 0.05f, 0.0f, 1.0f);
                Debug.WriteLine($"BlockThreshold: {extractor.BlockThreshold:0.00} %");
            }
            if (e.KeyChar == '-')
            {
                extractor.BlockThreshold = Clamp(extractor.BlockThreshold - 0.05f, 0.0f, 1.0f);
                Debug.WriteLine($"BlockThreshold: {extractor.BlockThreshold:0.00} %");
            }

            if (e.KeyChar == 'c')
            {
                // clear
                keypoints.Clear();
                Debug.WriteLine("cleared already recorded keypoints");
            }
            if (e.KeyChar == 's')
            {
                // save
                if (keypointsApplied.Count == 8)
                {
                    var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                    config.AppSettings.Settings["Robot.Quantizer.Transformation.KeyPoints"].Value =  string.Join(",", keypointsApplied);
                    config.AppSettings.Settings["Game.Tetris.Extractor.BlockThreshold"].Value = extractor.BlockThreshold.ToString();
                    config.Save(ConfigurationSaveMode.Modified);
                    Debug.WriteLine("saved configuration");
                }
            }
        }

        private void MouseClickEvent(object sender, MouseEventArgs e)
        {
            keypoints.Add(e.X);
            keypoints.Add(e.Y);
            Debug.WriteLine("added keypoint");

            if (keypoints.Count >= 8)
            {
                keypointsApplied = keypoints.Take(8).ToList();
                quantizer.CalculatePerspectiveTransform(keypointsApplied.Select(x => (float)x));
                keypoints.Clear();
                Debug.WriteLine("applied keypoints");
            }
        }

        private void Loaded(object sender, EventArgs e)
        {
            var t = new Thread(Loop);
            t.IsBackground = true;
            t.Start();
        }
        
        public void Loop()
        {
            while (true)
            {
                var src = capture.QueryFrame();
                var dst = new Image<Gray, byte>(src.Width, src.Height);
                CvInvoke.CvtColor(src, dst, ColorConversion.Rgb2Gray);

                var screenshot = quantizer.Quantize(dst, new TimeSpan());
                extractor.Extract(screenshot);

                var imageLeft = src.ToImage<Bgr, byte>();

                var imageRight = new Image<Bgr, byte>(quantizer.ImageOutput.Bitmap);
                foreach (var rectangle in extractor.Rectangles)
                {
                    imageRight.Draw(new Rectangle(8 * rectangle.X, 8 * rectangle.Y, 8, 8), new Bgr(0, 0, 255), 1);
                }
                imageRight = imageRight.Resize(rightWidth, rightHeight, Inter.Linear);

                SetImage(imageLeft, imageRight);
            }
        }

        public void SetImage(IImage image1, IImage image2)
        {
            try
            {
                ImageBoxLeft.Image = image1;
                ImageBoxRight.Image = image2;
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }
    }
}
