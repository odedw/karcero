using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;

namespace Karcero.Engine.Implementations
{
    public class Randomizer : IRandomizer
    {
        private Random mRandom = new Random();

        public void SetSeed(int seed)
        {
            mRandom = new Random(seed);
        }

        public T GetRandomCell<T>(Map<T> map) where T : class, IBaseCell, new()
        {
            if (map.Height == 0 || map.Width == 0) return null;
            return map.GetCell(mRandom.Next(map.Height), mRandom.Next(map.Width));
        }

        public TItem GetRandomEnumValue<TItem>(IEnumerable<TItem> excluded = null)
        {
            excluded = excluded ?? new List<TItem>();
            var values = GetAll.ValuesOf<TItem>().Except(excluded).ToList();
            return values[mRandom.Next(values.Count)];
        }

        public TItem GetRandomItem<TItem>(IEnumerable<TItem> collection, IEnumerable<TItem> excluded = null) 
        {
            excluded = excluded ?? new List<TItem>();
            var cleanCollection = collection.Except(excluded).ToList();
            return !cleanCollection.Any() ? default(TItem) : cleanCollection[(mRandom.Next(cleanCollection.Count))];
        }

        public double GetRandomDouble()
        {
            return mRandom.NextDouble();
        }

        public Size GetRandomRoomSize(int maxWidth, int minWidth, int maxHeight, int minHeight)
        {
            return new Size(mRandom.Next(maxWidth - minWidth) + minWidth + 1, mRandom.Next(maxHeight - minHeight) + minHeight + 1);
        }
    }
}
