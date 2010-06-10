using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.Activation;

namespace encog_test.Encog.Neural.Activation
{
    [TestFixture]
    class TestActivationUtil
    {
        [Test]
        public void testActivationUtil()
        {
            double[] d = ActivationUtil.ToArray(1.0);
            double d2 = ActivationUtil.FromArray(d);
            Assert.AreEqual(1.0, d2, 0.1);
        }
    }
}
