﻿using GameBot.Game.Tetris.Data;
using System.Collections.Generic;

namespace GameBot.Game.Tetris.Searching
{
    public class Node
    {
        public Node Parent { get; set; }
        public TetrisGameState GameState { get; set; }
        public Way Moves { get; set; }
        public double Score { get; set; }

        public Node(TetrisGameState gameState, Node parent)
        {
            GameState = gameState;
            Parent = parent;
        }

        public Node(TetrisGameState gameState) : this(gameState, null)
        {
        }

        public IEnumerable<Node> GetSuccessors()
        {
            if (GameState.Piece != null)
            {
                foreach (var setting in Way.GetAllSettings(GameState.Piece.Tetromino))
                {
                    var orientation = setting.Rotation;
                    var translation = setting.Translation;

                    var newPiece = new Piece(GameState.Piece.Tetromino, orientation, translation);
                    if (GameState.Board.CanDrop(newPiece))
                    {
                        var successor = new TetrisGameState(GameState, newPiece);
                        var fall = successor.Drop();

                        var node = new Node(successor, this);
                        node.Moves = new Way(orientation, translation, fall);

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