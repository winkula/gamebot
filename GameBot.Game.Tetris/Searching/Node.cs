using GameBot.Game.Tetris.Data;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Searching
{
    public class Node
    {
        public Node Parent { get; }
        public GameState GameState { get; }
        public Way Way { get; private set; }
        public double Score { get; set; }

        public Node(GameState gameState, Node parent)
        {
            GameState = gameState;
            Parent = parent;
        }

        public Node(GameState gameState) : this(gameState, null)
        {
        }

        public IEnumerable<Node> GetSuccessors()
        {
            if (GameState.Piece != null)
            {
                foreach (var pose in GameState.Piece.Tetromino.GetPoses())
                {
                    var newPiece = pose;
                    if (GameState.Board.CanDrop(newPiece))
                    {
                        var successor = new GameState(GameState, newPiece);
                        var fall = successor.Drop();

                        yield return new Node(successor, this) { Way = new Way(pose.Orientation, pose.X, fall) };
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"Node {{ State: \n{GameState} }}";
        }
    }
}
