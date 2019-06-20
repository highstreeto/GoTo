using GoTo.Service.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoTo.Service.Tests {
    [TestClass]
    public class DestinationTests {
        [TestMethod]
        public void TestDistanceSamePoint() {
            var dest = new Destination("A", 46.0, 13.0);

            Assert.AreEqual(0, dest.DistanceTo(dest));
        }

        [TestMethod]
        public void TestDistanceKnown() {
            var source = new Destination("Linz", 48.305598, 14.286601);
            var dest = new Destination("Wien", 48.208344, 16.371313);

            Assert.AreEqual(155, source.DistanceTo(dest));
        }
    }
}
