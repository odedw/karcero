using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Models;
using NUnit.Framework;
using Randomizer = DunGen.Engine.Implementations.Randomizer;

namespace DunGen.Tests
{
    [TestFixture]
    public class RoomGeneratorTests
    {
        private const int SOME_WIDTH = 30;
        private const int SOME_HEIGHT = 30;
        private int mSeed;
        private readonly Engine.Implementations.Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration =
            new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH, ChanceToRemoveDeadends = 1, Sparseness = 2, Randomness = 1 };

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

    }
}
