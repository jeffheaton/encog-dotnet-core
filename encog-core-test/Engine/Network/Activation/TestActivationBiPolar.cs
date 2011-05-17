using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationBiPolar
    {
        [TestMethod]
        public void TestActivation()
        {
            var activation = new ActivationBiPolar();
            Assert.IsTrue(activation.HasDerivative());

            var clone = (ActivationBiPolar)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.5, -0.5 };

            activation.ActivationFunction(input, 0, input.Length);

            Assert.AreEqual(1.0, input[0], 0.1);
            Assert.AreEqual(-1.0, input[1], 0.1);
		
		
        }
    }
}
