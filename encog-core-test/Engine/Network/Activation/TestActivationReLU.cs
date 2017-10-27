using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivatioReLU
    {
        [TestMethod]
        public void TestRELU()
        {
            var activation = new ActivationReLU();
            Assert.IsTrue(activation.HasDerivative);

            var clone = activation.Clone();
            Assert.IsInstanceOfType(clone, typeof(ActivationReLU));

            double[] input = { -2, -1, 0, 1, 2 };

            activation.ActivationFunction(input, 0, 5);

            CollectionAssert.AreEqual(new double[] { 0, 0, 0, 1, 2 }, input);

            input[2] = activation.DerivativeFunction(0, input[2]);
            Assert.AreEqual(0.0, input[2], EncogFramework.DefaultDoubleEqual);
        }
    }
}
