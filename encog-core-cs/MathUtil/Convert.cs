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

namespace Encog.MathUtil
{
    /// <summary>
    /// This class is used to convert strings into numeric values.  If the
    /// string holds a non-numeric value, a zero is returned.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Convert a string to a double.  Just make the number a zero
        /// if the string is invalid.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string converted to numeric.</returns>
        public static double String2Double(String str)
        {
            double result = 0;
            try
            {
                if (str != null)
                {
                    result = double.Parse(str);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// Convert a string to an int.  Just make the number a zero
        /// if the string is invalid.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string converted to numeric.</returns>
        public static int String2Int(String str)
        {
            int result = 0;
            try
            {
                if (str != null)
                {
                    result = int.Parse(str);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }
    }
}
