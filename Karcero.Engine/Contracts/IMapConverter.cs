using Karcero.Engine.Models;

namespace Karcero.Engine.Contracts
{
    internal interface IMapConverter<TPost,TPre> where TPre : class, IBinaryCell, new() where TPost : class,  ICell, new()
    {
        Map<TPost> ConvertMap(Map<TPre> map, DungeonConfiguration configuration, IRandomizer randomizer);
 
    }
}
