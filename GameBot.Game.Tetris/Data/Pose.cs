using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Data
{
    // TODO: every Piece is a Pose. Make inheritance!
    public class Pose
    {
        public Tetromino Tetromino { get; private set; }
        public Shape Shape { get; private set; }

        /// <summary>
        /// Orientation is counted in clockwise direction from the start position of a Tetromino.
        /// </summary>
        public int Orientation { get; private set; }

        private Pose(Tetromino tetromino, int orientation)
        {
            Tetromino = tetromino;
            Shape = Shape.Get(tetromino, orientation);
            Orientation = orientation;
        }

        private static IEnumerable<Pose> _all;
        public static IEnumerable<Pose> All
        {
            get
            {
                if (_all == null)
                {
                    _all = Generate();
                }
                return _all;
            }
        }

        private static IEnumerable<Pose> Generate()
        {
            var tetrominos = Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
            foreach (var tetromino in tetrominos)
            {
                foreach (var orientation in tetromino.GetPossibleOrientations())
                {
                    yield return new Pose(tetromino, orientation);
                }
            }
        }
    }
}
