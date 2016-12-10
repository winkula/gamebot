using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.Data
{
    public class TetriminoLookups
    {
        private static TetriminoLookups _instance;
        public static TetriminoLookups Instance => _instance ?? (_instance = new TetriminoLookups());

        private readonly int[] _allTranslations = { 0, 1, -1, 2, -2, 3, -3, 4, -4, 5 };
        private readonly IEnumerable<Pose>[] _allPoses;

        private TetriminoLookups()
        {
            _allPoses = new IEnumerable<Pose>[Tetriminos.All.Length];

            Init();
        }

        private void Init()
        {
            foreach (var tetrimino in Tetriminos.All)
            {
                var poses = new List<Pose>();

                foreach (var translation in _allTranslations)
                {
                    foreach (var orientation in tetrimino.GetPossibleOrientations())
                    {
                        var possibleTranslations = tetrimino.GetPossibleTranslations(orientation);
                        if (possibleTranslations.Contains(translation))
                        {
                            poses.Add(new Pose { Orientation = orientation, Translation = translation });
                        }
                    }
                }

                _allPoses[(int)tetrimino] = poses;
            }
        }

        public IEnumerable<Pose> GetPoses(Tetrimino tetrimino)
        {
            return _allPoses[(int)tetrimino];
        }
    }
}
