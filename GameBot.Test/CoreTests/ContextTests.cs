using GameBot.Core;
using GameBot.Core.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace GameBot.Test
{
    [TestFixture]
    public class ContextTests
    {
        class GameStateMock : IGameState
        {
            public TimeSpan Timestamp { get; }
        }

        [Test]
        public void Constructor()
        {
            var context = new Context<GameStateMock>();
            
            Assert.NotNull(context.GameStates);
        }

        [Test]
        public void Add()
        {
            var context = new Context<GameStateMock>();

            Assert.IsEmpty(context.GameStates);

            context.Add(new GameStateMock());

            Assert.IsNotEmpty(context.GameStates);
            Assert.AreEqual(1, context.GameStates.Count);
        }
    }
}
