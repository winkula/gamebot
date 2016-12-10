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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _originalWidth;
        private int _originalHeight;
        private int _processedWidth;
        private int _processedHeight;

        private readonly IConfig _config;
        private readonly IEngine _engine;
        private readonly ICamera _camera;
        private readonly IActuator _actuator;
        private readonly IQuantizer _quantizer;

        private const int _maxKeypointCount = 4;
        private readonly List<Point> _keypoints = new List<Point>();
        private List<Point> _keypointsApplied = new List<Point>();

        public Window(IConfig config, IEngine engine, ICamera camera, IActuator actuator, IQuantizer quantizer)
        {
            _config = config;
            _engine = engine;
            _camera = camera;
            _actuator = actuator;
            _quantizer = quantizer;
            if (quantizer == null)
            {
                _logger.Warn("Quantizer is not calibrateable");
            }

            InitializeComponent();

            RegisterEvents();
            CheckForIllegalCrossThreadCalls = false;

            InitImageBoxes();
            InitTimer();
            InitForm();
        }

        private void RegisterEvents()
        {
            FormClosed += (sender, args) => Application.Exit();
            Load += Loaded;
            KeyPress += KeyPressed;
            ImageBoxOriginal.MouseClick += MouseClicked;
        }

        private void InitImageBoxes()
        {
            _originalWidth = Math.Max(_camera.Width, 2 * GameBoyConstants.ScreenWidth);
            _originalHeight = Math.Max(_camera.Height, GameBoyConstants.ScreenHeight);
            _processedWidth = GameBoyConstants.ScreenWidth;
            _processedHeight = GameBoyConstants.ScreenHeight;

            ImageBoxOriginal.Size = new Size(_originalWidth, _originalHeight);
            ImageBoxOriginal.Location = new Point(0, 0);
            ImageBoxOriginal.Width = _originalWidth;
            ImageBoxOriginal.Height = _originalHeight;
            ImageBoxOriginal.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;

            ImageBoxProcessed.Size = new Size(_processedWidth, _processedHeight);
            ImageBoxProcessed.Location = new Point(_originalWidth - _processedWidth, 0);
            ImageBoxProcessed.Width = _processedWidth;
            ImageBoxProcessed.Height = _processedHeight;
            ImageBoxProcessed.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
        }

        private void InitTimer()
        {
            double framerate = _config.Read("Robot.Ui.CamFramerate", 20);
            Timer.Interval = (int)Math.Max(1000 / framerate, 10);
        }

        private void InitForm()
        {
            KeyPreview = true;

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            AutoSize = true;
            ClientSize = new Size(_originalWidth, _originalHeight);

            CenterToScreen();
        }

        private void KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'y')
            {
                _actuator.Hit(Core.Data.Button.A);
            }
            if (e.KeyChar == 'x')
            {
                _actuator.Hit(Core.Data.Button.B);
            }
            if (e.KeyChar == 'a')
            {
                _actuator.Hit(Core.Data.Button.Left);
            }
            if (e.KeyChar == 'd')
            {
                _actuator.Hit(Core.Data.Button.Right);
            }
            if (e.KeyChar == 'w')
            {
                _actuator.Hit(Core.Data.Button.Up);
            }
            if (e.KeyChar == 's')
            {
                _actuator.Hit(Core.Data.Button.Down);
            }
            if (e.KeyChar == 'c')
            {
                _actuator.Hit(Core.Data.Button.Start);
            }
            if (e.KeyChar == 'v')
            {
                _actuator.Hit(Core.Data.Button.Select);
            }
            if (e.KeyChar == 'r')
            {
                _engine.Reset();
            }

            if (e.KeyChar == 'p')
            {
                _engine.Play = !_engine.Play;
                _logger.Info(_engine.Play ? "Play Agent" : "Pause Agent");
            }
            if (e.KeyChar == 'q')
            {
                Application.Exit();
            }
            if (e.KeyChar == 'k')
            {
                // clear
                _keypoints.Clear();
                _logger.Info("Reset temporary keypoints");
            }
            if (e.KeyChar == 's')
            {
                // save
                if (_keypointsApplied.Count == _maxKeypointCount)
                {
                    _logger.Info("Keypoints: " + string.Join(",", _keypointsApplied));

                    var keypointsList = new[]
                    {
                        _keypointsApplied[0].X, _keypointsApplied[0].Y,
                        _keypointsApplied[1].X, _keypointsApplied[1].Y,
                        _keypointsApplied[2].X, _keypointsApplied[2].Y,
                        _keypointsApplied[3].X, _keypointsApplied[3].Y
                    };

                    _config.Write("Robot.Quantizer.Transformation.KeyPoints", string.Join(",", keypointsList));
                    _config.Save();

                    _logger.Info("Saved configuration");
                }
            }
        }

        private void MouseClicked(object sender, MouseEventArgs e)
        {
            _keypoints.Add(new Point(e.X, e.Y));
            _logger.Info($"Added keypoint ({e.X}, {e.Y})");

            if (_keypoints.Count >= _maxKeypointCount && _quantizer != null)
            {
                _keypointsApplied = _keypoints.Take(_maxKeypointCount).ToList();

                var keypointsList = new[] { _keypointsApplied[0].X, _keypointsApplied[0].Y, _keypointsApplied[1].X, _keypointsApplied[1].Y, _keypointsApplied[2].X, _keypointsApplied[2].Y, _keypointsApplied[3].X, _keypointsApplied[3].Y };

                _quantizer.Keypoints = _keypointsApplied;
                _keypoints.Clear();
                _logger.Info($"Applied keypoints {string.Join(",", keypointsList)}");
            }
        }

        private void Loaded(object sender, EventArgs e)
        {
            var t = new Thread(Run) { IsBackground = true };
            t.Start();
        }

        private void Run()
        {
            var stopwatch = new Stopwatch();

            _engine.Initialize();

            while (true)
            {
                try
                {
                    _engine.Step(ShowOriginal, ShowProcessed);

                    stopwatch.Stop();
                    long ms = stopwatch.ElapsedMilliseconds;
                    stopwatch.Restart();

                    if (ms != 0)
                    {
                        Text = $"GameBot.Robot.Ui ({ms} ms)";
                    }
                }
                catch (GameOverException)
                {
                    _logger.Info("Game over");
                    _engine.Play = false;
                }
            }
        }

        private void ShowOriginal(Mat original)
        {
            try
            {
                ImageBoxOriginal.Image = original;
            }
            catch (ObjectDisposedException)
            {
                throw;
                // ignore
            }
        }

        private void ShowProcessed(Mat processed)
        {
            try
            {
                ImageBoxProcessed.Image = processed;
            }
            catch (ObjectDisposedException)
            {
                throw;
                // ignore
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var image = _camera.Capture();
            ShowOriginal(image);
        }
    }
}
