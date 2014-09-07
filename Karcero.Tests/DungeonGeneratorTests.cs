using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Karcero.Engine;
using Karcero.Engine.Contracts;
using Karcero.Engine.Models;
using NUnit.Framework;

namespace Karcero.Tests
{
    [TestFixture]
    public class DungeonGeneratorTests
    {
      

        [Test]
        public void AddMapProcessor_SomeProcessor_ProcessMethodCalled()
        {
            var fakeProcessor = A.Fake<IMapProcessor<Cell>>();

            var generator = new DungeonGenerator<Cell>();
            generator.AddMapProcessor(fakeProcessor);
            generator.GenerateA()
                    .MediumDungeon()
                    .ABitRandom()
                    .SomewhatSparse()
                    .WithMediumChanceToRemoveDeadEnds()
                    .WithMediumSizeRooms()
                    .WithRoomCount(10)
                    .Now();

            A.CallTo(() => fakeProcessor.ProcessMap(A<Map<Cell>>.Ignored, A<DungeonConfiguration>.Ignored, A<IRandomizer>.Ignored))
                .MustHaveHappened();
        }
    }
}
