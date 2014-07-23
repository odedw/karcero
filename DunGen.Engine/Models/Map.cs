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
            mMap = new Cell[width][];
            for (int i = 0; i < width; i++)
            {
                mMap[i] = new Cell[height];
                for (int j = 0; j < height; j++)
                {
                    mMap[i][j] = new Cell() {Location = new Point(i,j)};
                }
            }

            Rooms = new List<Room>();
        }

        public Cell GetAdjacentCell(Cell cell, Direction direction)
        {
            switch (direction)
            {
                case Direction.South:
                    return cell.Location.Y + 1 >= Height ? null : GetCell(cell.Location.X, cell.Location.Y + 1);
                case Direction.East:
                    return cell.Location.X + 1 >= Width ? null : GetCell(cell.Location.X + 1, cell.Location.Y);
                case Direction.North:
                    return cell.Location.Y - 1 < 0 ? null : GetCell(cell.Location.X, cell.Location.Y - 1);
                case Direction.West:
                    return cell.Location.X - 1 < 0 ? null : GetCell(cell.Location.X - 1, cell.Location.Y);
            }
            return null;
        }

        public Cell GetCell(int x, int y)
        {
            return mMap[x][y];
        }

        public IEnumerable<Cell> GetRectangleOfCells(Rectangle rect)
        {
            var cells = new List<Cell>();
            for (var y = rect.Top; y < Math.Min(rect.Bottom, Height); y++)
            {
                for (var x = rect.Left; x < Math.Min(rect.Right, Width); x++)
                {
                    cells.Add(GetCell(x,y));
                }    
            }
            return cells;
        }

        public IEnumerable<Cell> GetCellsAdjacentToRectangle(Rectangle rect)
        {
            var cells = new List<Cell>();
            for (var x = rect.Left; x < Math.Min(rect.Right, Width); x++)
            {
                if (rect.Top >= 1) cells.Add(GetAdjacentCell(GetCell(x, rect.Top), Direction.North));
                if (rect.Top < Height - 1) cells.Add(GetAdjacentCell(GetCell(x, rect.Top), Direction.South));
            }

            for (var y = rect.Top; y < Math.Min(rect.Bottom, Height); y++)
            {

                if (rect.Left >= 1) cells.Add(GetAdjacentCell(GetCell(rect.Left, y), Direction.West));
                if (rect.Right < Width - 1) cells.Add(GetAdjacentCell(GetCell(rect.Right, y), Direction.East));

            }
            return cells;
        } 

        public bool IsCellLocationInRoom(int x, int y)
        {
            return Rooms.Any(room => room.Measurements.Top <= y && room.Measurements.Bottom >= y &&
                                     room.Measurements.Left <= x && room.Measurements.Right >= x);
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
