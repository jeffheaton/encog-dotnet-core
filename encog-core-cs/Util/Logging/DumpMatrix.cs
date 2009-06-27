// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Logging
{
    /// <summary>
    /// A utility for writing matrixes to the log.
    /// </summary>
    public class DumpMatrix
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private DumpMatrix()
        {

        }

        /// <summary>
        /// Maximum precision.
        /// </summary>
        public const int MAX_PRECIS = 3;


        /// <summary>
        /// Dump an array of numbers to a string.
        /// </summary>
        /// <param name="d">The array to dump.</param>
        /// <returns>The array as a string.</returns>
        public static String DumpArray(double[] d)
        {

            StringBuilder result = new StringBuilder();
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
        public static String DumpMatrixString(Matrix.Matrix matrix)
        {

            StringBuilder result = new StringBuilder();
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
