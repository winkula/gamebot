using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Robot.Executors;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameBot.Test.RobotTests
{
    [TestFixture]
    public class ExecutorTests
    {
        [Test]
        public void Constructor()
        {
            var repositoryActor = new Mock<IActor>();

            var executor = new Executor(repositoryActor.Object);
        }

        [Test]
        public void ExecuteSingleHit()
        {
            int b = 0;

            var repositoryActor = new Mock<IActor>();
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.A))).Callback(() => Assert.Fail());
            repositoryActor.Setup(x => x.Hit(It.Is<Button>(p => p == Button.B))).Callback(() => b++);

            var executor = new Executor(repositoryActor.Object);

            var command = new HitCommand(Button.B);
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

            var executor = new Executor(repositoryActor.Object);

            var command = new HitCommand(Button.B);
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

            var executor = new Executor(repositoryActor.Object);

            var commands = new Commands();
            commands.Add(new HitCommand(Button.A));
            commands.Add(new HitCommand(Button.A));
            commands.Add(new HitCommand(Button.A));

            executor.Execute(commands);

            Thread.Sleep(100);

            Assert.AreEqual(3, a);
        }
    }
}
