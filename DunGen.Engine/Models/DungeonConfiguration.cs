namespace DunGen.Engine.Models
{
    public class DungeonConfiguration
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public double Randomness { get; set; }

        public DungeonConfiguration()
        {
            Randomness = 1;
        }
    }
}
