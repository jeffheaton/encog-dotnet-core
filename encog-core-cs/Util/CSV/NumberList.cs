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
using Encog.Persist;
using Encog.ML.Data;

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
            if( str.Trim().Length==0)
            {
                return new double[0];
            }
            // first count the numbers

            String[] tok = str.Split(format.Separator);
            int count = tok.Length;

            // now allocate an object to hold that many numbers
            var result = new double[count];

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
        /// Get an array of ints's from a string of comma separated text.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="str">The string that contains a list of numbers.</param>
        /// <returns>An array of ints parsed from the string.</returns>
        public static int[] FromListInt(CSVFormat format, String str)
        {
            if (str.Trim().Length == 0)
            {
                return new int[0];
            }
            // first count the numbers

            String[] tok = str.Split(format.Separator);
            int count = tok.Length;

            // now allocate an object to hold that many numbers
            var result = new int[count];

            // and finally parse the numbers
            for (int index = 0; index < tok.Length; index++)
            {
                try
                {
                    String num = tok[index];
                    int value = int.Parse(num);
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
            ToList(format, 20, result, data);
        }

        /// <summary>
        /// Convert an array of doubles to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToList(CSVFormat format, int precision, StringBuilder result,
                                  double[] data)
        {
            result.Length = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(format.Separator);
                }
                result.Append(format.Format(data[i], precision));
            }
        }

        /// <summary>
        /// Convert an IMLData to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToList(CSVFormat format, StringBuilder result,
                                  IMLData data)
        {
            result.Length = 0;
            for (int i = 0; i < data.Count; i++)
            {
                if (i != 0)
                {
                    result.Append(format.Separator);
                }
                result.Append(format.Format(data[i], EncogFramework.DefaultPrecision));
            }
        }

        /// <summary>
        /// Convert an array of ints to a comma separated list.
        /// </summary>
        /// <param name="format">The way to format this list.</param>
        /// <param name="result">This string will have the values appended to it.</param>
        /// <param name="data">The array of doubles to use.</param>
        public static void ToListInt(CSVFormat format, StringBuilder result,
                                     int[] data)
        {
            result.Length = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i != 0)
                {
                    result.Append(format.Separator);
                }
                result.Append(data[i]);
            }
        }
    }
}
