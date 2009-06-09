using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.MathUtil
{
    /// <summary>
    /// This class is used to convert strings into numeric values.  If the
    /// string holds a non-numeric value, a zero is returned.
    /// </summary>
    public sealed class Convert
    {

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Convert()
        {
        }

        /// <summary>
        /// Convert a string to a double.  Just make the number a zero
        /// if the string is invalid.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The string converted to numeric.</returns>
        public static double String2double(String str)
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
        public static int String2int(String str)
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
