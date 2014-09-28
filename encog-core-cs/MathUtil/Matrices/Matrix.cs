//
// Encog(tm) Core v3.3 - .Net Version
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
using System;
using Encog.MathUtil.Matrices.Decomposition;
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
#if logging

#endif

namespace Encog.MathUtil.Matrices
{
    /// <summary>
    /// Matrix: This class implements a mathematical matrix.  Matrix
    /// math is very important to neural network processing.  Many
    /// of the classes developed in this book will make use of the
    /// matrix classes in this package.
    /// </summary>
    [Serializable]
    public class Matrix
    {
        /// <summary>
        /// Allows index access to the elements of the matrix.
        /// Warning: This can be a somewhat slow way to access the matrix.  
        /// Do not put this in performance critical loops.  Make sure to use
        /// the Data property and access the matrix array directly.
        /// </summary>
        /// <param name="row">The row to access.</param>
        /// <param name="col">The column to access.</param>
        /// <returns>The element at the specified position in the matrix.</returns>
        public double this[int row, int col]
        {
            get
            {
                Validate(row, col);
                return matrix[row][col];
            }
            set
            {
                Validate(row, col);
                if (double.IsInfinity(value) || double.IsNaN(value))
                {
                    throw new MatrixError("Trying to assign invalud number to matrix: "
                                          + value);
                }
                matrix[row][col] = value;
            }
        }

        /// <summary>
        /// Create a matrix that is a single column.
        /// </summary>
        /// <param name="input">A 1D array to make the matrix from.</param>
        /// <returns>A matrix that contains a single column.</returns>
        public static Matrix CreateColumnMatrix(double[] input)
        {
            var d = new double[input.Length][];
            for (int row = 0; row < d.Length; row++)
            {
                d[row] = new double[1];
                d[row][0] = input[row];
            }
            return new Matrix(d);
        }

        /// <summary>
        /// Create a matrix that is a single row.
        /// </summary>
        /// <param name="input">A 1D array to make the matrix from.</param>
        /// <returns>A matrix that contans a single row.</returns>
        public static Matrix CreateRowMatrix(double[] input)
        {
            var d = new double[1][];

            d[0] = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                d[0][i] = input[i];
            }

            return new Matrix(d);
        }

		/// <summary>
		/// Create a matrix that is a single row.
		/// </summary>
		/// <param name="input">A 1D array to make the matrix from.</param>
		/// <returns>A matrix that contans a single row.</returns>
		public static Matrix CreateRowMatrix(IMLData input)
		{
			var d = new double[1][];

			d[0] = new double[input.Count];

			for(int i = 0; i < input.Count; i++)
			{
				d[0][i] = input[i];
			}

			return new Matrix(d);
		}

		/// <summary>
        /// The matrix data, stored as a 2D array.
        /// </summary>
        private readonly double[][] matrix;

        /// <summary>
        /// Construct a matrix from a 2D boolean array.  Translate true to 1, false to -1.
        /// </summary>
        /// <param name="sourceMatrix">A 2D array to construcat the matrix from.</param>
        public Matrix(bool[][] sourceMatrix)
        {
            matrix = new double[sourceMatrix.Length][];
            for (int r = 0; r < Rows; r++)
            {
                matrix[r] = new double[sourceMatrix[r].Length];
                for (int c = 0; c < Cols; c++)
                {
                    if (sourceMatrix[r][c])
                    {
                        matrix[r][c] = 1;
                    }
                    else
                    {
                        matrix[r][c] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Construct a matrix from a 2D double array.
        /// </summary>
        /// <param name="sourceMatrix">A 2D double array.</param>
        public Matrix(double[][] sourceMatrix)
        {
            matrix = new double[sourceMatrix.Length][];
            for (int r = 0; r < Rows; r++)
            {
                matrix[r] = new double[sourceMatrix[r].Length];
                for (int c = 0; c < Cols; c++)
                {
                    matrix[r][c] = sourceMatrix[r][c];
                }
            }
        }

        /// <summary>
        /// Construct a blank matrix with the specified number of rows and columns.
        /// </summary>
        /// <param name="rows">How many rows.</param>
        /// <param name="cols">How many columns.</param>
        public Matrix(int rows, int cols)
        {
            matrix = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new double[cols];
            }
        }

        /// <summary>
        /// Add the specified value to the specified row and column of the matrix.
        /// </summary>
        /// <param name="row">The row to add to.</param>
        /// <param name="col">The column to add to.</param>
        /// <param name="value_ren">The value to add.</param>
        public void Add(int row, int col, double value_ren)
        {
            Validate(row, col);
            double newValue = matrix[row][col] + value_ren;
            matrix[row][col] = newValue;
        }

        /// <summary>
        /// Clear the matrix.
        /// </summary>
        public void Clear()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    matrix[r][c] = 0;
                }
            }
        }

        /// <summary>
        /// Clone the matrix.
        /// </summary>
        /// <returns>A cloned copy of the matrix.</returns>
        public object Clone()
        {
            return new Matrix(matrix);
        }

        /// <summary>
        /// Determine if this matrix is equal to another.  Use a precision of 10 decimal places.
        /// </summary>
        /// <param name="other">The other matrix to compare.</param>
        /// <returns>True if the two matrixes are equal.</returns>
        public override bool Equals(Object other)
        {
            if (other is Matrix)
                return equals((Matrix) other, 10);
            else
                return false;
        }

        /// <summary>
        /// Generate a hash code, this is just rows+cols.  Probably a better way to 
        /// do this, but I really don't see the Matrix class being used as the key to
        /// a hash table.  
        /// </summary>
        /// <returns>A hash code for the matrix.</returns>
        public override int GetHashCode()
        {
            return Rows + Cols;
        }


        /// <summary>
        /// Compare the matrix to another with the specified level of precision.
        /// </summary>
        /// <param name="matrix">The other matrix to compare.</param>
        /// <param name="precision">The number of decimal places of precision to use.</param>
        /// <returns>True if the two matrixes are equal.</returns>
        public bool equals(Matrix matrix, int precision)
        {
            if (precision < 0)
            {
                throw new MatrixError("Precision can't be a negative number.");
            }

            double test = Math.Pow(10.0, precision);
            if (double.IsInfinity(test) || (test > long.MaxValue))
            {
                throw new MatrixError("Precision of " + precision
                                      + " decimal places is not supported.");
            }

            precision = (int) Math.Pow(10, precision);

            double[][] otherMatrix = matrix.Data;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if ((long) (this.matrix[r][c]*precision) != (long) (otherMatrix[r][c]*precision))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Take the values of thie matrix from a packed array.
        /// </summary>
        /// <param name="array">The packed array to read the matrix from.</param>
        /// <param name="index">The index to begin reading at in the array.</param>
        /// <returns>The new index after this matrix has been read.</returns>
        public int FromPackedArray(double[] array, int index)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    matrix[r][c] = array[index++];
                }
            }

            return index;
        }

        /// <summary>
        /// Get one column from this matrix as a column matrix.
        /// </summary>
        /// <param name="col">The desired column.</param>
        /// <returns>The column matrix.</returns>
        public Matrix GetCol(int col)
        {
            if (col > Cols)
            {
                throw new MatrixError("Can't get column #" + col
                                      + " because it does not exist.");
            }

            var newMatrix = new double[Rows][];

            for (int row = 0; row < Rows; row++)
            {
                newMatrix[row] = new double[1];
                newMatrix[row][0] = matrix[row][col];
            }

            return new Matrix(newMatrix);
        }

        /// <summary>
        /// Get the number of columns in this matrix
        /// </summary>
        public int Cols
        {
            get { return matrix[0].Length; }
        }

        /// <summary>
        /// Get the specified row as a row matrix.
        /// </summary>
        /// <param name="row">The desired row.</param>
        /// <returns>A row matrix.</returns>
        public Matrix GetRow(int row)
        {
            if (row > Rows)
            {
                throw new MatrixError("Can't get row #" + row
                                      + " because it does not exist.");
            }

            var newMatrix = new double[1][];
            newMatrix[0] = new double[Cols];

            for (int col = 0; col < Cols; col++)
            {
                newMatrix[0][col] = matrix[row][col];
            }

            return new Matrix(newMatrix);
        }

        /// <summary>
        /// Get the number of rows in this matrix
        /// </summary>
        public int Rows
        {
            get { return matrix.GetUpperBound(0) + 1; }
        }


        /// <summary>
        /// Determine if this matrix is a vector.  A vector matrix only has a single row or column.
        /// </summary>
        /// <returns>True if this matrix is a vector.</returns>
        public bool IsVector()
        {
            if (Rows == 1)
            {
                return true;
            }
            else
            {
                return Cols == 1;
            }
        }

        /// <summary>
        /// Determine if all of the values in the matrix are zero.
        /// </summary>
        /// <returns>True if all of the values in the matrix are zero.</returns>
        public bool IsZero()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (matrix[row][col] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Fill the matrix with random values in the specified range.
        /// </summary>
        /// <param name="min">The minimum value for the random numbers.</param>
        /// <param name="max">The maximum value for the random numbers.</param>
        public void Ramdomize(double min, double max)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    matrix[r][c] = (ThreadSafeRandom.NextDouble()*(max - min)) + min;
                }
            }
        }


        /// <summary>
        /// Get the size fo the matrix.  This is thr rows times the columns.
        /// </summary>
        public int Size
        {
            get { return Rows*Cols; }
        }

        /// <summary>
        /// Sum all of the values in the matrix.
        /// </summary>
        /// <returns>The sum of all of the values in the matrix.</returns>
        public double Sum()
        {
            double result = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    result += matrix[r][c];
                }
            }
            return result;
        }

        /// <summary>
        /// Convert the matrix to a packed array.
        /// </summary>
        /// <returns>A packed array.</returns>
        public double[] ToPackedArray()
        {
            var result = new double[Rows*Cols];

            int index = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    result[index++] = matrix[r][c];
                }
            }

            return result;
        }

        /// <summary>
        /// Validate that the specified row and column are inside of the range of the matrix.
        /// </summary>
        /// <param name="row">The row to check.</param>
        /// <param name="col">The column to check.</param>
        private void Validate(int row, int col)
        {
            if ((row >= Rows) || (row < 0))
            {
                throw new MatrixError("The row:" + row + " is out of range:"
                                      + Rows);
            }

            if ((col >= Cols) || (col < 0))
            {
                throw new MatrixError("The col:" + col + " is out of range:"
                                      + Cols);
            }
        }

        /// <summary>
        /// Add the specified matrix to this matrix.  This will modify the matrix
        /// to hold the result of the addition.
        /// </summary>
        /// <param name="matrix">The matrix to add.</param>
        public void Add(Matrix matrix)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Add(row, col, matrix[row, col]);
                }
            }
        }

        /// <summary>
        /// Set every value in the matrix to the specified value.
        /// </summary>
        /// <param name="value_ren">The value to set the matrix to.</param>
        public void Set(double value_ren)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    matrix[row][col] = value_ren;
                }
            }
        }

        /// <summary>
        /// Get the matrix array for this matrix.
        /// </summary>
        public double[][] Data
        {
            get { return matrix; }
        }

        /// <summary>
        /// Set the values from the other matrix into this one.
        /// </summary>
        /// <param name="other">The source matrix.</param>
        public void Set(Matrix other)
        {
            double[][] target = Data;
            double[][] source = other.Data;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    target[r][c] = source[r][c];
        }

        /// <summary>
        /// Make a copy of this matrix as an array.
        /// </summary>
        /// <returns>An array copy of this matrix.</returns>
        public double[][] GetArrayCopy()
        {
            var result = new double[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                result[row] = new double[Cols];
                for (int col = 0; col < Cols; col++)
                {
                    result[row][col] = matrix[row][col];
                }
            }
            return result;
        }

        /// <summary>
        /// Get a submatrix.
        /// </summary>
        /// <param name="i0">Initial row index.</param>
        /// <param name="i1">Final row index.</param>
        /// <param name="j0">Initial column index.</param>
        /// <param name="j1">Final column index.</param>
        /// <returns>The specified submatrix.</returns>
        public Matrix GetMatrix(
            int i0,
            int i1,
            int j0,
            int j1)
        {
            var result = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
            double[][] b = result.Data;
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        b[i - i0][j - j0] = matrix[i][j];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
            return result;
        }

        /// <summary>
        /// Get a submatrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="c">Array of column indices.</param>
        /// <returns>The specified submatrix.</returns>
        public Matrix GetMatrix(int[] r, int[] c)
        {
            var result = new Matrix(r.Length, c.Length);
            double[][] b = result.Data;
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        b[i][j] = matrix[r[i]][c[j]];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
            return result;
        }

        /// <summary>
        /// Get a submatrix.
        /// </summary>
        /// <param name="i0">Initial row index.</param>
        /// <param name="i1">Final row index.</param>
        /// <param name="c">Array of column indices.</param>
        /// <returns>The specified submatrix.</returns>
        public Matrix GetMatrix(
            int i0,
            int i1,
            int[] c)
        {
            var result = new Matrix(i1 - i0 + 1, c.Length);
            double[][] b = result.Data;
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        b[i - i0][j] = matrix[i][c[j]];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
            return result;
        }

        /// <summary>
        /// Get a submatrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <returns>The specified submatrix.</returns>
        public Matrix GetMatrix(
            int[] r,
            int j0,
            int j1)
        {
            var result = new Matrix(r.Length, j1 - j0 + 1);
            double[][] b = result.Data;
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        b[i][j - j0] = matrix[r[i]][j];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
            return result;
        }

        /// <summary>
        /// Multiply every row by the specified vector.
        /// </summary>
        /// <param name="vector">The vector to multiply by.</param>
        /// <param name="result">The result to hold the values.</param>
        public void Multiply(double[] vector, double[] result)
        {
            for (int i = 0; i < Rows; i++)
            {
                result[i] = 0;
                for (int j = 0; j < Cols; j++)
                {
                    result[i] += matrix[i][j]*vector[j];
                }
            }
        }

        /// <summary>
        /// The matrix inverted.
        /// </summary>
        /// <returns>The inverse of the matrix.</returns>
        public Matrix Inverse()
        {
            return Solve(MatrixMath.Identity(Rows));
        }


        /// <summary>
        /// Solve A*X = B
        /// </summary>
        /// <param name="b">right hand side.</param>
        /// <returns>Solution if A is square, least squares solution otherwise.</returns>
        public Matrix Solve(Matrix b)
        {
            if (Rows == Cols)
            {
                return (new LUDecomposition(this)).Solve(b);
            }
            else
            {
                return (new QRDecomposition(this)).Solve(b);
            }
        }

        /// <summary>
        /// Set a submatrix.
        /// </summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <param name="x">A(i0:i1,j0:j1)</param>
        public void SetMatrix(
            int i0,
            int i1,
            int j0,
            int j1,
            Matrix x)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        matrix[i][j] = x[i - i0, j - j0];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
        }


        /// <summary>
        /// Set a submatrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="c">Array of column indices.</param>
        /// <param name="x">The matrix to set.</param>
        public void SetMatrix(
            int[] r,
            int[] c,
            Matrix x)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        matrix[r[i]][c[j]] = x[i, j];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
        }

        /// <summary>
        /// Set a submatrix.
        /// </summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="j0">Initial column index</param>
        /// <param name="j1">Final column index</param>
        /// <param name="x">A(r(:),j0:j1)</param>
        public void SetMatrix(
            int[] r,
            int j0,
            int j1,
            Matrix x)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        matrix[r[i]][j] = x[i, j - j0];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
        }

        /// <summary>
        /// Set a submatrix. 
        /// </summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="c">Array of column indices.</param>
        /// <param name="x">The submatrix.</param>
        public void SetMatrix(
            int i0,
            int i1,
            int[] c,
            Matrix x)
        {
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        matrix[i][c[j]] = x[i - i0, j];
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new MatrixError("Submatrix indices");
            }
        }

        /// <summary>
        /// Randomize the matrix.
        /// </summary>
        ///
        /// <param name="min">Minimum random value.</param>
        /// <param name="max">Maximum random value.</param>
        public void Randomize(double min, double max)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    matrix[row][col] = RangeRandomizer.Randomize(min, max);
                }
            }
        }
    }
}
