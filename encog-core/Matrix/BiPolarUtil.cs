// Encog Neural Network and Bot Library for DotNet v0.5
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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

namespace Encog.Matrix
{
    public class BiPolarUtil
    {
        /// <summary>
        /// Convert binary to bipolar, true is 1 and false is -1.
        /// </summary>
        /// <param name="b">The binary value.</param>
        /// <returns>The bipolar value.</returns>
        public static double Bipolar2double(bool b)
        {
            if (b)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Convert a boolean array to bipolar, true is 1 and false is -1.
        /// </summary>
        /// <param name="b">The binary array to convert.</param>
        /// <returns></returns>
        public static double[] Bipolar2double(bool[] b)
        {
            double[] result = new double[b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                result[i] = Bipolar2double(b[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert a 2D boolean array to bipolar, true is 1 and false is -1.
        /// </summary>
        /// <param name="b">The 2D array to convert.</param>
        /// <returns>A bipolar array.</returns>
        public static double[,] Bipolar2double(bool[,] b)
        {
            double[,] result = new double[b.GetUpperBound(0), b.GetUpperBound(1)];

            for (int row = 0; row < b.GetUpperBound(0); row++)
            {
                for (int col = 0; col < b.GetUpperBound(1); col++)
                {
                    result[row, col] = Bipolar2double(b[row, col]);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert biploar to boolean, true is 1 and false is -1.
        /// </summary>
        /// <param name="d">A bipolar value.</param>
        /// <returns>A boolean value.</returns>
        public static bool Double2bipolar(double d)
        {
            if (d > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert a bipolar array to a boolean array, true is 1 and false is -1.
        /// </summary>
        /// <param name="d">A bipolar array.</param>
        /// <returns>A boolean array.</returns>
        public static bool[] Double2bipolar(double[] d)
        {
            bool[] result = new bool[d.Length];

            for (int i = 0; i < d.Length; i++)
            {
                result[i] = Double2bipolar(d[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert a 2D bipolar array to a boolean array, true is 1 and false is -1.
        /// </summary>
        /// <param name="d">A 2D bipolar array.</param>
        /// <returns>A 2D boolean array.</returns>
        public static bool[,] Double2bipolar(double[,] d)
        {
            bool[,] result = new bool[d.GetUpperBound(0), d.GetUpperBound(1)];

            for (int row = 0; row < d.GetUpperBound(0); row++)
            {
                for (int col = 0; col < d.GetUpperBound(0); col++)
                {
                    result[row, col] = Double2bipolar(d[row, col]);
                }
            }

            return result;
        }

        /// <summary>
        /// Normalize a binary number.  Greater than 0 becomes 1, zero and below are false.
        /// </summary>
        /// <param name="d">A binary number in a double.</param>
        /// <returns>A double that will be 0 or 1.</returns>
        public static double NormalizeBinary(double d)
        {
            if (d > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Convert a single number from bipolar to binary.
        /// </summary>
        /// <param name="d">a bipolar number.</param>
        /// <returns>A binary number.</returns>
        public static double ToBinary(double d)
        {
            return (d + 1) / 2.0;
        }

        /// <summary>
        /// Convert a number to bipolar.
        /// </summary>
        /// <param name="d">A binary number.</param>
        /// <returns></returns>
        public static double ToBiPolar(double d)
        {
            return (2 * NormalizeBinary(d)) - 1;
        }

        /// <summary>
        /// Normalize a number and convert to binary.
        /// </summary>
        /// <param name="d">A bipolar number.</param>
        /// <returns>A binary number stored as a double</returns>
        public static double ToNormalizedBinary(double d)
        {
            return NormalizeBinary(ToBinary(d));
        }
    }
}
