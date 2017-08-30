
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationRectifier
    {
        [TestMethod]
        public void TestRectifier()
        {
            var activation = new ActivationRectifier();
            Assert.IsTrue(activation.HasDerivative);

            var clone = (ActivationRectifier)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.0 };

            activation.ActivationFunction(input, 0, 1);

            Assert.AreEqual(0.0, input[0], 0.5);

            // test derivative, wiki says this is logistic function (test may be wrong - jeroldhaas)
            input[0] = activation.DerivativeFunction(input[0], input[0]);
            Assert.AreEqual(1.0, input[0], 0.1);
        }
    }
}
