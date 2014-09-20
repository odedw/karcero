using System.Collections.Generic;
using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    public interface IRandomizer
    {
        void SetSeed(int seed);
        T GetRandomCell<T>(Map<T> map) where T : class, IBaseCell, new();
        TItem GetRandomEnumValue<TItem>(IEnumerable<TItem> excluded = null);
        TItem GetRandomItem<TItem>(IEnumerable<TItem> collection);
        double GetRandomDouble();
        Size GetRandomRoomSize(int minWidth, int maxWidth, int minHeight, int maxHeight);

    }
}
