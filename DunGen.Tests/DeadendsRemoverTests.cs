using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Implementations;
using DunGen.Engine.Models;
using NUnit.Framework;
using Randomizer = DunGen.Engine.Implementations.Randomizer;

namespace DunGen.Tests
{
    [TestFixture]
    public class DeadendsRemoverTests
    {
        private const int SOME_WIDTH = 30;
        private const int SOME_HEIGHT = 30;
        private readonly Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration = 
            new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH, ChanceToRemoveDeadends = 1};

        [Test]
        public void ProcessMap_RemoveAllDeadEnds_AllDeadEndsRemoved()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator(mRandomizer);
            mazeGenerator.ProcessMap(map, mConfiguration);
            var remover = new DeadendsRemover(mRandomizer);
            remover.ProcessMap(map, mConfiguration);

            Assert.AreEqual(0, map.AllCells.Count(cell => cell.Sides.Values.Count(type => type == SideType.Open) == 1));
        }
    }
}
