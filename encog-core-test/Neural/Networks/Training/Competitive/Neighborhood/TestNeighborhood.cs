using Encog.MathUtil.RBF;
using Encog.Neural.SOM.Training.Neighborhood;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    [TestClass]
    public class TestNeighborhood
    {
        [TestMethod]
        public void TestBubble()
        {
            var bubble = new NeighborhoodBubble(2);
            Assert.AreEqual(0.0, bubble.Function(5, 0), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 4), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 5), 0.1);
        }

        [TestMethod]
        public void TestSingle()
        {
            var bubble = new NeighborhoodSingle();
            Assert.AreEqual(0.0, bubble.Function(5, 0), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 5), 0.1);
        }

        [TestMethod]
        public void TestGaussian()
        {
            IRadialBasisFunction radial = new GaussianFunction(0.0, 1.0, 1.0);
            var bubble = new NeighborhoodRBF1D(radial);
            Assert.AreEqual(0.0, bubble.Function(5, 0), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 5), 0.1);
            Assert.AreEqual(0.6, bubble.Function(5, 4), 0.1);
        }
    }
}