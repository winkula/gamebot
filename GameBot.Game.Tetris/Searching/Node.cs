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

        public Node(GameState gameState, Node parent = null)
        {
            GameState = gameState;
            Parent = parent;
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

        public override int GetHashCode()
        {
            int hashCode = GameState.Board.GetHashCode();
            hashCode ^= (GameState.Lines << 29);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            Node other = obj as Node;
            if (other != null)
            {
                return GameState.Board.Equals(other.GameState.Board) 
                    && GameState.Lines == other.GameState.Lines;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Node {{ State: \n{GameState} }}";
        }
    }
}
