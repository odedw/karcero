using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Implementations
{
    public class DoorGenerator<T> : IMapProcessor<T> where T : class, ICell, new()
    {
        public void ProcessMap(Map<T> map, DungeonConfiguration configuration, IRandomizer randomizer)
        {
            foreach (var room in map.Rooms)
            {
                //need to tunnel to a nearby area
                if (map.GetCellsAdjacentToRoom(room).All(cell => cell.Terrain == TerrainType.Rock))
                {
                    List<T> adjacentCells = null;
                    var distance = 2;
                    do
                    {
                        adjacentCells = map.GetCellsAdjacentToRoom(room, distance).ToList();
                        if (adjacentCells.Any(cell => cell.Terrain != TerrainType.Rock))
                        {
                            var targetCell = randomizer.GetRandomItem(adjacentCells.Where(cell => cell.Terrain != TerrainType.Rock));
                            var targetDirection = Direction.East;
                            if (targetCell.Row < room.Row) targetDirection = Direction.South;
                            else if (targetCell.Column >= room.Right) targetDirection = Direction.West;
                            else if (targetCell.Row >= room.Bottom) targetDirection = Direction.North;
                            do
                            {
                                var nextCell = map.GetAdjacentCell(targetCell, targetDirection);
                                nextCell.Sides[targetDirection.Opposite()] = targetCell.Sides[targetDirection] = SideType.Open;
                                nextCell.Terrain = TerrainType.Floor;

                                targetCell = nextCell;
                            } while (!room.IsLocationInRoom(targetCell.Row, targetCell.Column));

                            break;
                        }
                        distance++;
                    } while (adjacentCells.Any());
                }

                //place doors
                foreach (var cell in map.GetCellsAdjacentToRoom(room)
                    .Where(cell => cell.Terrain == TerrainType.Floor && map.GetAllAdjacentCells(cell).All(c => c.Terrain != TerrainType.Door)))
                {
                    //don't place a door if it leads to nowhere
                    if (cell.Sides.Any(pair => pair.Value != cell.Sides[pair.Key.Opposite()])) continue;

                    cell.Terrain = TerrainType.Door;
                }
            }
        }
    }
}
