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
            var start = DateTime.Now;
            var generator = new DunGenerator();
            generator.Generate(new DungeonConfiguration()
            {
                Width = 50,
                Height = 50,
                Sparseness = 30,
                Randomness = 1,
                ChanceToRemoveDeadends = 1
            });
            double totalSeconds = DateTime.Now.Subtract(start).TotalSeconds;
        }
    }
}
