using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunGen.Engine;
using DunGen.Engine.Models;
using NUnit.Framework;

namespace DunGen.Tests
{
    [TestFixture]
    public class DunGeneratorTests
    {
        [Test]
        public void SpeedTest()
        {
            var iterations = 10;
            var start = DateTime.Now;
            var generator = new DunGenerator();
            for (int i = 0; i < iterations; i++)
            {

                generator.Generate(new DungeonConfiguration()
                 {
                     Height = 16,
                     Width = 16,
                     Randomness = 0.4,
                     Sparseness = 0.6,
                     ChanceToRemoveDeadends = 0.6,
                     MinRoomHeight = 3,
                     MaxRoomHeight = 6,
                     MinRoomWidth = 3,
                     MaxRoomWidth = 6,
                     RoomCount = 10
                 });
            }

            var totalSecs = DateTime.Now.Subtract(start).TotalSeconds / iterations;
            Console.WriteLine("Average time = {0} seconds", totalSecs);
        }
    }
}
