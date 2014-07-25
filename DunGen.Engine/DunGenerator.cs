using System;
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

        public event MapChangedDelegate MapChanged;
        public DunGenerator()
        {
            mMapProcessors = new List<IMapProcessor>()
            {
                new MazeGenerator(),
                new SparsenessReducer(),
                new DeadendsRemover(),
                new MapDoubler(),
                new RoomGenerator()
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

        public Map Generate(DungeonConfiguration config, int? seed = null)
        {
            var randomizer = new Randomizer();
            if (!seed.HasValue) seed = Guid.NewGuid().GetHashCode();
            Console.WriteLine(seed);
            randomizer.SetSeed(seed.Value);
            var map = new Map(config.Width, config.Height);
            TriggerMapChanged(null, new MapChangedDelegateArgs(){Map = map,CellsChanged = map.AllCells});
            foreach (var mapProcessor in mMapProcessors)
            {
               mapProcessor.ProcessMap(map, config, randomizer);
            }
            return map;
        }
    }
}
