using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Implementations;
using Karcero.Engine.Models;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Implementations.Randomizer;

namespace Karcero.Tests
{
    [TestFixture]
    public class MazeGeneratorTests
    {
        private const int SOME_WIDTH = 5;
        private const int SOME_HEIGHT = 5;
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
        public void ProcessMap_ValidInput_AllCellsHasFloorTileType()
        {
            var map = GenerateMap();

            foreach (var cell in map.AllCells)
            {
                Assert.IsTrue(cell.IsOpen);
            }
        }

        [Test]
        public void ProcessMap_ValidInput_AllCellsAreReachable()
        {
            var map = GenerateMap();

            var visitedCells = new HashSet<BinaryCell>();
            var discoveredCells = new HashSet<BinaryCell>() { map.GetCell(0, 0) };
            while (discoveredCells.Any())
            {
                foreach (var discoveredCell in discoveredCells)
                {
                    visitedCells.Add(discoveredCell);
                }
                var newDiscoveredCells = new HashSet<BinaryCell>();
                foreach (var newDiscoveredCell in discoveredCells.SelectMany(cell => cell.Sides
                    .Where(pair => pair.Value && !visitedCells.Contains(map.GetAdjacentCell(cell, pair.Key)))
                    .Select(pair => map.GetAdjacentCell(cell, pair.Key))))
                {
                    newDiscoveredCells.Add(newDiscoveredCell);
                }
                discoveredCells = newDiscoveredCells;
            }
            Assert.AreEqual(map.AllCells.Count(), visitedCells.Count);
        }

        [Test]
        public void ProcessMap_ValidInput_SideTypesInAdjacentCellsMatch()
        {
            var map = GenerateMap();

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

        private static Map<BinaryCell> GenerateMap()
        {
            var map = new Map<BinaryCell>(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator<BinaryCell>();
            mazeGenerator.ProcessMap(map, new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH },
                new Randomizer());
            return map;
        }
    }
}
