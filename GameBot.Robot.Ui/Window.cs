using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using GameBot.Core.Ui;
using GameBot.Robot.Quantizers;
using GameBot.Core.Exceptions;

namespace GameBot.Robot.Ui
{
    public partial class Window : Form, IUi
    {
        private int leftWidth;
        private int leftHeight;
        private int rightWidth;
        private int rightHeight;
        private int textWidth = 200;

        private readonly IConfig config;
        private readonly IEngine engine;
        private readonly ICamera camera;
        private readonly IActuator actuator;
        private readonly Quantizer quantizer;
        private readonly IDebugger debugger;

        private bool play = false;

        private List<int> keypoints = new List<int>();
        private List<int> keypointsApplied = new List<int>();

        public Window(IConfig config, IEngine engine, ICamera camera, IActuator actuator, IDebugger debugger, IQuantizer quantizer)
        {
            this.config = config;
            this.engine = engine;
            this.camera = camera;
            this.actuator = actuator;
            this.debugger = debugger;
            this.quantizer = quantizer as Quantizer;

            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            InitDimensions();

            ImageBoxLeft.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            ImageBoxLeft.Width = leftWidth;
            ImageBoxLeft.Height = leftHeight;
            ImageBoxRight.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            ImageBoxRight.Width = rightWidth;
            ImageBoxRight.Height = rightHeight;
            TextboxStatic.Width = textWidth;
            TextboxStatic.Height = rightHeight;
            TextboxDynamic.Width = textWidth;
            TextboxDynamic.Height = rightHeight;

            var size = new Size(leftWidth + rightWidth + 2 * textWidth, leftHeight);
            MinimumSize = size;
            ClientSize = size;
            AutoSize = true;

            Load += Loaded;
            ImageBoxLeft.MouseClick += MouseClicked;
            KeyPreview = true;
            KeyPress += KeyPressed;
        }

        private void InitDimensions()
        {
            leftWidth = camera.Width;
            leftHeight = camera.Height;
            rightWidth = (leftHeight * 160) / 144;
            rightHeight = leftHeight;
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
                debugger.WriteDynamic("> press A");
            }
            if (e.KeyChar == 'x')
            {
                actuator.Hit(Core.Data.Button.B);
                debugger.WriteDynamic("> press B");
            }
            if (e.KeyChar == 'a')
            {
                actuator.Hit(Core.Data.Button.Left);
                debugger.WriteDynamic("> press Left");
            }
            if (e.KeyChar == 'd')
            {
                actuator.Hit(Core.Data.Button.Right);
                debugger.WriteDynamic("> press Right");
            }
            if (e.KeyChar == 'c')
            {
                actuator.Hit(Core.Data.Button.Start);
                debugger.WriteDynamic("> press Start");
            }
            if (e.KeyChar == 'v')
            {
                actuator.Hit(Core.Data.Button.Select);
                debugger.WriteDynamic("> press Select");
            }

            if (e.KeyChar == 'p')
            {
                play = !play;
                debugger.WriteDynamic(play ? "> play" : "> don't play");
            }
            if (e.KeyChar == 'q')
            {
                Application.Exit();
            }
            if (e.KeyChar == 'r')
            {
                // clear
                keypoints.Clear();
                debugger.ClearDynamic();
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
                    var result = engine.Step(play);
                    result.Processed = result.Processed.Resize(rightWidth, rightHeight, Inter.Linear);

                    stopwatch.Stop();
                    long ms = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();
                    if (ms != 0)
                    {
                        debugger.WriteStatic($"FPS: {1000 / ms}");
                    }

                    Show(result.Original, result.Processed);

                    TextboxDynamic.Text = string.Join(Environment.NewLine, debugger.ReadDynamic());
                    TextboxStatic.Text = string.Join(Environment.NewLine, debugger.ReadStatic());
                    debugger.ClearStatic();
                }
                catch (GameOverException)
                {
                    debugger.WriteDynamic("> Game over!");
                    play = false;
                }
            }
        }

        public void Show(IImage original, IImage processed)
        {
            try
            {
                ImageBoxLeft.Image = original;
                ImageBoxRight.Image = processed;
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }
    }
}
