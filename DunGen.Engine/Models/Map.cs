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

        private readonly Cell[][] mMap;
        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            mMap = new Cell[height][];
            for (int i = 0; i < height; i++)
            {
                mMap[i] = new Cell[width];
                for (int j = 0; j < width; j++)
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

        public IEnumerable<Cell> GetRectangleOfCells(Rectangle rect)
        {
            var cells = new List<Cell>();
            for (var i = rect.Top; i < Math.Min(rect.Bottom, Height); i++)
            {
                for (var j = rect.Left; j < Math.Min(rect.Right, Width); j++)
                {
                    cells.Add(GetCell(i,j));
                }    
            }
            return cells;
        }

        public IEnumerable<Cell> GetCellsAdjacentToRectangle(Rectangle rect)
        {
            var cells = new List<Cell>();
            for (var j = rect.Left; j < Math.Min(rect.Right, Width); j++)
            {
                if (rect.Top >= 1) cells.Add(GetAdjacentCell(GetCell(rect.Top, j), Direction.North));
                if (rect.Top < Height - 1) cells.Add(GetAdjacentCell(GetCell(rect.Top, j), Direction.South));
            }

            for (var i = rect.Top; i < Math.Min(rect.Bottom, Height); i++)
            {

                if (rect.Left >= 1) cells.Add(GetAdjacentCell(GetCell(i, rect.Left), Direction.West));
                if (rect.Right < Width - 1) cells.Add(GetAdjacentCell(GetCell(i, rect.Right), Direction.East));

            }
            return cells;
        } 

        public bool IsCellLocationInRoom(int row, int column)
        {
            return Rooms.Any(room => room.Measurements.Top <= row && room.Measurements.Bottom >= row &&
                                     room.Measurements.Left <= column && room.Measurements.Right >= column);
        }

        public void AddRoomAtRectangle(Rectangle rectangle)
        {
            Rooms.Add(new Room(){Measurements = rectangle});
            for (var x = rectangle.Left; x < Math.Min(rectangle.Right, Width); x++)
            {
                for (var y = rectangle.Top; y < Math.Min(rectangle.Bottom, Height); y++)
                {
                    var currentCell = GetCell(x, y);
                    currentCell.Terrain = TerrainType.Floor;
                    currentCell.Sides[Direction.North] = y == rectangle.Top ? SideType.Wall : SideType.Open;
                    currentCell.Sides[Direction.West] = x == rectangle.Left ? SideType.Wall : SideType.Open;
                    currentCell.Sides[Direction.East] = x == Math.Min(rectangle.Right - 1, Width - 1) ? SideType.Wall : SideType.Open;
                    currentCell.Sides[Direction.South] = y == Math.Min(rectangle.Bottom - 1, Height - 1) ? SideType.Wall : SideType.Open;
                }
            }
        }
    }
}
