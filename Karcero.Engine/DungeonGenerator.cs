using System;
using System.Collections.Generic;
using System.Threading;
using Karcero.Engine.Contracts;
using Karcero.Engine.Implementations;
using Karcero.Engine.Models;

namespace Karcero.Engine
{
    public class DungeonGenerator<T> where T : class, ICell, new()
    {
        private readonly IEnumerable<IMapProcessor<T>> mMapProcessors;

        public DungeonGenerator()
        {
            mMapProcessors = new List<IMapProcessor<T>>()
            {
                new MazeGenerator<T>(),
                new SparsenessReducer<T>(),
                new DeadendsRemover<T>(),
                new MapDoubler<T>(),
                new RoomGenerator<T>(),
                new DoorGenerator<T>()
            };
        }

        public Map<T> Generate(DungeonConfiguration config, int? seed = null)
        {
            var randomizer = new Randomizer();
            if (!seed.HasValue) seed = Guid.NewGuid().GetHashCode();
            Console.WriteLine(seed);
            randomizer.SetSeed(seed.Value);
            var map = new Map<T>(config.Width, config.Height);
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
