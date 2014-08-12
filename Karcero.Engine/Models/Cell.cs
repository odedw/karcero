using System;
using System.Collections.Generic;
using System.Drawing;
using Karcero.Engine.Contracts;

namespace Karcero.Engine.Models
{
    public class Cell : ICell
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public TerrainType Terrain { get; set; }

        public Cell()
        {
        }

        public String Description
        {
            get
            {
                return string.Format("Location: {0},{1}{2}", Row, Column, Environment.NewLine) +
                       string.Format("Terrain: {0}", Terrain);
            }
        }
    }

    public enum TerrainType
    {
        Rock,
        Floor,
        Door
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
