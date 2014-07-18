using System.Collections.Generic;
using DunGen.Engine.Models;

namespace DunGen.Engine.Contracts
{
    public interface IRandomizer
    {
        Cell GetRandomCell(Map map);
        T GetRandomEnumValue<T>(IEnumerable<T> excluded = null);
        T GetRandomItem<T>(ICollection<T> collection, ICollection<T> excluded);

    }
}
