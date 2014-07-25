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
        private const int SOME_WIDTH = 16;
        private const int SOME_HEIGHT = 16;
        private int mSeed;
        private readonly Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration = 
            new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH, ChanceToRemoveDeadends = 1, Sparseness = 2, Randomness = 1};

        [SetUp]
        public void SetUp()
        {
            mSeed = Guid.NewGuid().GetHashCode();
            mRandomizer.SetSeed(mSeed);    
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("Seed = {0}", mSeed);            
        }

        [Test]
        public void ProcessMap_RemoveAllDeadEnds_AllDeadEndsRemoved()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator();
            mazeGenerator.ProcessMap(map, mConfiguration, mRandomizer);
            var sparsenessReducer = new SparsenessReducer();
            sparsenessReducer.ProcessMap(map, mConfiguration, mRandomizer);

            var remover = new DeadendsRemover();
            remover.ProcessMap(map, mConfiguration, mRandomizer);

            Assert.AreEqual(0, map.AllCells.Count(cell => cell.Sides.Values.Count(type => type == SideType.Open) == 1));
        }
    }
}
