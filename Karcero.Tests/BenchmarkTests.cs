using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine;
using Karcero.Engine.Helpers;
using Karcero.Engine.Models;
using NUnit.Framework;

namespace Karcero.Tests
{
    [TestFixture]
    public class BenchmarkTests
    {
        const int ITERATIONS = 10;

        [Test]
        public void SpeedTest()
        {
            var start = DateTime.Now;
            var generator = new DungeonGenerator<Cell>();
            for (var i = 0; i < ITERATIONS; i++)
            {

                generator.GenerateA()
                    .MediumDungeon()
                    .ABitRandom()
                    .SomewhatSparse()
                    .WithMediumChanceToRemoveDeadEnds()
                    .WithMediumSizeRooms()
                    .WithRoomCount(10)
                    .Now();
            }

            var totalSecs = DateTime.Now.Subtract(start).TotalSeconds / ITERATIONS;
            Console.WriteLine("Average time = {0} seconds", totalSecs);
        }

        [Test]
        public void BenchmarkTest()
        {
            var start = DateTime.Now;

            var roomCounts = new List<int>() { 10, 15, 20, 25, 30, 35, 40 };
            var roomSizeFuncs = new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
            {

                {"Small", builder => builder.WithSmallSizeRooms()},
                {"Medium", builder => builder.WithMediumSizeRooms()},
                {"Large", builder => builder.WithLargeSizeRooms()},
            };
            var dungeonSizeFuncs = new Dictionary<string, Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>>>
            {
                {"Huge Dungeon", builder => builder.HugeDungeon()},
                {"Large Dungeon", builder => builder.LargeDungeon()},
                {"Medium Dungeon", builder => builder.MediumDungeon()},
            };

            var filename = GetFilename();
            using (var writer = new StreamWriter(filename))
            {
                foreach (var dungeonKvp in dungeonSizeFuncs)
                {
                    writer.WriteLine(dungeonKvp.Key);
                    writer.WriteLine(@"Room Size \ Room Count,{0}", String.Join(",", roomCounts.Select(i => i.ToString())));
                    foreach (var kvp in roomSizeFuncs)
                    {
                        writer.Write(kvp.Key);
                        foreach (var roomCount in roomCounts)
                        {
                            var results = RunGeneration(dungeonKvp.Value, kvp.Value, roomCount);
                            writer.Write(",{0} ({1})", results.Item1, results.Item2);
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine();
                }
                var totalminutes = DateTime.Now.Subtract(start).TotalMinutes;
                writer.WriteLine();
                writer.WriteLine("Total running time - {0} minutes", totalminutes);
            }
        }

        private string GetFilename()
        {
            var index = 1;
            while (File.Exists(string.Format(@"Benchmark_{0}_{1}.csv", DateTime.Now.ToString("MMddyyyy"), index)))
                index++;
            return string.Format(@"Benchmark_{0}_{1}.csv", DateTime.Now.ToString("MMddyyyy"), index);

        }

        private static Tuple<double,double> RunGeneration(Func<DungeonConfigurationGenerator<Cell>, DungeonConfigurationGenerator<Cell>> dungeonSizeFunc,
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

            var totalSecs = DateTime.Now.Subtract(start).TotalSeconds / ITERATIONS;
            var averageRoomCount = roomCount/ITERATIONS;
            return new Tuple<double,double>(totalSecs, averageRoomCount);
        }
    }
}
