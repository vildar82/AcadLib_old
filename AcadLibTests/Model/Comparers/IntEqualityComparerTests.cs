using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AcadLib.Comparers.Tests
{
    [TestClass()]
    public class IntEqualityComparerTests
    {
        [TestMethod()]
        public void IntEqualityComparerTest ()
        {
            var source = new List<int> { 1, 5, 5, 2,7,12,15,18,20,24,25,10,5,1,8,11,30 };

            var comparer = new IntEqualityComparer(5);

            source.Sort();
            var res = source.GroupBy(g => g, comparer);

            Assert.AreEqual(5, res.Count());
        }

        [TestMethod()]
        public void EqualsTest ()
        {
            var comparer = new IntEqualityComparer(5);
            var res = comparer.Equals(1, 2);
            Assert.AreEqual(true, res);
        }
    }
}