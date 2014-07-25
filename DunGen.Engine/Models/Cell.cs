using System;
using System.Collections.Generic;
using System.Drawing;

namespace DunGen.Engine.Models
{
    public class Cell
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public TerrainType Terrain { get; set; }

        public Dictionary<Direction,SideType> Sides { get; set; }

        public Cell()
        {
            Sides = new Dictionary<Direction, SideType>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Sides[direction] = SideType.Wall;
            }
        }

        public String Description
        {
            get
            {
                return string.Format("Location: {0},{1}{2}", Row, Column, Environment.NewLine) +
                       string.Format("Terrain: {0}{1}{1}", Terrain, Environment.NewLine) +
                       string.Format("       {0}      {1}", Sides[Direction.North], Environment.NewLine) +
                       string.Format("{0}       {1}{2}", Sides[Direction.West], Sides[Direction.East], Environment.NewLine) +
                       string.Format("       {0}      {1}", Sides[Direction.South], Environment.NewLine);
            }
        }

    }

    public enum TerrainType
    {
        Rock,
        Floor
    }

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public static class Extensions
    {
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                    case Direction.East: return Direction.West;
                    case Direction.North: return Direction.South;
                    case Direction.South: return Direction.North;
                    case Direction.West: return Direction.East;
            }
            return Direction.East;
        }
        
        public static Direction Rotate(this Direction direction, bool clockwise = true)
        {
            switch (direction)
            {
                case Direction.East: return clockwise ? Direction.South : Direction.North;
                case Direction.North: return clockwise ? Direction.East : Direction.West;
                case Direction.South: return clockwise ? Direction.West : Direction.East;
                case Direction.West: return clockwise ? Direction.North : Direction.South;
            }
            return Direction.East;
        }
    }

    public enum SideType
    {
        Wall,
        Open
        
    }
}
