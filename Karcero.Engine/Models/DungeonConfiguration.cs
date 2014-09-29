namespace Karcero.Engine.Models
{
    /// <summary>
    /// Contains several properties that will set and affect characteristics of the map generation process.
    /// </summary>
    public class DungeonConfiguration
    {
        /// <summary>
        /// The height of the map.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// The width of the map.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Controls how random the map is (how many twists and turns).
        /// Value is between 0 and 1, higher value means more random.
        /// </summary>
        public double Randomness { get; set; }

        /// <summary>
        /// Controls how sparse the map is. 
        /// Value is between 0 and 1, higher value means more sparse.
        /// </summary>
        public double Sparseness { get; set; }

        /// <summary>
        /// Controls how many dead ends are left.
        /// Value is between 0 and 1, higher value == less dead ends.
        /// </summary>
        public double ChanceToRemoveDeadends { get; set; }

        /// <summary>
        /// Minimum width for room generation.
        /// </summary>
        public int MinRoomWidth { get; set; }
        /// <summary>
        /// Maximum width for room generation.
        /// </summary>
        public int MaxRoomWidth { get; set; }
        
        /// <summary>
        /// Minimum height for room generation.
        /// </summary>
        public int MinRoomHeight { get; set; }
        /// <summary>
        /// Maximum height for room generation.
        /// </summary>
        public int MaxRoomHeight { get; set; }

        /// <summary>
        /// Number of rooms to generate.
        /// </summary>
        public int RoomCount { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DungeonConfiguration()
        {
            Randomness = 1;
        }
    }
}
