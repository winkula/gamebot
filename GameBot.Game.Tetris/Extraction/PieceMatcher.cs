using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Extensions;
using GameBot.Game.Tetris.Data;

namespace GameBot.Game.Tetris.Extraction
{
    public class PieceMatcher
    {
        private const int _maxShiftDistance = 1; // must be 0 or greater
        
        // template data
        private const int _templateSize = 4 * GameBoyConstants.TileSize;
        private static readonly int[,] _templateIndexTable = { { 0, 0, 0, 0 }, { 1, 2, 1, 2 }, { 3, 4, 3, 4 }, { 5, 6, 5, 6 }, { 7, 8, 9, 10 }, { 11, 12, 13, 14 }, { 15, 16, 17, 18 } };
        private readonly Image<Gray, byte>[] _templates = new Image<Gray, byte>[Tetriminos.AllPossibleOrientations];
        private readonly Image<Gray, byte>[] _templateMasks = new Image<Gray, byte>[Tetriminos.AllPossibleOrientations];

        private static readonly double[] _maxErrors = 
        {
            255.0 * 176, // O
            255.0 * 76, // I
            255.0 * 160, // S
            255.0 * 128, // Z
            255.0 * 112, // L
            255.0 * 160, // J
            255.0 * 160 // T
        };

        public PieceMatcher()
        {
            // init template tiles
            //var templateTiles = new Image<Gray, byte>(Resources.TemplatesGrayscale);
            //var templateTiles = new Image<Gray, byte>(Resources.TemplatesBinary);
            var templateTiles = new Image<Gray, byte>(Resources.TemplatesEdges);
            for (int i = 0; i < Tetriminos.AllPossibleOrientations; i++)
            {
                var template = new Image<Gray, byte>(_templateSize, _templateSize);
                var templateMask = new Image<Gray, byte>(_templateSize, _templateSize);
                templateTiles.ROI = new Rectangle(0, i * _templateSize, _templateSize, _templateSize);
                templateTiles.CopyTo(template);
                templateTiles.ROI = new Rectangle(_templateSize, i * _templateSize, _templateSize, _templateSize);
                templateTiles.CopyTo(templateMask);

                _templates[i] = template;
                _templateMasks[i] = templateMask;
            }
        }

        /// <summary>
        /// Gets the probability that a specific piece is visible on the screenshot.
        /// Test results:
        /// - With adaptive binarization:     piece (68 - 91 %), empty (41 - 45 %)
        /// - Without any filter (greyscale): piece (%), empty (%)        
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="piece">The piece to match.</param>
        /// <returns>The probability.</returns>
        public double GetProbability(IScreenshot screenshot, Piece piece)
        {
            // we need some extra space over the screenshot, so that the template is not off the image
            // we add an extra one that we can make 1 pixel shifts to compensate errors in the camera calibration
            const int yTopPadding = GameBoyConstants.TileSize + _maxShiftDistance;
            const int yBottomPadding = GameBoyConstants.TileSize + _maxShiftDistance;

            // take upper left block in the 4 by 4 search window
            var tileCoordinates = Coordinates.PieceToTileSearchWindowOrigin(piece.X, piece.Y);

            // load screenshot
            // we handle to cases (one with more performance for the physical engine, the other for the emulator)
            Mat originalMat = screenshot.Image as Mat;
            Image<Gray, byte> originalImage = null;
            if (originalMat == null)
            {
                originalImage = screenshot.Image as Image<Gray, byte>;
                if (originalImage == null)
                {
                    originalImage = new Image<Gray, byte>(screenshot.Image.Bitmap);
                }
            }

            // get template and it's mask
            var templateIndex = GetTemplateIndex(piece);
            var template = _templates[templateIndex];
            var templateMask = _templateMasks[templateIndex];

            double bestProbability = 0;

            foreach (var shift in GetShifts())
            {
                // TODO: use CvInvoke instead of generic Image objects

                // create combined and reference image (that we can compare them later)
                var combined = new Image<Gray, byte>(GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight + yTopPadding + yBottomPadding);
                var mainRoi = new Rectangle(0, yTopPadding, GameBoyConstants.ScreenWidth, GameBoyConstants.ScreenHeight);
                combined.ROI = mainRoi;
                if (originalMat != null)
                {
                    originalMat.CopyTo(combined);
                }
                else
                {
                    originalImage.CopyTo(combined);
                }
                combined.ROI = Rectangle.Empty;
                var reference = combined.Clone();
                combined.ROI = mainRoi;

                // combine image with template
                var roiX = tileCoordinates.X * GameBoyConstants.TileSize + shift.X;
                var roiY = tileCoordinates.Y * GameBoyConstants.TileSize + yTopPadding + shift.Y;
                combined.ROI = new Rectangle(roiX, roiY, _templateSize, _templateSize);
                template.Copy(combined, templateMask);
                reference.ROI = combined.ROI;

                // calculate error
                var difference = new Mat();
                CvInvoke.AbsDiff(reference, combined, difference);
                var sum = CvInvoke.Sum(difference);

                // calculate probability
                var newProbability = GetProbability(sum.V0, piece.Tetrimino);
                bestProbability = Math.Max(bestProbability, newProbability);

                //CvInvoke.Imshow("Combined", combined);
                //CvInvoke.Imshow("Ori", original);
                //CvInvoke.Imshow("diff", difference);
                //CvInvoke.WaitKey();
            }

            return bestProbability;
        }

        private int GetTemplateIndex(Piece piece)
        {
            return _templateIndexTable[(int)piece.Tetrimino, piece.Orientation];
        }
        
        private IEnumerable<Point> GetShifts()
        {
            for (int x = -_maxShiftDistance; x <= _maxShiftDistance; x++)
            {
                for (int y = -_maxShiftDistance; y <= _maxShiftDistance; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        private double GetError(double sum, Tetrimino tetromino)
        {
            double maxError = _maxErrors[(int) tetromino];
            double error = sum / maxError;
            return error.Clamp(0.0, 1.0);
        }

        private double GetProbability(double sum, Tetrimino tetromino)
        {
            return 1 - GetError(sum, tetromino);
        }
    }
}
