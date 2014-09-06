using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Helpers.Randomizer;

namespace Karcero.Tests
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
            var map = GenerateMap();

            var doubler = new MapDoubler<Cell,BinaryCell>();
            var newMap = doubler.ConvertMap(map, mConfiguration, mRandomizer);

            Assert.AreEqual(SOME_EVEN_HEIGHT * 2 + 1, newMap.Height);
            Assert.AreEqual(SOME_EVEN_WIDTH * 2 + 1, newMap.Width);
        }
      
        [Test]
        public void ProcessMap_DoubleMap_TerrainAndSidesSetProperly()
        {
            var map = GenerateMap();
            var oldCells = map.AllCells.ToList();

            var doubler = new MapDoubler<Cell, BinaryCell>();
            var newMap = doubler.ConvertMap(map, mConfiguration, mRandomizer);
          
            foreach (var oldCell in oldCells.Where(cell => cell.IsOpen))
            {
                //assert the cell in the new location
                var newCell = newMap.GetCell(oldCell.Row * 2 + 1, oldCell.Column * 2 + 1);
                Assert.AreEqual(TerrainType.Floor, newCell.Terrain);

                //Where the side is open there should be floor, where the side is closed there should be rock
                foreach (var kvp in oldCell.Sides)
                {
                    Assert.AreEqual(kvp.Value ? TerrainType.Floor : TerrainType.Rock, newMap.GetAdjacentCell(newCell, kvp.Key).Terrain);
                }
            }
          
        }

        private Map<BinaryCell> GenerateMap(int width = SOME_EVEN_WIDTH, int height = SOME_EVEN_HEIGHT)
        {
            var map = new Map<BinaryCell>(width, height);
            var mazeGenerator = new MazeGenerator<BinaryCell>();
            mazeGenerator.ProcessMap(map, mConfiguration, mRandomizer);
            return map;
        }
    }
}
