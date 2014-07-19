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
                cell.TileType = TileType.Floor;
            }
            map.GetCell(0, 0).Sides[Direction.South] = map.GetCell(1, 0).Sides[Direction.South] = SideType.Open;
            map.GetCell(0, 1).Sides[Direction.North] = map.GetCell(1, 1).Sides[Direction.North] = SideType.Open;
            map.GetCell(0, 1).Sides[Direction.East] = map.GetCell(1, 1).Sides[Direction.West] = SideType.Open;

            var sparsenessReducer = new SparsenessReducer();
            sparsenessReducer.ProcessMap(map, new DungeonConfiguration(){Sparseness = 1});

            //Assert tile types
            Assert.AreEqual(TileType.Rock, map.GetCell(0,0).TileType);
            Assert.AreEqual(TileType.Rock, map.GetCell(1, 0).TileType);
            Assert.AreEqual(TileType.Floor, map.GetCell(0, 1).TileType);
            Assert.AreEqual(TileType.Floor, map.GetCell(1, 1).TileType);

            //Assert walls
            Assert.AreEqual(SideType.Wall, map.GetCell(0, 0).Sides[Direction.South]);
            Assert.AreEqual(SideType.Wall, map.GetCell(1, 0).Sides[Direction.South]);
            Assert.AreEqual(SideType.Wall, map.GetCell(0, 1).Sides[Direction.North]);
            Assert.AreEqual(SideType.Wall, map.GetCell(1, 1).Sides[Direction.North]);
        }

        [Test]
        public void ProcessMap_ValidInput_SideTypesInAdjacentCellsMatch()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            var mazeGenerator = new MazeGenerator(new Randomizer());
            mazeGenerator.ProcessMap(map, new DungeonConfiguration() { Height = SOME_HEIGHT, Width = SOME_WIDTH });
            var sparseness = new SparsenessReducer();
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
