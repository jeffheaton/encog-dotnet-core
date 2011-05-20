//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
    public class CSVFormat
    {
        /// <summary>
        /// The maximum number of digits.
        /// </summary>
        public const int MAX_FORMATS = 100;

        private static readonly CSVFormat _DECIMAL_POINT = new CSVFormat('.', ',');
        private static readonly CSVFormat _DECIMAL_COMMA = new CSVFormat(',', ';');
        private static CSVFormat _ENGLISH = DECIMAL_POINT;
        private static CSVFormat _NONENGLISH = DECIMAL_COMMA;
        private static CSVFormat _EG_FORMAT = DECIMAL_POINT;

        private readonly char decimalChar;

        private readonly NumberFormatInfo numberFormat;
        private readonly char separatorChar;
        private String[] formats;


        /// <summary>
        /// Create a CSV format for the specified decimal char and separator char.
        /// </summary>
        /// <param name="decimalChar">The character for a decimal point or comma.</param>
        /// <param name="separatorChar">The separator char for a number list, likely comma or semicolon.</param>
        public CSVFormat(char decimalChar, char separatorChar)
        {
            this.decimalChar = decimalChar;
            this.separatorChar = separatorChar;

            if (decimalChar == '.')
            {
                numberFormat = (new CultureInfo("en-us")).NumberFormat;
            }
            else if (decimalChar == ',')
            {
                numberFormat = (new CultureInfo("de-DE")).NumberFormat;
            }
            else
            {
                numberFormat = NumberFormatInfo.CurrentInfo;
            }

            // There might be a better way to do this, but I can't seem to find a way in
            // C# where I can specify "x" decimal places, and not end up with trailing zeros.
            formats = new String[MAX_FORMATS];
            for (int i = 0; i < MAX_FORMATS; i++)
            {
                StringBuilder str = new StringBuilder();
                str.Append("#0");
                if (i > 0)
                {
                    str.Append(".");
                }
                for (int j = 0; j < i; j++)
                {
                    str.Append("#");
                }
                this.formats[i] = str.ToString();
            }
        }

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public CSVFormat()
        {
        }

        /// <summary>
        /// A format that uses a decimal point and a comma to separate fields.
        /// </summary>
        public static CSVFormat DECIMAL_POINT
        {
            get { return _DECIMAL_POINT; }
        }

        /// <summary>
        /// A format that uses a decimal comma and a semicolon to separate fields.
        /// </summary>
        public static CSVFormat DECIMAL_COMMA
        {
            get { return _DECIMAL_COMMA; }
        }

        /// <summary>
        /// The typical format for English speaking countries is a decimal
        /// point and a comma for field separation.  
        /// </summary>
        public static CSVFormat ENGLISH
        {
            get { return _DECIMAL_POINT; }
        }

        /// <summary>
        /// It is important that an EG file produced on one system, in one region
        /// be readable by another system in a different region.  Because of this
        /// EG files internally use a decimal point and comma separator.  Of course
        /// programs should display numbers to the user using regional settings.
        /// </summary>
        public static CSVFormat EG_FORMAT
        {
            get { return _DECIMAL_POINT; }
        }

        /// <summary>
        /// The decimal character, usually either a period or comma.
        /// </summary>
        public char DecimalChar
        {
            get { return decimalChar; }
        }

        /// <summary>
        /// The separator character for a list of fields in CSV, usually either comma or
        /// semicolon.
        /// </summary>
        public char Separator
        {
            get { return separatorChar; }
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
            return double.Parse(str, numberFormat);
        }

        /// <summary>
        /// Format the specified number into a string.
        /// </summary>
        /// <param name="d">The number to parse.</param>
        /// <param name="digits">The number of fractional digits.</param>
        /// <returns>The formatted number.</returns>
        public String Format(double d, int digits)
        {
            int digits2 = Math.Min(digits, MAX_FORMATS);
            digits2 = Math.Max(digits2, 0);
            return d.ToString(this.formats[digits2], numberFormat);
        }
    }
}
