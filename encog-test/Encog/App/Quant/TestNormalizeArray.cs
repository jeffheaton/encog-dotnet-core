using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.App.Quant.Normalize;

namespace encog_test.Encog.App.Quant
{
    [TestFixture]
    public class TestNormalizeArray
    {
        public void TestNormalize()
        {
            NormalizeArray norm = new NormalizeArray();
            double[] input = { 1,5,10 };
            double[] output = norm.Process(input);
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual(-1, output[0]);
            Assert.AreEqual(1, output[2]);
            Assert.AreEqual(1, norm.Stats.ActualLow);
            Assert.AreEqual(10, norm.Stats.ActualHigh);
        }
    }
}
