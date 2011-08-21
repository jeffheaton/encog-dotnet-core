//
// Encog(tm) Unit Tests v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
