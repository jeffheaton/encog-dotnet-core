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

            var clone = (ActivationReLU)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.0 };

            activation.ActivationFunction(input, 0, 1);

            Assert.AreEqual(0.0, input[0], EncogFramework.DefaultDoubleEqual);

            // test derivative, wiki says this is logistic function (test may be wrong - jeroldhaas)
            input[0] = activation.DerivativeFunction(input[0], input[0]);
            Assert.AreEqual(0.5, input[0], EncogFramework.DefaultDoubleEqual);
        }
    }
}
