using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using System.Runtime.Serialization;
using log4net;

namespace Encog.Matrix
{
/**
 * This class implements a mathematical matrix. Matrix math is very important to
 * neural network processing. Many of the neural network classes make use of the
 * matrix classes in this package.
 */
    /// <summary>
    /// Matrix: This class implements a mathematical matrix.  Matrix
    /// math is very important to neural network processing.  Many
    /// of the classes developed in this book will make use of the
    /// matrix classes in this package.
    /// </summary>
    [Serializable]
    public class Matrix : IEncogPersistedObject
    {
        private String name;
        private String description;

        /// <summary>
        /// Allows index access to the elements of the matrix.
        /// </summary>
        /// <param name="row">The row to access.</param>
        /// <param name="col">The column to access.</param>
        /// <returns>The element at the specified position in the matrix.</returns>
        public double this[int row, int col]
        {
            get
            {
                Validate(row, col);
                return this.matrix[row, col];
            }
            set
            {
                Validate(row, col);
                if (double.IsInfinity(value) || double.IsNaN(value))
                {
                    throw new MatrixError("Trying to assign invalud number to matrix: "
                            + value);
                }
                this.matrix[row, col] = value;
            }
        }

        /// <summary>
        /// Create a matrix that is a single column.
        /// </summary>
        /// <param name="input">A 1D array to make the matrix from.</param>
        /// <returns>A matrix that contains a single column.</returns>
        public static Matrix CreateColumnMatrix(double[] input)
        {
            double[,] d = new double[input.Length, 1];
            for (int row = 0; row < d.Length; row++)
            {
                d[row, 0] = input[row];
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
            double[,] d = new double[1, input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                d[0, i] = input[i];
            }

            return new Matrix(d);
        }

        /// <summary>
        /// The matrix data, stored as a 2D array.
        /// </summary>
        double[,] matrix;

        /// <summary>
        /// Construct a matrix from a 2D boolean array.  Translate true to 1, false to -1.
        /// </summary>
        /// <param name="sourceMatrix">A 2D array to construcat the matrix from.</param>
        public Matrix(bool[,] sourceMatrix)
        {

            this.matrix = new double[sourceMatrix.GetUpperBound(0) + 1, sourceMatrix.GetUpperBound(1) + 1];
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    if (sourceMatrix[r, c])
                    {
                        this[r, c] = 1;
                    }
                    else
                    {
                        this[r, c] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Construct a matrix from a 2D double array.
        /// </summary>
        /// <param name="sourceMatrix">A 2D double array.</param>
        public Matrix(double[,] sourceMatrix)
        {
            this.matrix = new double[sourceMatrix.GetUpperBound(0) + 1, sourceMatrix.GetUpperBound(1) + 1];
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    this[r, c] = sourceMatrix[r, c];
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
            this.matrix = new double[rows, cols];
        }

        /// <summary>
        /// Add the specified value to the specified row and column of the matrix.
        /// </summary>
        /// <param name="row">The row to add to.</param>
        /// <param name="col">The column to add to.</param>
        /// <param name="value">The value to add.</param>
        public void Add(int row, int col, double value)
        {
            Validate(row, col);
            double newValue = this[row, col] + value;
            this[row, col] = newValue;
        }

        /// <summary>
        /// Clear the matrix.
        /// </summary>
        public void Clear()
        {
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    this[r, c] = 0;
                }
            }
        }

        /// <summary>
        /// Clone the matrix.
        /// </summary>
        /// <returns>A cloned copy of the matrix.</returns>
        public Object Clone()
        {
            return new Matrix(this.matrix);
        }

        /// <summary>
        /// Determine if this matrix is equal to another.  Use a precision of 10 decimal places.
        /// </summary>
        /// <param name="matrix">The other matrix to compare.</param>
        /// <returns>True if the two matrixes are equal.</returns>
        public bool Equals(Matrix matrix)
        {
            return equals(matrix, 10);
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

            precision = (int)Math.Pow(10, precision);

            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    if ((long)(this[r, c] * precision) != (long)(matrix[r, c] * precision))
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

            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    this.matrix[r, c] = array[index++];
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
            if (col > this.Cols)
            {
                throw new MatrixError("Can't get column #" + col
                        + " because it does not exist.");
            }

            double[,] newMatrix = new double[this.Rows, 1];

            for (int row = 0; row < this.Rows; row++)
            {
                newMatrix[row, 0] = this.matrix[row, col];
            }

            return new Matrix(newMatrix);
        }

        /// <summary>
        /// Get the number of columns in this matrix
        /// </summary>
        public int Cols
        {
            get
            {
                return this.matrix.GetUpperBound(1) + 1;
            }
        }

        /// <summary>
        /// Get the specified row as a row matrix.
        /// </summary>
        /// <param name="row">The desired row.</param>
        /// <returns>A row matrix.</returns>
        public Matrix GetRow(int row)
        {
            if (row > this.Rows)
            {
                throw new MatrixError("Can't get row #" + row
                        + " because it does not exist.");
            }

            double[,] newMatrix = new double[1, this.Cols];

            for (int col = 0; col < this.Cols; col++)
            {
                newMatrix[0, col] = this.matrix[row, col];
            }

            return new Matrix(newMatrix);
        }

        /// <summary>
        /// Get the number of rows in this matrix
        /// </summary>
        public int Rows
        {
            get
            {
                return this.matrix.GetUpperBound(0) + 1;
            }
        }


        /// <summary>
        /// Determine if this matrix is a vector.  A vector matrix only has a single row or column.
        /// </summary>
        /// <returns>True if this matrix is a vector.</returns>
        public bool IsVector()
        {
            if (this.Rows == 1)
            {
                return true;
            }
            else
            {
                return this.Cols == 1;
            }
        }

        /// <summary>
        /// Determine if all of the values in the matrix are zero.
        /// </summary>
        /// <returns>True if all of the values in the matrix are zero.</returns>
        public bool IsZero()
        {
            for (int row = 0; row < this.Rows; row++)
            {
                for (int col = 0; col < this.Cols; col++)
                {
                    if (this.matrix[row, col] != 0)
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

            Random rand = new Random();

            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    this.matrix[r, c] = (rand.NextDouble() * (max - min)) + min;
                }
            }
        }


        /// <summary>
        /// Get the size fo the matrix.  This is thr rows times the columns.
        /// </summary>
        public int Size
        {
            get
            {
                return this.Rows * this.Cols;
            }
        }

        /// <summary>
        /// Sum all of the values in the matrix.
        /// </summary>
        /// <returns>The sum of all of the values in the matrix.</returns>
        public double Sum()
        {
            double result = 0;
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    result += this.matrix[r, c];
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
            double[] result = new double[this.Rows * this.Cols];

            int index = 0;
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Cols; c++)
                {
                    result[index++] = this.matrix[r, c];
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
            if ((row >= this.Rows) || (row < 0))
            {
                throw new MatrixError("The row:" + row + " is out of range:"
                        + this.Rows);
            }

            if ((col >= this.Cols) || (col < 0))
            {
                throw new MatrixError("The col:" + col + " is out of range:"
                        + this.Cols);
            }
        }

        /// <summary>
        /// The description of this object.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The name of this object.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Create a persistor for this object.
        /// </summary>
        /// <returns>A persistor for this object.</returns>
        public IPersistor CreatePersistor()
        {
            // Matrixes are not persisted directly.
            return null;
        }


    }

}
