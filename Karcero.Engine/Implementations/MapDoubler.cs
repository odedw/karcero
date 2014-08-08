using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Implementations
{
    public class MapDoubler<T> : IMapProcessor<T> where T : class, ICell, new()
    {
        public void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var oldCells = map.AllCells.Select(cell => cell.Clone()).ToList();
            map.Width = configuration.Width*2 + 1;
            map.Height = configuration.Height*2 + 1;
            map.Init();

            foreach (var oldCell in oldCells.Where(cell => cell.Terrain == TerrainType.Floor))
            {
                var newCell = map.GetCell(oldCell.Row*2 + 1, oldCell.Column*2 + 1);
                newCell.Terrain = TerrainType.Floor;
                newCell.Sides = oldCell.Sides;
                var cellsChanged = new HashSet<T>(){newCell};
                foreach (var kvp in newCell.Sides.Where(pair => pair.Value == SideType.Open))
                {
                    var adjacentCell = map.GetAdjacentCell(newCell, kvp.Key);
                    adjacentCell.Terrain = TerrainType.Floor;
                    adjacentCell.Sides[kvp.Key.Opposite()] = SideType.Open;
                    cellsChanged.Add(adjacentCell);
                }
            }
        }
    }
}
