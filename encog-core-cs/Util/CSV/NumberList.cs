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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;

namespace Encog.Util.CSV
{
    /// <summary>
    /// Utility class to take numbers to/from a list.
    /// </summary>
    public class NumberList
    {
        private NumberList()
        {
        }

        /// <summary>
        /// Get an array of double's from a string of comma separated text.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="str">The string that contains a list of numbers.</param>
        /// <returns>An array of doubles parsed from the string.</returns>
        public static double[] FromList(CSVFormat format, String str)
        {
            // first count the numbers

            String[] tok = str.Split(format.Separator);
            int count = tok.Length;

            // now allocate an object to hold that many numbers
            double[] result = new double[count];

            // and finally parse the numbers
            for (int index = 0; index < tok.Length; index++)
            {
                try
                {
                    String num = tok[index];
                    double value = format.Parse(num);
                    result[index] = value;
                }
                catch (Exception e)
                {
                    throw new PersistError(e);
                }

            }

            return result;
        }

        /// <summary>
        /// Convert an array of doubles to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToList(CSVFormat format, StringBuilder result,
                 double[] data)
        {
            result.Length = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(format.Separator);
                }
                result.Append(format.Format(data[i],20));
            }
        }
    }
}
