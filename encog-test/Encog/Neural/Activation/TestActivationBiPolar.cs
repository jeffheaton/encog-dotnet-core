using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.Neural.Activation;
using Encog.Persist.Persistors;
using Encog;

namespace encog_test.Encog.Neural.Activation
{
    [TestFixture]
    public class TestActivationBiPolar
    {
        [Test]
        public void testBiPolar()
        {
            ActivationBiPolar activation = new ActivationBiPolar();
            Assert.IsFalse(activation.HasDerivative);

            ActivationBiPolar clone = (ActivationBiPolar)activation.Clone();
            Assert.IsNotNull(clone);

            double[] input = { 0.5, -0.5 };

            activation.ActivationFunction(input);

            Assert.AreEqual(1.0, input[0], 0.1);
            Assert.AreEqual(-1.0, input[1], 0.1);

            // test derivative, should throw an error
            try
            {
                activation.DerivativeFunction(input);
                Assert.IsTrue(false);// mark an error
            }
            catch (EncogError )
            {
                // good, this should happen
            }

            // test name and description
            // names and descriptions are not stored for these
            activation.Name = "name";
            activation.Description = "name";
            Assert.AreEqual(null, activation.Name);
            Assert.AreEqual(null, activation.Description);


        }
    }
}
