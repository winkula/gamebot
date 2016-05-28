using NUnit.Framework;
using GameBot.Core;
using GameBot.Game.Tetris;
using GameBot.Robot;
using GameBot.Robot.Engines;

namespace GameBot.Test
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void DependencyInjection()
        {
            var container = Bootstrapper.GetInitializedContainer(EngineMode.Emulator);
            
            container.Verify();

            Assert.NotNull(container.GetInstance<ICamera>());
            Assert.NotNull(container.GetInstance<IQuantizer>());
            Assert.NotNull(container.GetInstance<IExecutor>());

            Assert.NotNull(container.GetInstance<IExtractor<TetrisGameState>>());
            Assert.NotNull(container.GetInstance<IPlayer<TetrisGameState>>());

            Assert.NotNull(container.GetInstance<IAgent>());
        }        
    }
}
