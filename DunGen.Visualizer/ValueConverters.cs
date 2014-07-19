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
            var tile = (TileType) value;
            return new SolidColorBrush(tile == TileType.Floor ? Colors.Lavender : Colors.Black);
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
                sides[Direction.West] == SideType.Open ? 0 : 1,
                sides[Direction.North] == SideType.Open ? 0 : 1,
                sides[Direction.East] == SideType.Open ? 0 : 1,
                sides[Direction.South] == SideType.Open ? 0 : 1);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
