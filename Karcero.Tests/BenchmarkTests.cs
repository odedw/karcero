using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Karcero.Engine;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;
using Karcero.Engine.Processors;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Helpers.Randomizer;

namespace Karcero.Tests
{
    [TestFixture]
    public class BenchmarkTests
    {
        const int ITERATIONS = 5;
        object mLock = new object();

        [Test]
        public void ParallelMapGenerationTest()
        {
            const int SIZE = 1000;
            const int NUMBER_OF_ROOMS = 320;
            const int NUMBER_OF_PARALLEL_CALLS = 16;
            var generator = new DungeonGenerator<Cell>();
            DateTime start = DateTime.Now;
            //One big map
            for (var i = 0; i < ITERATIONS; i++)
            {
                Console.Write(i + ",");

                generator.GenerateA()
                         .DungeonOfSize(SIZE, SIZE)
                         .VeryRandom()
                         .SomewhatSparse()
                         .WithMediumChanceToRemoveDeadEnds()
                         .WithLargeSizeRooms()
                         .WithRoomCount(NUMBER_OF_ROOMS)
                         .Now();

            }
            Console.WriteLine("One Big map: Average of {0} seconds", DateTime.Now.Subtract(start).TotalSeconds / ITERATIONS);

            var sizeOfSmallMap = (int)(SIZE / Math.Sqrt(NUMBER_OF_PARALLEL_CALLS));
            var roomsPerMap = NUMBER_OF_ROOMS / NUMBER_OF_PARALLEL_CALLS;
            start = DateTime.Now;
            //Several small maps in parallel
            for (var i = 0; i < ITERATIONS; i++)
            {
                Console.Write(i + ",");

                Parallel.For(0, NUMBER_OF_PARALLEL_CALLS, (j) => generator.GenerateA()
                    .DungeonOfSize(sizeOfSmallMap, sizeOfSmallMap)
                    .VeryRandom()
                    .SomewhatSparse()
                    .WithMediumChanceToRemoveDeadEnds()
                    .WithLargeSizeRooms()
                    .WithRoomCount(roomsPerMap)
                    .Now());

            }
            Console.WriteLine("Several small maps in parallel: Average of {0} seconds", DateTime.Now.Subtract(start).TotalSeconds / ITERATIONS);
        }

        [Test]
      
        public void SpeedTest()
        {
            var generator = new DungeonGenerator<Cell>();
            Dictionary<string, double> results = null;
            var config = generator.GenerateA()
                    .HugeDungeon()
                    .VeryRandom()
                    .SomewhatSparse()
                    .WithMediumChanceToRemoveDeadEnds()
                    .WithLargeSizeRooms()
                    .WithRoomCount(100)
                    .GetConfiguration();
            for (var i = 0; i < ITERATIONS; i++)
            {


                var generationResult = generator.GenerateAndMeasure(config);

                if (results == null)
                {
                    results = generationResult.Item2;
                }
                else
                {
                    foreach (var kvp in generationResult.Item2)
                    {
                        results[kvp.Key] += kvp.Value;
                    }
                }
            }

            foreach (var kvp in results)
            {
                Console.WriteLine("{0}: {1} seconds", kvp.Key, (kvp.Value / ITERATIONS));
            }
        }

        [Test]
        public void SinglePreProcessorTest()
        {
            var config = new DungeonConfigurationGenerator<Cell>(null)
                    .DungeonOfSize(500,500)
                    .NotRandom()
                    .GetConfiguration();
            var mazeGenerator = new MazeGenerator<BinaryCell>();

            for (var i = 0; i < ITERATIONS; i++)
            {
                Console.Write("{0},", i);
                var map = new Map<BinaryCell>(config.Width / 2, config.Height / 2);
                mazeGenerator.ProcessMap(map, config, new Randomizer());
            }
            Console.WriteLine();
            //StaticTimer.WriteResults(ITERATIONS);
        }

        [Test]
        public void FindMaxRoomCountTest()
        {
            var dimensions = new List<Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>>()
            {
                //size
                new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
                {
                    {"Small", builder => builder.SmallDungeon()},
                    {"Medium", builder => builder.MediumDungeon()},
                    {"Large", builder => builder.LargeDungeon()},
                },

                //sparseness
                new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
                {
                    {"NotSparse", builder => builder.NotSparse()},
                    {"ABitSparse", builder => builder.ABitSparse()},
                    {"SomewhatSparse", builder => builder.SomewhatSparse()},
                    {"VerySparse", builder => builder.VerySparse()}
                },

                //sparseness
                new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
                {
                    {"WithSmallSizeRooms", builder => builder.WithSmallSizeRooms()},
                    {"WithMediumSizeRooms", builder => builder.WithMediumSizeRooms()},
                    {"WithLargeSizeRooms", builder => builder.WithLargeSizeRooms()}
                },
            };

            var defaultBuilder = new DungeonGenerator<Cell>()
                .GenerateA()
                .SomewhatRandom()
                .WithMediumChanceToRemoveDeadEnds();

            Console.WriteLine("{0},{1},{2},{3}", "CellCount", "Sparseness", "MedianRoomSize", "MaxRooms");

            CallRecursivelyWithFixedDimensions(defaultBuilder,
                new List<KeyValuePair<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>>(), 
                dimensions);
        }

        private void CallRecursivelyWithFixedDimensions(DungeonConfigurationGenerator<Cell> defaultBuilder, 
            List<KeyValuePair<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>> fixedDimensions, 
            List<Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>> restOfDimensions)
        {
            if (restOfDimensions.Count > 0)
            {
                foreach (var dimension in restOfDimensions[0])
                {
                    var newFixed = fixedDimensions.ToList();
                    newFixed.Add(dimension);
                    CallRecursivelyWithFixedDimensions(defaultBuilder, newFixed, restOfDimensions.Skip(1).ToList());
                }
                return;
            }

            TestWithFixedDimensions(defaultBuilder, fixedDimensions);
        }

        private void TestWithFixedDimensions(DungeonConfigurationGenerator<Cell> defaultBuilder, 
            IEnumerable<KeyValuePair<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>> fixedDimensions)
        {
            var maxRooms = 5;
            var failedIterationCount = 0;
            const int MAX_FAILED_ITERATION_COUNT = 3;
            var kvps = fixedDimensions.ToList();
            defaultBuilder = kvps.Aggregate(defaultBuilder, (current, dimension) => dimension.Value(current));

            try
            {
                while (failedIterationCount < MAX_FAILED_ITERATION_COUNT)
                {
                    defaultBuilder = defaultBuilder.WithRoomCount(maxRooms);
                    var map = defaultBuilder.Now();
                    if (map.Rooms.Count == maxRooms)
                    {
                        maxRooms++;
                    }
                    else
                    {
                        failedIterationCount++;
                    }
                }
                var config = defaultBuilder.GetConfiguration();
                var medianRoomHeight = (config.MaxRoomHeight + config.MinRoomHeight) / 2f;
                var medianRoomWidth = (config.MaxRoomWidth + config.MinRoomWidth) / 2f;
                var maxPotentialRooms = (int) (config.Width/medianRoomWidth)*(int) (config.Height/medianRoomHeight);
                var result = (maxPotentialRooms*(1 - config.Sparseness));
                Console.WriteLine("{0},{1},{2},{3}, ratio: {4}", (config.Width * config.Height), config.Sparseness, maxRooms-1,
                    (maxPotentialRooms * (1-config.Sparseness)),
                    (maxRooms-1)/result);
            }
            catch (Exception)
            {
                Console.WriteLine(String.Join(" ", kvps.Select(kvp => kvp.Key)) + ": failed on " + (maxRooms-1));    
            }
            

        }

        [TestCase(typeof(BenchmarkResultsCsvWriter))]
        [TestCase(typeof(BenchmarkResultsHtmlWriter))]
        public void BenchmarkTest(Type writerImplementationType)
        {
            var start = DateTime.Now;

            var roomCounts = new List<int> { 10, 15, 20, 25, 30,35,40 };
            var roomSizeFuncs = new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
            {

                {"Small", builder => builder.WithSmallSizeRooms()},
                {"Medium", builder => builder.WithMediumSizeRooms()},
                {"Large", builder => builder.WithLargeSizeRooms()},
            };
            var dungeonSizeFuncs = new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
            {
                {"Huge", builder => builder.HugeDungeon()},
                {"Large", builder => builder.LargeDungeon()},
                {"Medium", builder => builder.MediumDungeon()},
            };

            var writer = Activator.CreateInstance(writerImplementationType) as IBenchmarkResultsWriter;
            writer.Init();
            foreach (var dungeonKvp in dungeonSizeFuncs)
            {
                var resultsByRoomSize = new Dictionary<string, List<Engine.Models.Tuple<double, double>>>();
                foreach (var kvp in roomSizeFuncs)
                {
                    var results = roomCounts.Select(i => RunGeneration(dungeonKvp.Value, kvp.Value, i)).ToList();
                    resultsByRoomSize[kvp.Key] = results;
                }
                writer.WriteResultsForDungeonSize(dungeonKvp.Key, roomCounts, resultsByRoomSize);
            }
            writer.WriteTotalRunningTime(DateTime.Now.Subtract(start));
            writer.Close();
        }

        private static Engine.Models.Tuple<double, double> RunGeneration(Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>> dungeonSizeFunc,
            Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>> roomSizeFunc,
            int numberOfRooms)
        {
            var start = DateTime.Now;
            var generator = new DungeonGenerator<Cell>();
            var roomCount = 0;
            for (var i = 0; i < ITERATIONS; i++)
            {
                var map = roomSizeFunc(dungeonSizeFunc(generator.GenerateA()))
                     .SomewhatRandom()
                     .SomewhatSparse()
                     .WithMediumChanceToRemoveDeadEnds()
                     .WithRoomCount(numberOfRooms)
                     .Now();
                roomCount += map.Rooms.Count;
            }

            var totalSecs = DateTime.Now.Subtract(start).TotalMilliseconds / ITERATIONS;
            var averageRoomCount = roomCount / ITERATIONS;
            return new Engine.Models.Tuple<double, double>(totalSecs, averageRoomCount);
        }

        internal interface IBenchmarkResultsWriter
        {
            void Init();
            void WriteResultsForDungeonSize(string dungeonSize, List<int> roomCounts, Dictionary<string, List<Engine.Models.Tuple<double, double>>> results);
            void WriteTotalRunningTime(TimeSpan span);
            void Close();
        }

        internal class BenchmarkResultsCsvWriter : IBenchmarkResultsWriter
        {
            private StreamWriter mWriter;

            private static string GetFilename()
            {
                var index = 1;
                while (File.Exists(string.Format(@"Benchmark_{0}_{1}.csv", DateTime.Now.ToString("MMddyyyy"), index)))
                    index++;
                return string.Format(@"Benchmark_{0}_{1}.csv", DateTime.Now.ToString("MMddyyyy"), index);
            }

            public void Init()
            {
                var filename = GetFilename();
                mWriter = new StreamWriter(filename);
            }

            public void WriteResultsForDungeonSize(string dungeonSize, List<int> roomCounts, Dictionary<string, List<Engine.Models.Tuple<double, double>>> results)
            {
                mWriter.WriteLine(@"{0} Dungeon,{1}", dungeonSize, String.Join(",", roomCounts.Select(i => i.ToString(CultureInfo.InvariantCulture) + " Rooms")));
                foreach (var kvp in results)
                {
                    mWriter.Write("{0} Rooms", kvp.Key);
                    foreach (var result in kvp.Value)
                    {
                        mWriter.Write(",{0} ({1})", result.Item1, result.Item2);
                    }
                    mWriter.WriteLine();
                }
                mWriter.WriteLine();
            }

            public void WriteTotalRunningTime(TimeSpan span)
            {
                mWriter.WriteLine();
                mWriter.WriteLine("Total running time - {0} minutes", span.TotalMinutes);
            }

            public void Close()
            {
                mWriter.Flush();
                mWriter.Close();
            }
        }

        internal class BenchmarkResultsHtmlWriter : IBenchmarkResultsWriter
        {
            private StreamWriter mWriter;

            public void Init()
            {
                mWriter = new StreamWriter("Benchamrk.html");
            }

            public void WriteResultsForDungeonSize(string dungeonSize, List<int> roomCounts, Dictionary<string, List<Engine.Models.Tuple<double, double>>> results)
            {
                mWriter.WriteLine("<table>");
                mWriter.WriteLine("<tr>");
                mWriter.WriteLine("<th>{0} Map</th>", dungeonSize);
                foreach (var roomCount in roomCounts)
                {
                    mWriter.WriteLine("<th>{0} Rooms</th>", roomCount);
                }
                mWriter.WriteLine("</tr>");
                foreach (var kvp in results)
                {
                    mWriter.WriteLine("<tr>");
                    mWriter.WriteLine("<td>{0} Rooms</td>", kvp.Key);
                    foreach (var result in kvp.Value)
                    {
                        bool generatedAllRooms = (int)result.Item2 == roomCounts[kvp.Value.IndexOf(result)];
                        mWriter.WriteLine("<td>{0}</td>", (generatedAllRooms ? Math.Round(result.Item1).ToString() : "-"));
                    }
                    mWriter.WriteLine("</tr>");
                }
                mWriter.WriteLine("</table>");
            }

            public void WriteTotalRunningTime(TimeSpan span)
            {
            }

            public void Close()
            {
                mWriter.Flush();
                mWriter.Close();
            }
        }
    }
}
