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
    }
}
