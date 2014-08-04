using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class SparsenessReducer : IMapProcessor
    {
        public void ProcessMap(Map map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var cellsToRemove = (int) (map.Width*map.Height*configuration.Sparseness);
            while (cellsToRemove != 0)
            {
                //Look at every cell in the maze grid. If the given cell contains a corridor that exits the cell in only one direction 
                //"erase" that cell by removing the corridor
                var changedCells = new HashSet<Cell>();
                
                var deadEndCells = map.AllCells.Where(cell => cell.Sides.Values.Count(side => side == SideType.Open) == 1).ToList();
                foreach (var deadEndCell in deadEndCells)
                {
                    deadEndCell.Terrain = TerrainType.Rock;
                    var openDirection = deadEndCell.Sides.First(pair => pair.Value == SideType.Open).Key;
                    deadEndCell.Sides[openDirection] = SideType.Wall;
                    var oppositeCell = map.GetAdjacentCell(deadEndCell, openDirection);
                    oppositeCell.Sides[openDirection.Opposite()] = SideType.Wall;
                    changedCells.Add(deadEndCell);
                    changedCells.Add(oppositeCell);
                    cellsToRemove--;
                    if (cellsToRemove == 0) break;
                }
                //Repeat step #1 sparseness times
            }
        }
    }
}
