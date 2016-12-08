using System;
using System.Diagnostics;
using System.Threading;
using GameBot.Core;
using GameBot.Core.Data;
using GameBot.Core.Executors;
using Moq;
using NUnit.Framework;

namespace GameBot.Test.Core.Executors
{
    [TestFixture]
    public class ExecutorTests
    {
        [Test]
        public void Hit()
        {
            Button button = Button.A;
            
            var actuatorMock = new Mock<IActuator>();
            var clockMock = new Mock<IClock>();

            var executor = new Executor(actuatorMock.Object, clockMock.Object);
            
            executor.Hit(button);
            
            actuatorMock.Verify(x => x.Hit(button), Times.Once);
            actuatorMock.Verify(x => x.Press(button), Times.Never);
            actuatorMock.Verify(x => x.Release(button), Times.Never);
        }
        
        [Test]
        public void Press()
        {
            Button button = Button.A;

            var actuatorMock = new Mock<IActuator>();
            var clockMock = new Mock<IClock>();

            var executor = new Executor(actuatorMock.Object, clockMock.Object);

            executor.Press(button);

            actuatorMock.Verify(x => x.Hit(button), Times.Never);
            actuatorMock.Verify(x => x.Press(button), Times.Once);
            actuatorMock.Verify(x => x.Release(button), Times.Never);
        }

        [Test]
        public void Release()
        {
            Button button = Button.A;

            var actuatorMock = new Mock<IActuator>();
            var clockMock = new Mock<IClock>();

            var executor = new Executor(actuatorMock.Object, clockMock.Object);

            executor.Release(button);

            actuatorMock.Verify(x => x.Hit(button), Times.Never);
            actuatorMock.Verify(x => x.Press(button), Times.Never);
            actuatorMock.Verify(x => x.Release(button), Times.Once);
        }

        [Test]
        public void HitAsyncDuration()
        {
            Button button = Button.A;
            double miliseconds = 1000;

            var actuatorMock = new Mock<IActuator>();
            var clockMock = new Mock<IClock>();
            clockMock.Setup(x => x.Sleep(It.IsAny<int>())).Callback<int>(Thread.Sleep);
            clockMock.Setup(x => x.Sleep(It.IsAny<TimeSpan>())).Callback<TimeSpan>(Thread.Sleep);

            var executor = new Executor(actuatorMock.Object, clockMock.Object);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            executor.HoldAsync(button, TimeSpan.FromMilliseconds(miliseconds));

            stopwatch.Stop();

            Assert.Less(stopwatch.ElapsedMilliseconds, miliseconds);
            actuatorMock.Verify(x => x.Press(button), Times.Once);
            actuatorMock.Verify(x => x.Release(button), Times.Never);

            Thread.Sleep((int) miliseconds);

            actuatorMock.Verify(x => x.Release(button), Times.Once);
        }
    }
}
