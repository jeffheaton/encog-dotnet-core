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
            Assert.AreEqual(1, output[0]);
        }
    }
}
