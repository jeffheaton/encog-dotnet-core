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
using System;

namespace Encog.Engine.Network.Activation
{
    [TestClass]
    public class TestActivationSIN
    {
        [TestMethod]
        public void TestSIN()
        {
            var activation = new ActivationSIN();
            Assert.IsTrue(activation.HasDerivative);

            var clone = activation.Clone();
            Assert.IsInstanceOfType(clone, typeof(ActivationSIN));

            double[] input = { 0.0, Math.PI / 4, Math.PI / 2 };

            activation.ActivationFunction(input, 0, 3); //it's actually Sin(2x)

            Assert.AreEqual(0.0, input[0], 0.01);
            Assert.AreEqual(1.0, input[1], 0.01);
            Assert.AreEqual(0.0, input[2], 0.01);

            // test derivative
            input[0] = activation.DerivativeFunction(0, input[0]);
            input[1] = activation.DerivativeFunction(Math.PI / 4, input[1]);
            input[2] = activation.DerivativeFunction(Math.PI / 2, input[2]);
            Assert.AreEqual(1.0, input[0], 0.01);
            Assert.AreEqual(0.0, input[1], 0.01);
            Assert.AreEqual(-1.0, input[2], 0.01);
        }
    }
}
