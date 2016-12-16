using Emgu.CV;
using Emgu.CV.UI;
using GameBot.Core;
using System;
using System.Drawing;
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

        private readonly KeyHandler _keyHandler;
        private readonly Calibrator _calibrator;

        public Window(IClock clock, IConfig config, IEngine engine, ICamera camera, IActuator actuator, IQuantizer quantizer)
        {
            _clock = clock;
            _config = config;
            _engine = engine;
            _camera = camera;
            _actuator = actuator;

            _keyHandler = new KeyHandler();
            _calibrator = new Calibrator(config, quantizer);

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
                _engine.Play = false;
                _engine.Send("reset");
            });
            _keyHandler.OnKeyDown(Keys.K, () => _calibrator.ClearKeypoints());

            _keyHandler.OnKeyDown(Keys.Up, () => _actuator.Hit(Button.Up));
            _keyHandler.OnKeyDown(Keys.Down, () => _actuator.Press(Button.Down));
            _keyHandler.OnKeyUp(Keys.Down, () => _actuator.Release(Button.Down));
            _keyHandler.OnKeyDown(Keys.Left, () => _actuator.Hit(Button.Left));
            _keyHandler.OnKeyDown(Keys.Right, () => _actuator.Hit(Button.Right));

            _keyHandler.OnKeyDown(Keys.Y, () => _actuator.Hit(Button.A));
            _keyHandler.OnKeyDown(Keys.X, () => _actuator.Hit(Button.B));

            _keyHandler.OnKeyDown(Keys.Enter, () => _actuator.Hit(Button.Start));
            _keyHandler.OnKeyDown(Keys.Back, () => _actuator.Hit(Button.Select));

            _keyHandler.OnKeyDown(Keys.L, () => _engine.Send("select level"));
            _keyHandler.OnKeyDown(Keys.H, () => _engine.Send("highscore"));
            _keyHandler.OnKeyDown(Keys.M, () => _engine.Send("menu"));
            _keyHandler.OnKeyDown(Keys.G, () =>
            {
                _engine.Play = true;
                _engine.Send("start from game over");
            });
        }

        private void RegisterEvents()
        {
            Load += (s, e) => new Thread(Run) { IsBackground = true }.Start();

            FormClosed += (sender, args) => Application.Exit();
            KeyDown += (s, e) => _keyHandler.KeyDown(e.KeyCode);
            KeyUp += (s, e) => _keyHandler.KeyUp(e.KeyCode);

            ImageBoxOriginal.MouseClick += (s, e) => _calibrator.AddKeypoint(new Point(e.X, e.Y));
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
#if DEBUG
                throw;
#endif
#if RELEASE
                MessageBox.Show($"{ex.Message}\nRead the log for details.", "Error");           
#endif
            }
        }

        private void ShowOriginal(Mat original)
        {
            try
            {
                ImageBoxOriginal.Image = original;
            }
            catch (ObjectDisposedException ex)
            {
                // ignore in release version
                _logger.Error(ex);
#if DEBUG
                throw;
#endif
            }
        }

        private void ShowProcessed(Mat processed)
        {
            try
            {
                ImageBoxProcessed.Image = processed;
            }
            catch (ObjectDisposedException ex)
            {
                // ignore in release version
                _logger.Error(ex);
#if DEBUG
                throw;
#endif
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
