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


namespace Encog.MathUtil.Matrices
{
    [TestClass]
    public class TestBiPolarUtil
    {
        [TestMethod]
        public void Bipolar2Double()
        {
            // test a 1x4
            bool[] boolData1 = { true, false, true, false };
            double[] checkData1 = { 1, -1, 1, -1 };
            Matrix matrix1 = Matrix.CreateRowMatrix(BiPolarUtil.Bipolar2double(boolData1));
            Matrix checkMatrix1 = Matrix.CreateRowMatrix(checkData1);
            Assert.IsTrue(matrix1.Equals(checkMatrix1));

            // test a 2x2
            bool[][] boolData2 = { 
                new[]{ true, false }, 
                new[]{ false, true } };
            double[][] checkData2 = { 
                new[] { 1.0, -1.0 }, 
                new[] { -1.0, 1.0 } };
            var matrix2 = new Matrix(BiPolarUtil.Bipolar2double(boolData2));
            var checkMatrix2 = new Matrix(checkData2);
            Assert.IsTrue(matrix2.Equals(checkMatrix2));
        }

        [TestMethod]
        public void Double2Bipolar()
        {
            // test a 1x4
            double[] doubleData1 = { 1, -1, 1, -1 };
            bool[] checkData1 = { true, false, true, false };
            bool[] result1 = BiPolarUtil.Double2bipolar(doubleData1);
            for (int i = 0; i < checkData1.Length; i++)
            {
                Assert.AreEqual(checkData1[i], result1[i]);
            }

            // test a 2x2
            double[][] doubleData2 =  { 
                new[] { 1.0, -1.0 },
                new[] { -1.0, 1.0 } };
            bool[][] checkData2 =  { 
                new[] { true, false }, 
                new[] { false, true } };
            var result2 = BiPolarUtil.Double2bipolar(doubleData2);

            Assert.AreEqual(result2[0][0], checkData2[0][0]);
            Assert.AreEqual(result2[0][1], checkData2[0][1]);
            Assert.AreEqual(result2[1][0], checkData2[1][0]);
            Assert.AreEqual(result2[1][1], checkData2[1][1]);

        }

        [TestMethod]
        public void Binary()
        {
            Assert.AreEqual(0.0, BiPolarUtil.NormalizeBinary(-1));
            Assert.AreEqual(1.0, BiPolarUtil.NormalizeBinary(2));
            Assert.AreEqual(1.0, BiPolarUtil.ToBinary(1));
            Assert.AreEqual(-1.0, BiPolarUtil.ToBiPolar(0));
            Assert.AreEqual(1.0, BiPolarUtil.ToNormalizedBinary(10));
        }

    }
}
