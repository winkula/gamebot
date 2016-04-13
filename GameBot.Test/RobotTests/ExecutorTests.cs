using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Data.Commands;
using GameBot.Robot;
using GameBot.Robot.Executors;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;

namespace GameBot.Test.RobotTests
{
    [TestFixture]
    public class ExecutorTests
    {
        [Test]
        public void Constructor()
        {
            var repositoryActor = new Mock<IActor>();

            var executor = new Executor(repositoryActor.Object, new TimeProvider());
        }

        [Test]
        public void ExecuteSingleHit()
        {
            int b = 0;

            var repositoryActor = new Mock<IActor>();
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.A))).Callback(() => Assert.Fail());
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.B))).Callback(() => b++);

            var timeProvider = new TimeProvider();
            var executor = new Executor(repositoryActor.Object, timeProvider);

            var command = new HitCommand(Button.B);

            timeProvider.Start();
            executor.Execute(command);

            Thread.Sleep(100);
            
            Assert.AreEqual(1, b);
        }

        [Test]
        public void ExecuteSinglePress()
        {
            int b = 0;

            var repositoryActor = new Mock<IActor>();
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.A))).Callback(() => Assert.Fail());
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.B))).Callback(() => b++);

            var timeProvider = new TimeProvider();
            var executor = new Executor(repositoryActor.Object, timeProvider);

            var command = new HitCommand(Button.B);
            timeProvider.Start();
            executor.Execute(command);

            Thread.Sleep(100);

            Assert.AreEqual(1, b);
        }

        [Test]
        public void ExecuteManyHits()
        {
            int a = 0;

            var repositoryActor = new Mock<IActor>();
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.A))).Callback(() => a++);
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.B))).Callback(() => Assert.Fail());

            var timeProvider = new TimeProvider();
            var executor = new Executor(repositoryActor.Object, timeProvider);

            var commands = new CommandCollection();
            commands.Add(new HitCommand(Button.A));
            commands.Add(new HitCommand(Button.A));
            commands.Add(new HitCommand(Button.A));

            timeProvider.Start();
            executor.Execute(commands);

            Thread.Sleep(100);

            Assert.AreEqual(3, a);
        }

        [Test]
        public void ExecuteTimed()
        {
            int a = 0;
            int b = 0;

            var repositoryActor = new Mock<IActor>();
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.A))).Callback(() => a++);
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.B))).Callback(() => b++);

            var timeProvider = new TimeProvider();
            var executor = new Executor(repositoryActor.Object, timeProvider);

            var commands = new CommandCollection();
            commands.Add(new HitCommand(Button.A, TimeSpan.FromSeconds(0.5)));
            commands.Add(new HitCommand(Button.B, TimeSpan.FromSeconds(1)));
            
            timeProvider.Start();
            executor.Execute(commands);

            Assert.AreEqual(0, a);
            Assert.AreEqual(0, b);

            Thread.Sleep(250);

            Assert.AreEqual(0, a);
            Assert.AreEqual(0, b);

            Thread.Sleep(500);

            Assert.AreEqual(0, b);

            Thread.Sleep(500);

            Assert.AreEqual(1, a);
            Assert.AreEqual(1, b);
        }
    }
}
