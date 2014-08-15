using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;

namespace Karcero.Engine
{
    public class DungeonGenerator<T> where T : class, ICell, new()
    {
        #region Properties
        private readonly IEnumerable<IMapPreProcessor<BinaryCell>> mPreProcessors;
        private readonly IEnumerable<IMapProcessor<T>> mPostProcessors;
        private readonly IMapConverter<T, BinaryCell> mMapConverter;
        #endregion

        #region Constructors
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
        #endregion

        #region Methods

        public DungeonConfigurationGenerator<T> GenerateA()
        {
            return new DungeonConfigurationGenerator<T>(this);
        } 

        public virtual Map<T> Generate(DungeonConfiguration config, int? seed = null)
        {
            var randomizer = new Randomizer();
            if (!seed.HasValue) seed = Guid.NewGuid().GetHashCode();
            Console.WriteLine(seed);
            randomizer.SetSeed(seed.Value);
            var map = new Map<BinaryCell>(config.Width, config.Height);

            //pre processing
            foreach (var preProcessor in mPreProcessors)
            {
               preProcessor.ProcessMap(map, config, randomizer);
            }

            //double map
            var postMap = mMapConverter.ConvertMap(map, config, randomizer);

            //post processing
            foreach (var postProcessor in mPostProcessors)
            {
                postProcessor.ProcessMap(postMap, config, randomizer);
            }

            return postMap;
        }

        public void BeginGenerate(Action<Map<T>> callback, DungeonConfiguration config, int? seed = null)
        {
            Task.Run(() =>
            {
                var map = Generate(config, seed);
                callback(map);
            });
        }
        #endregion
    }
}
