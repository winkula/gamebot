using System;

namespace GameBot.Core.Exceptions
{
    public class GameOverException : Exception
    {        
        public GameOverException()
        {
        }

        public GameOverException(string message) : base(message)
        {
        }
    }
}
