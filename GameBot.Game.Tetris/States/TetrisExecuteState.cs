using GameBot.Core.Data;
using GameBot.Game.Tetris.Agents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBot.Game.Tetris.States
{
    public class TetrisExecuteState : ITetrisState
    {
        private TetrisAgent agent;

        private Queue<ICommand> commands;
        private TetrisGameState currentGameState;
        
        private TimeSpan timeNextAction = TimeSpan.Zero;
        private ICommand lastCommand;
        private IScreenshot lastScreenshot;

        public TetrisExecuteState(Queue<ICommand> commands, TetrisGameState currentGameState)
        {
            this.commands = commands;
            this.currentGameState = currentGameState;
        }

        public void Act(TetrisAgent agent)
        {
            this.agent = agent;

            if (commands.Any())
            {
                // there are commands to execute

                if (lastCommand != null)
                {
                    //tetrisExtractor.FindPiece(screenshot, );

                    // check if last command was executed
                    // if not, repeat    

                    // TODO: implement                            
                }

                var command = commands.Peek();
                if (command != null && command.Timestamp <= agent.TimeProvider.Time)
                {
                    command.Execute(agent.Actuator);
                    lastCommand = command;
                    lastScreenshot = agent.Screenshot;
                    commands.Dequeue();
                }
            }
        }
    }
}
