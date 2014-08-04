using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class DeadendsRemover : IMapProcessor
    {
        public event MapChangedDelegate MapChanged;

        public void ProcessMap(Map map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var deadends = map.AllCells.Where(cell => cell.Sides.Values.Count(type => type == SideType.Open) == 1).ToList();
            foreach (var cell in deadends)
            {
                if (randomizer.GetRandomDouble() > configuration.ChanceToRemoveDeadends) continue;

                var currentCell = cell;
                var previousCell = map.GetAdjacentCell(cell, cell.Sides.First(pair => pair.Value == SideType.Open).Key);
                var connected = false;
                while (!connected)
                {
                    var direction = GetRandomValidDirection(map, currentCell, previousCell, randomizer);
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


        private Direction? GetRandomValidDirection(Map map, Cell currentCell, Cell previousCell, IRandomizer randomizer)
        {
            var invalidDirections = new List<Direction>();
            var squareDirections = new List<Direction>();
            while (invalidDirections.Count + squareDirections.Count < Enum.GetValues(typeof(Direction)).Length)
            {
                var direction = randomizer.GetRandomEnumValue(invalidDirections.Union(squareDirections));
                if (IsDirectionValid(map, currentCell, direction, previousCell))
                {
                    var nextCell = map.GetAdjacentCell(currentCell, direction);

                    //Try to avoid creating squares, but do it if there's no other way
                    if (nextCell.Terrain == TerrainType.Floor &&
                        ((nextCell.Sides[direction.Rotate()] == SideType.Open &&
                          currentCell.Sides[direction.Rotate()] == SideType.Open) ||
                         (nextCell.Sides[direction.Rotate(false)] == SideType.Open &&
                          currentCell.Sides[direction.Rotate(false)] == SideType.Open)))
                    {
                        squareDirections.Add(direction);
                    }
                    else
                    {
                        return direction;
                    }
                }
                else
                {
                    invalidDirections.Add(direction);
                }
            }
            return squareDirections.Any() ? randomizer.GetRandomItem(squareDirections) : (Direction?)null;
        }

        private bool IsDirectionValid(Map map, Cell cell, Direction direction, Cell previousCell)
        {
            Cell adjacent;
            return map.TryGetAdjacentCell(cell, direction, out adjacent) && adjacent != previousCell;
        }       
    }
}
