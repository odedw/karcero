using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine.Contracts;
using DunGen.Engine.Models;

namespace DunGen.Engine.Implementations
{
    public class RoomGenerator : IMapProcessor
    {
        private readonly IRandomizer mRandomizer;

        public RoomGenerator(IRandomizer randomizer)
        {
            mRandomizer = randomizer;
        }

        public void ProcessMap(Map map, DungeonConfiguration configuration)
        {
            for (var i = 0; i < configuration.RoomCount; i++)
            {
                //Set the "best" score to infinity (or some arbitrarily huge number)
                var bestScore = int.MaxValue;
                var bestLocation = Point.Empty;

                //Generate a room such that Wmin <= Rw <= Wmax and Hmin <= Rh <= Hmax. 
                var size = mRandomizer.GetRandomRoomSize(configuration.MaxRoomWidth, configuration.MinRoomWidth,
                    configuration.MaxRoomHeight, configuration.MinRoomHeight);

                //For each cell C in the dungeon, do the following:
                foreach (var cell in map.AllCells)
                {
                    //Set the "current" score to 0
                    int currentScore = 0;

                    var rectangle = new Rectangle(cell.Location, size);
                    if (rectangle.Right >= map.Width || rectangle.Bottom >= map.Height) continue;

                    var cells = map.GetRectangleOfCells(rectangle).ToList();

                    //For each cell of the room that is adjacent to a corridor, add 1 to the current score.                     currentScore += map.GetCellsAdjacentToRectangle(rectangle).Count(c => c.Terrain == TerrainType.Floor);

                    //For each cell of the room that overlaps a corridor, add 3 to the current score.
                    currentScore += cells.Count(c => !map.IsCellLocationInRoom(c.Location.X, c.Location.Y) && c.Terrain == TerrainType.Floor) * 3;

                    //For each cell of the room that overlaps a room, add 100 to the current score.
                    currentScore += cells.Count(c => map.IsCellLocationInRoom(c.Location.X, c.Location.Y)) * 100;

                    if (currentScore < bestScore)
                    {
                        bestScore = currentScore;
                        bestLocation = cell.Location;
                    }
                }

                //Place the room at the best position (where the best score was found). 
                if (bestLocation != Point.Empty)
                {
                    var roomRect = new Rectangle(bestLocation, size);
                    map.AddRoomAtRectangle(roomRect);
                    if (MapChanged != null)
                    {
                        MapChanged(this, new MapChangedDelegateArgs(){Map = map, CellsChanged = map.GetRectangleOfCells(roomRect)});
                    }
                }

            }
        }

        

        public event MapChangedDelegate MapChanged;
        public string ActionString { get; private set; }
    }
}
