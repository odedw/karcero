using System;
using System.Collections.Generic;
using System.Linq;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Helpers
{
    /// <summary>
    /// Default implementation of IRandomizer using the Random class.
    /// </summary>
    public class Randomizer : IRandomizer
    {
        #region Properties
        private Random mRandom = new Random();
        #endregion

        #region Methods
        /// <summary>
        /// The seed for any randomized operation. A random seed will be generated if one is not supplied.
        /// </summary>
        /// <param name="seed"></param>
        public void SetSeed(int seed)
        {
            mRandom = new Random(seed);
        }

        /// <summary>
        /// Get a random cell of any terrain from the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <typeparam name="T">The actual type of the cell class the map is comprised of.</typeparam>
        /// <returns>A random cell from the map.</returns>
        public T GetRandomCell<T>(Map<T> map) where T : class, IBaseCell, new()
        {
            if (map.Height == 0 || map.Width == 0) return null;
            return map.GetCell(mRandom.Next(map.Height), mRandom.Next(map.Width));
        }

        /// <summary>
        /// Get a random value from the values of an enum.
        /// </summary>
        /// <param name="excluded">Any values to be excluded from the enum's set of values.</param>
        /// <typeparam name="TItem">The type of the enum.</typeparam>
        /// <returns>A random value of TItem.</returns>
        public TItem GetRandomEnumValue<TItem>(IEnumerable<TItem> excluded = null)
        {
            excluded = excluded ?? new List<TItem>();
            var values = GetAll.ValuesOf<TItem>().Except(excluded).ToList();
            return values[mRandom.Next(values.Count)];
        }

        /// <summary>
        /// Get a random value from a collection.
        /// </summary>
        /// <param name="collection">The collection of values.</param>
        /// <typeparam name="TItem">The type of values the collection holds.</typeparam>
        /// <returns>A random item from the collection.</returns>
        public TItem GetRandomItem<TItem>(IEnumerable<TItem> collection) 
        {
            return collection.ElementAt(mRandom.Next(collection.Count()));
        }

        /// <summary>
        /// Get a random double.
        /// </summary>
        /// <returns>A random double.</returns>
        public double GetRandomDouble()
        {
            return mRandom.NextDouble();
        }

        /// <summary>
        /// Get a random room size within the measurements supplied.
        /// </summary>
        /// <param name="minWidth">The min room width.</param>
        /// <param name="maxWidth">the max room width.</param>
        /// <param name="minHeight">The min room height.</param>
        /// <param name="maxHeight">The max room height.</param>
        /// <returns>A random room size within the measurements supplied.</returns>
        public Size GetRandomRoomSize(int maxWidth, int minWidth, int maxHeight, int minHeight)
        {
            return new Size(mRandom.Next(1 + maxWidth - minWidth) + minWidth, mRandom.Next(1 + maxHeight - minHeight) + minHeight);
        }

        #endregion
    }
}
