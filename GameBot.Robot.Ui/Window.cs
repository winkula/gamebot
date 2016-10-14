using Emgu.CV;
using Emgu.CV.UI;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GameBot.Core.Exceptions;
using NLog;

namespace GameBot.Robot.Ui
{
    public partial class Window : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private int originalWidth;
        private int originalHeight;
        private int processedWidth;
        private int processedHeight;

        private readonly IConfig config;
        private readonly IEngine engine;
        private readonly ICamera camera;
        private readonly IActuator actuator;
        private readonly ICalibrateableQuantizer quantizer;

        private const int maxKeypointCount = 4;
        private List<Point> keypoints = new List<Point>();
        private List<Point> keypointsApplied = new List<Point>();

        public Window(IConfig config, IEngine engine, ICamera camera, IActuator actuator, IQuantizer quantizer)
        {
            this.config = config;
            this.engine = engine;
            this.camera = camera;
            this.actuator = actuator;
            this.quantizer = quantizer as ICalibrateableQuantizer;
            if (quantizer == null)
            {
                logger.Warn("Quantizer is not calibrateable");
            }

            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            InitImageBoxes();
            InitForm();

            Load += Loaded;
            ImageBoxOriginal.MouseClick += MouseClicked;
            KeyPreview = true;
            KeyPress += KeyPressed;
        }

        private void InitImageBoxes()
        {
            originalWidth = camera.Width;
            originalHeight = camera.Height;
            processedWidth = GameBoyConstants.ScreenWidth;
            processedHeight = GameBoyConstants.ScreenHeight;

            ImageBoxOriginal.Size = new Size(originalWidth, originalHeight);
            ImageBoxOriginal.Location = new Point(0, 0);
            ImageBoxOriginal.Width = originalWidth;
            ImageBoxOriginal.Height = originalHeight;
            ImageBoxOriginal.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;

            ImageBoxProcessed.Size = new Size(processedWidth, processedHeight);
            ImageBoxProcessed.Location = new Point(originalWidth - processedWidth, 0);
            ImageBoxProcessed.Width = processedWidth;
            ImageBoxProcessed.Height = processedHeight;
            ImageBoxProcessed.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
        }

        private void InitForm()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            AutoSize = true;
            ClientSize = new Size(originalWidth, originalHeight);

            CenterToScreen();
        }

        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'y')
            {
                actuator.Hit(Core.Data.Button.A);
            }
            if (e.KeyChar == 'x')
            {
                actuator.Hit(Core.Data.Button.B);
            }
            if (e.KeyChar == 'a')
            {
                actuator.Hit(Core.Data.Button.Left);
            }
            if (e.KeyChar == 'd')
            {
                actuator.Hit(Core.Data.Button.Right);
            }
            if (e.KeyChar == 'c')
            {
                actuator.Hit(Core.Data.Button.Start);
            }
            if (e.KeyChar == 'v')
            {
                actuator.Hit(Core.Data.Button.Select);
            }

            if (e.KeyChar == 'p')
            {
                engine.Play = !engine.Play;
                logger.Info(engine.Play ? "Play A.I." : "Pause A.I.");
            }
            if (e.KeyChar == 'q')
            {
                Application.Exit();
            }
            if (e.KeyChar == 'r')
            {
                // clear
                keypoints.Clear();
                logger.Info("Reset temporary keypoints");
            }
            if (e.KeyChar == 's')
            {
                // save
                if (keypointsApplied.Count == maxKeypointCount)
                {
                    logger.Info("Keypoints: " + string.Join(",", keypointsApplied));

                    var keypointsList = new int[] { keypointsApplied[0].X, keypointsApplied[0].Y, keypointsApplied[1].X, keypointsApplied[1].Y, keypointsApplied[2].X, keypointsApplied[2].Y, keypointsApplied[3].X, keypointsApplied[3].Y };
                    config.Write("Robot.Quantizer.Transformation.KeyPoints", string.Join(",", keypointsList));
                    config.Save();

                    logger.Info("Saved configuration");
                }
            }
        }

        private void MouseClicked(object sender, MouseEventArgs e)
        {
            keypoints.Add(new Point(e.X, e.Y));
            logger.Info($"Added keypoint ({e.X}, {e.Y})");

            if (keypoints.Count >= maxKeypointCount && quantizer != null)
            {                
                keypointsApplied = keypoints.Take(maxKeypointCount).ToList();

                var keypointsList = new int[] { keypointsApplied[0].X, keypointsApplied[0].Y, keypointsApplied[1].X, keypointsApplied[1].Y, keypointsApplied[2].X, keypointsApplied[2].Y, keypointsApplied[3].X, keypointsApplied[3].Y };

                quantizer.Calibrate(keypointsApplied);
                keypoints.Clear();
                logger.Info($"Applied keypoints {string.Join(",", keypointsList)}");
            }
        }

        private void Loaded(object sender, EventArgs e)
        {
            var t = new Thread(Run);
            t.IsBackground = true;
            t.Start();
        }

        public void Run()
        {
            var stopwatch = new Stopwatch();

            engine.Initialize();

            while (true)
            {
                try
                {
                    engine.Step(Show);

                    stopwatch.Stop();
                    long ms = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                    if (ms != 0)
                    {
                        Text = $"GameBot.Robot.Ui ({1000 / ms} fps)";
                    }
                }
                catch (GameOverException)
                {
                    logger.Info("Game over");
                    engine.Play = false;
                }
            }
        }

        public void Show(IImage original, IImage processed)
        {
            try
            {
                ImageBoxOriginal.Image = original;
                ImageBoxProcessed.Image = processed;
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }
    }
}
