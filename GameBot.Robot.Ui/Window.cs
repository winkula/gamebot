using Emgu.CV;
using Emgu.CV.UI;
using GameBot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Button = GameBot.Core.Data.Button;

namespace GameBot.Robot.Ui
{
    public partial class Window : Form
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int _originalWidth;
        private int _originalHeight;
        private int _processedWidth;
        private int _processedHeight;

        private readonly IClock _clock;
        private readonly IConfig _config;
        private readonly IEngine _engine;
        private readonly ICamera _camera;
        private readonly IActuator _actuator;
        private readonly IQuantizer _quantizer;

        private readonly KeyHandler _keyHandler = new KeyHandler();

        private bool _enabledKeypoints;
        private const int _maxKeypointCount = 4;
        private readonly List<Point> _keypoints = new List<Point>();
        //private List<Point> _keypointsApplied = new List<Point>();

        public Window(IClock clock, IConfig config, IEngine engine, ICamera camera, IActuator actuator, IQuantizer quantizer)
        {
            _clock = clock;
            _config = config;
            _engine = engine;
            _camera = camera;
            _actuator = actuator;
            _quantizer = quantizer;

            InitializeComponent();

            DefineActions();
            RegisterEvents();
            CheckForIllegalCrossThreadCalls = false;

            InitTitle();
            InitImageBoxes();
            InitTimer();
            InitForm();
        }

        private void DefineActions()
        {
            _keyHandler.OnKeyDown(Keys.Escape, Application.Exit);

            _keyHandler.OnKeyDown(Keys.P, () =>
            {
                _logger.Info("Play");
                _engine.Play = true;
            });
            _keyHandler.OnKeyDown(Keys.S, () =>
            {
                _logger.Info("Stop");
                _engine.Play = false;
            });
            _keyHandler.OnKeyDown(Keys.R, () =>
            {
                _logger.Info("Reset");
                _engine.Reset();
            });
            _keyHandler.OnKeyDown(Keys.K, ClearKeypoints);
            _keyHandler.OnKeyUp(Keys.K, SaveKeypoints);

            _keyHandler.OnKeyDown(Keys.Up, () => _actuator.Hit(Button.Up));
            _keyHandler.OnKeyDown(Keys.Down, () => _actuator.Press(Button.Down));
            _keyHandler.OnKeyUp(Keys.Down, () => _actuator.Release(Button.Down));
            _keyHandler.OnKeyDown(Keys.Left, () => _actuator.Hit(Button.Left));
            _keyHandler.OnKeyDown(Keys.Right, () => _actuator.Hit(Button.Right));

            _keyHandler.OnKeyDown(Keys.Y, () => _actuator.Hit(Button.A));
            _keyHandler.OnKeyDown(Keys.X, () => _actuator.Hit(Button.B));

            _keyHandler.OnKeyDown(Keys.Enter, () => _actuator.Hit(Button.Start));
            _keyHandler.OnKeyDown(Keys.Back, () => _actuator.Hit(Button.Select));
        }

        private void RegisterEvents()
        {
            FormClosed += (sender, args) => Application.Exit();
            Load += Loaded;
            KeyDown += (s, e) => _keyHandler.KeyDown(e.KeyCode);
            KeyUp += (s, e) => _keyHandler.KeyUp(e.KeyCode);
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
            double framerate = _config.Read("Robot.Ui.CamFramerate", 20.0);
            Timer.Interval = (int)Math.Max(1000 / framerate, 10);
        }

        private void InitTitle()
        {
            var time = _clock.Time;
            var playState = _engine.Play ? "Play" : "Pause";
            Text = $@"GameBot - {time:hh\:mm\:ss\.f} - [{playState}]";
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

        private void ClearKeypoints()
        {
            // clear
            _logger.Info("Reset keypoints");
            _keypoints.Clear();
            _enabledKeypoints = true;
        }

        private void SaveKeypoints()
        {
            // save
            if (_keypoints.Count >= _maxKeypointCount)
            {
                _logger.Info("Keypoints: " + string.Join(",", _keypoints));

                var keypointsArray = new[]
                {
                    _keypoints[0].X, _keypoints[0].Y,
                    _keypoints[1].X, _keypoints[1].Y,
                    _keypoints[2].X, _keypoints[2].Y,
                    _keypoints[3].X, _keypoints[3].Y
                };

                _quantizer.Keypoints = _keypoints.ToList();

                _config.Write("Robot.Quantizer.Transformation.KeyPoints", string.Join(",", keypointsArray));
                _config.Save();

                _logger.Info("Saved configuration");
            }
            _enabledKeypoints = false;
        }

        private void MouseClicked(object sender, MouseEventArgs e)
        {
            if (_enabledKeypoints && _keypoints.Count < _maxKeypointCount)
            {
                _keypoints.Add(new Point(e.X, e.Y));
            }
        }

        private void Loaded(object sender, EventArgs e)
        {
            var mainThread = new Thread(Run) { IsBackground = true };
            mainThread.Start();
        }

        private void Run()
        {
            try
            {
                _engine.Initialize();

                while (true)
                {
                    _engine.Step(ShowOriginal, ShowProcessed);
                }
            }
            catch (ThreadAbortException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                MessageBox.Show($"{ex.Message}\nRead the log for details.", "Error");
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
#if DEBUG
                throw;
#endif
                // ignore in release version
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
#if DEBUG
                throw;
#endif
                // ignore in release version
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var image = _camera.Capture();

            ShowOriginal(image);
            InitTitle();
        }
    }
}
