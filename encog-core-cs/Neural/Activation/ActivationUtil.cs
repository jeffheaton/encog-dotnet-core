using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// Utility classes for activation functions. Used to convert a single value
    /// to/from an array. This is necessary because the activation functions are
    /// designed to operate on arrays, rather than single values.
    /// </summary>
    public sealed class ActivationUtil
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ActivationUtil()
        {
        }

        /// <summary>
        /// Get a single value from an array. Return the first element in the 
        /// array.
        /// </summary>
        /// <param name="d">The array.</param>
        /// <returns>The first element in the array.</returns>
        public static double FromArray(double[] d)
        {
            return d[0];
        }

        /// <summary>
        /// Take a single value and create an array that holds it.
        /// </summary>
        /// <param name="d">The single value.</param>
        /// <returns>The array.</returns>
        public static double[] ToArray(double d)
        {
            double[] result = new double[1];
            result[0] = d;
            return result;
        }
    }

}
