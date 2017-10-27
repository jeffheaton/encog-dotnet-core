
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationSmoothReLU
    {
        [TestMethod]
        public void TestSmoothReLU()
        {
            var activation = new ActivationSmoothReLU();
            Assert.IsTrue(activation.HasDerivative);

            var clone = activation.Clone();
            Assert.IsInstanceOfType(clone, typeof(ActivationSmoothReLU));

            double[] input = { 0.0 };

            activation.ActivationFunction(input, 0, 1);

            Assert.AreEqual(0.69314718055994529, input[0], EncogFramework.DefaultDoubleEqual);

            input[0] = activation.DerivativeFunction(0, input[0]);
            Assert.AreEqual(0.5, input[0], EncogFramework.DefaultDoubleEqual);
        }
    }
}
