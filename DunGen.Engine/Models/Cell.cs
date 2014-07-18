using System;
using System.Collections.Generic;
using System.Drawing;

namespace DunGen.Engine.Models
{
    public class Cell
    {
        public Point Location { get; set; }

        public TileType TileType { get; set; }

        public Dictionary<Direction,SideType> Sides { get; set; }

        public Cell()
        {
            Sides = new Dictionary<Direction, SideType>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Sides[direction] = SideType.Wall;
            }
        }

        public String Text { get; set; }
    }

    public enum TileType
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
    }

    public enum SideType
    {
        Wall,
        Open
        
    }
}
