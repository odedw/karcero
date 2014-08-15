using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    public interface IMapProcessor<T> where T : class, ICell, new()
    {
        void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer);
    }
}
