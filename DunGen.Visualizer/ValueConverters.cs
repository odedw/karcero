using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DunGen.Engine.Models;

namespace DunGen.Visualizer
{
    public class TileTypeToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var tile = (TerrainType)value;
            return new SolidColorBrush(tile == TerrainType.Rock ? Colors.Black : Colors.Lavender);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class WidthToContainerWidth : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var width = (int)value;
            return width * int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class SideTypeToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var sides = (Dictionary<Direction, SideType>)value;
            return sides[(Direction)Enum.Parse(typeof(Direction), parameter.ToString())] == SideType.Open ?
                Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class SidesToBorderThickness : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var sides = (Dictionary<Direction, SideType>)value;
            return new Thickness(
                sides[Direction.West] == SideType.Open ? 0.2 : 1,
                sides[Direction.North] == SideType.Open ? 0.2 : 1,
                sides[Direction.East] == SideType.Open ? 0.2 : 1,
                sides[Direction.South] == SideType.Open ? 0.2 : 1);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CellToDoorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = (Cell)values[0];
            if (cell.Terrain != TerrainType.Door) return Visibility.Hidden;
            var map = (Map<Cell>)values[1];
            if (map == null) return Visibility.Hidden;
            var doorCanvas = parameter.ToString();
            Cell adjacent;
            if (map.TryGetAdjacentCell(cell, Direction.West, out adjacent) && adjacent.Terrain != TerrainType.Rock)
                return doorCanvas == "Horizontal" ? Visibility.Visible : Visibility.Hidden;

            return doorCanvas == "Vertical" ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CellToSpriteRect : IMultiValueConverter
    {
        private const short NORTH = 1;
        private const short EAST = 2;
        private const short SOUTH = 4;
        private const short WEST = 8;
        private const short NORTH_EAST = 16;
        private const short SOUTH_EAST = 32;
        private const short NORTH_WEST = 64;
        private const short SOUTH_WEST = 128;
        private readonly Dictionary<short, Point> mFloorLocationByWalls = new Dictionary<short, Point>();
        private readonly Dictionary<short, Point> mWallLocationByAdjacentWallsCells = new Dictionary<short, Point>();
        private const int TILE_SIZE = 16;

        public CellToSpriteRect()
        {
            mFloorLocationByWalls[NORTH | WEST] = new Point(0, 0);
            mFloorLocationByWalls[WEST] = new Point(0, TILE_SIZE);
            mFloorLocationByWalls[SOUTH | WEST] = new Point(0, TILE_SIZE * 2);
            mFloorLocationByWalls[NORTH] = new Point(TILE_SIZE,0);
            mFloorLocationByWalls[0] = new Point(TILE_SIZE, TILE_SIZE);
            mFloorLocationByWalls[SOUTH] = new Point(TILE_SIZE, TILE_SIZE*2);
            mFloorLocationByWalls[NORTH | EAST] = new Point(TILE_SIZE*2,0);
            mFloorLocationByWalls[EAST] = new Point(TILE_SIZE * 2, TILE_SIZE);
            mFloorLocationByWalls[SOUTH | EAST] = new Point(TILE_SIZE * 2, TILE_SIZE*2);
            mFloorLocationByWalls[NORTH | WEST | EAST] = new Point(TILE_SIZE*3,0);
            mFloorLocationByWalls[EAST | WEST] = new Point(TILE_SIZE * 3, TILE_SIZE);
            mFloorLocationByWalls[SOUTH | WEST | EAST] = new Point(TILE_SIZE * 3, TILE_SIZE*2);
            mFloorLocationByWalls[NORTH | WEST | SOUTH | EAST] = new Point(TILE_SIZE*5,0);
            mFloorLocationByWalls[NORTH | WEST | SOUTH] = new Point(TILE_SIZE * 4, TILE_SIZE);
            mFloorLocationByWalls[NORTH | SOUTH] = new Point(TILE_SIZE * 5, TILE_SIZE);
            mFloorLocationByWalls[NORTH | EAST | SOUTH] = new Point(TILE_SIZE * 6, TILE_SIZE);


            mWallLocationByAdjacentWallsCells[EAST | SOUTH] = new Point(0, TILE_SIZE * 3);
            mWallLocationByAdjacentWallsCells[NORTH | SOUTH] = new Point(0, TILE_SIZE * 4);
            mWallLocationByAdjacentWallsCells[SOUTH] = new Point(0, TILE_SIZE * 4);
            mWallLocationByAdjacentWallsCells[NORTH] = new Point(0, TILE_SIZE * 4);
            mWallLocationByAdjacentWallsCells[NORTH | EAST] = new Point(0, TILE_SIZE * 5);
            mWallLocationByAdjacentWallsCells[WEST | EAST] = new Point(TILE_SIZE, TILE_SIZE * 3);
            mWallLocationByAdjacentWallsCells[EAST] = new Point(TILE_SIZE, TILE_SIZE * 3);
            mWallLocationByAdjacentWallsCells[WEST] = new Point(TILE_SIZE, TILE_SIZE * 3);
            mWallLocationByAdjacentWallsCells[WEST | SOUTH] = new Point(TILE_SIZE*2, TILE_SIZE * 3);
            mWallLocationByAdjacentWallsCells[WEST | NORTH] = new Point(TILE_SIZE*2, TILE_SIZE * 5);
            mWallLocationByAdjacentWallsCells[EAST | NORTH | SOUTH] = new Point(TILE_SIZE*3, TILE_SIZE * 4);
            mWallLocationByAdjacentWallsCells[WEST | EAST | SOUTH] = new Point(TILE_SIZE*4, TILE_SIZE * 3);
            mWallLocationByAdjacentWallsCells[NORTH | EAST | SOUTH | WEST] = new Point(TILE_SIZE*4, TILE_SIZE * 4);
            mWallLocationByAdjacentWallsCells[EAST | NORTH | WEST] = new Point(TILE_SIZE*4, TILE_SIZE * 5);
            mWallLocationByAdjacentWallsCells[SOUTH | NORTH | WEST] = new Point(TILE_SIZE*5, TILE_SIZE * 4);

        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = values[0] as Cell;
            var map = values[1] as Map<Cell>;
            var x = (double)(TILE_SIZE * 6);
            var y = (double)(TILE_SIZE * 5);
            if (cell != null && map != null)
                switch (cell.Terrain)
                {
                    case TerrainType.Rock:
                        var wallLocation = ConstructWallLocationAccordingToAdjacentWalls(cell, map);
                        x = wallLocation.X;
                        y = wallLocation.Y;
                        break;
                    case TerrainType.Floor:
                        var location = ConstructFloorLocationAccordingToWalls(cell.Sides);
                        x = location.X;
                        y = location.Y;
                        break;
                    case TerrainType.Door:
                        var doorLocation = ConstructLocationAccordingToDoorOrientation(cell.Sides);
                        x = doorLocation.X;
                        y = doorLocation.Y;
                        break;
                }
            return new Rect(x, y, TILE_SIZE, TILE_SIZE);
        }

        private Point ConstructWallLocationAccordingToAdjacentWalls(Cell cell, Map<Cell> map)
        {
            if (map.GetAllAdjacentCells(cell, true).All(c => c.Terrain == TerrainType.Rock))
                return new Point(TILE_SIZE*6, TILE_SIZE*5);

            short wallsFlag = 0;
            if (ShouldConsiderRockCell(cell, Direction.North, map)) wallsFlag |= NORTH;
            if (ShouldConsiderRockCell(cell, Direction.South, map)) wallsFlag |= SOUTH;
            if (ShouldConsiderRockCell(cell, Direction.West, map)) wallsFlag |= WEST;
            if (ShouldConsiderRockCell(cell, Direction.East, map)) wallsFlag |= EAST;

            if (mWallLocationByAdjacentWallsCells.ContainsKey(wallsFlag)) return mWallLocationByAdjacentWallsCells[wallsFlag];
            return new Point(TILE_SIZE*6, TILE_SIZE*5);
        }

        private bool ShouldConsiderRockCell(Cell cell, Direction direction, Map<Cell> map)
        {
            Cell adjacentCell;
            return (map.TryGetAdjacentCell(cell, direction, out adjacentCell) && adjacentCell.Terrain == TerrainType.Rock &&
                    map.GetAllAdjacentCells(adjacentCell, true).Any(c => c.Terrain != TerrainType.Rock));
        }
        private Point ConstructLocationAccordingToDoorOrientation(Dictionary<Direction, SideType> sides)
        {
            if (sides[Direction.East] == SideType.Open) return new Point(TILE_SIZE * 6,0);
            return new Point(TILE_SIZE * 4,0);
        }

        private Point ConstructFloorLocationAccordingToWalls(Dictionary<Direction, SideType> sides)
        {
            short wallsFlag = 0;
            if (sides[Direction.North] == SideType.Wall) wallsFlag |= NORTH;
            if (sides[Direction.West] == SideType.Wall) wallsFlag |= WEST;
            if (sides[Direction.South] == SideType.Wall) wallsFlag |= SOUTH;
            if (sides[Direction.East] == SideType.Wall) wallsFlag |= EAST;
            return mFloorLocationByWalls[wallsFlag];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
