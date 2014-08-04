using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class RoomGenerator : IMapProcessor
    {
        public void ProcessMap(Map map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            GenerateRooms(map, configuration, randomizer);

            FixMapIntegrity(map);
        }

        private void GenerateRooms(Map map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            for (var i = 0; i < configuration.RoomCount; i++)
            {
                //Set the "best" score to infinity (or some arbitrarily huge number)
                var bestScore = int.MaxValue;
                Cell bestCell = null;

                //Generate a room such that Wmin <= Rw <= Wmax and Hmin <= Rh <= Hmax. 
                var size = randomizer.GetRandomRoomSize(configuration.MaxRoomWidth, configuration.MinRoomWidth,
                    configuration.MaxRoomHeight, configuration.MinRoomHeight);

                var room = new Room() {Size = size};
                var visitedCells = new List<Cell>();

                //For each cell C in the dungeon, do the following:
                while (visitedCells.Count < map.AllCells.Count())
                {
                    var cell = randomizer.GetRandomItem(map.AllCells, visitedCells);
                    visitedCells.Add(cell);
                    room.Row = cell.Row;
                    room.Column = cell.Column;

                    if (room.Right >= map.Width || room.Bottom >= map.Height) continue;

                    var cells = map.GetRoomCells(room).ToList();

                    //overlapping another room
                    if (cells.Any(c => map.IsCellLocationInRoom(c.Row, c.Column))) continue;

                    //adjacent to another room
                    if (map.GetCellsAdjacentToRoom(room).Any(c => map.IsCellLocationInRoom(c.Row, c.Column))) continue;

                    //corners are rock
                    if (map.GetCell(room.Row - 1, room.Column - 1) != null &&
                        map.GetCell(room.Row - 1, room.Column - 1).Terrain != TerrainType.Rock) continue; //NW corner
                    if (map.GetCell(room.Row - 1, room.Right) != null &&
                        map.GetCell(room.Row - 1, room.Right).Terrain != TerrainType.Rock) continue; //NE corner
                    if (map.GetCell(room.Bottom, room.Column - 1) != null &&
                        map.GetCell(room.Bottom, room.Column - 1).Terrain != TerrainType.Rock) continue; //SW corner
                    if (map.GetCell(room.Bottom, room.Right) != null &&
                        map.GetCell(room.Bottom, room.Right).Terrain != TerrainType.Rock) continue; //SE corner

                    //all corridors leading into room can become doors (are isolated)
                    bool foundNonIsolatedCell = false;
                    for (var j = room.Column; j < room.Right; j++)
                    {
                        Cell northCell, southCell;
                        if ((map.TryGetAdjacentCell(map.GetCell(room.Row, j), Direction.North, out northCell) && 
                            northCell.Terrain == TerrainType.Floor && 
                            !IsCellIsolatedOnSides(northCell, new[] {Direction.East, Direction.West}, map)) ||
                            (map.TryGetAdjacentCell(map.GetCell(room.Bottom - 1, j), Direction.South, out southCell) 
                            && southCell.Terrain == TerrainType.Floor &&
                            !IsCellIsolatedOnSides(southCell, new[] { Direction.East, Direction.West }, map)))
                        {
                            foundNonIsolatedCell = true;
                            break;
                        }
                    }

                    if (foundNonIsolatedCell) continue;

                    for (var r = room.Row; r < room.Bottom; r++)
                    {
                        Cell eastCell, westCell;
                        if ((map.TryGetAdjacentCell(map.GetCell(r, room.Right - 1), Direction.East, out eastCell) && 
                            eastCell.Terrain == TerrainType.Floor &&
                            !IsCellIsolatedOnSides(eastCell, new[] {Direction.North, Direction.South}, map)) ||
                            (map.TryGetAdjacentCell(map.GetCell(r, room.Column), Direction.West, out westCell)
                            && westCell.Terrain == TerrainType.Floor &&
                            !IsCellIsolatedOnSides(westCell, new[] { Direction.North, Direction.South }, map)))
                        {
                            foundNonIsolatedCell = true;
                            break;
                        }
                    }

                    if (foundNonIsolatedCell) continue;

                    map.AddRoom(room);
                    if (MapChanged != null) MapChanged(this, new MapChangedDelegateArgs() { Map = map, CellsChanged = map.GetRoomCells(room) });
                    break;
                }             
            }
        }

        private void FixMapIntegrity(Map map)
        {
            var cells = map.Rooms.SelectMany(map.GetRoomCells).Union(map.Rooms.SelectMany(room => map.GetCellsAdjacentToRoom(room))).ToArray();
            foreach (var cell in cells)
            {
                foreach (var kvp in cell.Sides.ToList())
                {
                    Cell adjacent;
                    cell.Sides[kvp.Key] = !map.TryGetAdjacentCell(cell, kvp.Key, out adjacent) || adjacent.Terrain == TerrainType.Rock
                        ? SideType.Wall
                        : SideType.Open;
                }

            }
            if (MapChanged != null) MapChanged(this, new MapChangedDelegateArgs() { Map = map, CellsChanged = cells });

        }

        private bool IsCellIsolatedOnSides(Cell cell, IEnumerable<Direction> directions, Map map)
        {
            Cell adjacent;
            return directions.All(direction => map.TryGetAdjacentCell(cell, direction, out adjacent)
                || adjacent.Terrain == TerrainType.Rock);
        }

        public event MapChangedDelegate MapChanged;
    }
}
