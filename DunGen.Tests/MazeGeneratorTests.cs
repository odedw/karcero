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
    public class MazeGeneratorTests
    {
        private const int SOME_WIDTH = 30;
        private const int SOME_HEIGHT = 30;

        [Test]
        public void ProcessMap_ValidInput_AllCellsHasFloorTileType()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator(new Randomizer());
            mazeGenerator.ProcessMap(map, new DungeonConfiguration(){Height = SOME_HEIGHT, Width = SOME_WIDTH});

            foreach (var cell in map.AllCells)
            {
                Assert.AreEqual(TerrainType.Floor, cell.Terrain);
            }
        }
  
        [Test]
        public void ProcessMap_ValidInput_AllCellsAreReachable()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator(new Randomizer());
            mazeGenerator.ProcessMap(map, new DungeonConfiguration(){Height = SOME_HEIGHT, Width = SOME_WIDTH});

            var visitedCells = new HashSet<Cell>();
            var discoveredCells = new HashSet<Cell>(){map.GetCell(0,0)};
            while (discoveredCells.Any())
            {
                foreach (var discoveredCell in discoveredCells)
                {
                    visitedCells.Add(discoveredCell);
                }
                var newDiscoveredCells = new HashSet<Cell>();
                foreach (var newDiscoveredCell in discoveredCells.SelectMany(cell => cell.Sides
                    .Where(pair => pair.Value == SideType.Open && !visitedCells.Contains(map.GetAdjacentCell(cell, pair.Key)))
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
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator(new Randomizer());
            mazeGenerator.ProcessMap(map, new DungeonConfiguration(){Height = SOME_HEIGHT, Width = SOME_WIDTH});

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
