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
    public class SparsenessReducerTests
    {
        private const int SOME_WIDTH = 30;
        private const int SOME_HEIGHT = 30;

        [Test]
        public void ProcessMap_ValidInput_OnlyDeadEndCellsRemoved()
        {
            var map = new Map(2, 2);
            foreach (var cell in map.AllCells)
            {
                cell.Terrain = TerrainType.Floor;
            }
            map.GetCell(0, 0).Sides[Direction.South] = map.GetCell(0, 1).Sides[Direction.South] = SideType.Open;
            map.GetCell(1, 0).Sides[Direction.North] = map.GetCell(1, 1).Sides[Direction.North] = SideType.Open;
            map.GetCell(1, 0).Sides[Direction.East] = map.GetCell(1, 1).Sides[Direction.West] = SideType.Open;

            var sparsenessReducer = new SparsenessReducer(new Randomizer());
            sparsenessReducer.ProcessMap(map, new DungeonConfiguration(){Sparseness = 2, Height = 2, Width = 2});

            //Assert tile types
            Assert.AreEqual(TerrainType.Rock, map.GetCell(0,0).Terrain);
            Assert.AreEqual(TerrainType.Rock, map.GetCell(0, 1).Terrain);
            Assert.AreEqual(TerrainType.Floor, map.GetCell(1, 0).Terrain);
            Assert.AreEqual(TerrainType.Floor, map.GetCell(1, 1).Terrain);

            //Assert walls
            Assert.AreEqual(SideType.Wall, map.GetCell(0, 0).Sides[Direction.South]);
            Assert.AreEqual(SideType.Wall, map.GetCell(0, 1).Sides[Direction.South]);
            Assert.AreEqual(SideType.Wall, map.GetCell(1, 0).Sides[Direction.North]);
            Assert.AreEqual(SideType.Wall, map.GetCell(1, 1).Sides[Direction.North]);
        }

        [Test]
        public void ProcessMap_ValidInput_SideTypesInAdjacentCellsMatch()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator(new Randomizer());
            mazeGenerator.ProcessMap(map, new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH });
            var sparseness = new SparsenessReducer(new Randomizer());
            sparseness.ProcessMap(map, new DungeonConfiguration() { Sparseness = 10 });

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
