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

        public DunGenerator()
        {
            mMapProcessors = new List<IMapProcessor>()
            {
                new MazeGenerator(),
                new SparsenessReducer(),
                new DeadendsRemover(),
                new MapDoubler(),
                new RoomGenerator(),
                new DoorGenerator()
            };
        }

        public Map Generate(DungeonConfiguration config, int? seed = null)
        {
            var randomizer = new Randomizer();
            if (!seed.HasValue) seed = Guid.NewGuid().GetHashCode();
            Console.WriteLine(seed);
            randomizer.SetSeed(seed.Value);
            var map = new Map(config.Width, config.Height);
            foreach (var mapProcessor in mMapProcessors)
            {
                DateTime start = DateTime.Now;
               mapProcessor.ProcessMap(map, config, randomizer);
                //Console.WriteLine("{0} took {1} ms", mapProcessor.GetType().Name, DateTime.Now.Subtract(start).TotalMilliseconds);
            }
            return map;
        }
    }
}
