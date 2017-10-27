//
// Encog(tm) Core v3.3 - .Net Version (unit test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationLinear
    {
        [TestMethod]
        public void TestLinear()
        {
            var activation = new ActivationLinear();
            Assert.IsTrue(activation.HasDerivative);

            var clone = activation.Clone();
            Assert.IsInstanceOfType(clone, typeof(ActivationLinear));

            double[] input = {1, 2, 3};

            activation.ActivationFunction(input, 0, 3);

            Assert.AreEqual(1.0, input[0], 0.0);
            Assert.AreEqual(2.0, input[1], 0.0);
            Assert.AreEqual(3.0, input[2], 0.0);


            // test derivative, should not throw an error
            input[0] = activation.DerivativeFunction(input[0], input[0]);
            input[1] = activation.DerivativeFunction(input[1], input[1]);
            input[2] = activation.DerivativeFunction(input[2], input[2]);
            Assert.AreEqual(1.0, input[0], 0.0);
            Assert.AreEqual(1.0, input[1], 0.0);
            Assert.AreEqual(1.0, input[2], 0.0);
        }
    }
}
