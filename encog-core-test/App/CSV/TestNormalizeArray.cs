using Encog.Util.Arrayutil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestNormalizeArray
    {
        [TestMethod]
        public void TestNormalize()
        {
            var norm = new NormalizeArray();
            double[] input = {1, 5, 10};
            double[] output = norm.Process(input);
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual(-1.0, output[0]);
            Assert.AreEqual(1.0, output[2]);
            Assert.AreEqual(1.0, norm.Stats.ActualLow);
            Assert.AreEqual(10.0, norm.Stats.ActualHigh);
        }
    }
}