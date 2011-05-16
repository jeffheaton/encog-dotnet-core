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

namespace Encog.MathUtil
{
    /// <summary>
    /// Used to produce an array of activations to classify data into groups. This
    /// class is provided the number of groups, as well as the range that the
    /// activations should fall into.
    /// </summary>
    public class Equilateral
    {
        public const int MIN_EQ = 3;

        /// <summary>
        /// The matrix of values that was generated.
        /// </summary>
        private readonly double[][] matrix;

        /// <summary>
        /// Construct an equilateral matrix.
        /// </summary>
        /// <param name="count">The number of sets, these will be the rows in the matrix.</param>
        /// <param name="high">The high value for the outputs.</param>
        /// <param name="low">The low value for the outputs.</param>
        public Equilateral(int count, double high, double low)
        {
            matrix = Equilat(count, high, low);
        }


        /// <summary>
        /// Decode a set of activations and see which set it has the lowest Euclidean
        /// distance from.
        /// </summary>
        /// <param name="activations">The output from the neural network.</param>
        /// <returns>The set that these activations were closest too.</returns>
        public int Decode(double[] activations)
        {
            double minValue = double.PositiveInfinity;
            int minSet = -1;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                double dist = GetDistance(activations, i);
                if (dist < minValue)
                {
                    minValue = dist;
                    minSet = i;
                }
            }
            return minSet;
        }

        /// <summary>
        /// Get the activations for the specified set.
        /// </summary>
        /// <param name="set">The set to determine the activations for.</param>
        /// <returns>The activations for the specified sets.</returns>
        public double[] Encode(int set)
        {
            return matrix[set];
        }

        /// <summary>
        /// Called internally to generate the matrix.
        /// </summary>
        /// <param name="n">The number of sets to generate for.</param>
        /// <param name="high">The high end of the range of values to generate.</param>
        /// <param name="low"> The low end of the range of values to generate.</param>
        /// <returns>One row for each set, the columns are the activations for that set.</returns>
        private double[][] Equilat(int n,
                                   double high, double low)
        {
            double r, f;

            var result = new double[n][]; // n - 1
            for (int i = 0; i < n; i++)
            {
                result[i] = new double[n - 1];
            }

            result[0][0] = -1;
            result[1][0] = 1.0;

            for (int k = 2; k < n; k++)
            {
                // scale the matrix so far
                r = k;
                f = Math.Sqrt(r*r - 1.0)/r;
                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < k - 1; j++)
                    {
                        result[i][j] *= f;
                    }
                }

                r = -1.0/r;
                for (int i = 0; i < k; i++)
                {
                    result[i][k - 1] = r;
                }

                for (int i = 0; i < k - 1; i++)
                {
                    result[k][i] = 0.0;
                }
                result[k][k - 1] = 1.0;
            }

            // scale it
            for (int row = 0; row < result.GetLength(0); row++)
            {
                for (int col = 0; col < result[0].GetLength(0); col++)
                {
                    double min = -1;
                    double max = 1;
                    result[row][col] = ((result[row][col] - min)/(max - min))
                                       *(high - low) + low;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the Euclidean distance between the specified data and the set number.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="set">The set to check.</param>
        /// <returns>The distance.</returns>
        public double GetDistance(double[] data, int set)
        {
            double result = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                result += Math.Pow(data[i] - matrix[set][i], 2);
            }
            return Math.Sqrt(result);
        }

        /// <summary>
        /// Get the smallest distance.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <returns>The set with the smallest distance.</returns>
        public int GetSmallestDistance(double[] data)
        {
            int bestSet = -1;
            double bestDistance = double.MaxValue;

            for (int i = 0; i < matrix.Length; i++)
            {
                double d = GetDistance(data, i);
                if (bestSet == -1 || d < bestDistance)
                {
                    bestSet = i;
                    bestDistance = d;
                }
            }

            return bestSet;
        }
    }
}