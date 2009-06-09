using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Data;

namespace Encog.Matrix
{
    /**
     * This class can perform many different mathematical operations on matrixes.
     * The matrixes passed in will not be modified, rather a new matrix, with the
     * operation performed, will be returned.
     */
    /// <summary>
    /// MatrixMath: This class can perform many different mathematical
    /// operations on matrixes.
    /// </summary>
    public class MatrixMath
    {
        /// <summary>
        /// Add two matrixes together, producing a third.
        /// </summary>
        /// <param name="a">The first matrix to add.</param>
        /// <param name="b">The second matrix to add.</param>
        /// <returns>The two matrixes added together.</returns>
        public static Matrix Add(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
            {
                throw new MatrixError(
                        "To add the matrixes they must have the same number of rows and columns.  Matrix a has "
                                + a.Rows
                                + " rows and matrix b has "
                                + b.Rows + " rows.");
            }

            if (a.Cols != b.Cols)
            {
                throw new MatrixError(
                        "To add the matrixes they must have the same number of rows and columns.  Matrix a has "
                                + a.Cols
                                + " cols and matrix b has "
                                + b.Cols + " cols.");
            }

            double[,] result = new double[a.Rows, a.Cols];

            for (int resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                for (int resultCol = 0; resultCol < a.Cols; resultCol++)
                {
                    result[resultRow, resultCol] = a[resultRow, resultCol]
                            + b[resultRow, resultCol];
                }
            }

            return new Matrix(result);
        }

        /// <summary>
        /// Copy the source matrix to the target matrix.  Both matrixes must have the same dimensions.
        /// </summary>
        /// <param name="source">The source matrix.</param>
        /// <param name="target">The target matrix.</param>
        public static void Copy(Matrix source, Matrix target)
        {
            for (int row = 0; row < source.Rows; row++)
            {
                for (int col = 0; col < source.Cols; col++)
                {
                    target[row, col] = source[row, col];
                }
            }

        }

        /// <summary>
        /// Delete a single column from a matrix.  A new matrix, with the delete is returned.
        /// </summary>
        /// <param name="matrix">The matrix to delete from.</param>
        /// <param name="deleted">The column to delete.</param>
        /// <returns>The matrix, with the delete.</returns>
        public static Matrix DeleteCol(Matrix matrix, int deleted)
        {
            if (deleted >= matrix.Cols)
            {
                throw new MatrixError("Can't delete column " + deleted
                        + " from matrix, it only has " + matrix.Cols
                        + " columns.");
            }
            double[,] newMatrix = new double[matrix.Rows, matrix
                    .Cols - 1];

            for (int row = 0; row < matrix.Rows; row++)
            {
                int targetCol = 0;

                for (int col = 0; col < matrix.Cols; col++)
                {
                    if (col != deleted)
                    {
                        newMatrix[row, targetCol] = matrix[row, col];
                        targetCol++;
                    }

                }

            }
            return new Matrix(newMatrix);
        }

        /// <summary>
        /// Delete a row from a matrix.  A new matrix, with the row deleted, is returned.
        /// </summary>
        /// <param name="matrix">The matrix to delete from.</param>
        /// <param name="deleted">The row to delete.</param>
        /// <returns>The matrix, with the row deleted.</returns>
        public static Matrix DeleteRow(Matrix matrix, int deleted)
        {
            if (deleted >= matrix.Rows)
            {
                throw new MatrixError("Can't delete row " + deleted
                        + " from matrix, it only has " + matrix.Rows
                        + " rows.");
            }
            double[,] newMatrix = new double[matrix.Rows - 1, matrix
                    .Cols];
            int targetRow = 0;
            for (int row = 0; row < matrix.Rows; row++)
            {
                if (row != deleted)
                {
                    for (int col = 0; col < matrix.Cols; col++)
                    {
                        newMatrix[targetRow, col] = matrix[row, col];
                    }
                    targetRow++;
                }
            }
            return new Matrix(newMatrix);
        }

        /// <summary>
        /// Divide every cell in the matrix by the specified number.
        /// </summary>
        /// <param name="a">The matrix to divide.</param>
        /// <param name="b">The number to divide by.</param>
        /// <returns>The divided matrix.</returns>
        public static Matrix Divide(Matrix a, double b)
        {
            double[,] result = new double[a.Rows, a.Cols];
            for (int row = 0; row < a.Rows; row++)
            {
                for (int col = 0; col < a.Cols; col++)
                {
                    result[row, col] = a[row, col] / b;
                }
            }
            return new Matrix(result);
        }

        /// <summary>
        /// Compute the dot product for two matrixes.  Note: both matrixes must be vectors.
        /// </summary>
        /// <param name="a">The first matrix, must be a vector.</param>
        /// <param name="b">The second matrix, must be a vector.</param>
        /// <returns>The dot product of the two matrixes.</returns>
        public static double DotProduct(Matrix a, Matrix b)
        {
            if (!a.IsVector() || !b.IsVector())
            {
                throw new MatrixError(
                        "To take the dot product, both matrixes must be vectors.");
            }

            Double[] aArray = a.ToPackedArray();
            Double[] bArray = b.ToPackedArray();

            if (aArray.Length != bArray.Length)
            {
                throw new MatrixError(
                        "To take the dot product, both matrixes must be of the same length.");
            }

            double result = 0;
            int length = aArray.Length;

            for (int i = 0; i < length; i++)
            {
                result += aArray[i] * bArray[i];
            }

            return result;
        }

        /// <summary>
        /// Create an identiry matrix, of the specified size.  An identity matrix is always square.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Matrix Identity(int size)
        {
            if (size < 1)
            {
                throw new MatrixError("Identity matrix must be at least of size 1.");
            }

            Matrix result = new Matrix(size, size);

            for (int i = 0; i < size; i++)
            {
                result[i, i] = 1;
            }

            return result;
        }

        /// <summary>
        /// Multiply every cell in the matrix by the specified value.
        /// </summary>
        /// <param name="a">Multiply every cell in a matrix by the specified value.</param>
        /// <param name="b">The value to multiply by.</param>
        /// <returns>The new multiplied matrix.</returns>
        public static Matrix Multiply(Matrix a, double b)
        {
            double[,] result = new double[a.Rows, a.Cols];
            for (int row = 0; row < a.Rows; row++)
            {
                for (int col = 0; col < a.Cols; col++)
                {
                    result[row, col] = a[row, col] * b;
                }
            }
            return new Matrix(result);
        }

        /// <summary>
        /// Multiply two matrixes.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix Multiply(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
            {
                throw new MatrixError(
                        "To use ordinary matrix multiplication the number of columns on the first matrix must mat the number of rows on the second.");
            }

            double[,] result = new double[a.Rows, b.Cols];

            for (int resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                for (int resultCol = 0; resultCol < b.Cols; resultCol++)
                {
                    double value = 0;

                    for (int i = 0; i < a.Cols; i++)
                    {

                        value += a[resultRow, i] * b[i, resultCol];
                    }
                    result[resultRow, resultCol] = value;
                }
            }

            return new Matrix(result);
        }

        /// <summary>
        /// Subtract one matrix from another.  The two matrixes must have the same number of rows and columns.
        /// </summary>
        /// <param name="a">The first matrix.</param>
        /// <param name="b">The second matrix.</param>
        /// <returns>The subtracted matrix.</returns>
        public static Matrix Subtract(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
            {
                throw new MatrixError(
                        "To subtract the matrixes they must have the same number of rows and columns.  Matrix a has "
                                + a.Rows
                                + " rows and matrix b has "
                                + b.Rows + " rows.");
            }

            if (a.Cols != b.Cols)
            {
                throw new MatrixError(
                        "To subtract the matrixes they must have the same number of rows and columns.  Matrix a has "
                                + a.Cols
                                + " cols and matrix b has "
                                + b.Cols + " cols.");
            }

            double[,] result = new double[a.Rows, a.Cols];

            for (int resultRow = 0; resultRow < a.Rows; resultRow++)
            {
                for (int resultCol = 0; resultCol < a.Cols; resultCol++)
                {
                    result[resultRow, resultCol] = a[resultRow, resultCol]
                            - b[resultRow, resultCol];
                }
            }

            return new Matrix(result);
        }

        /// <summary>
        /// Transpose the specified matrix.
        /// </summary>
        /// <param name="input">The matrix to transpose.</param>
        /// <returns>The transposed matrix.</returns>
        public static Matrix Transpose(Matrix input)
        {
            double[,] inverseMatrix = new double[input.Cols, input
                    .Rows];

            for (int r = 0; r < input.Rows; r++)
            {
                for (int c = 0; c < input.Cols; c++)
                {
                    inverseMatrix[c, r] = input[r, c];
                }
            }

            return new Matrix(inverseMatrix);
        }

        /// <summary>
        /// Calculate the vector length of the matrix.
        /// </summary>
        /// <param name="input">The vector to calculate for.</param>
        /// <returns>The vector length.</returns>
        public static double VectorLength(Matrix input)
        {
            if (!input.IsVector())
            {
                throw new MatrixError(
                        "Can only take the vector length of a vector.");
            }
            Double[] v = input.ToPackedArray();
            double rtn = 0.0;
            for (int i = 0; i < v.Length; i++)
            {
                rtn += Math.Pow(v[i], 2);
            }
            return Math.Sqrt(rtn);
        }

        /// <summary>
        /// Create an input matrix for a neural network.
        /// </summary>
        /// <param name="pattern">The pattern to create the matrix for.</param>
        /// <returns>The newly created matrix.</returns>
        public static Matrix CreateInputMatrix(INeuralData pattern)
        {
            Matrix result = new Matrix(1, pattern.Count);
            for (int i = 0; i < pattern.Count; i++)
            {
                result[0, i] = pattern[i];
            }

            return result;
        }

        /// <summary>
        /// Private constructor.  All methods are static.
        /// </summary>
        private MatrixMath()
        {
        }

    }
}
