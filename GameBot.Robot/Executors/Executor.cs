using GameBot.Core;
using System;
using GameBot.Core.Data;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GameBot.Core.Ui;

namespace GameBot.Robot.Executors
{
    public class Executor : IExecutor
    {
        private readonly IActuator actuator;
        private readonly ITimeProvider timeProvider;
        private readonly IDebugger debugger;

        private readonly ConcurrentQueue<ICommand> queue = new ConcurrentQueue<ICommand>();
        
        private Queue<ICommand> queueInternal = new Queue<ICommand>();
        private Queue<ICommand> queueInternalSwap = new Queue<ICommand>();
        private Task worker;

        public Executor(IActuator actuator, ITimeProvider timeProvider, IDebugger debugger)
        {
            this.actuator = actuator;
            this.timeProvider = timeProvider;
            this.debugger = debugger;
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

                Thread.Sleep(10);
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
            if (command.Timestamp <= time)
            {
                if (debugger != null)
                {
                    debugger.WriteDynamic(command);
                }
                command.Execute(actuator);
            }
            else
            {
                queueInternalSwap.Enqueue(command);
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
