using GameBot.Core.Data;
using System;
using System.Collections.Generic;

namespace GameBot.Core.Searching
{
    public abstract class Node<T> : INode
    {
        public T GameState { get; private set; }
        public INode Parent { get; private set; }
        public int Depth { get; private set; }

        public Node(T gameState, INode parent)
        {
            if (gameState == null) throw new ArgumentNullException("state is null");

            GameState = gameState;

            if (parent != null)
            {
                Parent = parent;
                Depth = parent.Depth + 1;
            }
        }
        
        public Node(T gameState) : this(gameState, null)
        {            
        }
        
        public bool HasImmediateLoop()
        {
            INode parent = Parent;
            if (parent != null)
            {
                INode grandParent = parent.Parent;
                if (grandParent != null)
                {
                    // loop found
                    if (grandParent.Equals(this)) return true;
                }
            }
            return false;
        }
        
        public bool HasLoop()
        {
            INode parent = Parent;
            while (parent != null)
            {
                if (parent.Equals(this)) return true; // loop found
                parent = parent.Parent;
            }
            return false;
        }
        
        public abstract double GetScore();

        public abstract IEnumerable<INode> GetSuccessors();

        public override string ToString()
        {
            return string.Format("{0} (depth: {1})", GameState, Depth);
        }
    }
}
