using GameBot.Core;
using System;
using GameBot.Core.Data;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GameBot.Robot.Executors
{
    public class Executor : IExecutor
    {
        private readonly IActor actor;
        private readonly ITimeProvider timeProvider;
        private readonly ConcurrentQueue<ICommand> queue = new ConcurrentQueue<ICommand>();
        private Queue<ICommand> queueInternal = new Queue<ICommand>();
        private Queue<ICommand> queueInternalSwap = new Queue<ICommand>();
        private Task worker;

        public Executor(IActor actor, ITimeProvider timeProvider)
        {
            this.actor = actor;
            this.timeProvider = timeProvider;
        }

        public void Execute(IEnumerable<ICommand> commands)
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
            while (queueInternal.Count > 0 || !queue.IsEmpty)
            {
                CopyCommands();
                ExecuteCommands();

                Thread.Sleep(20);
            }
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
            var time = timeProvider.Time;
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
                    Debug.Write("Hit " + command.Button + "\n");
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
                        Debug.Write("Press " + command.Button + "\n");
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
                        Debug.Write("Release " + command.Button + "\n");
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
