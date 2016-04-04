using GameBot.Game.Tetris.Data;
using System.Collections.Generic;

namespace GameBot.Game.Tetris
{
    public class TetrisNode
    {
        public TetrisNode Parent { get; set; }
        public TetrisGameState GameState { get; set; }
        public Move Move { get; set; }
        public double Score { get; set; }

        public TetrisNode(TetrisGameState gameState, TetrisNode parent)
        {
            GameState = gameState;
            Parent = parent;
        }

        public TetrisNode(TetrisGameState gameState) : this(gameState, null)
        {
        }

        public IEnumerable<TetrisNode> GetSuccessors()
        {
            if (GameState.Piece != null)
            {
                foreach (var setting in Move.GetAllSettings(GameState.Piece.Tetromino))
                {
                    var orientation = setting.Rotation;
                    var translation = setting.Translation;

                    var newPiece = new Piece(GameState.Piece.Tetromino, orientation, translation);
                    if (GameState.Board.CanDrop(newPiece))
                    {
                        var successor = new TetrisGameState(GameState, newPiece);
                        var fall = successor.Drop();

                        var node = new TetrisNode(successor, this);
                        node.Move = new Move(orientation, translation, fall);

                        yield return node;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Node {{ State: \n{0} }}", GameState);
        }
    }
}
