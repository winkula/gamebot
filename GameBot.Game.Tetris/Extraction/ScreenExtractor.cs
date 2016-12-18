using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using GameBot.Core;
using GameBot.Core.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public class ScreenExtractor : IScreenExtractor
    {
        private const double _thresholdGameoverSingleplayer = 0.05; // TODO: test with real screenshots?
        private const double _thresholdGameoverMultiplayer = 0.05; // TODO: test with real screenshots?
        private const double _thresholdStartMultiplayer = 0.05; // TODO: test with real screenshots?
        
        private static readonly Mat _gameoverSingleplayerReferenceImage;
        private static readonly Mat _gameoverMultiplayerReferenceImage;
        private static readonly Mat _startMultiplayerReferenceImage;

        private static readonly Rectangle _roiBoard;
        private static readonly Rectangle _roiMultiplayerGameover;

        static ScreenExtractor()
        {
            // board dimensions on screen
            _roiBoard = new Rectangle(
                GameBoyConstants.TileSize * 2,
                GameBoyConstants.TileSize * 0,
                GameBoyConstants.TileSize * 10,
                GameBoyConstants.TileSize * 18);

            // interesting region of the multiplayer gameover screen
            _roiMultiplayerGameover = new Rectangle(
                GameBoyConstants.TileSize * 0,
                GameBoyConstants.TileSize * 3,
                GameBoyConstants.TileSize * 20,
                GameBoyConstants.TileSize * 12);

            _gameoverSingleplayerReferenceImage = new Mat("Screenshots/gameover_ref.png", LoadImageType.Grayscale);
            _gameoverMultiplayerReferenceImage = new Mat("Screenshots/multiplayer_gameover_ref.png", LoadImageType.Grayscale);
            _startMultiplayerReferenceImage = new Mat("Screenshots/multiplayer_start_ref.png", LoadImageType.Grayscale);
        }

        public bool IsGameOverSingleplayer(IScreenshot screenshot)
        {
            return IsMatch(screenshot,
                _gameoverSingleplayerReferenceImage,
                _roiBoard,
                _thresholdGameoverSingleplayer);
        }

        public bool IsGameOverMultiplayer(IScreenshot screenshot)
        {
            return IsMatch(screenshot, 
                _gameoverMultiplayerReferenceImage,
                _roiMultiplayerGameover,
                _thresholdGameoverMultiplayer);
        }

        public bool IsStart(IScreenshot screenshot)
        {
            return IsMatch(screenshot,
                _startMultiplayerReferenceImage,
                _roiBoard,
                _thresholdStartMultiplayer);
        }

        /*
        private bool IsMatch(IScreenshot screenshot, Mat reference, double threshold)
        {
            var result = new Mat();

            CvInvoke.AbsDiff(screenshot.Image, reference, result);
            var mean = CvInvoke.Mean(result);

            return mean.V0 <= threshold * 255;
        }
        */

        private bool IsMatch(IScreenshot screenshot, Mat reference, Rectangle roi, double threshold)
        {
            var result = new Mat();
            var screenshotRoi = new Mat(screenshot.Image, roi);
            var referenceRoi = new Mat(reference, roi);

            CvInvoke.AbsDiff(screenshotRoi, referenceRoi, result);
            var mean = CvInvoke.Mean(result);

            return mean.V0 <= threshold * 255;
        }
    }
}
