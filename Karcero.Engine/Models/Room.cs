
namespace Karcero.Engine.Models
{
    /// <summary>
    /// A room contained in a map.
    /// </summary>
    public class Room
    {
        #region Properties
        /// <summary>
        /// The room's top row (inclusive).
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// The room's left column (inclusive).
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// The size of the room.
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// The room's bottom row (exclusive).
        /// </summary>
        public int Bottom { get { return Row + Size.Height; }}
        /// <summary>
        /// The room's right column (exclusive).
        /// </summary>
        public int Right { get { return Column + Size.Width; }}
        #endregion

        #region Methods
        /// <summary>
        /// Returns true if a certain location is within the specified room.
        /// </summary>
        /// <param name="row">The desired row.</param>
        /// <param name="column">The desired column.</param>
        /// <returns>True if a certain location is within the specified room.</returns>
        public bool IsLocationInRoom(int row, int column)
        {
            return Row <= row && Bottom > row &&
                   Column <= column && Right > column;
        }
        #endregion
    }
}
