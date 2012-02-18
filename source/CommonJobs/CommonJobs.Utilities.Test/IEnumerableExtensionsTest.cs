using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonJobs.Utilities;

namespace CommonJobs.Utilities.Test
{
    [TestClass]
    public class IEnumerableExtensionsTest
    {
        [TestMethod]
        public void Batch_ReturnsGroupsOfN()
        {
            var numbers = Enumerable.Range(1, 100);
            var groups = IEnumerableExtensions.Batch(numbers, 10);
            Assert.IsTrue(groups.All(g => g.Count() == 10));
        }

        [TestMethod]
        public void Batch_OnlyLastGroupHasLessItems()
        {
            var numbers = Enumerable.Range(1, 100);
            var groups = IEnumerableExtensions.Batch(numbers, 13);

            var lastGroup = groups.Last();
            var restOfGroups = groups.Take(groups.Count() - 1);

            Assert.IsTrue(restOfGroups.All(g => g.Count() == 13));
            Assert.IsTrue(lastGroup.Count() == 9); //13*7 + 9 = 100
        }

        [TestMethod]
        public void EmptyIfNull_ReturnsEmptyEnumerationIfNull()
        {
            var nullInts = (IEnumerable<int>)null;
            var result = IEnumerableExtensions.EmptyIfNull(nullInts);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 0);
        }

        [TestMethod]
        public void EmptyIfNull_ReturnsOriginalEnumerationIfNotNull()
        {
            var ints = Enumerable.Range(1, 100);
            var result = IEnumerableExtensions.EmptyIfNull(ints);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, ints);
        }
    }
}
