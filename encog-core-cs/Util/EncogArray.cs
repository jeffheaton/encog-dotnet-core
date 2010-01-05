using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// Simple array utilities for Encog.
    /// </summary>
    public class EncogArray
    {
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
    }
}
