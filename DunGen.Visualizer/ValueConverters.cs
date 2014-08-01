using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            var tile = (TerrainType) value;
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
            return width*int.Parse(parameter.ToString());
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
            var sides = (Dictionary<Direction, SideType>) value;
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
            var cell = (Cell) values[0];
            if (cell.Terrain != TerrainType.Door) return Visibility.Hidden;
            var map = (Map) values[1];
            if (map == null) return Visibility.Hidden;
            var doorCanvas = parameter.ToString();
                if (map.GetAdjacentCell(cell, Direction.West) != null &&
                    map.GetAdjacentCell(cell, Direction.West).Terrain != TerrainType.Rock)
                    return doorCanvas == "Horizontal" ? Visibility.Visible : Visibility.Hidden;
                
                return doorCanvas == "Vertical" ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
