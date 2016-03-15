﻿using NUnit.Framework;
using GameBot.Core;
using GameBot.Game.Tetris;
using GameBot.Robot;
using GameBot.Core.Data;

namespace GameBot.Test
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void DependencyInjection()
        {
            var container = Bootstrapper.GetInitializedContainer();
            
            container.Verify();

            Assert.NotNull(container.GetInstance<ICamera>());
            Assert.NotNull(container.GetInstance<IQuantizer>());
            Assert.NotNull(container.GetInstance<IExecutor>());

            Assert.NotNull(container.GetInstance<IExtractor<TetrisGameStateFull>>());
            Assert.NotNull(container.GetInstance<IDecider<TetrisGameStateFull>>());

            Assert.NotNull(container.GetInstance<IAgent>());
            Assert.NotNull(container.GetInstance<IGameState>());
        }        
    }
}
