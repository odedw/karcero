using System;
using System.Collections.Generic;
using System.Linq;

namespace Karcero.Engine.Helpers
{
    /// <summary>
    /// Helper methods for enumerations
    /// </summary>
    public class GetAll
    {
        /// <summary>
        /// Returns a collection that contains all of the enum's values.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <returns>A collection that contains all of the enum's values.</returns>
        public static IEnumerable<T> ValuesOf<T>()
        {
            return typeof (T).IsEnum ? Enum.GetValues(typeof (T)).OfType<T>() : null;
        }
    }
}
