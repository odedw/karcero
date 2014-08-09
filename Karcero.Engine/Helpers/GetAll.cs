using System;
using System.Collections.Generic;
using System.Linq;

namespace Karcero.Engine.Helpers
{
    public class GetAll
    {
        public static IEnumerable<T> ValuesOf<T>()
        {
            return typeof (T).IsEnum ? Enum.GetValues(typeof (T)).OfType<T>() : null;
        }

        public static IEnumerable<T> NamesOf<T>()
        {
            return typeof(T).IsEnum ? Enum.GetNames(typeof (T)).OfType<T>() : null;
        }
    }
}
