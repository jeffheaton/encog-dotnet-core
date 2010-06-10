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
    public class TestActivationSoftMax
    {
        	[Test]
	public void testSIN()  {
		ActivationSIN activation = new ActivationSIN();
		Assert.IsTrue(activation.HasDerivative);

		ActivationSIN clone = (ActivationSIN) activation.Clone();
		Assert.IsNotNull(clone);

		double[] input = { 0.0  };

		activation.ActivationFunction(input);

		Assert.AreEqual(0.0, input[0], 0.1);		

		// test derivative, should throw an error
		activation.DerivativeFunction(input);
		Assert.AreEqual(1.0, input[0], 0.1);

		// test name and description
		// names and descriptions are not stored for these
		activation.Name = "name";
		activation.Description = "name";
		Assert.AreEqual(null, activation.Name);
        Assert.AreEqual(null, activation.Description);

	}
    }
}
