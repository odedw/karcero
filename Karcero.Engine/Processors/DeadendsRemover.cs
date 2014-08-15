using System.Collections.Generic;
using System.Linq;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;

namespace Karcero.Engine.Processors
{
    internal class DeadendsRemover<T> : IMapPreProcessor<T> where T : class, IBinaryCell, new()
    {
        public void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            var deadends = map.AllCells.Where(cell => cell.Sides.Values.Count(type => type) == 1).ToList();
            foreach (var cell in deadends)
            {
                if (randomizer.GetRandomDouble() > configuration.ChanceToRemoveDeadends) continue;

                var currentCell = cell;
                var previousCell = map.GetAdjacentCell(cell, cell.Sides.First(pair => pair.Value).Key);
                var connected = false;
                while (!connected)
                {
                    var direction = GetRandomValidDirection(map, currentCell, previousCell, randomizer);
                    if (!direction.HasValue) break;

                    var adjacentCell = map.GetAdjacentCell(currentCell, direction.Value);
                    connected = adjacentCell.IsOpen;
                    adjacentCell.IsOpen = true;
                    currentCell.Sides[direction.Value] = adjacentCell.Sides[direction.Value.Opposite()] = true;
                    previousCell = currentCell;
                    currentCell = adjacentCell;
                }
            }
        }


        private Direction? GetRandomValidDirection(Map<T> map, T currentCell, T previousCell, IRandomizer randomizer)
        {
            var invalidDirections = new List<Direction>();
            var squareDirections = new List<Direction>();
            while (invalidDirections.Count + squareDirections.Count < GetAll.ValuesOf<Direction>().Count())
            {
                var direction = randomizer.GetRandomEnumValue(invalidDirections.Union(squareDirections));
                if (IsDirectionValid(map, currentCell, direction, previousCell))
                {
                    var nextCell = map.GetAdjacentCell(currentCell, direction);

                    //Try to avoid creating squares, but do it if there's no other way
                    if (nextCell.IsOpen &&
                        ((nextCell.Sides[direction.Rotate()] &&
                          currentCell.Sides[direction.Rotate()]) ||
                         (nextCell.Sides[direction.Rotate(false)] &&
                          currentCell.Sides[direction.Rotate(false)])))
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

        private bool IsDirectionValid(Map<T> map, T cell, Direction direction, T previousCell)
        {
            T adjacent;
            return map.TryGetAdjacentCell(cell, direction, out adjacent) && adjacent != previousCell;
        }       
    }
}
