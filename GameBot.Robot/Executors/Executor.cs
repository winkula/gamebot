using GameBot.Core;
using System;
using GameBot.Core.Data;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GameBot.Robot.Executors
{
    public class Executor : IExecutor
    {
        private readonly IActor actor;
        private readonly ConcurrentQueue<ICommand> queue = new ConcurrentQueue<ICommand>();
        private Queue<ICommand> queueInternal = new Queue<ICommand>();
        private Queue<ICommand> queueInternalSwap = new Queue<ICommand>();
        private Task worker;
        public ITimeProvider TimeProvider { private get; set; }

        public Executor(IActor actor)
        {
            this.actor = actor;
        }

        public void Execute(ICommands commands)
        {
            foreach (var command in commands)
            {
                queue.Enqueue(command);
            }
            AwakeWorker();
        }

        public void Execute(ICommand command)
        {
            queue.Enqueue(command);
            AwakeWorker();
        }

        private void WorkerCode()
        {
            CopyCommands();
            ExecuteCommands();
        }

        private void CopyCommands()
        {
            // copy commands from concurrent queue to internal queue
            while (!queue.IsEmpty)
            {
                ICommand command;
                if (queue.TryDequeue(out command))
                {
                    queueInternal.Enqueue(command);
                }
            }
        }

        private void ExecuteCommands()
        {
            var time = TimeProvider.Time;
            foreach (var command in queueInternal)
            {
                HandleCommand(command, time);
            }
            queueInternal.Clear();

            // swap queues
            var temp = queueInternal;
            queueInternal = queueInternalSwap;
            queueInternalSwap = temp;
        }

        private void HandleCommand(ICommand command, TimeSpan time)
        {
            if (command.Press.HasValue && command.Release.HasValue)
            {
                if (command.Press.Value <= time)
                {
                    actor.Hit(command.Button);
                }
                else
                {
                    queueInternalSwap.Enqueue(command);
                }
            }
            else
            {
                if (command.Press.HasValue)
                {
                    if (command.Press.Value <= time)
                    {
                        actor.Press(command.Button);
                    }
                    else
                    {
                        queueInternalSwap.Enqueue(command);
                    }
                }
                else if (command.Release.HasValue)
                {
                    if (command.Release.Value <= time)
                    {
                        actor.Release(command.Button);
                    }
                    else
                    {
                        queueInternalSwap.Enqueue(command);
                    }
                }
            }            
        }

        private void AwakeWorker()
        {
            if (worker == null || worker.Status == TaskStatus.RanToCompletion)
            {
                worker = Task.Run(() => WorkerCode());
            }
        }
    }
}
