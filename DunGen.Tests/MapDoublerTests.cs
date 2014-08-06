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
    public class MapDoublerTests
    {
        private const int SOME_EVEN_WIDTH = 30;
        private const int SOME_EVEN_HEIGHT = 30;
        private int mSeed;
        private readonly Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration =
            new DungeonConfiguration() { Height = SOME_EVEN_HEIGHT, Width = SOME_EVEN_WIDTH, Randomness = 1 };

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
        public void ProcessMap_DoubleMap_MapIsDoubled()
        {
            var map = new Map<Cell>(SOME_EVEN_WIDTH, SOME_EVEN_HEIGHT);
            var mazeGenerator = new MazeGenerator<Cell>();
            mazeGenerator.ProcessMap(map, mConfiguration, mRandomizer);

            var doubler = new MapDoubler<Cell>();
            doubler.ProcessMap(map, mConfiguration, mRandomizer);

            Assert.AreEqual(SOME_EVEN_HEIGHT * 2 + 1, map.Height);
            Assert.AreEqual(SOME_EVEN_WIDTH * 2 + 1, map.Width);
        }
        
        [Test]
        public void ProcessMap_DoubleMap_TerrainAndSidesSetProperly()
        {
            var map = new Map<Cell>(SOME_EVEN_WIDTH, SOME_EVEN_HEIGHT);
            var mazeGenerator = new MazeGenerator<Cell>();
            mazeGenerator.ProcessMap(map, mConfiguration, mRandomizer);
            var oldCells = map.AllCells.Select(cell => cell.Clone()).ToList();

            var doubler = new MapDoubler<Cell>();
            doubler.ProcessMap(map, mConfiguration, mRandomizer);
            
            foreach (var oldCell in oldCells.Where(cell => cell.Terrain == TerrainType.Floor))
            {
                //assert the cell in the new location
                var newCell = map.GetCell(oldCell.Row*2 + 1, oldCell.Column*2 + 1);
                Assert.AreEqual(TerrainType.Floor, newCell.Terrain);

                //assert the sides are kept
                foreach (var kvp in oldCell.Sides)
                {
                    Assert.AreEqual(kvp.Value, newCell.Sides[kvp.Key]);
                }

                //assert the sides where there's passage are set as floor
                foreach (var passageDirection in oldCell.Sides.Where(pair => pair.Value == SideType.Open).Select(pair => pair.Key))
                {
                    Assert.AreEqual(TerrainType.Floor, map.GetAdjacentCell(newCell, passageDirection).Terrain);
                }
            }
            
        }
    }
}
