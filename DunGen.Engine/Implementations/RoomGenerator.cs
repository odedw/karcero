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
            for (var i = 0; i < configuration.RoomCount; i++)
            {
                //Set the "best" score to infinity (or some arbitrarily huge number)
                var bestScore = int.MaxValue;
                Cell bestCell = null;

                //Generate a room such that Wmin <= Rw <= Wmax and Hmin <= Rh <= Hmax. 
                var size = randomizer.GetRandomRoomSize(configuration.MaxRoomWidth, configuration.MinRoomWidth,
                    configuration.MaxRoomHeight, configuration.MinRoomHeight);

                var room = new Room() { Size = size };
                var visitedCells = new List<Cell>();

                //For each cell C in the dungeon, do the following:
                while (visitedCells.Count < map.AllCells.Count())
                {
                    var cell = randomizer.GetRandomItem(map.AllCells, visitedCells);
                    visitedCells.Add(cell);
                    room.Row = cell.Row;
                    room.Column = cell.Column;

                    //Set the "current" score to 0
                    int currentScore = 0;

                    if (room.Right >= map.Width || room.Bottom >= map.Height) continue;

                    var cells = map.GetRoomCells(room).ToList();
                    var cellsAdjacentToRoom = map.GetCellsAdjacentToRoom(room).ToList();

                    //For each cell of the room that is adjacent to a corridor, add 1 to the current score. 
                    currentScore +=
                        cellsAdjacentToRoom.Where(c => c.Terrain == TerrainType.Floor)
                        .Sum(c => map.IsCellLocationInRoom(c.Row, c.Column) ? 0 : 1);

                    //For each cell of the room that overlaps a corridor, add 3 to the current score.
                    currentScore += cells.Count(c => !map.IsCellLocationInRoom(c.Row, c.Column) && c.Terrain == TerrainType.Floor) * 3;

                    //For each cell of the room that overlaps a room, add 100 to the current score.
                    currentScore += cells.Count(c => map.IsCellLocationInRoom(c.Row, c.Column)) * 100;

                    if (!VerifyHasPlaceForDoor(map, room)) continue;
                    //if (cellsAdjacentToRoom.All(c => c.Terrain == TerrainType.Rock)) currentScore += 50;
                    if (currentScore < bestScore)
                    {
                        bestScore = currentScore;
                        bestCell = cell;
                    }
                    if (currentScore == 0) break; //found it, no need to continue
                }

                //Place the room at the best position (where the best score was found). 
                if (bestCell != null)
                {
                    room.Row = bestCell.Row;
                    room.Column = bestCell.Column;

                    map.AddRoom(room);
                    if (MapChanged != null)
                    {
                        MapChanged(this, new MapChangedDelegateArgs(){Map = map, CellsChanged = map.GetRoomCells(room)});
                    }
                }

            }
        }

        private bool VerifyHasPlaceForDoor(Map map, Room room)
        {
            var cellsAdjacentToRoom = map.GetCellsAdjacentToRoom(room);
            foreach (var cell in cellsAdjacentToRoom)
            {
                if (cell.Terrain == TerrainType.Floor) continue;
                
                Cell oneCellBeyond = null;
                if (cell.Row < room.Row) oneCellBeyond = map.GetAdjacentCell(cell, Direction.North);
                if (cell.Row >= room.Bottom) oneCellBeyond = map.GetAdjacentCell(cell, Direction.South);
                if (cell.Column < room.Column) oneCellBeyond = map.GetAdjacentCell(cell, Direction.West);
                if (cell.Column >= room.Right) oneCellBeyond = map.GetAdjacentCell(cell, Direction.East);

                if (oneCellBeyond != null && oneCellBeyond.Terrain == TerrainType.Floor) return true;
            }
            return false;
        }


        public event MapChangedDelegate MapChanged;
    }
}
