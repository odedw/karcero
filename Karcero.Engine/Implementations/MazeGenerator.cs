using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Implementations
{
    internal class MazeGenerator<T> : IMapPreProcessor<T> where T : class, IBinaryCell, new()
    {
        public void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            //Start with a rectangular grid, x units wide and y units tall. Mark each cell in the grid unvisited
            var visitedCells = new HashSet<T>();
            var deadEndCells = new HashSet<T>();
            Direction? previousDirection = null;

            //Pick a random cell in the grid and mark it visited. This is the current cell. 
            var currentCell = randomizer.GetRandomCell(map);
            currentCell.IsOpen = true;
            while (visitedCells.Count < map.Width*map.Height)
            {
                var oldCell = currentCell;
                var changed = false;
                visitedCells.Add(currentCell);

                //From the current cell, pick a random direction (north, south, east, or west). 
                //If (1) there is no cell adjacent to the current cell in that direction, or (2) if 
                //the adjacent cell in that direction has been visited, then that direction 
                //is invalid, and you must pick a different random direction. 
                var direction = GetRandomValidDirection(map, currentCell, visitedCells, configuration.Randomness, previousDirection, randomizer);
                if (direction.HasValue)
                {
                    //Let's call the cell in the chosen direction C. Create a corridor between the 
                    //current cell and C, and then make C the current cell. Mark C visited.
                    changed = !currentCell.Sides[direction.Value];
                    currentCell = map.GetAdjacentCell(currentCell, direction.Value);
                    currentCell.Sides[direction.Value.Opposite()] = oldCell.Sides[direction.Value] = true;
                    previousDirection = direction;
                }
                else
                {
                    //If all directions are invalid, pick a different random visited cell in the grid and start this step over again.
                    deadEndCells.Add(currentCell);
                    currentCell = randomizer.GetRandomItem(visitedCells, deadEndCells);
                }
                if (currentCell.IsOpen && !changed) continue;

                currentCell.IsOpen = true;
                //Repeat until all cells in the grid have been visited.
            }
        }

        private Direction? GetRandomValidDirection(Map<T> map, T cell, ICollection<T> visitedCells, double randomness, Direction? previousDirection, IRandomizer randomizer)
        {
            //Randomness determines how often the direction of a corridor changes
            if (previousDirection.HasValue &&
                randomness < 1 &&
                randomizer.GetRandomDouble() > randomness &&
                IsDirectionValid(map, cell, previousDirection.Value, visitedCells))
            {
                return previousDirection;
            }

            var invalidDirections = new List<Direction>();
            while (invalidDirections.Count < Enum.GetValues(typeof (Direction)).Length)
            {
                var direction = randomizer.GetRandomEnumValue(invalidDirections);
                if (IsDirectionValid(map, cell, direction, visitedCells))
                    return direction;
                invalidDirections.Add(direction);
            }
            return null;
        }

        private bool IsDirectionValid(Map<T> map, T cell, Direction direction, ICollection<T> visitedCells)
        {
            T adjacentCell;
            return map.TryGetAdjacentCell(cell, direction, out adjacentCell) && !visitedCells.Contains(adjacentCell);

        }
    }
}
