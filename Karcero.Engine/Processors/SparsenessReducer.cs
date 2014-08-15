using System.Collections.Generic;
using System.Linq;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Processors
{
    internal class SparsenessReducer<T> : IMapPreProcessor<T> where T : class, IBinaryCell, new()
    {
        public void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var cellsToRemove = (int) (map.Width*map.Height*configuration.Sparseness);
            while (cellsToRemove != 0)
            {
                //Look at every cell in the maze grid. If the given cell contains a corridor that exits the cell in only one direction 
                //"erase" that cell by removing the corridor
                var changedCells = new HashSet<T>();
                
                var deadEndCells = map.AllCells.Where(cell => cell.Sides.Values.Count(side => side) == 1).ToList();
                foreach (var deadEndCell in deadEndCells)
                {
                    deadEndCell.IsOpen = false;
                    var openDirection = deadEndCell.Sides.First(pair => pair.Value).Key;
                    deadEndCell.Sides[openDirection] = false;
                    var oppositeCell = map.GetAdjacentCell(deadEndCell, openDirection);
                    oppositeCell.Sides[openDirection.Opposite()] = false;
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
