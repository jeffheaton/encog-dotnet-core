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
using System.Collections.Generic;
using Encog.ML.Data;
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// Simple array utilities for Encog.
    /// </summary>
    public class EngineArray
    {
        public const int DoubleSize = sizeof(double);

        /// <summary>
        /// Copy a double array.
        /// </summary>
        /// <param name="input">The array to copy.</param>
        /// <returns>The result of the copy.</returns>
        public static double[] ArrayCopy(double[] input)
        {
            var result = new double[input.Length];
            ArrayCopy(input, result);
            return result;
        }

        /// <summary>
        /// Copy an int array.
        /// </summary>
        /// <param name="input">The array to copy.</param>
        /// <returns>The result of the copy.</returns>
        public static int[] ArrayCopy(int[] input)
        {
            var result = new int[input.Length];
            ArrayCopy(input, result);
            return result;
        }


        /// <summary>
        /// Completely copy one array into another. 
        /// </summary>
        /// <param name="src">Source array.</param>
        /// <param name="dst">Destination array.</param>
        public static void ArrayCopy(double[] src, double[] dst)
        {
            Array.Copy(src, dst, src.Length);
        }

        /// <summary>
        /// Completely copy one array into another. 
        /// </summary>
        /// <param name="src">Source array.</param>
        /// <param name="dst">Destination array.</param>
        public static void ArrayCopy(int[] src, int[] dst)
        {
            Array.Copy(src, dst, src.Length);
        }

        /// <summary>
        /// Calculate the product of two vectors (a scalar value).
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <returns>The product of the two vectors (a scalar value).</returns>
        public static double VectorProduct(double[] a, double[] b)
        {
            int length = a.Length;
            double value = 0;

            for (int i = 0; i < length; ++i)
                value += a[i] * b[i];

            return value;
        }

        /// <summary>
        /// Allocate a 2D array of doubles.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <returns>The array.</returns>
        public static double[][] AllocateDouble2D(int rows, int cols)
        {
            var result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
            }
            return result;
        }

        /// <summary>
        /// Allocate a 2D array of bools.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <returns>The array.</returns>
        public static bool[][] AllocateBool2D(int rows, int cols)
        {
            var result = new bool[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new bool[cols];
            }
            return result;
        }

        /// <summary>
        /// Copy an array of doubles.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="output">The output array.</param>
        /// <param name="targetIndex">The output index.</param>
        /// <param name="size">The size to copy.</param>
        public static void ArrayCopy(double[] source, int sourceIndex, double[] output, int targetIndex, int size)
        {
            //Array.Copy(source, sourceIndex, output, targetIndex, size);
            Buffer.BlockCopy(source, sourceIndex * DoubleSize, output, targetIndex * DoubleSize, size * DoubleSize);
        }

        /// <summary>
        /// Convert the collection to an array list of doubles.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The array of doubles.</returns>
        public static double[] ListToDouble(IList<double> list)
        {
            var result = new double[list.Count];
            int index = 0;
            foreach (double obj in list)
            {
                result[index++] = obj;
            }

            return result;
        }

        /// <summary>
        /// Fill the specified array with the specified value.
        /// </summary>
        /// <param name="p">The array to fill.</param>
        /// <param name="v">The value to fill.</param>
        internal static void Fill(double[] p, double v)
        {
            for (int i = 0; i < p.Length; i++)
                p[i] = v;
        }

        /// <summary>
        /// Search for a string in an array. 
        /// </summary>
        /// <param name="search">Where to search.</param>
        /// <param name="searchFor">What we are looking for.</param>
        /// <returns>The index that the string occurs at.</returns>
        public static int FindStringInArray(String[] search, String searchFor)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (search[i].Equals(searchFor))
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Gets the last N closing values of a double serie;
        /// copied in a new double serie.
        /// </summary>
        /// <param name="lenth">The lenth to get.</param>
        /// <param name="closes"></param>
        /// <returns>a double serie with the last n requested values.</returns>
        public double[] TransferNvaluesOfSerie(int lenth, double[] closes)
        {
            if (closes != null)
            {
                double[] output;

                if (closes.Length > lenth)
                {
                    //we have more closing values than our length so we'll return values based on last - Length.
                    int startIndex = closes.Length - lenth;
                    output = new double[lenth];
                    EngineArray.ArrayCopy(closes, startIndex, output, 0, lenth);
                    return output;
                }
                if (closes.Length == lenth)
                {
                    //we have the same values , so we just return the full closing values.
                    int startIndex = closes.Length - lenth;
                    output = new double[lenth];
                    EngineArray.ArrayCopy(closes, startIndex, output, 0, lenth);
                    return output;
                }
            }

            //we didn't get any right values to return N lenght of the serie.
            return null;

        }

        /// <summary>
        /// Copy a 2d array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The result.</returns>
        public static double[][] ArrayCopy(double[][] source)
        {
            double[][] result = AllocateDouble2D(source.Length, source[0].Length);

            for (int row = 0; row < source.Length; row++)
            {
                for (int col = 0; col < source[0].Length; col++)
                {
                    result[row][col] = source[row][col];
                }
            }

            return result;
        }

        /// <summary>
        /// Copy a float array to a double array.
        /// </summary>
        /// <param name="source">The double array.</param>
        /// <param name="target">The float array.</param>
        public static void ArrayCopy(float[] source, double[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[i] = source[i];
            }
        }

        /// <summary>
        /// Copy a double array to a float array.
        /// </summary>
        /// <param name="source">The double array.</param>
        /// <param name="target">The float array.</param>
        public static void ArrayCopy(double[] source, float[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[i] = (float)source[i];
            }
        }

        /// <summary>
        /// Fill the array with the specified value.
        /// </summary>
        /// <param name="target">The array to fill.</param>
        /// <param name="v">The value to fill.</param>
        public static void Fill(double[] target, int v)
        {
            for (int i = 0; i < target.Length; i++)
                target[i] = v;
        }

        /// <summary>
        /// Fill the array with the specified value.
        /// </summary>
        /// <param name="target">The array to fill.</param>
        /// <param name="v">The value to fill.</param>
        public static void Fill(float[] target, int v)
        {
            for (int i = 0; i < target.Length; i++)
                target[i] = v;
        }

        /// <summary>
        /// Get the index of the largest value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The index.</returns>
        public static int IndexOfLargest(double[] data)
        {
            int result = -1;

            for (int i = 0; i < data.Length; i++)
            {
                if (result == -1 || data[i] > data[result])
                    result = i;
            }

            return result;
        }

        /// <summary>
        /// Get the index of the largest value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The index.</returns>
        public static int IndexOfLargest(IMLData data)
        {
            int result = -1;

            for (int i = 0; i < data.Count; i++)
            {
                if (result == -1 || data[i] > data[result])
                    result = i;
            }

            return result;
        }

        /// <summary>
        /// Get the min value in an array.
        /// </summary>
        /// <param name="weights">The array to search.</param>
        /// <returns>The result.</returns>
        public static double Min(double[] weights)
        {
            double result = double.MaxValue;
            for (int i = 0; i < weights.Length; i++)
            {
                result = Math.Min(result, weights[i]);
            }
            return result;
        }

        /// <summary>
        /// Get the max value from an array.
        /// </summary>
        /// <param name="weights">The array to search.</param>
        /// <returns>The value.</returns>
        public static double Max(double[] weights)
        {
            double result = Double.MinValue;
            for (int i = 0; i < weights.Length; i++)
            {
                result = Math.Max(result, weights[i]);
            }
            return result;
        }

        /// <summary>
        /// Put all elements from one dictionary to another.
        /// </summary>
        /// <typeparam name="TK">The key type.</typeparam>
        /// <typeparam name="TV">The value type.</typeparam>
        /// <param name="source">The source dictionary.</param>
        /// <param name="target">The target dictionary.</param>
        public static void PutAll<TK, TV>(IDictionary<TK, TV> source, IDictionary<TK, TV> target)
        {
            foreach (var x in source)
            {
                target.Add(x);
            }
        }

        /// <summary>
        /// Determine if the array contains the specified number.
        /// </summary>
        /// <param name="array">The array to search.</param>
        /// <param name="target">The number being searched for.</param>
        /// <returns>True, if the number was found.</returns>
        public static bool Contains(int[] array, int target)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == target)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the index of the max value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The result</returns>
        public static int MaxIndex(double[] data)
        {
            int result = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (result == -1 || data[i] > data[result])
                {
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Get the index of the max value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The result</returns>
        public static int MaxIndex(IMLData data)
        {
            int result = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (result == -1 || data[i] > data[result])
                {
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Get the index of the max value in the array.
        /// </summary>
        /// <param name="data">The array to search.</param>
        /// <returns>The result</returns>
        public static int MaxIndex(int[] data)
        {
            int result = -1;
            for (int i = 0; i < data.Length; i++)
            {
                if (result == -1 || data[i] > data[result])
                {
                    result = i;
                }
            }
            return result;
        }


        /// <summary>
        /// Creates a jagged array and initializes it.
        /// You can virtually create any kind of jagged array up to N dimension.
        /// double[][] resultingArray = CreateArray  <double[ ]> (2, () => CreateArray<double>(100, () => 0));
        /// Create a double[2] [100] , with all values at 0..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnt">The CNT.</param>
        /// <param name="itemCreator">The item creator.</param>
        /// <returns></returns>
        public static T[] CreateArray<T>(int cnt, Func<T> itemCreator)
        {
            T[] result = new T[cnt];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = itemCreator();
            }
            return result;
        }

        /// <summary>
        /// Fill the array with the specified value.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="v">The value.</param>
        public static void Fill(bool[] array, bool v)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = v;
            }
        }

        /// <summary>
        /// Add two vectors.
        /// </summary>
        /// <param name="d">First vector.</param>
        /// <param name="m">Second vector.</param>
        /// <returns>Result vector.</returns>
        public static double[] Add(double[] d, double[] m)
        {
            double[] result = new double[d.Length];
            for (int i = 0; i < d.Length; i++)
            {
                result[i] = d[i] + m[i];
            }
            return result;
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Result vector.</returns>
        public static double[] Subtract(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }

        /// <summary>
        /// Subtract two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Result vector.</returns>
        public static double[] Subtract(IMLData a, double[] b)
        {
            double[] result = new double[a.Count];
            for (int i = 0; i < a.Count; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }

        internal static int[][] AllocateInt2D(int rows, int cols)
        {
            var result = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new int[cols];
            }
            return result;

        }

        public static double[][][] AllocDouble3D(int x, int y, int z)
        {
            var result = new double[x][][];
            for (int i = 0; i < x; i++)
            {
                result[i] = new double[y][];
                for (int j = 0; j < y; j++)
                {
                    result[i][j] = new double[z];
                }
            }
            return result;

        }

        /// <summary>
        /// Copy one double 2d array to another.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="target">The target array.</param>
        public static void ArrayCopy(double[][] source, double[][] target)
        {
            for (var row = 0; row < source.Length; row++)
            {
                for (var col = 0; col < source[row].Length; col++)
                {
                    target[row][col] = source[row][col];
                }
            }
        }

        /// <summary>
        /// Calculate the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="p1">The first vector.</param>
        /// <param name="p2">The second vector.</param>
        /// <returns>The distance.</returns>
        public static double EuclideanDistance(double[] p1, double[] p2)
        {
            double sum = 0;
            for (int i = 0; i < p1.Length; i++)
            {
                double d = p1[i] - p2[i];
                sum += d * d;
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculate the Euclidean distance between two vectors.
        /// </summary>
        /// <param name="p1">The first vector.</param>
        /// <param name="p2">The second vector.</param>
        /// <returns>The distance.</returns>
        public static double EuclideanDistance(IMLData p1, double[] p2)
        {
            double sum = 0;
            for (int i = 0; i < p1.Count; i++)
            {
                double d = p1[i] - p2[i];
                sum += d * d;
            }
            return Math.Sqrt(sum);
        }

        public static String Replace(String str, String searchFor, String replace)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                bool append = true;

                if (i + searchFor.Length < str.Length)
                {
                    String cmp = str.Substring(i, i + searchFor.Length);
                    if (cmp.Equals(searchFor))
                    {
                        i += searchFor.Length - 1;
                        result.Append(replace);
                        append = false;
                    }
                }
                if (append)
                    result.Append(str[i]);
            }
            return result.ToString();
        }

        public static void ArrayCopy(byte[] source, int sourcePos,
                byte[] target, int targetPos, int length)
        {
            Array.Copy(source, sourcePos, target, targetPos, length);

        }

        public static void ArrayCopy(int[] source, int sourcePos, int[] target,
                int targetPos, int length)
        {
            Array.Copy(source, sourcePos, target, targetPos, length);

        }

        public static void Fill(double[][] sigma, int value)
        {
            for (int i = 0; i < sigma.Length; i++)
            {
                for (int j = 0; j < sigma[i].Length; j++)
                {
                    sigma[i][j] = value;
                }
            }
        }

        public static void ArrayAdd(double[][] target, double[][] h)
        {
            for (int row = 0; row < target.Length; row++)
            {
                for (int col = 0; col < target[row].Length; col++)
                {
                    target[row][col] += h[row][col];
                }
            }
		
	}
    }

}
