//
// Encog(tm) Core v3.2 - .Net Version (Unit Test)
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2013 Heaton Research, Inc.
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
using Encog.MathUtil.RBF;
using Encog.Neural.SOM.Training.Neighborhood;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    [TestClass]
    public class TestNeighborhood
    {
        [TestMethod]
        public void TestBubble()
        {
            var bubble = new NeighborhoodBubble(2);
            Assert.AreEqual(0.0, bubble.Function(5, 0), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 4), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 5), 0.1);
        }

        [TestMethod]
        public void TestSingle()
        {
            var bubble = new NeighborhoodSingle();
            Assert.AreEqual(0.0, bubble.Function(5, 0), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 5), 0.1);
        }

        [TestMethod]
        public void TestGaussian()
        {
            IRadialBasisFunction radial = new GaussianFunction(0.0, 1.0, 1.0);
            var bubble = new NeighborhoodRBF1D(radial);
            Assert.AreEqual(0.0, bubble.Function(5, 0), 0.1);
            Assert.AreEqual(1.0, bubble.Function(5, 5), 0.1);
            Assert.AreEqual(0.6, bubble.Function(5, 4), 0.1);
        }
    }
}
