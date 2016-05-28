using Emgu.CV;
using Emgu.CV.Structure;
using GameBot.Core;
using GameBot.Core.Agents;
using System.Drawing;
using GameBot.Core.Data;
using System.Collections.Generic;
using System;

namespace GameBot.Game.Tetris
{
    public class TetrisAgent : AbstractAgent<TetrisGameState>
    {
        private readonly Random random = new Random();

        public TetrisAgent(IExtractor<TetrisGameState> extractor, IPlayer<TetrisGameState> player) : base(extractor, player)
        { 
        }
        
        public override IImage Visualize(IImage image)
        {
            var visualization = new Image<Bgr, byte>(image.Bitmap);
            var tetrisExtractor = Extractor as TetrisExtractor;
            if (tetrisExtractor != null)
            {
                foreach (var rectangle in tetrisExtractor.Rectangles)
                {
                    visualization.Draw(new Rectangle(8 * rectangle.X, 8 * rectangle.Y, 8, 8), new Bgr(0, 0, 255), 1);
                }
                return visualization;
            }
            return image;
        }
    }
}
