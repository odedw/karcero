using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    /// <summary>
    /// A map processor performs some sort of modification to the map. The DungeonGenerator will apply any map processors added after the
    /// initial map generation process.
    /// </summary>
    /// <typeparam name="T">The actual type of the cell class the map is comprised of.</typeparam>
    public interface IMapProcessor<T> where T : class, ICell, new()
    {
        /// <summary>
        /// The method that performs the map modification. Will be called by the DungeonGenerator.
        /// </summary>
        /// <param name="map">The map to perform the modification on.</param>
        /// <param name="configuration">The configuration for the map generation process.</param>
        /// <param name="randomizer">The randomizer to use during the processing.</param>
        void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer);
    }
}
