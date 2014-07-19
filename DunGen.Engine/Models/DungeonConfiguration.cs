namespace DunGen.Engine.Models
{
    public class DungeonConfiguration
    {
        public int Height { get; set; }
        public int Width { get; set; }

        /// <summary>
        /// Controls how random the map is (how many twists and turns).
        /// Value is between 0 and 1, higher value means more random.
        /// </summary>
        public double Randomness { get; set; }

        /// <summary>
        /// Controls how sparse the map is. 
        /// Higher value == less sparse.
        /// </summary>
        public int Sparseness { get; set; }
        public DungeonConfiguration()
        {
            Randomness = 1;
        }
    }
}
