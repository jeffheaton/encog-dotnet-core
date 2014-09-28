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
using System.Globalization;
using System.Text;


namespace Encog.Util.CSV
{
    /// <summary>
    /// Describes how to format number lists, such as CSV.
    /// </summary>
    [Serializable]
    public class CSVFormat
    {
        /// <summary>
        /// The maximum number of digits.
        /// </summary>
        public const int MaxFormats = 100;

        private readonly char _decimalChar;

        private readonly NumberFormatInfo _numberFormat;
        private readonly char _separatorChar;
        private readonly String[] _formats;


        /// <summary>
        /// Create a CSV format for the specified decimal char and separator char.
        /// </summary>
        /// <param name="decimalChar">The character for a decimal point or comma.</param>
        /// <param name="separatorChar">The separator char for a number list, likely comma or semicolon.</param>
        public CSVFormat(char decimalChar, char separatorChar)
        {
            _decimalChar = decimalChar;
            _separatorChar = separatorChar;

            if (decimalChar == '.')
            {
                _numberFormat = (new CultureInfo("en-us")).NumberFormat;
            }
            else if (decimalChar == ',')
            {
                _numberFormat = (new CultureInfo("de-DE")).NumberFormat;
            }
            else
            {
                _numberFormat = NumberFormatInfo.CurrentInfo;
            }

            // There might be a better way to do this, but I can't seem to find a way in
            // C# where I can specify "x" decimal places, and not end up with trailing zeros.
            _formats = new String[MaxFormats];
            for (int i = 0; i < MaxFormats; i++)
            {
                var str = new StringBuilder();
                str.Append("#0");
                if (i > 0)
                {
                    str.Append(".");
                }
                for (int j = 0; j < i; j++)
                {
                    str.Append("#");
                }
                _formats[i] = str.ToString();
            }
        }

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public CSVFormat()
        {
        }

        static CSVFormat()
        {
            DecimalComma = new CSVFormat(',', ';');
            DecimalPoint = new CSVFormat('.', ',');
        }

        /// <summary>
        /// A format that uses a decimal point and a comma to separate fields.
        /// </summary>
        public static CSVFormat DecimalPoint { get; private set; }

        /// <summary>
        /// A format that uses a decimal comma and a semicolon to separate fields.
        /// </summary>
        public static CSVFormat DecimalComma { get; private set; }

        /// <summary>
        /// The typical format for English speaking countries is a decimal
        /// point and a comma for field separation.  
        /// </summary>
        public static CSVFormat English
        {
            get { return DecimalPoint; }
        }

        /// <summary>
        /// It is important that an EG file produced on one system, in one region
        /// be readable by another system in a different region.  Because of this
        /// EG files internally use a decimal point and comma separator.  Of course
        /// programs should display numbers to the user using regional settings.
        /// </summary>
        public static CSVFormat EgFormat
        {
            get { return DecimalPoint; }
        }

        /// <summary>
        /// The decimal character, usually either a period or comma.
        /// </summary>
        public char DecimalChar
        {
            get { return _decimalChar; }
        }

        /// <summary>
        /// The separator character for a list of fields in CSV, usually either comma or
        /// semicolon.
        /// </summary>
        public char Separator
        {
            get { return _separatorChar; }
        }

        /// <summary>
        /// The decimal character for the current region.
        /// </summary>
        public static char DecimalCharacter
        {
            get { return NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0]; }
        }

        /// <summary>
        /// Parse the specified string into a number.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The number that has been parsed.</returns>
        public double Parse(String str)
        {
            if (string.Compare(str, "?", true)==0)
            {
                return double.NaN;
            }
            if (string.Compare(str, "NaN", true) == 0)
            {
                return double.NaN;
            }
            return double.Parse(str.Trim(), _numberFormat);
        }

        /// <summary>
        /// Format the specified number into a string.
        /// </summary>
        /// <param name="d">The number to parse.</param>
        /// <param name="digits">The number of fractional digits.</param>
        /// <returns>The formatted number.</returns>
        public String Format(double d, int digits)
        {
            int digits2 = Math.Min(digits, MaxFormats);
            digits2 = Math.Max(digits2, 0);
            return d.ToString(_formats[digits2], _numberFormat);
        }

    }
}
