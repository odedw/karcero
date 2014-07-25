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
    public class RoomGeneratorTests
    {
        private const int SOME_WIDTH = 16;
        private const int SOME_HEIGHT = 16;
        private int mSeed;
        private readonly Engine.Implementations.Randomizer mRandomizer = new Randomizer();
        private readonly DungeonConfiguration mConfiguration =
            new DungeonConfiguration()
            {
                Height = SOME_HEIGHT, Width = SOME_WIDTH, ChanceToRemoveDeadends = 1, Sparseness = 1, Randomness = 1,
                MinRoomHeight = 3, MaxRoomHeight = 6, MinRoomWidth = 3, MaxRoomWidth = 6, RoomCount = 10
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
        public void ProcessMap_GenerateRooms_AllCellsInRoomsAreFloors()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);
            
            new MazeGenerator().ProcessMap(map, mConfiguration, mRandomizer);
            new SparsenessReducer().ProcessMap(map, mConfiguration, mRandomizer);
            new MapDoubler().ProcessMap(map, mConfiguration, mRandomizer);

            var roomGenerator = new RoomGenerator();
            roomGenerator.ProcessMap(map, mConfiguration, mRandomizer);

            foreach (var room in map.Rooms)
            {
                Assert.IsTrue(map.GetRoomCells(room).All(cell => cell.Terrain == TerrainType.Floor));
            }
        }

    }
}
