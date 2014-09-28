using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    /// <summary>
    /// The interface a class must implement to be passed as the map cell type to the generator.
    /// </summary>
    public interface ICell : IBaseCell
    {
        /// <summary>
        /// The cell's terrain.
        /// </summary>
        TerrainType Terrain { get; set; }
    }
}