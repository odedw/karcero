using System;
using System.Diagnostics;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Helpers
{
    /// <summary>
    /// Generates a configuration with a set of fluent API methods.
    /// </summary>
    /// <typeparam name="T">The type of cell the map will be comprised of.</typeparam>
    public class DungeonConfigurationGenerator<T> where T : class, ICell, new()
    {
        #region Properties

        private readonly DungeonConfiguration mConfiguration = new DungeonConfiguration();
        private readonly DungeonGenerator<T> mGenerator;
        private int? mSeed;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance that is linked to the generator passed.
        /// </summary>
        /// <param name="dungeonGenerator">The generator this instance is linked to.</param>
        public DungeonConfigurationGenerator(DungeonGenerator<T> dungeonGenerator)
        {
            mGenerator = dungeonGenerator;
        }
        #endregion

        #region Methods

        #region Dungeon Size

        internal const int TINY_SIZE = 17;
        internal const int SMALL_SIZE = 25;
        internal const int MEDIUM_SIZE = 33;
        internal const int LARGE_SIZE = 45;
        internal const int HUGE_SIZE = 100;
        /// <summary>
        /// Sets the size to specific measurements.
        /// </summary>
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> DungeonOfSize(int width, int height)
        {
            mConfiguration.Width = width;
            mConfiguration.Height = height;
            return this;
        }

        /// <summary>
        /// Sets the desired size to 17x17.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> TinyDungeon()
        {
            return DungeonOfSize(TINY_SIZE, TINY_SIZE);
        }
        /// <summary>
        /// Sets the desired size to 25x25.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> SmallDungeon()
        {
            return DungeonOfSize(SMALL_SIZE, SMALL_SIZE);
        }
        /// <summary>
        /// Sets the desired size to 33x33.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> MediumDungeon()
        {
            return DungeonOfSize(MEDIUM_SIZE, MEDIUM_SIZE);
        }
        /// <summary>
        /// Sets the desired size to 45x45.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> LargeDungeon()
        {
            return DungeonOfSize(LARGE_SIZE, LARGE_SIZE);
        }
        /// <summary>
        /// Sets the desired size to 100x100.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> HugeDungeon()
        {
            return DungeonOfSize(HUGE_SIZE, HUGE_SIZE);
        }
        #endregion

        #region Randomness

        internal const double NOT_RANDOM = 0;
        internal const double A_BIT_RANDOM = 0.3;
        internal const double SOMEWHAT_RANDOM = 0.6;
        internal const double VERY_RANDOM = 1;

        /// <summary>
        /// Sets randomness to a specific value.
        /// </summary>
        /// <param name="randomness">The desired randomness value.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithRandomness(double randomness)
        {
            mConfiguration.Randomness = randomness;
            return this;
        }
        /// <summary>
        /// Sets randomness to 0.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> NotRandom()
        {
            return WithRandomness(NOT_RANDOM);
        }
        /// <summary>
        /// Sets randomness to 0.3.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> ABitRandom()
        {
            return WithRandomness(A_BIT_RANDOM);
        }
        /// <summary>
        /// Sets randomness to 0.6.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> SomewhatRandom()
        {
            return WithRandomness(SOMEWHAT_RANDOM);
        }
        /// <summary>
        /// Sets randomness to 1.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> VeryRandom()
        {
            return WithRandomness(VERY_RANDOM);
        }
        #endregion

        #region Sparseness

        internal const double NOT_SPARSE = 0;
        internal const double A_BIT_SPARSE = 0.3;
        internal const double SOMEWHAT_SPARSE = 0.6;
        internal const double VERY_SPARSE = 0.8;

        /// <summary>
        /// Sets sparseness to a specific value.
        /// </summary>
        /// <param name="sparseness">The desired sparseness value.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithSparseness(double sparseness)
        {
            mConfiguration.Sparseness = sparseness;
            return this;
        }
        /// <summary>
        /// Sets sparseness to 0.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> NotSparse()
        {
            return WithSparseness(NOT_SPARSE);
        }
        /// <summary>
        /// Sets sparseness to 0.3.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> ABitSparse()
        {
            return WithSparseness(A_BIT_SPARSE);
        }
        /// <summary>
        /// Sets sparseness to 0.6.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> SomewhatSparse()
        {
            return WithSparseness(SOMEWHAT_SPARSE);
        }
        /// <summary>
        /// Sets sparseness to 0.8.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> VerySparse()
        {
            return WithSparseness(VERY_SPARSE);
        }
        #endregion

        #region Chance to Remove Dead Ends

        internal const double DONT_REMOVE_DEAD_ENDS = 0;
        internal const double SMALL_CHANCE_TO_REMOVE_DEAD_ENDS = 0.2;
        internal const double MEDIUM_CHANCE_TO_REMOVE_DEAD_ENDS = 0.5;
        internal const double BIG_CHANCE_TO_REMOVE_DEAD_ENDS = 0.8;
        internal const double REMOVE_ALL_DEAD_ENDS = 1;

        /// <summary>
        /// Sets the chance to remove dead ends to a specific value.
        /// </summary>
        /// <param name="chance">The desired chance value.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithChanceToRemoveDeadEnds(double chance)
        {
            mConfiguration.ChanceToRemoveDeadends = chance;
            return this;
        }
        /// <summary>
        /// Sets the chance to remove dead ends to 0.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> DontRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(DONT_REMOVE_DEAD_ENDS);
        }
        /// <summary>
        /// Sets the chance to remove dead ends to 0.23
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithSmallChanceToRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(SMALL_CHANCE_TO_REMOVE_DEAD_ENDS);
        }
        /// <summary>
        /// Sets the chance to remove dead ends to 0.53
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithMediumChanceToRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(MEDIUM_CHANCE_TO_REMOVE_DEAD_ENDS);
        }
        /// <summary>
        /// Sets the chance to remove dead ends to 0.8.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithBigChanceToRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(BIG_CHANCE_TO_REMOVE_DEAD_ENDS);
        }
        /// <summary>
        /// Sets the chance to remove dead ends to 1.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> RemoveAllDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(REMOVE_ALL_DEAD_ENDS);
        }
        #endregion

        #region Room Size
        internal const int SMALL_ROOM_MIN = 2;
        internal const int SMALL_ROOM_MAX = 3;
        internal const int MEDIUM_ROOM_MIN = 3;
        internal const int MEDIUM_ROOM_MAX = 6;
        internal const int LARGE_ROOM_MIN = 5;
        internal const int LARGE_ROOM_MAX = 8;

        /// <summary>
        /// Sets the room size range to specific measurements.
        /// </summary>
        /// <param name="minWidth">The minimum desired width.</param>
        /// <param name="maxWidth">The maximum desired width.</param>
        /// <param name="minHeight">The minimum desired height.</param>
        /// <param name="maxHeight">The maximum desired height.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithRoomSize(int minWidth, int maxWidth, int minHeight, int maxHeight)
        {
            mConfiguration.MinRoomWidth = minWidth;
            mConfiguration.MaxRoomWidth = maxWidth;
            mConfiguration.MinRoomHeight = minHeight;
            mConfiguration.MaxRoomHeight = maxHeight;

            return this;
        }

        /// <summary>
        /// Sets the room size range to small to between 2x2 and 3x3.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithSmallSizeRooms()
        {
            return WithRoomSize(SMALL_ROOM_MIN, SMALL_ROOM_MAX, SMALL_ROOM_MIN, SMALL_ROOM_MAX);
        }
        /// <summary>
        /// Sets the room size range to small to between 3x3 and 6x6.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithMediumSizeRooms()
        {
            return WithRoomSize(MEDIUM_ROOM_MIN, MEDIUM_ROOM_MAX, MEDIUM_ROOM_MIN, MEDIUM_ROOM_MAX);
        }
        /// <summary>
        /// Sets the room size range to small to between 5x5 and 8x8.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithLargeSizeRooms()
        {
            return WithRoomSize(LARGE_ROOM_MIN, LARGE_ROOM_MAX, LARGE_ROOM_MIN, LARGE_ROOM_MAX);
        }
        #endregion

        #region Room Count
        internal const float SMALL_NUMBER_OF_ROOMS = 0.25f;
        internal const float MEDIUM_NUMBER_OF_ROOMS = 0.5f;
        internal const float LARGE_NUMBER_OF_ROOMS = 0.75f;
        internal const float ROOM_COUNT_PERCENTAGE_FACTOR = 0.5f;
        internal float? mRoomCountPercentage;

        /// <summary>
        /// Sets the desired room count to a specific value
        /// </summary>
        /// <param name="roomCount">The desired room count.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithRoomCount(int roomCount)
        {
            mConfiguration.RoomCount = roomCount;
            return this;
        }
        private DungeonConfigurationGenerator<T> WithRoomCountByPercentage(float roomCountByPercentage)
        {
            mRoomCountPercentage = roomCountByPercentage;
            return this;
        }

        /// <summary>
        /// Sets the desired room count to a small value computed according to room size and map size.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithSmallNumberOfRooms()
        {
            return WithRoomCountByPercentage(SMALL_NUMBER_OF_ROOMS);
        }
        /// <summary>
        /// Sets the desired room count to a medium value computed according to room size and map size.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithMediumNumberOfRooms()
        {
            return WithRoomCountByPercentage(MEDIUM_NUMBER_OF_ROOMS);

        }
        /// <summary>
        /// Sets the desired room count to a large value computed according to room size and map size.
        /// </summary>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithLargeNumberOfRooms()
        {
           return WithRoomCountByPercentage(LARGE_NUMBER_OF_ROOMS);
        }
        #endregion

        private void WrapUp()
        {
            if (!mRoomCountPercentage.HasValue) return;

            //compute room count according to room size and map size
            var medianRoomHeight = (mConfiguration.MaxRoomHeight + mConfiguration.MinRoomHeight)/2f;
            var medianRoomWidth = (mConfiguration.MaxRoomWidth + mConfiguration.MinRoomWidth)/2f;
            var maxPotentialRooms = (int) (mConfiguration.Width/medianRoomWidth)*
                                    (int) (mConfiguration.Height/medianRoomHeight)*ROOM_COUNT_PERCENTAGE_FACTOR;
            mConfiguration.RoomCount = (int) (maxPotentialRooms*mRoomCountPercentage.Value);
        }
        /// <summary>
        /// Sets the seed for the map generation to a specific value.
        /// </summary>
        /// <param name="seed">The desired seed.</param>
        /// <returns>The same instance.</returns>
        public DungeonConfigurationGenerator<T> WithSeed(int seed)
        {
            mSeed = seed;
            return this;
        }

        /// <summary>
        /// Generates the map with the generated configuration synchronously.
        /// </summary>
        /// <returns>The generated map.</returns>
        public Map<T> Now()
        {
            WrapUp();
            return mGenerator.Generate(mConfiguration, mSeed);
        }

        /// <summary>
        /// Generates the map with the generated configuration asynchronously.
        /// </summary>
        /// <param name="callback">Will be called when the generation process is complete (on a different thread).</param>
        public void AndTellMeWhenItsDone(Action<Map<T>> callback)
        {
            WrapUp();
            mGenerator.BeginGenerate(callback, mConfiguration, mSeed);
        }

        internal DungeonConfiguration GetConfiguration()
        {
            WrapUp();
            return mConfiguration;
        }

        #endregion
    }
}
