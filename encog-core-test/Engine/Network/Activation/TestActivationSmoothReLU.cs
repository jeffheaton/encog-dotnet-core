
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationSmoothReLU
    {
        [TestMethod]
        public void TestRectifier()
        {
            var activation = new ActivationSmoothReLU();
            Assert.IsTrue(activation.HasDerivative);

            var clone = (ActivationSmoothReLU)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.0 };

            activation.ActivationFunction(input, 0, 1);

            Assert.AreEqual(0.69314718055994529, input[0], EncogFramework.DefaultDoubleEqual);

            // test derivative, wiki says this is logistic function (test may be wrong - jeroldhaas)
            input[0] = activation.DerivativeFunction(input[0], input[0]);
            Assert.AreEqual(0.66666666666666666, input[0], EncogFramework.DefaultDoubleEqual);
        }
    }
}
