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
        private readonly IEnumerable<IMapPreProcessor<BinaryCell>> mPreProcessors;
        private readonly IEnumerable<IMapProcessor<T>> mPostProcessors;
        private readonly IMapConverter<T, BinaryCell> mMapConverter;

        public DungeonGenerator()
        {
            mPreProcessors = new List<IMapPreProcessor<BinaryCell>>()
            {
                new MazeGenerator<BinaryCell>(),
                new SparsenessReducer<BinaryCell>(),
                new DeadendsRemover<BinaryCell>()
              };

            mMapConverter = new MapDoubler<T, BinaryCell>();

            mPostProcessors = new List<IMapProcessor<T>>()
            {
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
            
            var map = new Map<BinaryCell>(config.Width, config.Height);
            foreach (var preProcessor in mPreProcessors)
            {
                DateTime start = DateTime.Now;
               preProcessor.ProcessMap(map, config, randomizer);
                //Console.WriteLine("{0} took {1} ms", mapProcessor.GetType().Name, DateTime.Now.Subtract(start).TotalMilliseconds);
            }

            var postMap = mMapConverter.ConvertMap(map, config, randomizer);

            foreach (var postProcessor in mPostProcessors)
            {
                postProcessor.ProcessMap(postMap, config, randomizer);
            }

            return postMap;
        }
    }
}
