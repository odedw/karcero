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
        public event MapChangedDelegate MapChanged;
        public string ActionString { get { return "Reducing sparseness"; } }

        public void ProcessMap(Map map, DungeonConfiguration configuration)
        {
            for (int i = 0; i < configuration.Sparseness; i++)
            {
                //Look at every cell in the maze grid. If the given cell contains a corridor that exits the cell in only one direction 
                //"erase" that cell by removing the corridor
                var changedCells = new HashSet<Cell>();
                
                var deadEndCells = map.AllCells.Where(cell => cell.Sides.Values.Count(side => side == SideType.Open) == 1).ToList();
                foreach (var deadEndCell in deadEndCells)
                {
                    deadEndCell.TileType = TileType.Rock;
                    var openDirection = deadEndCell.Sides.First(pair => pair.Value == SideType.Open).Key;
                    deadEndCell.Sides[openDirection] = SideType.Wall;
                    var oppositeCell = map.GetAdjacentCell(deadEndCell, openDirection);
                    oppositeCell.Sides[openDirection.Opposite()] = SideType.Wall;
                    changedCells.Add(deadEndCell);
                    changedCells.Add(oppositeCell);
                }

                if (MapChanged != null)
                {
                    MapChanged(this, new MapChangedDelegateArgs(){CellsChanged = changedCells, Map = map});
                }

                //Repeat step #1 sparseness times
            }
        }
    }
}
