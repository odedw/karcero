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
    }
}
