using System.Collections.Generic;
using DunGen.Engine.Models;

namespace DunGen.Engine.Contracts
{
    public interface IMapProcessor
    {
        void ProcessMap(Map map, DungeonConfiguration configuration);

        event MapChangedDelegate MapChanged;

        string ActionString { get; }
    }

    public delegate void MapChangedDelegate(IMapProcessor sender, MapChangedDelegateArgs args);

    public class MapChangedDelegateArgs
    {
        public Map Map { get; set; }
        public IEnumerable<Cell> CellsChanged { get; set; }
    }
}
