using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    public interface ICell : IBaseCell
    {
        TerrainType Terrain { get; set; }
    }
}