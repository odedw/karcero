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
using Karcero.Engine.Models;

namespace Karcero.Visualizer
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
        private readonly Dictionary<short, Point> mFloorLocationByAdjacentRockCells = new Dictionary<short, Point>();
        private readonly Dictionary<short, Point> mWallLocationByAdjacentRockCells = new Dictionary<short, Point>();
        private const int TILE_SIZE = 16;

        public CellToSpriteRect()
        {
            mFloorLocationByAdjacentRockCells[NORTH | WEST] = new Point(0, 0);
            mFloorLocationByAdjacentRockCells[WEST] = new Point(0, TILE_SIZE);
            mFloorLocationByAdjacentRockCells[SOUTH | WEST] = new Point(0, TILE_SIZE * 2);
            mFloorLocationByAdjacentRockCells[NORTH] = new Point(TILE_SIZE,0);
            mFloorLocationByAdjacentRockCells[0] = new Point(TILE_SIZE, TILE_SIZE);
            mFloorLocationByAdjacentRockCells[SOUTH] = new Point(TILE_SIZE, TILE_SIZE*2);
            mFloorLocationByAdjacentRockCells[NORTH | EAST] = new Point(TILE_SIZE*2,0);
            mFloorLocationByAdjacentRockCells[EAST] = new Point(TILE_SIZE * 2, TILE_SIZE);
            mFloorLocationByAdjacentRockCells[SOUTH | EAST] = new Point(TILE_SIZE * 2, TILE_SIZE*2);
            mFloorLocationByAdjacentRockCells[NORTH | WEST | EAST] = new Point(TILE_SIZE*3,0);
            mFloorLocationByAdjacentRockCells[EAST | WEST] = new Point(TILE_SIZE * 3, TILE_SIZE);
            mFloorLocationByAdjacentRockCells[SOUTH | WEST | EAST] = new Point(TILE_SIZE * 3, TILE_SIZE*2);
            mFloorLocationByAdjacentRockCells[NORTH | WEST | SOUTH | EAST] = new Point(TILE_SIZE*5,0);
            mFloorLocationByAdjacentRockCells[NORTH | WEST | SOUTH] = new Point(TILE_SIZE * 4, TILE_SIZE);
            mFloorLocationByAdjacentRockCells[NORTH | SOUTH] = new Point(TILE_SIZE * 5, TILE_SIZE);
            mFloorLocationByAdjacentRockCells[NORTH | EAST | SOUTH] = new Point(TILE_SIZE * 6, TILE_SIZE);


            mWallLocationByAdjacentRockCells[0] = new Point(TILE_SIZE, TILE_SIZE * 4);
            mWallLocationByAdjacentRockCells[EAST | SOUTH] = new Point(0, TILE_SIZE * 3);
            mWallLocationByAdjacentRockCells[NORTH | SOUTH] = new Point(0, TILE_SIZE * 4);
            mWallLocationByAdjacentRockCells[SOUTH] = new Point(0, TILE_SIZE * 4);
            mWallLocationByAdjacentRockCells[NORTH] = new Point(0, TILE_SIZE * 4);
            mWallLocationByAdjacentRockCells[NORTH | EAST] = new Point(0, TILE_SIZE * 5);
            mWallLocationByAdjacentRockCells[WEST | EAST] = new Point(TILE_SIZE, TILE_SIZE * 3);
            mWallLocationByAdjacentRockCells[EAST] = new Point(TILE_SIZE, TILE_SIZE * 3);
            mWallLocationByAdjacentRockCells[WEST] = new Point(TILE_SIZE, TILE_SIZE * 3);
            mWallLocationByAdjacentRockCells[WEST | SOUTH] = new Point(TILE_SIZE*2, TILE_SIZE * 3);
            mWallLocationByAdjacentRockCells[WEST | NORTH] = new Point(TILE_SIZE*2, TILE_SIZE * 5);
            mWallLocationByAdjacentRockCells[EAST | NORTH | SOUTH] = new Point(TILE_SIZE*3, TILE_SIZE * 4);
            mWallLocationByAdjacentRockCells[WEST | EAST | SOUTH] = new Point(TILE_SIZE*4, TILE_SIZE * 3);
            mWallLocationByAdjacentRockCells[NORTH | EAST | SOUTH | WEST] = new Point(TILE_SIZE*4, TILE_SIZE * 4);
            mWallLocationByAdjacentRockCells[EAST | NORTH | WEST] = new Point(TILE_SIZE*4, TILE_SIZE * 5);
            mWallLocationByAdjacentRockCells[SOUTH | NORTH | WEST] = new Point(TILE_SIZE*5, TILE_SIZE * 4);

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
                        var location = ConstructFloorLocationAccordingToWalls(cell, map);
                        x = location.X;
                        y = location.Y;
                        break;
                    case TerrainType.Door:
                        var doorLocation = ConstructLocationAccordingToDoorOrientation(cell, map);
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

            if (mWallLocationByAdjacentRockCells.ContainsKey(wallsFlag)) return mWallLocationByAdjacentRockCells[wallsFlag];
            return new Point(TILE_SIZE*6, TILE_SIZE*5);
        }

        private bool ShouldConsiderRockCell(Cell cell, Direction direction, Map<Cell> map)
        {
            Cell adjacentCell;
            return (map.TryGetAdjacentCell(cell, direction, out adjacentCell) && adjacentCell.Terrain == TerrainType.Rock &&
                    map.GetAllAdjacentCells(adjacentCell, true).Any(c => c.Terrain != TerrainType.Rock));
        }
        private Point ConstructLocationAccordingToDoorOrientation(Cell cell, Map<Cell> map )
        {
            Cell adjacentCell;
            if (map.TryGetAdjacentCell(cell, Direction.East, out adjacentCell) && adjacentCell.Terrain == TerrainType.Floor) 
                return new Point(TILE_SIZE * 6,0);
            return new Point(TILE_SIZE * 4,0);
        }

        private Point ConstructFloorLocationAccordingToWalls(Cell cell, Map<Cell> map)
        {
            short wallsFlag = 0;
            Cell adjacentCell = null;
            if (map.TryGetAdjacentCell(cell, Direction.North, out adjacentCell) && adjacentCell.Terrain == TerrainType.Rock) wallsFlag |= NORTH;
            if (map.TryGetAdjacentCell(cell, Direction.West, out adjacentCell) && adjacentCell.Terrain == TerrainType.Rock) wallsFlag |= WEST;
            if (map.TryGetAdjacentCell(cell, Direction.South, out adjacentCell) && adjacentCell.Terrain == TerrainType.Rock) wallsFlag |= SOUTH;
            if (map.TryGetAdjacentCell(cell, Direction.East, out adjacentCell) && adjacentCell.Terrain == TerrainType.Rock) wallsFlag |= EAST;
            return mFloorLocationByAdjacentRockCells[wallsFlag];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
