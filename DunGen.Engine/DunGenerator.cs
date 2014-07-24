using System.Collections.Generic;
using System.Threading;
using DunGen.Engine.Contracts;
using DunGen.Engine.Implementations;
using DunGen.Engine.Models;

namespace DunGen.Engine
{
    public class DunGenerator
    {
        private readonly IEnumerable<IMapProcessor> mMapProcessors;
        private readonly IRandomizer mRandomizer = new Randomizer();

        public event MapChangedDelegate MapChanged;
        public DunGenerator()
        {
            mMapProcessors = new List<IMapProcessor>()
            {
                new MazeGenerator(mRandomizer),
                new SparsenessReducer(mRandomizer),
                new DeadendsRemover(mRandomizer)
                //new RoomGenerator(mRandomizer)
            };
            foreach (var mapProcessor in mMapProcessors)
            {
                mapProcessor.MapChanged += TriggerMapChanged;
            }
        }

        private void TriggerMapChanged(IMapProcessor sender, MapChangedDelegateArgs args)
        {
            if (MapChanged != null)
                MapChanged(sender, args);
        }

        public Map Generate(DungeonConfiguration config)
        {
            var map = new Map(config.Width, config.Height);
            TriggerMapChanged(null, new MapChangedDelegateArgs(){Map = map,CellsChanged = map.AllCells});
            foreach (var mapProcessor in mMapProcessors)
            {
                mapProcessor.ProcessMap(map, config);
            }
            return map;
        }
    }
}
