// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
