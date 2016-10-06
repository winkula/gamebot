using GameBot.Game.Tetris.Data;
using NUnit.Framework;
using System.Linq;

namespace GameBot.Test.Tetris.Data
{
    [TestFixture]
    public class PoseTests
    {
        [Test]
        public void StaticAccess()
        {
            var poses = Pose.All.ToList();

            Assert.AreEqual(19, poses.Count());

            Assert.AreEqual(1, poses.Where(x => x.Tetromino == Tetromino.O).Count());
            Assert.AreEqual(2, poses.Where(x => x.Tetromino == Tetromino.I).Count());
            Assert.AreEqual(2, poses.Where(x => x.Tetromino == Tetromino.S).Count());
            Assert.AreEqual(2, poses.Where(x => x.Tetromino == Tetromino.Z).Count());
            Assert.AreEqual(4, poses.Where(x => x.Tetromino == Tetromino.L).Count());
            Assert.AreEqual(4, poses.Where(x => x.Tetromino == Tetromino.J).Count());
            Assert.AreEqual(4, poses.Where(x => x.Tetromino == Tetromino.T).Count());
        } 
    }
}
