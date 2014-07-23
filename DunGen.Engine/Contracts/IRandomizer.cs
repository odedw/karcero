using System.Collections.Generic;
using System.Drawing;
using DunGen.Engine.Models;

namespace DunGen.Engine.Contracts
{
    public interface IRandomizer
    {
        Cell GetRandomCell(Map map);
        T GetRandomEnumValue<T>(IEnumerable<T> excluded = null);
        T GetRandomItem<T>(IEnumerable<T> collection, IEnumerable<T> excluded = null);
        double GetRandomDouble();
        Size GetRandomRoomSize(int minWidth, int maxWidth, int minHeight, int maxHeight);

    }
}
