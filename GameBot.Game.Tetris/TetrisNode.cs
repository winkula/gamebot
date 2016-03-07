using GameBot.Core.Searching;
using GameBot.Game.Tetris.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameBot.Game.Tetris
{
    public class TetrisNode : Node
    {
        private readonly IHeuristic<TetrisGameState> heuristic = new TetrisHeuristic(); // TODO: inject?
        private readonly TetrisGameState gameState;

        public TetrisNode(TetrisGameState gameState, INode parent) : base(gameState, parent)
        {
            this.gameState = gameState;
        }

        public TetrisNode(TetrisGameState gameState) : base(gameState)
        {
            this.gameState = gameState;
        }

        public override double GetScore()
        {
            return heuristic.Score(gameState);
        }

        public override IEnumerable<INode> GetSuccessors()
        {
            var successors = new List<INode>();

            if (gameState.Piece != null)
            {
                // TODO: use constants
                for (int translation = -4; translation < 6; translation++)
                {
                    for (int orientation = 0; orientation < 4; orientation++)
                    {
                        var newPiece = new Piece(gameState.Piece.Tetromino, orientation, translation);
                        if (!gameState.Board.Intersects(newPiece))
                        {
                            var successor = new TetrisGameState(gameState, newPiece);                            
                            successor.Drop();

                            successors.Add(new TetrisNode(successor, this));
                        }
                    }
                }
            }

            return successors;
        }

        public override string ToString()
        {
            return gameState.Board.ToString();
        }
    }
}
