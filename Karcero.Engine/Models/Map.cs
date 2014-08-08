using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Karcero.Engine.Contracts;

namespace Karcero.Engine.Models
{
    public class Map<T> where T : class, ICell, new()
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public List<Room> Rooms { get; set; }

        public IEnumerable<T> AllCells
        {
            get
            {
                return mMap.SelectMany(cells => cells);
            }
        }

        private T[][] mMap;
        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            Init();
        }

        internal void Init()
        {
            
            mMap = new T[Height][];
            for (int i = 0; i < Height; i++)
            {
                mMap[i] = new T[Width];
                for (int j = 0; j < Width; j++)
                {
                    mMap[i][j] = new T() {Row = i, Column = j};
                }
            }

            Rooms = new List<Room>();
        }

        public T GetAdjacentCell(T cell, Direction direction, int distance = 1)
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

        public bool TryGetAdjacentCell(T cell, Direction direction, out T adjacentCell)
        {
            adjacentCell = GetAdjacentCell(cell, direction);
            return adjacentCell != null;
        }

        public T GetCell(int row, int column)
        {
            return row >= 0 && column >= 0 && row < Height && column < Width ? mMap[row][column] : null;
        }

        public IEnumerable<T> GetRoomCells(Room room)
        {
            var cells = new List<T>();
            for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
            {
                for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
                {
                    cells.Add(GetCell(i,j));
                }    
            }
            return cells;
        }

        public IEnumerable<T> GetCellsAdjacentToRoom(Room room, int distance = 1)
        {
            var cells = new List<T>();
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

        public bool IsLocationInRoom(int row, int column)
        {
            return Rooms.Any(room => room.IsLocationInRoom(row, column));
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
                }
            }
        }

        public IEnumerable<T> GetAllAdjacentCells(T cell, bool includeDiagonalCells = false)
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
