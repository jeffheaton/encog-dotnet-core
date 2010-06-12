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
using Encog.MathUtil.Matrices;
using NUnit.Framework;


namespace encog_test.TestMatrix
{
    [TestFixture]
    public class TestMatrix
    {
        [Test]
        public void RowsAndCols()
        {
            Matrix matrix = new Matrix(6, 3);
            Assert.AreEqual(matrix.Rows, 6);
            Assert.AreEqual(matrix.Cols, 3);

            matrix[1, 2] = 1.5;
            Assert.AreEqual(matrix[1, 2], 1.5);
        }

        [Test]
        public void RowAndColRangeUnder()
        {
            Matrix matrix = new Matrix(6, 3);

            // make sure set registers error on under-bound row
            try
            {
                matrix[-1, 0] = 1;
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }

            // make sure set registers error on under-bound col
            try
            {
                matrix[0, -1] = 1;
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }

            // make sure get registers error on under-bound row
            try
            {
                double d = matrix[-1, 0];
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }

            // make sure set registers error on under-bound col
            try
            {
                double d = matrix[0, -1];
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }
        }

        [Test]
        public void RowAndColRangeOver()
        {
            Matrix matrix = new Matrix(6, 3);

            // make sure set registers error on under-bound row
            try
            {
                matrix[6, 0] = 1;
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }

            // make sure set registers error on under-bound col
            try
            {
                matrix[0, 3] = 1;
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }

            // make sure get registers error on under-bound row
            try
            {
                double d = matrix[6, 0];
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }

            // make sure set registers error on under-bound col
            try
            {
                double d = matrix[0, 3];
                Assert.IsTrue(false); // should have thrown an exception
            }
            catch (MatrixError)
            {
            }
        }

        [Test]
        public void MatrixConstruct()
        {
            double[][] m = {
				new double[4] {1,2,3,4},
				new double[4] {5,6,7,8},
				new double[4] {9,10,11,12},
				new double[4] {13,14,15,16} };
            Matrix matrix = new Matrix(m);
            Assert.AreEqual(matrix.Rows, 4);
            Assert.AreEqual(matrix.Cols, 4);
        }

        [Test]
        public void MatrixEquals()
        {
            double[][] m1 = {
				new double[2] {1,2},
				new double[2] {3,4} };

            double[][] m2 = {
				new double[2] {0,2},
				new double[2] {3,4} };

            Matrix matrix1 = new Matrix(m1);
            Matrix matrix2 = new Matrix(m1);

            Assert.IsTrue(matrix1.Equals(matrix2));

            matrix2 = new Matrix(m2);

            Assert.IsFalse(matrix1.Equals(matrix2));
        }

        [Test]
        public void MatrixEqualsPrecision()
        {
            double[][] m1 = {
				new double[2] {1.1234,2.123},
				new double[2] {3.123,4.123} };

            double[][] m2 = {
				new double[2] {1.123,2.123},
				new double[2] {3.123,4.123} };

            Matrix matrix1 = new Matrix(m1);
            Matrix matrix2 = new Matrix(m2);

            Assert.IsTrue(matrix1.equals(matrix2, 3));
            Assert.IsFalse(matrix1.equals(matrix2, 4));

            double[][] m3 = {
				new double[2] {1.1,2.1},
				new double[2] {3.1,4.1} };

            double[][] m4 = {
				new double[2] {1.2,2.1},
				new double[2] {3.1,4.1} };

            Matrix matrix3 = new Matrix(m3);
            Matrix matrix4 = new Matrix(m4);
            Assert.IsTrue(matrix3.equals(matrix4, 0));
            Assert.IsFalse(matrix3.equals(matrix4, 1));

            try
            {
                matrix3.equals(matrix4, -1);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {

            }

            try
            {
                matrix3.equals(matrix4, 19);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {

            }

        }

        [Test]
        public void MatrixMultiply()
        {
            double[][] a = {
				new double[3] {1,0,2},
				new double[3] {-1,3,1}
		};

            double[][] b = {
				new double[2] {3,1},
				new double[2] {2,1},
				new double[2] {1,0}
		};

            double[][] c = {
				new double[2] {5,1},
				new double[2] {4,2}
		};

            Matrix matrixA = new Matrix(a);
            Matrix matrixB = new Matrix(b);
            Matrix matrixC = new Matrix(c);

            Matrix result = (Matrix)matrixA.Clone();
            result = MatrixMath.Multiply(matrixA, matrixB);

            Assert.IsTrue(result.Equals(matrixC));

            double[][] a2 = {
				new double[4] {1,2,3,4},
				new double[4] {5,6,7,8}
		};

            double[][] b2 = {
				new double[3] {1,2,3},
				new double[3] {4,5,6},
				new double[3] {7,8,9},
				new double[3] {10,11,12}
		};

            double[][] c2 = {
				new double[3] {70,80,90},
				new double[3] {158,184,210}
		};

            matrixA = new Matrix(a2);
            matrixB = new Matrix(b2);
            matrixC = new Matrix(c2);

            result = MatrixMath.Multiply(matrixA, matrixB);
            Assert.IsTrue(result.Equals(matrixC));

            result = (Matrix)matrixB.Clone();
            try
            {
                MatrixMath.Multiply(matrixB, matrixA);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {

            }
        }

        [Test]
        public void Boolean()
        {
            bool[][] matrixDataBoolean = { 
				new Boolean[2] {true,false},
				new Boolean[2] {false,true}
		};

            double[][] matrixDataDouble = {
				new double[2] {1.0,-1.0},
				new double[2] {-1.0,1.0},
		};

            Matrix matrixBoolean = new Matrix(matrixDataBoolean);
            Matrix matrixDouble = new Matrix(matrixDataDouble);

            Assert.IsTrue(matrixBoolean.Equals(matrixDouble));
        }

        [Test]
        public void GetRow()
        {
            double[][] matrixData1 = {
				new double[2] {1.0,2.0},
				new double[2] {3.0,4.0}
		};
            double[][] matrixData2 = {
				new double[2] {3.0,4.0}
		};

            Matrix matrix1 = new Matrix(matrixData1);
            Matrix matrix2 = new Matrix(matrixData2);

            Matrix matrixRow = matrix1.GetRow(1);
            Assert.IsTrue(matrixRow.Equals(matrix2));

            try
            {
                matrix1.GetRow(3);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void GetCol()
        {
            double[][] matrixData1 = {
				new double[2] {1.0,2.0},
				new double[2] {3.0,4.0}
		};
            double[][] matrixData2 = {
				new double[1] {2.0},
				new double[1] {4.0}
		};

            Matrix matrix1 = new Matrix(matrixData1);
            Matrix matrix2 = new Matrix(matrixData2);

            Matrix matrixCol = matrix1.GetCol(1);
            Assert.IsTrue(matrixCol.Equals(matrix2));

            try
            {
                matrix1.GetCol(3);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void Zero()
        {
            double[][] doubleData = { 
                new double[2] { 0, 0 }, 
                new double[2] { 0, 0 } };
            Matrix matrix = new Matrix(doubleData);
            Assert.IsTrue(matrix.IsZero());
        }

        [Test]
        public void Sum()
        {
            double[][] doubleData = { 
                new double[2] { 1, 2 }, 
                new double[2] { 3, 4 } };
            Matrix matrix = new Matrix(doubleData);
            Assert.AreEqual((int)matrix.Sum(), 1 + 2 + 3 + 4);
        }

        [Test]
        public void RowMatrix()
        {
            double[] matrixData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrix = Matrix.CreateRowMatrix(matrixData);
            Assert.AreEqual(matrix[0, 0], 1.0);
            Assert.AreEqual(matrix[0, 1], 2.0);
            Assert.AreEqual(matrix[0, 2], 3.0);
            Assert.AreEqual(matrix[0, 3], 4.0);
        }

        [Test]
        public void ColumnMatrix()
        {
            double[] matrixData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrix = Matrix.CreateColumnMatrix(matrixData);
            Assert.AreEqual(matrix[0, 0], 1.0);
            Assert.AreEqual(matrix[1, 0], 2.0);
            Assert.AreEqual(matrix[2, 0], 3.0);
            Assert.AreEqual(matrix[3, 0], 4.0);
        }

        [Test]
        public void Add()
        {
            double[] matrixData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrix = Matrix.CreateColumnMatrix(matrixData);
            matrix.Add(0, 0, 1);
            Assert.AreEqual(matrix[0, 0], 2.0);
        }

        [Test]
        public void Clear()
        {
            double[] matrixData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrix = Matrix.CreateColumnMatrix(matrixData);
            matrix.Clear();
            Assert.AreEqual(matrix[0, 0], 0.0);
            Assert.AreEqual(matrix[1, 0], 0.0);
            Assert.AreEqual(matrix[2, 0], 0.0);
            Assert.AreEqual(matrix[3, 0], 0.0);
        }

        [Test]
        public void IsVector()
        {
            double[] matrixData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrixCol = Matrix.CreateColumnMatrix(matrixData);
            Matrix matrixRow = Matrix.CreateRowMatrix(matrixData);
            Assert.IsTrue(matrixCol.IsVector());
            Assert.IsTrue(matrixRow.IsVector());
            double[][] matrixData2 = { 
                new double[2] { 1.0, 2.0 }, 
                new double[2] { 3.0, 4.0 } };
            Matrix matrix = new Matrix(matrixData2);
            Assert.IsFalse(matrix.IsVector());
        }

        [Test]
        public void IsZero()
        {
            double[] matrixData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrix = Matrix.CreateColumnMatrix(matrixData);
            Assert.IsFalse(matrix.IsZero());
            double[] matrixData2 = { 0.0, 0.0, 0.0, 0.0 };
            Matrix matrix2 = Matrix.CreateColumnMatrix(matrixData2);
            Assert.IsTrue(matrix2.IsZero());

        }

        [Test]
        public void PackedArray()
        {
            double[][] matrixData = { 
                new double[2] { 1.0, 2.0 }, 
                new double[2] { 3.0, 4.0 } };
            Matrix matrix = new Matrix(matrixData);
            Double[] matrixData2 = matrix.ToPackedArray();
            Assert.AreEqual(4, matrixData2.Length);
            Assert.AreEqual(1.0, matrix[0, 0]);
            Assert.AreEqual(2.0, matrix[0, 1]);
            Assert.AreEqual(3.0, matrix[1, 0]);
            Assert.AreEqual(4.0, matrix[1, 1]);

            Matrix matrix2 = new Matrix(2, 2);
            matrix2.FromPackedArray(matrixData2, 0);
            Assert.IsTrue(matrix.Equals(matrix2));
        }

        [Test]
        public void PackedArray2()
        {
            Double[] data = { 1.0, 2.0, 3.0, 4.0 };
            Matrix matrix = new Matrix(1, 4);
            matrix.FromPackedArray(data, 0);
            Assert.AreEqual(1.0, matrix[0, 0]);
            Assert.AreEqual(2.0, matrix[0, 1]);
            Assert.AreEqual(3.0, matrix[0, 2]);
        }

        [Test]
        public void Size()
        {
            double[][] data = { 
                new double[2] { 1.0, 2.0 }, 
                new double[2] { 3.0, 4.0 } };
            Matrix matrix = new Matrix(data);
            Assert.AreEqual(4, matrix.Size);
        }

        [Test]
        public void Randomize()
        {
            const double MIN = 1.0;
            const double MAX = 10.0;
            Matrix matrix = new Matrix(10, 10);
            matrix.Ramdomize(MIN, MAX);
            Double[] array = matrix.ToPackedArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < MIN || array[i] > MAX)
                    Assert.IsFalse(true);
            }
        }

        [Test]
        public void VectorLength()
        {
            double[] vectorData = { 1.0, 2.0, 3.0, 4.0 };
            Matrix vector = Matrix.CreateRowMatrix(vectorData);
            Assert.AreEqual(5, (int)MatrixMath.VectorLength(vector));

            Matrix nonVector = new Matrix(2, 2);
            try
            {
                MatrixMath.VectorLength(nonVector);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {

            }
        }
    }
}

