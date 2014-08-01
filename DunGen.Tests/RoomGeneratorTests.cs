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
            var map = GenerateMap();

            foreach (var room in map.Rooms)
            {
                Assert.IsTrue(map.GetRoomCells(room).All(cell => cell.Terrain == TerrainType.Floor));
            }
        }

        [Test]
        public void ProcessMap_GenerateRooms_AllRoomSizesAreInRange()
        {
            var map = GenerateMap();

            foreach (var room in map.Rooms)
            {
                Assert.IsTrue(room.Size.Width <= mConfiguration.MaxRoomWidth && room.Size.Width >= mConfiguration.MinRoomWidth);
                Assert.IsTrue(room.Size.Height <= mConfiguration.MaxRoomHeight && room.Size.Height >= mConfiguration.MinRoomHeight);
            }

        }

        [Test]
        public void ProcessMap_GenerateRooms_RoomsDontOverlap()
        {
            var map = GenerateMap();

            foreach (var room in map.Rooms)
            {
                var overlapping =
                    map.Rooms.FirstOrDefault(r =>
                    !(r.Bottom <= room.Row || room.Bottom <= r.Row || //A's top edge is below B's bottom edge or vice versa
                    r.Right <= room.Column || room.Right <= r.Column) && //A's left edge is to the right of the B's right edge or vice versa
                    r != room); 
                Assert.IsNull(overlapping);
            }

        }


        [Test]
        public void ProcessMap_GenerateRooms_RoomsAreNotAdjacent()
        {
            var map = GenerateMap();

            foreach (var cell in map.Rooms.SelectMany(map.GetCellsAdjacentToRoom))
            {
                Assert.IsFalse(map.IsCellLocationInRoom(cell.Row, cell.Column));
            }

        }

        [Test]
        public void ProcessMap_GenerateRooms_SidesMatchAdjacentCells()
        {
            var map = GenerateMap();

            foreach (var cell in map.AllCells)
            {
                foreach (var kvp in cell.Sides)
                {
                    var adjacent = map.GetAdjacentCell(cell, kvp.Key);
                    Assert.IsTrue((kvp.Value == SideType.Wall && (adjacent == null || adjacent.Terrain == TerrainType.Rock || cell.Terrain == TerrainType.Rock)) ||
                                   (kvp.Value == SideType.Open && adjacent.Terrain == TerrainType.Floor));
                }
                    
            }

        }
        
        [Test]
        public void ProcessMap_GenerateRooms_CorridorsLeadingIntoRoomsCanBeDoors()
        {
            var map = GenerateMap();

            foreach (var room in map.Rooms)
            {
                for (var j = room.Column; j < room.Right; j++)
                {
                    var northCell = map.GetAdjacentCell(map.GetCell(room.Row, j), Direction.North);
                    var southCell = map.GetAdjacentCell(map.GetCell(room.Bottom - 1, j), Direction.South);
                    if (northCell != null && northCell.Terrain == TerrainType.Floor)
                    {
                        AssertCellIsIsolatedOnSides(northCell, new[] {Direction.East, Direction.West}, map);
                    }
                    if (southCell != null && southCell.Terrain == TerrainType.Floor)
                    {
                        AssertCellIsIsolatedOnSides(southCell, new[] {Direction.East, Direction.West}, map);
                    }
                }

                for (var i = room.Row; i < room.Bottom; i++)
                {
                    var eastCell = map.GetAdjacentCell(map.GetCell(i, room.Right - 1), Direction.East);
                    var westCell = map.GetAdjacentCell(map.GetCell(i, room.Column), Direction.West);
                    if (eastCell != null && eastCell.Terrain == TerrainType.Floor)
                    {
                        AssertCellIsIsolatedOnSides(eastCell, new[] { Direction.North, Direction.South }, map);
                    }
                    if (westCell != null && westCell.Terrain == TerrainType.Floor)
                    {
                        AssertCellIsIsolatedOnSides(westCell, new[] { Direction.North, Direction.South }, map);
                    }
                }
            }
        }

        [Test]
        public void ProcessMap_GenerateRooms_AdjacentCornerCellsAreRock()
        {
            var map = GenerateMap();

            foreach (var room in map.Rooms)
            {
                Assert.IsTrue(map.GetCell(room.Row - 1, room.Column - 1) == null || 
                    map.GetCell(room.Row - 1, room.Column - 1).Terrain == TerrainType.Rock); //NW corner
                Assert.IsTrue(map.GetCell(room.Row - 1, room.Right) == null ||
                    map.GetCell(room.Row - 1, room.Right).Terrain == TerrainType.Rock); //NE corner
                Assert.IsTrue(map.GetCell(room.Bottom, room.Column - 1) == null ||
                    map.GetCell(room.Bottom, room.Column -1).Terrain == TerrainType.Rock); //SW corner
                Assert.IsTrue(map.GetCell(room.Bottom, room.Right) == null ||
                    map.GetCell(room.Bottom, room.Right).Terrain == TerrainType.Rock); //SE corner
            }

        }

        private void AssertCellIsIsolatedOnSides(Cell cell, IEnumerable<Direction> directions, Map map)
        {
            foreach (var direction in directions)
            {
                Assert.IsTrue(map.GetAdjacentCell(cell, direction) == null || map.GetAdjacentCell(cell, direction).Terrain == TerrainType.Rock);
            }
        }

        private Map GenerateMap()
        {
            var map = new Map(SOME_WIDTH, SOME_HEIGHT);

            new MazeGenerator().ProcessMap(map, mConfiguration, mRandomizer);
            new SparsenessReducer().ProcessMap(map, mConfiguration, mRandomizer);
            new MapDoubler().ProcessMap(map, mConfiguration, mRandomizer);

            var roomGenerator = new RoomGenerator();
            roomGenerator.ProcessMap(map, mConfiguration, mRandomizer);
            return map;
        }

    }
}
