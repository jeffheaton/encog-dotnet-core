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
using Encog.ML.Data;

namespace Encog.MathUtil
{
    /// <summary>
    /// Used to produce an array of activations to classify data into groups. This
    /// class is provided the number of groups, as well as the range that the
    /// activations should fall into.
    /// </summary>
    [Serializable]
    public class Equilateral
    {
        /// <summary>
        /// Minimum number of classes for equilateral.
        /// </summary>
        public const int MinEq = 3;

        /// <summary>
        /// The matrix of values that was generated.
        /// </summary>
        private readonly double[][] _matrix;

        /// <summary>
        /// Construct an equilateral matrix.
        /// </summary>
        /// <param name="count">The number of sets, these will be the rows in the matrix.</param>
        /// <param name="high">The high value for the outputs.</param>
        /// <param name="low">The low value for the outputs.</param>
        public Equilateral(int count, double high, double low)
        {
            _matrix = Equilat(count, high, low);
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

            for (int i = 0; i < _matrix.GetLength(0); i++)
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
		/// Decode a set of activations and see which set it has the lowest Euclidean
		/// distance from.
		/// </summary>
		/// <param name="activations">The output from the neural network.</param>
		/// <returns>The set that these activations were closest too.</returns>
		public int Decode(IMLData activations)
		{
			double minValue = double.PositiveInfinity;
			int minSet = -1;

			for(int i = 0; i < _matrix.GetLength(0); i++)
			{
				double dist = GetDistance(activations, i);
				if(dist < minValue)
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
            if (set < 0 || set > _matrix.Length)
            {
                throw new EncogError("Class out of range for equilateral: " + set);
            }
            return _matrix[set];
        }

        /// <summary>
        /// Called internally to generate the matrix.
        /// </summary>
        /// <param name="n">The number of sets to generate for.</param>
        /// <param name="high">The high end of the range of values to generate.</param>
        /// <param name="low"> The low end of the range of values to generate.</param>
        /// <returns>One row for each set, the columns are the activations for that set.</returns>
        private static double[][] Equilat(int n,
                                   double high, double low)
        {
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
                double r = k;
                double f = Math.Sqrt(r*r - 1.0)/r;
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
                    const double min = -1;
                    const double max = 1;
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
				var val = data[i] - _matrix[set][i];
                result += val * val;
            }
            return Math.Sqrt(result);
        }

		/// <summary>
		/// Get the Euclidean distance between the specified data and the set number.
		/// </summary>
		/// <param name="data">The data to check.</param>
		/// <param name="set">The set to check.</param>
		/// <returns>The distance.</returns>
		public double GetDistance(IMLData data, int set)
		{
			double result = 0;
			for(int i = 0; i < data.Count; i++)
			{
				var val = data[i] - _matrix[set][i];
				result += val * val;
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

            for (int i = 0; i < _matrix.Length; i++)
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
