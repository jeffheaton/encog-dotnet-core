using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationRamp
    {
        [TestMethod]
        public void TestRamp()
        {
            var activation = new ActivationRamp(2, -2, 3, 1);
            Assert.IsTrue(activation.HasDerivative);

            var clone = activation.Clone();
            Assert.IsInstanceOfType(clone, typeof(ActivationRamp));

            double[] input = { -3, -2, 0, 2, 3 };

            //Clone should have same parameters
            CollectionAssert.AreEqual(activation.Params, ((ActivationRamp)clone).Params);
            
            activation.ActivationFunction(input, 0, 5);

            Assert.AreEqual(1.0, input[0], EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(1.0, input[1], EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(2.0, input[2], EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(3.0, input[3], EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(3.0, input[4], EncogFramework.DefaultDoubleEqual);

            input[0] = activation.DerivativeFunction(-3, input[0]);
            input[2] = activation.DerivativeFunction(0, input[2]);
            input[4] = activation.DerivativeFunction(3, input[4]);
            Assert.AreEqual(0.0, input[0], EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(0.5, input[2], EncogFramework.DefaultDoubleEqual);
            Assert.AreEqual(0.0, input[4], EncogFramework.DefaultDoubleEqual);
        }
    }
}
