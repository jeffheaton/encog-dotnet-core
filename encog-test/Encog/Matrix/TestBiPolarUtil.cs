// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Encog.MathUtil.Matrices;


namespace encog_test.TestMatrix
{
    [TestFixture]
    public class TestBiPolarUtil
    {
        [Test]
        public void Bipolar2double()
        {
            // test a 1x4
            bool[] boolData1 = { true, false, true, false };
            double[] checkData1 = { 1, -1, 1, -1 };
            Matrix matrix1 = Matrix.CreateRowMatrix(BiPolarUtil.Bipolar2double(boolData1));
            Matrix checkMatrix1 = Matrix.CreateRowMatrix(checkData1);
            Assert.IsTrue(matrix1.Equals(checkMatrix1));

            // test a 2x2
            bool[][] boolData2 = { 
                new bool[2]{ true, false }, 
                new bool[2]{ false, true } };
            double[][] checkData2 = { 
                new double[2] { 1, -1 }, 
                new double[2] { -1, 1 } };
            Matrix matrix2 = new Matrix(BiPolarUtil.Bipolar2double(boolData2));
            Matrix checkMatrix2 = new Matrix(checkData2);
            Assert.IsTrue(matrix2.Equals(checkMatrix2));
        }

        [Test]
        public void Double2bipolar()
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
                new double[2] { 1, -1 },
                new double[2] { -1, 1 } };
            bool[][] checkData2 =  { 
                new bool[2] { true, false }, 
                new bool[2] { false, true } };
            bool[][] result2 = BiPolarUtil.Double2bipolar(doubleData2);

            Assert.AreEqual(result2[0][0], checkData2[0][0]);
            Assert.AreEqual(result2[0][1], checkData2[0][1]);
            Assert.AreEqual(result2[1][0], checkData2[1][0]);
            Assert.AreEqual(result2[1][1], checkData2[1][1]);

        }

        [Test]
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
