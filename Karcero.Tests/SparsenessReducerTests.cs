using System;
using System.Linq;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Helpers.Randomizer;

namespace Karcero.Tests
{
    [TestFixture]
    public class SparsenessReducerTests
    {
        private const int SOME_WIDTH = 30;
        private const int SOME_HEIGHT = 30;
        private readonly Randomizer mRandomizer = new Randomizer();
        private int mSeed;

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
        public void ProcessMap_ValidInput_OnlyDeadEndCellsRemoved()
        {
            var map = new Map<BinaryCell>(2, 2);
            foreach (var cell in map.AllCells)
            {
                cell.IsOpen = true;
            }
            map.GetCell(0, 0).Sides[Direction.South] = map.GetCell(0, 1).Sides[Direction.South] = true;
            map.GetCell(1, 0).Sides[Direction.North] = map.GetCell(1, 1).Sides[Direction.North] = true;
            map.GetCell(1, 0).Sides[Direction.East] = map.GetCell(1, 1).Sides[Direction.West] = true;

            var sparsenessReducer = new SparsenessReducer<BinaryCell>();
            sparsenessReducer.ProcessMap(map, new DungeonConfiguration(){Sparseness = 0.5, Height = 2, Width = 2}, mRandomizer);

            //Assert tile types
            Assert.IsFalse(map.GetCell(0,0).IsOpen);
            Assert.IsFalse(map.GetCell(0, 1).IsOpen);
            Assert.IsTrue(map.GetCell(1, 0).IsOpen);
            Assert.IsTrue(map.GetCell(1, 1).IsOpen);

            //Assert walls
            Assert.IsFalse(map.GetCell(0, 0).Sides[Direction.South]);
            Assert.IsFalse(map.GetCell(0, 1).Sides[Direction.South]);
            Assert.IsFalse(map.GetCell(1, 0).Sides[Direction.North]);
            Assert.IsFalse(map.GetCell(1, 1).Sides[Direction.North]);
        }

        [Test]
        public void ProcessMap_ValidInput_SideTypesInAdjacentCellsMatch()
        {
            var map = new Map<BinaryCell>(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator<BinaryCell>();
            mazeGenerator.ProcessMap(map, new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH }, mRandomizer);
            var sparseness = new SparsenessReducer<BinaryCell>();
            sparseness.ProcessMap(map, new DungeonConfiguration() { Sparseness = 0.6 }, mRandomizer);

            for (int j = 0; j < SOME_HEIGHT; j++)
            {
                for (var i = 0; i < SOME_WIDTH; i++)
                {
                    var currentCell = map.GetCell(i, j);
                    var adjacentCellsByDirection = currentCell.Sides.Keys.ToDictionary(key => key, key => map.GetAdjacentCell(currentCell, key));
                    foreach (var kvp in adjacentCellsByDirection.Where(kvp => kvp.Value != null))
                    {
                        Assert.AreEqual(currentCell.Sides[kvp.Key], kvp.Value.Sides[kvp.Key.Opposite()]);
                    }
                }
            }
        }
    }
}
