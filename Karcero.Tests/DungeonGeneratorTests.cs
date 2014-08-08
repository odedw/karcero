using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine;
using Karcero.Engine.Models;
using NUnit.Framework;

namespace Karcero.Tests
{
    [TestFixture]
    public class DungeonGeneratorTests
    {
        [Test]
        public void SpeedTest()
        {
            const int ITERATIONS = 10;
            var start = DateTime.Now;
            var generator = new DungeonGenerator<Cell>();
            for (var i = 0; i < ITERATIONS; i++)
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

            var totalSecs = DateTime.Now.Subtract(start).TotalSeconds / ITERATIONS;
            Console.WriteLine("Average time = {0} seconds", totalSecs);
        }
    }
}
