using DunGen.Engine.Models;

namespace DunGen.Engine.Contracts
{
    public interface ICell
    {
        int Row { get; set; }
        int Column { get; set; }
        TerrainType Terrain { get; set; }

    }
}