using System;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;

namespace Karcero.Engine.Helpers
{
    public class DungeonConfigurationGenerator<T> where T : class, ICell, new()
    {
        #region Properties

        private DungeonConfiguration mConfiguration = new DungeonConfiguration();
        private DungeonGenerator<T> mGenerator;
        private int? mSeed;

        #endregion

        #region Constructors
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
        public DungeonConfigurationGenerator<T> DungeonOfSize(int width, int height)
        {
            mConfiguration.Width = width;
            mConfiguration.Height = height;
            return this;
        }

        public DungeonConfigurationGenerator<T> TinyDungeon()
        {
            return DungeonOfSize(TINY_SIZE, TINY_SIZE);
        }
        public DungeonConfigurationGenerator<T> SmallDungeon()
        {
            return DungeonOfSize(SMALL_SIZE, SMALL_SIZE);
        }
        public DungeonConfigurationGenerator<T> MediumDungeon()
        {
            return DungeonOfSize(MEDIUM_SIZE, MEDIUM_SIZE);
        }
        public DungeonConfigurationGenerator<T> LargeDungeon()
        {
            return DungeonOfSize(LARGE_SIZE, LARGE_SIZE);
        }
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

        public DungeonConfigurationGenerator<T> WithRandomness(double randomness)
        {
            mConfiguration.Randomness = randomness;
            return this;
        }
        public DungeonConfigurationGenerator<T> NotRandom()
        {
            return WithRandomness(NOT_RANDOM);
        }
        public DungeonConfigurationGenerator<T> ABitRandom()
        {
            return WithRandomness(A_BIT_RANDOM);
        }
        public DungeonConfigurationGenerator<T> SomewhatRandom()
        {
            return WithRandomness(SOMEWHAT_RANDOM);
        }
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

        public DungeonConfigurationGenerator<T> WithSparseness(double sparseness)
        {
            mConfiguration.Sparseness = sparseness;
            return this;
        }
        public DungeonConfigurationGenerator<T> NotSparse()
        {
            return WithSparseness(NOT_SPARSE);
        }
        public DungeonConfigurationGenerator<T> ABitSparse()
        {
            return WithSparseness(A_BIT_SPARSE);
        }
        public DungeonConfigurationGenerator<T> SomewhatSparse()
        {
            return WithSparseness(SOMEWHAT_SPARSE);
        }
        public DungeonConfigurationGenerator<T> VerySparse()
        {
            return WithSparseness(VERY_SPARSE);
        }
        #endregion

        #region Chance to Remove Dead Ends

        internal const double DONT_REMOVE_DEAD_ENDS = 0;
        internal const double SMALL_CHANCE_TO_REMOVE_DEAD_ENDS = 0.2;
        internal const double BIG_CHANCE_TO_REMOVE_DEAD_ENDS = 0.8;
        internal const double MEDIUM_CHANCE_TO_REMOVE_DEAD_ENDS = 0.5;
        internal const double REMOVE_ALL_DEAD_ENDS = 1;

        public DungeonConfigurationGenerator<T> WithChanceToRemoveDeadEnds(double chance)
        {
            mConfiguration.ChanceToRemoveDeadends = chance;
            return this;
        }
        public DungeonConfigurationGenerator<T> DontRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(DONT_REMOVE_DEAD_ENDS);
        }
        public DungeonConfigurationGenerator<T> WithSmallChanceToRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(SMALL_CHANCE_TO_REMOVE_DEAD_ENDS);
        }
        public DungeonConfigurationGenerator<T> WithMediumChanceToRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(MEDIUM_CHANCE_TO_REMOVE_DEAD_ENDS);
        }
        public DungeonConfigurationGenerator<T> WithBigChanceToRemoveDeadEnds()
        {
            return WithChanceToRemoveDeadEnds(BIG_CHANCE_TO_REMOVE_DEAD_ENDS);
        }
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

        public DungeonConfigurationGenerator<T> WithRoomSize(int minWidth, int maxWidth, int minHeight, int maxHeight)
        {
            mConfiguration.MinRoomWidth = minWidth;
            mConfiguration.MaxRoomWidth = maxWidth;
            mConfiguration.MinRoomHeight = minHeight;
            mConfiguration.MaxRoomHeight = maxHeight;

            return this;
        }

        public DungeonConfigurationGenerator<T> WithSmallSizeRooms()
        {
            return WithRoomSize(SMALL_ROOM_MIN, SMALL_ROOM_MAX, SMALL_ROOM_MIN, SMALL_ROOM_MAX);
        }
        public DungeonConfigurationGenerator<T> WithMediumSizeRooms()
        {
            return WithRoomSize(MEDIUM_ROOM_MIN, MEDIUM_ROOM_MAX, MEDIUM_ROOM_MIN, MEDIUM_ROOM_MAX);
        }
        public DungeonConfigurationGenerator<T> WithLargeSizeRooms()
        {
            return WithRoomSize(LARGE_ROOM_MIN, LARGE_ROOM_MAX, LARGE_ROOM_MIN, LARGE_ROOM_MAX);
        }
        #endregion

        #region Room Count
        internal const int SMALL_NUMBER_OF_ROOMS = 3;
        internal const int MEDIUM_NUMBER_OF_ROOMS = 6;
        internal const int LARGE_NUMBER_OF_ROOMS = 10;


        public DungeonConfigurationGenerator<T> WithRoomCount(int roomCount)
        {
            mConfiguration.RoomCount = roomCount;

            return this;
        }

        public DungeonConfigurationGenerator<T> WithSmallNumberOfRooms()
        {
            return WithRoomCount(SMALL_NUMBER_OF_ROOMS);
        }
        public DungeonConfigurationGenerator<T> WithMediumNumberOfRooms()
        {
            return WithRoomCount(MEDIUM_NUMBER_OF_ROOMS);
        }
        public DungeonConfigurationGenerator<T> WithLargeNumberOfRooms()
        {
            return WithRoomCount(LARGE_NUMBER_OF_ROOMS);
        }
        #endregion

        public DungeonConfigurationGenerator<T> WithSeed(int seed)
        {
            mSeed = seed;
            return this;
        }

        public Map<T> Now()
        {
            return mGenerator.Generate(mConfiguration, mSeed);
        }

        public void AndTellMeWhenItsDone(Action<Map<T>> callback)
        {
            mGenerator.BeginGenerate(callback, mConfiguration, mSeed);
        }

        public DungeonConfiguration GetConfiguration()
        {
            return mConfiguration;
        }

        #endregion
    }
}
