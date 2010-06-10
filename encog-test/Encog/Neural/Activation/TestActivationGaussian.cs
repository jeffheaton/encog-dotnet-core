using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.Activation;
using Encog.Persist.Persistors;

namespace encog_test.Encog.Neural.Activation
{
    [TestFixture]
    public class TestActivationGaussian
    {
        [Test]
        public void testGaussian()
        {
            ActivationGaussian activation = new ActivationGaussian(0.0, 0.5, 1.0);
            Assert.IsTrue(activation.HasDerivative);

            ActivationGaussian clone = (ActivationGaussian)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.0 };

            activation.ActivationFunction(input);

            Assert.AreEqual(0.5, input[0], 0.1);

            // this will throw an error if it does not work
            ActivationGaussianPersistor p = (ActivationGaussianPersistor)activation.CreatePersistor();

            // test derivative, should throw an error

            activation.DerivativeFunction(input);
            Assert.AreEqual(-33, (int)(input[0] * 100), 0.1);

            // test name and description
            // names and descriptions are not stored for these
            activation.Name = "name";
            activation.Description = "name";
            Assert.AreEqual(null, activation.Name);
            Assert.AreEqual(null, activation.Description);
        }
    }
}
