using System.Collections.Generic;
using System.Linq;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Processors
{
    internal class RoomGenerator<T> : IMapProcessor<T> where T : class, ICell, new()
    {
        public void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            for (var i = 0; i < configuration.RoomCount; i++)
            {
                //Generate a room such that Wmin <= Rw <= Wmax and Hmin <= Rh <= Hmax. 
                var room = CreateRoom(configuration, randomizer);

                var visitedCells = new List<T>();

                while (visitedCells.Count < map.AllCells.Count())
                {
                    //get a random cell
                    var cell = randomizer.GetRandomItem(map.AllCells, visitedCells);
                    visitedCells.Add(cell);

                    //place the room
                    room.Row = cell.Row;
                    room.Column = cell.Column;
                    if (room.Right > map.Width || room.Bottom > map.Height) continue; //out of bounds

                    var cells = map.GetRoomCells(room).ToList();

                    //don't place room where it is overlapping another room
                    if (cells.Any(c => map.IsLocationInRoom(c.Row, c.Column))) continue;

                    //don't place room where it is adjacent to another room
                    if (map.GetCellsAdjacentToRoom(room).Any(c => map.IsLocationInRoom(c.Row, c.Column))) continue;

                    //corners are rock
                    if (!AreAllCornerCellsRocks(map, room)) continue; //NW corner

                    //all corridors leading into room can become doors (are isolated)
                    if (!CanAllCorridorsLeadingToRoomBeDoors(map, room)) continue;          

                    PlaceRoom(map, room);
                    break;
                }
            }
        }

        private bool CanAllCorridorsLeadingToRoomBeDoors(Map<T> map, Room room)
        {
            //check south and north edges
            for (var j = room.Column; j < room.Right; j++)
            {
                T northCell, southCell;
                if ((map.TryGetAdjacentCell(map.GetCell(room.Row, j), Direction.North, out northCell) &&
                     northCell.Terrain == TerrainType.Floor &&
                     !IsCellIsolatedOnSides(northCell, new[] {Direction.East, Direction.West}, map)) ||
                    (map.TryGetAdjacentCell(map.GetCell(room.Bottom - 1, j), Direction.South, out southCell)
                     && southCell.Terrain == TerrainType.Floor &&
                     !IsCellIsolatedOnSides(southCell, new[] {Direction.East, Direction.West}, map)))
                {
                    return false;
                }
            }

            for (var r = room.Row; r < room.Bottom; r++)
            {
                T eastCell, westCell;
                if ((map.TryGetAdjacentCell(map.GetCell(r, room.Right - 1), Direction.East, out eastCell) &&
                    eastCell.Terrain == TerrainType.Floor &&
                    !IsCellIsolatedOnSides(eastCell, new[] { Direction.North, Direction.South }, map)) ||
                    (map.TryGetAdjacentCell(map.GetCell(r, room.Column), Direction.West, out westCell)
                    && westCell.Terrain == TerrainType.Floor &&
                    !IsCellIsolatedOnSides(westCell, new[] { Direction.North, Direction.South }, map)))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreAllCornerCellsRocks(Map<T> map, Room room)
        {
            //check south and north edges
            if (map.GetCell(room.Row - 1, room.Column - 1) != null &&
                map.GetCell(room.Row - 1, room.Column - 1).Terrain != TerrainType.Rock) return false;
            if (map.GetCell(room.Row - 1, room.Right) != null &&
                map.GetCell(room.Row - 1, room.Right).Terrain != TerrainType.Rock) return false;
            if (map.GetCell(room.Bottom, room.Column - 1) != null &&
                map.GetCell(room.Bottom, room.Column - 1).Terrain != TerrainType.Rock) return false;
            if (map.GetCell(room.Bottom, room.Right) != null &&
                map.GetCell(room.Bottom, room.Right).Terrain != TerrainType.Rock) return false;
            return true;
        }

        private static Room CreateRoom(DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var size = randomizer.GetRandomRoomSize(configuration.MaxRoomWidth, configuration.MinRoomWidth,
                configuration.MaxRoomHeight, configuration.MinRoomHeight);

            var room = new Room() {Size = size};
            return room;
        }

        public void PlaceRoom(Map<T> map, Room room)
        {
            map.AddRoom(room);
            foreach (var cell in map.GetRoomCells(room))
            {
                cell.Terrain = TerrainType.Floor;
            }
        }

        private bool IsCellIsolatedOnSides(T cell, IEnumerable<Direction> directions, Map<T> map)
        {
            T adjacent;
            return directions.All(direction => !map.TryGetAdjacentCell(cell, direction, out adjacent)
                || adjacent.Terrain == TerrainType.Rock);
        }

    }
}
