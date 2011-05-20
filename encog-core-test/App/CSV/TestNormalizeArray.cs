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
using Encog.Util.Arrayutil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.App.CSV
{
    [TestClass]
    public class TestNormalizeArray
    {
        [TestMethod]
        public void TestNormalize()
        {
            var norm = new NormalizeArray();
            double[] input = {1, 5, 10};
            double[] output = norm.Process(input);
            Assert.AreEqual(3, output.Length);
            Assert.AreEqual(-1.0, output[0]);
            Assert.AreEqual(1.0, output[2]);
            Assert.AreEqual(1.0, norm.Stats.ActualLow);
            Assert.AreEqual(10.0, norm.Stats.ActualHigh);
        }
    }
}
