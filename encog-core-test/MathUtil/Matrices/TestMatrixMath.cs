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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Encog.MathUtil.Matrices
{
    [TestClass]
    public class TestMatrixMath
    {
        [TestMethod]
        public void Inverse()
        {
            double[][] matrixData1 = {
                                         new[] {1.0, 2.0, 3.0, 4.0}
                                     };
            double[][] matrixData2 = {
                                         new[] {1.0},
                                         new[] {2.0},
                                         new[] {3.0},
                                         new[] {4.0}
                                     };

            var matrix1 = new Matrix(matrixData1);
            var checkMatrix = new Matrix(matrixData2);

            Matrix matrix2 = MatrixMath.Transpose(matrix1);

            Assert.IsTrue(matrix2.Equals(checkMatrix));
        }

        [TestMethod]
        public void DotProduct()
        {
            double[][] matrixData1 = {new[] {1.0, 2.0, 3.0, 4.0}};
            double[][] matrixData2 = {
                                         new[] {5.0},
                                         new[] {6.0},
                                         new[] {7.0},
                                         new[] {8.0}
                                     };

            var matrix1 = new Matrix(matrixData1);
            var matrix2 = new Matrix(matrixData2);

            double dotProduct = MatrixMath.DotProduct(matrix1, matrix2);

            Assert.AreEqual(dotProduct, 70.0);

            // test dot product errors
            double[][] nonVectorData = {
                                           new[] {1.0, 2.0},
                                           new[] {3.0, 4.0}
                                       };
            double[][] differentLengthData = {
                                                 new[] {1.0}
                                             };
            var nonVector = new Matrix(nonVectorData);
            var differentLength = new Matrix(differentLengthData);

            try
            {
                MatrixMath.DotProduct(matrix1, nonVector);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
            }

            try
            {
                MatrixMath.DotProduct(nonVector, matrix2);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
            }

            try
            {
                MatrixMath.DotProduct(matrix1, differentLength);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
            }
        }

        [TestMethod]
        public void Multiply()
        {
            double[][] matrixData1 = {
                                         new[] {1.0, 4.0},
                                         new[] {2.0, 5.0},
                                         new[] {3.0, 6.0}
                                     };
            double[][] matrixData2 = {
                                         new[] {7.0, 8.0, 9.0},
                                         new[] {10.0, 11.0, 12.0}
                                     };


            double[][] matrixData3 = {
                                         new[] {47.0, 52.0, 57.0},
                                         new[] {64.0, 71.0, 78.0},
                                         new[] {81.0, 90.0, 99.0}
                                     };

            var matrix1 = new Matrix(matrixData1);
            var matrix2 = new Matrix(matrixData2);

            var matrix3 = new Matrix(matrixData3);

            Matrix result = MatrixMath.Multiply(matrix1, matrix2);

            Assert.IsTrue(result.Equals(matrix3));
        }

        [TestMethod]
        public void VerifySame()
        {
            double[][] dataBase = {
                                      new[] {1.0, 2.0},
                                      new[] {3.0, 4.0}
                                  };
            double[][] dataTooManyRows = {
                                             new[] {1.0, 2.0},
                                             new[] {3.0, 4.0},
                                             new[] {5.0, 6.0}
                                         };
            double[][] dataTooManyCols = {
                                             new[] {1.0, 2.0, 3.0},
                                             new[] {4.0, 5.0, 6.0}
                                         };
            var baseMatrix = new Matrix(dataBase);
            var tooManyRows = new Matrix(dataTooManyRows);
            var tooManyCols = new Matrix(dataTooManyCols);
            MatrixMath.Add(baseMatrix, baseMatrix);
            try
            {
                MatrixMath.Add(baseMatrix, tooManyRows);
                Assert.IsFalse(true);
            }
            catch (MatrixError)
            {
            }
            try
            {
                MatrixMath.Add(baseMatrix, tooManyCols);
                Assert.IsFalse(true);
            }
            catch (MatrixError)
            {
            }
        }

        [TestMethod]
        public void Divide()
        {
            double[][] data = {
                                  new[] {2.0, 4.0},
                                  new[] {6.0, 8.0}
                              };
            var matrix = new Matrix(data);
            Matrix result = MatrixMath.Divide(matrix, 2.0);
            Assert.AreEqual(1.0, result[0, 0]);
        }

        [TestMethod]
        public void Identity()
        {
            try
            {
                MatrixMath.Identity(0);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
            }

            double[][] checkData = {
                                       new[] {1.0, 0.0},
                                       new[] {0.0, 1.0}
                                   };
            var check = new Matrix(checkData);
            Matrix matrix = MatrixMath.Identity(2);
            Assert.IsTrue(check.Equals(matrix));
        }

        [TestMethod]
        public void MultiplyScalar()
        {
            double[][] data = {
                                  new[] {2.0, 4.0},
                                  new[] {6.0, 8.0}
                              };
            var matrix = new Matrix(data);
            Matrix result = MatrixMath.Multiply(matrix, 2.0);
            Assert.AreEqual(4.0, result[0, 0]);
        }

        [TestMethod]
        public void DeleteRow()
        {
            double[][] origData = {
                                      new[] {1.0, 2.0},
                                      new[] {3.0, 4.0}
                                  };
            double[][] checkData = {new[] {3.0, 4.0}};
            var orig = new Matrix(origData);
            Matrix matrix = MatrixMath.DeleteRow(orig, 0);
            var check = new Matrix(checkData);
            Assert.IsTrue(check.Equals(matrix));

            try
            {
                MatrixMath.DeleteRow(orig, 10);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
            }
        }

        [TestMethod]
        public void DeleteCol()
        {
            double[][] origData = {
                                      new[] {1.0, 2.0},
                                      new[] {3.0, 4.0}
                                  };
            double[][] checkData = {
                                       new[] {2.0},
                                       new[] {4.0}
                                   };
            var orig = new Matrix(origData);
            Matrix matrix = MatrixMath.DeleteCol(orig, 0);
            var check = new Matrix(checkData);
            Assert.IsTrue(check.Equals(matrix));

            try
            {
                MatrixMath.DeleteCol(orig, 10);
                Assert.IsTrue(false);
            }
            catch (MatrixError)
            {
            }
        }

        [TestMethod]
        public void Copy()
        {
            double[][] data = {
                                  new[] {1.0, 2.0},
                                  new[] {3.0, 4.0}
                              };
            var source = new Matrix(data);
            var target = new Matrix(2, 2);
            MatrixMath.Copy(source, target);
            Assert.IsTrue(source.Equals(target));
        }
    }
}