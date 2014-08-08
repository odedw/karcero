using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karcero.Engine.Models;
using Karcero.Engine.Implementations;
using NUnit.Framework;
using Randomizer = Karcero.Engine.Implementations.Randomizer;

namespace Karcero.Tests
{
    [TestFixture]
    public class RandomizerTests
    {
        [Test]
        public void GetRandomCell_ValidInput_ReturnsCellFromMap()
        {
            var map = new Map<Cell>(5, 5);

            var randomizer = new Randomizer();
            var randomCell = randomizer.GetRandomCell(map);

            Assert.AreEqual(map.GetCell(randomCell.Row, randomCell.Column), randomCell);
        }

        [Test]
        public void GetRandomCell_EmptyMap_ReturnsNull()
        {
            var map = new Map<Cell>(0, 0);

            var randomizer = new Randomizer();
            var randomCell = randomizer.GetRandomCell(map);

            Assert.IsNull(randomCell);
        }

        [Test]
        public void GetRandomEnumValue_InputWithExcludeList_ReturnsValueNotInExcludeList()
        {
            var randomizer = new Randomizer();
            var value = randomizer.GetRandomEnumValue<SomeEnum>(new List<SomeEnum>(Enum.GetValues(typeof(SomeEnum)).OfType<SomeEnum>().Skip(1)));

            Assert.AreEqual(SomeEnum.Value1, value);
        }

        [Test]
        public void GetRandomItem_ValidInput_ReturnsItemFromCollection()
        {
            var collection = new List<object>() {new object(),new object(),new object(),new object()};

            var randomizer = new Randomizer();
            var item = randomizer.GetRandomItem(collection);

            Assert.IsTrue(collection.Contains(item));
        }

        [Test]
        public void GetRandomItem_ValidInputWithExcludeList_ReturnsValueNotInExcludeList()
        {
            var collection = new List<object>() {new object(),new object(),new object(),new object()};

            var randomizer = new Randomizer();
            var item = randomizer.GetRandomItem(collection, collection.Skip(1));

            Assert.AreEqual(collection[0], item);
        }

        private enum SomeEnum
        {
            Value1,
            Value2,
            Value3,
            Value4,
            Value5,
        }
    }
}
