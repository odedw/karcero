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

        public Cell GetAdjacentCell(Cell cell, Direction direction)
        {
            switch (direction)
            {
                case Direction.South:
                    return cell.Row + 1 >= Height ? null : GetCell(cell.Row + 1, cell.Column);
                case Direction.East:
                    return cell.Column + 1 >= Width ? null : GetCell(cell.Row, cell.Column + 1);
                case Direction.North:
                    return cell.Row - 1 < 0 ? null : GetCell(cell.Row - 1, cell.Column);
                case Direction.West:
                    return cell.Column - 1 < 0 ? null : GetCell(cell.Row, cell.Column-1);
            }
            return null;
        }

        public Cell GetCell(int row, int column)
        {
            return mMap[row][column];
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

        public IEnumerable<Cell> GetCellsAdjacentToRoom(Room room)
        {
            var cells = new List<Cell>();
            for (var j = room.Column; j < Math.Min(room.Right, Width); j++)
            {
                if (room.Row >= 1) cells.Add(GetAdjacentCell(GetCell(room.Row, j), Direction.North));
                if (room.Row < Height - 1) cells.Add(GetAdjacentCell(GetCell(room.Row, j), Direction.South));
            }

            for (var i = room.Row; i < Math.Min(room.Bottom, Height); i++)
            {

                if (room.Column >= 1) cells.Add(GetAdjacentCell(GetCell(i, room.Column), Direction.West));
                if (room.Right < Width - 1) cells.Add(GetAdjacentCell(GetCell(i, room.Right), Direction.East));

            }
            return cells;
        } 

        public bool IsCellLocationInRoom(int row, int column)
        {
            return Rooms.Any(room => room.Row <= row && room.Bottom >= row &&
                                     room.Column <= column && room.Right >= column);
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
    }
}
