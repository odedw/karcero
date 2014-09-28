using System.Collections.Generic;
using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    /// <summary>
    /// Contains several methods that fetch a value randomly.
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// The seed for any randomized operation. A random seed will be generated if one is not supplied.
        /// </summary>
        /// <param name="seed"></param>
        void SetSeed(int seed);
        /// <summary>
        /// Get a random cell of any terrain from the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <typeparam name="T">The actual type of the cell class the map is comprised of.</typeparam>
        /// <returns>A random cell from the map.</returns>
        T GetRandomCell<T>(Map<T> map) where T : class, IBaseCell, new();
        /// <summary>
        /// Get a random value from the values of an enum.
        /// </summary>
        /// <param name="excluded">Any values to be excluded from the enum's set of values.</param>
        /// <typeparam name="TItem">The type of the enum.</typeparam>
        /// <returns>A random value of TItem.</returns>
        TItem GetRandomEnumValue<TItem>(IEnumerable<TItem> excluded = null);
        /// <summary>
        /// Get a random value from a collection.
        /// </summary>
        /// <param name="collection">The collection of values.</param>
        /// <typeparam name="TItem">The type of values the collection holds.</typeparam>
        /// <returns>A random item from the collection.</returns>
        TItem GetRandomItem<TItem>(IEnumerable<TItem> collection);
        /// <summary>
        /// Get a random double.
        /// </summary>
        /// <returns>A random double.</returns>
        double GetRandomDouble();
        /// <summary>
        /// Get a random room size within the measurements supplied.
        /// </summary>
        /// <param name="minWidth">The min room width.</param>
        /// <param name="maxWidth">the max room width.</param>
        /// <param name="minHeight">The min room height.</param>
        /// <param name="maxHeight">The max room height.</param>
        /// <returns>A random room size within the measurements supplied.</returns>
        Size GetRandomRoomSize(int minWidth, int maxWidth, int minHeight, int maxHeight);

    }
}
