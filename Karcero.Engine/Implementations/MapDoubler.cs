using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Implementations
{
    internal class MapDoubler<TPost, TPre> : IMapConverter<TPost, TPre>
        where TPre : class, IBinaryCell, new()
        where TPost : class,  ICell, new()
    {
        public Map<TPost> ConvertMap(Map<TPre> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var oldCells = map.AllCells.ToList();
            var newMap = new Map<TPost>(configuration.Width * 2 + 1, configuration.Height * 2 + 1);

            foreach (var oldCell in oldCells.Where(cell => cell.IsOpen))
            {
                var newCell = newMap.GetCell(oldCell.Row * 2 + 1, oldCell.Column * 2 + 1);
                newCell.Terrain = TerrainType.Floor;
                foreach (var kvp in oldCell.Sides.Where(pair => pair.Value))
                {
                    var adjacentCell = newMap.GetAdjacentCell(newCell, kvp.Key);
                    adjacentCell.Terrain = TerrainType.Floor;
                    adjacentCell.Sides[kvp.Key.Opposite()] = SideType.Open;
                }
            }
            return newMap;
        }
    }
}
