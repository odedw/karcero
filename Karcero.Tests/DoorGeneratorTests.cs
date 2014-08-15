using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Helpers.Randomizer;

namespace Karcero.Tests
{
    [TestFixture]
    public class DoorGeneratorTests
    {
        private const int SOME_WIDTH = 16;
        private const int SOME_HEIGHT = 16;
        private int mSeed;
        private readonly Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration =
            new DungeonConfiguration()
            {
                Height = SOME_HEIGHT,
                Width = SOME_WIDTH,
                ChanceToRemoveDeadends = 0.6,
                Sparseness = 0.5,
                Randomness = 0.6,
                MinRoomHeight = 3,
                MaxRoomHeight = 6,
                MinRoomWidth = 3,
                MaxRoomWidth = 6,
                RoomCount = 10
            };

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
        public void ProcessMap_ValidInput_AllCellsAreReachable()
        {
            var map = GenerateMap();

            var visitedCells = new HashSet<Cell>();
            var discoveredCells = new HashSet<Cell>() { map.AllCells.FirstOrDefault(cell => cell.Terrain == TerrainType.Floor) };
            while (discoveredCells.Any())
            {
                foreach (var discoveredCell in discoveredCells)
                {
                    visitedCells.Add(discoveredCell);
                }
                var newDiscoveredCells = new HashSet<Cell>();
                foreach (var newDiscoveredCell in discoveredCells.SelectMany(cell => map.GetAllAdjacentCells(cell)
                    .Where(c => c.Terrain != TerrainType.Rock && !visitedCells.Contains(c))))
                {
                    newDiscoveredCells.Add(newDiscoveredCell);
                }
                discoveredCells = newDiscoveredCells;
            }
            var unReachable = map.AllCells.Where(cell => cell.Terrain != TerrainType.Rock).Except(visitedCells).ToList();
            Assert.AreEqual(map.AllCells.Count(cell => cell.Terrain != TerrainType.Rock), visitedCells.Count);
        }

        [Test]
        public void ProcessMap_ValidInput_NoTwoDoorsAreAdjacent()
        {
            var map = GenerateMap();

            foreach (var cell in map.AllCells.Where(cell => cell.Terrain == TerrainType.Door).SelectMany(cell => map.GetAllAdjacentCells(cell)))
            {
                Assert.AreNotEqual(TerrainType.Door, cell.Terrain);       
            }
        }
      
        [Test]
        public void ProcessMap_ValidInput_AllDoorsLeadSomewhere()
        {
            var map = GenerateMap();

            foreach (var cell in map.AllCells.Where(cell => cell.Terrain == TerrainType.Door))
            {
                var dict = map.GetAllAdjacentCellsByDirection(cell);
                foreach (var kvp in dict)
                {
                    Assert.IsTrue((kvp.Value == null && 
                        (dict[kvp.Key.Opposite()] == null || dict[kvp.Key.Opposite()].Terrain == TerrainType.Rock) ||
                        (kvp.Value.Terrain == dict[kvp.Key.Opposite()].Terrain)));
                    
                }
                foreach (var direction in GetAll.ValuesOf<Direction>())
                {
                    Cell adjacentCell;
                    if (map.TryGetAdjacentCell(cell, direction, out adjacentCell))
                    {
                        Assert.IsNotNull(map.GetAdjacentCell(cell, direction.Opposite()));
                        Assert.AreEqual(adjacentCell.Terrain, map.GetAdjacentCell(cell, direction.Opposite()).Terrain);
                    }

                }
            }
        }

        private Map<Cell> GenerateMap()
        {
            var map = new Map<BinaryCell>(SOME_WIDTH, SOME_HEIGHT);

            new MazeGenerator<BinaryCell>().ProcessMap(map, mConfiguration, mRandomizer);
            new SparsenessReducer<BinaryCell>().ProcessMap(map, mConfiguration, mRandomizer);
            new DeadendsRemover<BinaryCell>().ProcessMap(map, mConfiguration, mRandomizer);
            var newMap = new MapDoubler<Cell, BinaryCell>().ConvertMap(map, mConfiguration, mRandomizer);
            new RoomGenerator<Cell>().ProcessMap(newMap, mConfiguration, mRandomizer);
            new DoorGenerator<Cell>().ProcessMap(newMap, mConfiguration, mRandomizer);

            return newMap;
        }
    }
}
  