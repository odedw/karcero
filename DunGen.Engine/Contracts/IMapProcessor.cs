using System.Collections.Generic;
using DunGen.Engine.Models;

namespace DunGen.Engine.Contracts
{
    public interface IMapProcessor<T> where T : class, ICell, new()
    {
        void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer);
    }
}
