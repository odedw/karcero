using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class DeadendsRemover : IMapProcessor
    {
        private readonly IRandomizer mRandomizer;
        public event MapChangedDelegate MapChanged;
        public string ActionString { get { return "Removing dead ends"; } }

        public DeadendsRemover(IRandomizer randomizer)
        {
            mRandomizer = randomizer;
        }

        public void ProcessMap(Map map, DungeonConfiguration configuration)
        {
            var deadends = map.AllCells.Where(cell => cell.Sides.Values.Count(type => type == SideType.Open) == 1).ToList();
            foreach (var cell in deadends)
            {
                if (mRandomizer.GetRandomDouble() > configuration.ChanceToRemoveDeadends) continue;

                var currentCell = cell;
                var previousCell = map.GetAdjacentCell(cell, cell.Sides.First(pair => pair.Value == SideType.Open).Key);
                var connected = false;
                while (!connected)
                {
                    var direction = GetRandomValidDirection(map, currentCell, previousCell);
                    if (!direction.HasValue) break;

                    var adjacentCell = map.GetAdjacentCell(currentCell, direction.Value);
                    connected = adjacentCell.Terrain == TerrainType.Floor;
                    adjacentCell.Terrain = TerrainType.Floor;
                    currentCell.Sides[direction.Value] = adjacentCell.Sides[direction.Value.Opposite()] = SideType.Open;
                    if (MapChanged != null)
                    {
                        MapChanged(this, new MapChangedDelegateArgs(){Map = map, CellsChanged = new List<Cell>(){currentCell, adjacentCell}});
                    }
                    previousCell = currentCell;
                    currentCell = adjacentCell;
                }
            }
        }


        private Direction? GetRandomValidDirection(Map map, Cell currentCell, Cell previousCell)
        {
            var invalidDirections = new List<Direction>();
            while (invalidDirections.Count < Enum.GetValues(typeof(Direction)).Length)
            {
                var direction = mRandomizer.GetRandomEnumValue(invalidDirections);
                if (IsDirectionValid(map, currentCell, direction, previousCell))
                {
                    var nextCell = map.GetAdjacentCell(currentCell, direction);
                    var willCreateSquare = nextCell.Terrain == TerrainType.Floor && 
                        ((nextCell.Sides[direction.Rotate()] == SideType.Open && currentCell.Sides[direction.Rotate()] == SideType.Open) ||
                        (nextCell.Sides[direction.Rotate(false)] == SideType.Open && currentCell.Sides[direction.Rotate(false)] == SideType.Open));
                    if (!willCreateSquare) return direction;
                }
                invalidDirections.Add(direction);
            }
            return null;
        }

        private bool IsDirectionValid(Map map, Cell cell, Direction direction, Cell previousCell)
        {
            return map.GetAdjacentCell(cell, direction) != null && map.GetAdjacentCell(cell, direction) != previousCell;
        }       
    }
}
