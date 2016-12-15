using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameBot.Core;
using NLog;

namespace GameBot.Robot.Ui
{
    public class Calibrator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int _maxKeypointCount = 4;

        private readonly IConfig _config;
        private readonly IQuantizer _quantizer;

        private readonly List<Point> _keypointsTemp = new List<Point>();
        private readonly List<Point> _keypoints = new List<Point>();
        
        public Calibrator(IConfig config, IQuantizer quantizer)
        {
            _config = config;
            _quantizer = quantizer;
        }

        public void AddKeypoint(Point point)
        {
            lock (_keypointsTemp)
            {
                if (_keypointsTemp.Count < _maxKeypointCount)
                {
                    _keypointsTemp.Add(point);
                }
                
                if (_keypointsTemp.Count == _maxKeypointCount)
                {
                    // we have the desired number of keypoints

                    var keypoints = _keypointsTemp.ToList();
                    _keypointsTemp.Clear();

                    _keypoints.Clear();
                    _keypoints.AddRange(keypoints);
                    
                    Apply(keypoints);
                    Save(keypoints);
                }
            }
        }

        private void Apply(IList<Point> keypoints)
        {
            _quantizer.Keypoints = keypoints;
        }
        
        public void ClearKeypoints()
        {
            // clear
            _logger.Info("Clear keypoints");
            _keypoints.Clear();

            lock (_keypointsTemp)
            {
                _keypointsTemp.Clear();
            }
        }

        private void Save(IList<Point> keypoints)
        {
            var keypointsArray = new[]
            {
                keypoints[0].X, keypoints[0].Y,
                keypoints[1].X, keypoints[1].Y,
                keypoints[2].X, keypoints[2].Y,
                keypoints[3].X, keypoints[3].Y
            };

            _logger.Info("Keypoints: " + string.Join(",", keypointsArray));

            _config.Write("Robot.Quantizer.Transformation.KeyPoints", string.Join(",", keypointsArray));
            _config.Save();

            _logger.Info("Saved keypoints");
        }
    }
}
