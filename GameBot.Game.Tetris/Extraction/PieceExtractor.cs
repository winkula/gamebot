namespace GameBot.Game.Tetris.Extraction
{
    public class PieceExtractor
    {
        public PieceExtractor()
        {

        }

        /// <summary>
        /// Extracts a piece from 4 times 4 square of blocks.
        /// The filled blocks in the mask represents ones, the empty zeroes.
        /// The index of the bitmask is build by the formula 4 * (3 - y) + (x).
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public PieceExtractionResult Extract(ushort mask)
        {
            return null;
        }
    }
}
