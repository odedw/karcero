using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Karcero.Engine;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;
using NUnit.Framework;

namespace Karcero.Tests
{
    [TestFixture]
    public class DungeonConfigurationGeneratorTests
    {

        [Test]
        public void GenerateConfiguration_DifferentSizes_CorrespondingConfigurationSizeSet()
        {
            var SOME_HEIGHT = 5;
            var SOME_WIDTH = 6;

            GenerateAndAssert(generator => generator.DungeonOfSize(SOME_WIDTH, SOME_HEIGHT),
                configuration =>
                {
                    Assert.AreEqual(SOME_WIDTH, configuration.Width);
                    Assert.AreEqual(SOME_HEIGHT, configuration.Height);
                });
            GenerateAndAssert(generator => generator.TinyDungeon(),
                 configuration =>
                 {
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.TINY_SIZE, configuration.Width);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.TINY_SIZE, configuration.Height);
                 });
            GenerateAndAssert(generator => generator.SmallDungeon(),
                 configuration =>
                 {
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_SIZE, configuration.Width);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_SIZE, configuration.Height);
                 });
            GenerateAndAssert(generator => generator.MediumDungeon(),
                 configuration =>
                 {
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_SIZE, configuration.Width);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_SIZE, configuration.Height);
                 });
            GenerateAndAssert(generator => generator.LargeDungeon(),
                 configuration =>
                 {
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_SIZE, configuration.Width);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_SIZE, configuration.Height);
                 });
        }

        [Test]
        public void GenerateConfiguration_DifferentRandomness_CorrespondingRandomnessSet()
        {
            var SOME_RANDOM_VALUE = 0.25;

            GenerateAndAssert(generator => generator.WithRandomness(SOME_RANDOM_VALUE),
               configuration => Assert.AreEqual(SOME_RANDOM_VALUE, configuration.Randomness));
            GenerateAndAssert(generator => generator.NotRandom(),
              configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.NOT_RANDOM, configuration.Randomness));
            GenerateAndAssert(generator => generator.ABitRandom(),
               configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.A_BIT_RANDOM, configuration.Randomness));
            GenerateAndAssert(generator => generator.SomewhatRandom(),
               configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SOMEWHAT_RANDOM, configuration.Randomness));
            GenerateAndAssert(generator => generator.VeryRandom(),
               configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.VERY_RANDOM, configuration.Randomness));
            
        }

         [Test]
        public void GenerateConfiguration_DifferentSparseness_CorrespondingSparsenessSet()
        {
            var SOME_SPARSE_VALUE = 0.25;

            GenerateAndAssert(generator => generator.WithSparseness(SOME_SPARSE_VALUE),
                 configuration => Assert.AreEqual(SOME_SPARSE_VALUE, configuration.Sparseness));
            GenerateAndAssert(generator => generator.NotSparse(),
              configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.NOT_SPARSE, configuration.Sparseness));
           GenerateAndAssert(generator => generator.ABitSparse(),
              configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.A_BIT_SPARSE, configuration.Sparseness));
           GenerateAndAssert(generator => generator.SomewhatSparse(),
              configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SOMEWHAT_SPARSE, configuration.Sparseness));
           GenerateAndAssert(generator => generator.VerySparse(),
              configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.VERY_SPARSE, configuration.Sparseness));
         }

        [Test]
        public void GenerateConfiguration_DifferentChanceToRemoveDeadEnds_CorrespondingChanceSet()
        {
            var SOME_CHANCE_VALUE = 0.25;

            GenerateAndAssert(generator => generator.WithChanceToRemoveDeadEnds(SOME_CHANCE_VALUE),
                configuration => Assert.AreEqual(SOME_CHANCE_VALUE, configuration.ChanceToRemoveDeadends));
           GenerateAndAssert(generator => generator.WithSmallChanceToRemoveDeadEnds(),
                configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_CHANCE_TO_REMOVE_DEAD_ENDS, configuration.ChanceToRemoveDeadends));
           GenerateAndAssert(generator => generator.WithMediumChanceToRemoveDeadEnds(),
                configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_CHANCE_TO_REMOVE_DEAD_ENDS, configuration.ChanceToRemoveDeadends));
           GenerateAndAssert(generator => generator.WithBigChanceToRemoveDeadEnds(),
                configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.BIG_CHANCE_TO_REMOVE_DEAD_ENDS, configuration.ChanceToRemoveDeadends));
           GenerateAndAssert(generator => generator.RemoveAllDeadEnds(),
                configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.REMOVE_ALL_DEAD_ENDS, configuration.ChanceToRemoveDeadends));
           
         }
 
        [Test]
        public void GenerateConfiguration_DifferentSizeRooms_CorrespondingSizeSet()
        {
            var SOME_MIN_HEIGHT = 1;
            var SOME_MAX_HEIGHT = 2;
            var SOME_MIN_WIDTH = 3;
            var SOME_MAX_WIDTH = 4;

            GenerateAndAssert(generator => generator.WithRoomSize(SOME_MIN_WIDTH, SOME_MAX_WIDTH, SOME_MIN_HEIGHT, SOME_MAX_HEIGHT),
                configuration =>
                {
                    Assert.AreEqual(SOME_MIN_HEIGHT, configuration.MinRoomHeight);
                    Assert.AreEqual(SOME_MAX_HEIGHT, configuration.MaxRoomHeight);
                    Assert.AreEqual(SOME_MIN_WIDTH, configuration.MinRoomWidth);
                    Assert.AreEqual(SOME_MAX_WIDTH, configuration.MaxRoomWidth);
                });
           GenerateAndAssert(generator => generator.WithSmallSizeRooms(),
                configuration =>
                {
                    Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_ROOM_MIN, configuration.MinRoomHeight);
                    Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_ROOM_MAX, configuration.MaxRoomHeight);
                    Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_ROOM_MIN, configuration.MinRoomHeight);
                    Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_ROOM_MAX, configuration.MaxRoomHeight);
                });
           GenerateAndAssert(generator => generator.WithMediumSizeRooms(),
                 configuration =>
                 {
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_ROOM_MIN, configuration.MinRoomHeight);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_ROOM_MAX, configuration.MaxRoomHeight);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_ROOM_MIN, configuration.MinRoomHeight);
                     Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_ROOM_MAX, configuration.MaxRoomHeight);
                 });
           GenerateAndAssert(generator => generator.WithLargeSizeRooms(),
              configuration =>
              {
                  Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_ROOM_MIN, configuration.MinRoomHeight);
                  Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_ROOM_MAX, configuration.MaxRoomHeight);
                  Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_ROOM_MIN, configuration.MinRoomHeight);
                  Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_ROOM_MAX, configuration.MaxRoomHeight);
              });
           
         }

        [Test]
        public void GenerateConfiguration_DifferentNumberOfRooms_CorrespondingRoomCountSet()
        {
            var SOME_NUMBER = 5;

            GenerateAndAssert(generator => generator.WithRoomCount(SOME_NUMBER),
                configuration => Assert.AreEqual(SOME_NUMBER, configuration.RoomCount));
            GenerateAndAssert(generator => generator.WithSmallNumberOfRooms(),
                 configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.SMALL_NUMBER_OF_ROOMS, configuration.RoomCount));
            GenerateAndAssert(generator => generator.WithMediumNumberOfRooms(),
                 configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.MEDIUM_NUMBER_OF_ROOMS, configuration.RoomCount));
            GenerateAndAssert(generator => generator.WithLargeNumberOfRooms(),
                 configuration => Assert.AreEqual(DungeonConfigurationGenerator<Cell>.LARGE_NUMBER_OF_ROOMS, configuration.RoomCount));
            
        }

        private void GenerateAndAssert(
            Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>> method,
            Action<DungeonConfiguration> assertMethod)
        {
            DungeonConfiguration targetConfiguration = null;
            var fakeGenerator = A.Fake<DungeonGenerator<Cell>>();
            A.CallTo(() => fakeGenerator.Generate(null, null))
                .WithAnyArguments()
                .Invokes(callObject => targetConfiguration = callObject.Arguments[0] as DungeonConfiguration);

            var configGenerator = new DungeonConfigurationGenerator<Cell>(fakeGenerator);
            method(configGenerator).Now();

            assertMethod(targetConfiguration);
        }
    }
}
