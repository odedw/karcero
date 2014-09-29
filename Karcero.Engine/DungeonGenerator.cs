using System;
using System.Collections.Generic;
using System.Threading;
using Karcero.Engine.Contracts;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;

namespace Karcero.Engine
{
    /// <summary>
    /// Generates a map of cells of type T.
    /// </summary>
    /// <typeparam name="T">The type of cells the map is comprised of.</typeparam>
    public class DungeonGenerator<T> where T : class, ICell, new()
    {
        #region Properties
        private readonly IEnumerable<IMapPreProcessor<BinaryCell>> mPreProcessors;
        private readonly List<IMapProcessor<T>> mPostProcessors;
        private readonly IMapConverter<T, BinaryCell> mMapConverter;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
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

        /// <summary>
        /// Add a map processor that will be called at the end of the map generation library to further modify the map.
        /// </summary>
        /// <param name="mapProcessor">The processor to add.</param>
        public void AddMapProcessor(IMapProcessor<T> mapProcessor)
        {
            mPostProcessors.Add(mapProcessor);
        }

        /// <summary>
        /// Creates an instance of the DungeonConfigurationGenerator that is linked to this instance.
        /// </summary>
        /// <returns>An instance of DungeonConfigurationGenerator linked to this instance of DungeonGenerator.</returns>
        public DungeonConfigurationGenerator<T> GenerateA()
        {
            return new DungeonConfigurationGenerator<T>(this);
        }

        /// <summary>
        /// Generate a map according the configuration received.
        /// </summary>
        /// <param name="config">The configuration used to generate the map.</param>
        /// <param name="seed">A seed to be used for the generation. If null a random seed will be generated.</param>
        /// <returns>The generated map.</returns>
        public virtual Map<T> Generate(DungeonConfiguration config, int? seed = null)
        {
            var randomizer = new Randomizer();
            if (!seed.HasValue) seed = Guid.NewGuid().GetHashCode();
            randomizer.SetSeed(seed.Value);
            var halfHeight = config.Height / 2;
            var halfWidth = config.Width / 2;
            var map = new Map<BinaryCell>(halfWidth, halfHeight);

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

        /// <summary>
        /// Generates a map on a different thread.
        /// </summary>
        /// <param name="callback">Will be called when the generation is complete (on a different thread).</param>
        /// <param name="config">The configuration used to generate the map.</param>
        /// <param name="seed">A seed to be used for the generation. If null a random seed will be generated.</param>
        public void BeginGenerate(Action<Map<T>> callback, DungeonConfiguration config, int? seed = null)
        {
            new Thread(() => {
                var map = Generate(config, seed);
                callback(map);
            }).Start();
        }

        internal Tuple<Map<T>, Dictionary<string, double>> GenerateAndMeasure(DungeonConfiguration config)
        {

            var randomizer = new Randomizer();
            var seed = Guid.NewGuid().GetHashCode();
            randomizer.SetSeed(seed);
            var halfHeight = config.Height / 2;
            var halfWidth = config.Width / 2;
            var map = new Map<BinaryCell>(halfWidth, halfHeight);
            var results = new Dictionary<string, double>();
            DateTime start = DateTime.Now;
            var totalStart = DateTime.Now;

            //pre processing
            foreach (var preProcessor in mPreProcessors)
            {
                start = DateTime.Now;
                preProcessor.ProcessMap(map, config, randomizer);
                results[preProcessor.GetType().Name] = DateTime.Now.Subtract(start).TotalSeconds;
            }

            //double map
            start = DateTime.Now;
            var postMap = mMapConverter.ConvertMap(map, config, randomizer);
            results[mMapConverter.GetType().Name] = DateTime.Now.Subtract(start).TotalSeconds;

            //post processing
            foreach (var postProcessor in mPostProcessors)
            {
                start = DateTime.Now;
                postProcessor.ProcessMap(postMap, config, randomizer);
                results[postProcessor.GetType().Name] = DateTime.Now.Subtract(start).TotalSeconds;
            }
            results["Total"] = DateTime.Now.Subtract(totalStart).TotalSeconds;
            return new Tuple<Map<T>, Dictionary<string, double>>(postMap, results);
        }
        #endregion
    }
}
