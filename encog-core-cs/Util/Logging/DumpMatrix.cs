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
using System.Text;
using Encog.MathUtil.Matrices;

namespace Encog.Util.Logging
{
    /// <summary>
    /// A utility for writing matrixes to the log.
    /// </summary>
    public class DumpMatrix
    {
        /// <summary>
        /// Maximum precision.
        /// </summary>
        public const int MaxPrecis = 3;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private DumpMatrix()
        {
        }


        /// <summary>
        /// Dump an array of numbers to a string.
        /// </summary>
        /// <param name="d">The array to dump.</param>
        /// <returns>The array as a string.</returns>
        public static String DumpArray(double[] d)
        {
            var result = new StringBuilder();
            result.Append("[");
            for (int i = 0; i < d.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(",");
                }
                result.Append(d[i]);
            }
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        /// Dump a matrix to a string.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The matrix as a string.</returns>
        public static String DumpMatrixString(Matrix matrix)
        {
            var result = new StringBuilder();
            result.Append("==");
            result.Append(matrix.ToString());
            result.Append("==\n");
            for (int row = 0; row < matrix.Rows; row++)
            {
                result.Append("  [");
                for (int col = 0; col < matrix.Cols; col++)
                {
                    if (col != 0)
                    {
                        result.Append(",");
                    }
                    result.Append(matrix[row, col]);
                }
                result.Append("]\n");
            }
            return result.ToString();
        }
    }
}
