using System.Collections.Generic;
using DunGen.Engine.Models;

namespace DunGen.Engine.Contracts
{
    public interface IMapProcessor
    {
        void ProcessMap(Map map, DungeonConfiguration configuration, IRandomizer randomizer);
    }
}
