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
using GameBot.Core.Ui;
using GameBot.Robot.Quantizers;
using GameBot.Core.Exceptions;

namespace GameBot.Robot.Ui
{
    public partial class Window : Form, IUi
    {
        private int originalWidth;
        private int originalHeight;
        private int processedWidth;
        private int processedHeight;

        private readonly IConfig config;
        private readonly IEngine engine;
        private readonly ICamera camera;
        private readonly IActuator actuator;
        private readonly Quantizer quantizer;

        private bool play = false;

        private List<int> keypoints = new List<int>();
        private List<int> keypointsApplied = new List<int>();

        public Window(IConfig config, IEngine engine, ICamera camera, IActuator actuator, IQuantizer quantizer)
        {
            this.config = config;
            this.engine = engine;
            this.camera = camera;
            this.actuator = actuator;
            this.quantizer = quantizer as Quantizer;

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
            originalWidth = 800;// camera.Width;
            originalHeight = 600;// camera.Height;
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
                Debug.WriteLine("> press A");
            }
            if (e.KeyChar == 'x')
            {
                actuator.Hit(Core.Data.Button.B);
                Debug.WriteLine("> press B");
            }
            if (e.KeyChar == 'a')
            {
                actuator.Hit(Core.Data.Button.Left);
                Debug.WriteLine("> press Left");
            }
            if (e.KeyChar == 'd')
            {
                actuator.Hit(Core.Data.Button.Right);
                Debug.WriteLine("> press Right");
            }
            if (e.KeyChar == 'c')
            {
                actuator.Hit(Core.Data.Button.Start);
                Debug.WriteLine("> press Start");
            }
            if (e.KeyChar == 'v')
            {
                actuator.Hit(Core.Data.Button.Select);
                Debug.WriteLine("> press Select");
            }

            if (e.KeyChar == 'p')
            {
                play = !play;
                Debug.WriteLine(play ? "> play" : "> don't play");
            }
            if (e.KeyChar == 'q')
            {
                Application.Exit();
            }
            if (e.KeyChar == 'r')
            {
                // clear
                keypoints.Clear();
                Debug.WriteLine("reset temp stuff");
            }
            if (e.KeyChar == 's')
            {
                // save
                if (keypointsApplied.Count == 8)
                {
                    Debug.WriteLine("keypoints: " + string.Join(",", keypointsApplied));

                    config.Write("Robot.Quantizer.Transformation.KeyPoints", string.Join(",", keypointsApplied));
                    config.Save();

                    Debug.WriteLine("saved configuration");
                }
            }
        }

        private void MouseClicked(object sender, MouseEventArgs e)
        {
            keypoints.Add(e.X);
            keypoints.Add(e.Y);
            Debug.WriteLine("added keypoint");

            if (keypoints.Count >= 8 && quantizer != null)
            {
                keypointsApplied = keypoints.Take(8).ToList();
                quantizer.CalculatePerspectiveTransform(keypointsApplied.Select(x => (float)x));
                keypoints.Clear();
                Debug.WriteLine("applied keypoints");
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
                    engine.Step(play, Show);

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
                    Debug.WriteLine("> Game over!");
                    play = false;
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
