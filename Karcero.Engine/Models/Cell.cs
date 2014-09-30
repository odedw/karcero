using System;
using Karcero.Engine.Contracts;

namespace Karcero.Engine.Models
{
    /// <summary>
    /// Default implementation of the ICell interface.
    /// </summary>
    public class Cell : ICell
    {
        #region Properties
        /// <summary>
        /// The cell's row in the map (will be set by the generator).
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// The cell's column in the map (will be set by the generator).
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The cell's terrain.
        /// </summary>
        public TerrainType Terrain { get; set; }
        #endregion
    }

    /// <summary>
    /// An enumeration containing the types of terrain a cell can have.
    /// </summary>
    public enum TerrainType
    {
        /// <summary>
        /// A rock/wall, probably impassable.
        /// </summary>
        Rock,
        /// <summary>
        /// Open ground.
        /// </summary>
        Floor,
        /// <summary>
        /// A doorway to a room.
        /// </summary>
        Door
    }

    /// <summary>
    /// Cardinal directions.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Represented on the map as up.
        /// </summary>
        North,
        /// <summary>
        /// Represented on the map as right.
        /// </summary>
        East,
        /// <summary>
        /// Represented on the map as down.
        /// </summary>
        South,
        /// <summary>
        /// Represented on the map as left.
        /// </summary>
        West

    }

    /// <summary>
    /// Helper methods for the Direction enumeration.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the opposite cardinal direction to.
        /// </summary>
        /// <param name="direction">The initial direction.</param>
        /// <returns>The opposite cardinal direction.</returns>
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
        
        /// <summary>
        /// Rotate a cardinal direction clockwise or anti-clockwise.
        /// </summary>
        /// <param name="direction">The direction to rotate</param>
        /// <param name="clockwise">True for clockwise, false for anti-clockwise.</param>
        /// <returns>The rotated direction.</returns>
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
}
