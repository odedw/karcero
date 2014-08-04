using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
namespace DunGen.Engine.Models
{
    public class Map
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public List<Room> Rooms { get; set; }

        public IEnumerable<Cell> AllCells
        {
            get
            {
                return mMap.SelectMany(cells => cells);
            }
        }

        private Cell[][] mMap;
        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            Init();
        }

        internal void Init()
        {
            
            mMap = new Cell[Height][];
            for (int i = 0; i < Height; i++)
            {
                mMap[i] = new Cell[Width];
                for (int j = 0; j < Width; j++)
                {
                    mMap[i][j] = new Cell() {Row = i, Column = j};
                }
            }

            Rooms = new List<Room>();
        }

        public Cell GetAdjacentCell(Cell cell, Direction direction, int distance = 1)
        {
            switch (direction)
            {
                case Direction.South:
                    return cell.Row + distance >= Height ? null : GetCell(cell.Row + distance, cell.Column);
                case Direction.East:
                    return cell.Column + distance >= Width ? null : GetCell(cell.Row, cell.Column + distance);
                case Direction.North:
                    return cell.Row - distance < 0 ? null : GetCell(cell.Row - distance, cell.Column);
                case Direction.West:
                    return cell.Column - distance < 0 ? null : GetCell(cell.Row, cell.Column-distance);
            }
            return null;
        }

        public bool TryGetAdjacentCell(Cell cell, Direction direction, out Cell adjacentCell)
        {
            adjacentCell = GetAdjacentCell(cell, direction);
            return adjacentCell != null;
        }

        public Cell GetCell(int row, int column)
        {
            return row >= 0 && column >= 0 && row < Height && column < Width ? mMap[row][column] : null;
        }

        public IEnumerable<Cell> GetRoomCells(Room room)
        {
            var cells = new List<Cell>();
            for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
            {
                for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
                {
                    cells.Add(GetCell(i,j));
                }    
            }
            return cells;
        }

        public IEnumerable<Cell> GetCellsAdjacentToRoom(Room room, int distance = 1)
        {
            var cells = new List<Cell>();
            for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
            {
                if (room.Row >= distance) cells.Add(GetAdjacentCell(GetCell(room.Row, j), Direction.North, distance));
                if (room.Bottom <= Height - distance) cells.Add(GetAdjacentCell(GetCell(room.Bottom - 1, j), Direction.South, distance));
            }

            for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
            {

                if (room.Column >= distance) cells.Add(GetAdjacentCell(GetCell(i, room.Column), Direction.West, distance));
                if (room.Right <= Width - distance) cells.Add(GetAdjacentCell(GetCell(i, room.Right - 1), Direction.East, distance));

            }
            return cells;
        } 

        public bool IsCellLocationInRoom(int row, int column)
        {
            return Rooms.Any(room => room.IsCellInRoom(GetCell(row, column)));
        }

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
            for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
            {
                for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
                {
                    var currentCell = GetCell(i, j);
                    currentCell.Terrain = TerrainType.Floor;
                    currentCell.Sides[Direction.North] = i == room.Row ? SideType.Wall : SideType.Open;
                    currentCell.Sides[Direction.West] = j == room.Column ? SideType.Wall : SideType.Open;
                    currentCell.Sides[Direction.East] = j == Math.Min(room.Right - 1, Width - 1) ? SideType.Wall : SideType.Open;
                    currentCell.Sides[Direction.South] = i == Math.Min(room.Bottom - 1, Height - 1) ? SideType.Wall : SideType.Open;
                }
            }
        }

        public IEnumerable<Cell> GetAllAdjacentCells(Cell cell, bool includeDiagonalCells = false)
        {
            var cells = Enum.GetValues(typeof (Direction)).OfType<Direction>()
                .Where(direction => GetAdjacentCell(cell, direction) != null)
                .Select(direction => GetAdjacentCell(cell, direction)).ToList();

            if (includeDiagonalCells)
            {
                if (GetCell(cell.Row - 1, cell.Column - 1) != null) cells.Add(GetCell(cell.Row - 1, cell.Column - 1));
                if (GetCell(cell.Row + 1, cell.Column - 1) != null) cells.Add(GetCell(cell.Row + 1, cell.Column - 1));
                if (GetCell(cell.Row - 1, cell.Column + 1) != null) cells.Add(GetCell(cell.Row - 1, cell.Column + 1));
                if (GetCell(cell.Row + 1, cell.Column + 1) != null) cells.Add(GetCell(cell.Row + 1, cell.Column + 1));
            }
            return cells;
        }
    }
}
