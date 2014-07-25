using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class MapDoubler : IMapProcessor
    {
        public void ProcessMap(Map map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var oldCells = map.AllCells.Select(cell => cell.Clone()).ToList();
            map.Width = configuration.Width*2 + 1;
            map.Height = configuration.Height*2 + 1;
            map.Init();
            if (MapChanged != null) MapChanged(this, new MapChangedDelegateArgs(){Map = map, CellsChanged = map.AllCells});

            foreach (var oldCell in oldCells.Where(cell => cell.Terrain == TerrainType.Floor))
            {
                var newCell = map.GetCell(oldCell.Row*2 + 1, oldCell.Column*2 + 1);
                newCell.Terrain = TerrainType.Floor;
                newCell.Sides = oldCell.Sides;
                var cellsChanged = new HashSet<Cell>(){newCell};
                foreach (var kvp in newCell.Sides.Where(pair => pair.Value == SideType.Open))
                {
                    var adjacentCell = map.GetAdjacentCell(newCell, kvp.Key);
                    adjacentCell.Terrain = TerrainType.Floor;
                    adjacentCell.Sides[kvp.Key.Opposite()] = SideType.Open;
                    cellsChanged.Add(adjacentCell);
                }


                if (MapChanged != null) MapChanged(this, new MapChangedDelegateArgs() { Map = map, CellsChanged = cellsChanged });
            }
        }

        public event MapChangedDelegate MapChanged;
    }
}
