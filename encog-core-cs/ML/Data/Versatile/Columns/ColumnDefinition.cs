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
using System.Collections.Generic;
using System.Text;
using Encog.Util;

namespace Encog.ML.Data.Versatile.Columns
{
    /// <summary>
    ///     Defines a column definition.
    /// </summary>
    [Serializable]
    public class ColumnDefinition
    {
        /// <summary>
        ///     The classes of a catagorical.
        /// </summary>
        private readonly IList<String> _classes = new List<String>();

        /// <summary>
        ///     The normalization helper.
        /// </summary>
        public NormalizationHelper Owner { get; set; }

        /// <summary>
        ///     The column definition.
        /// </summary>
        /// <param name="theName">The name of the column.</param>
        /// <param name="theDataType">The type of the column.</param>
        public ColumnDefinition(String theName, ColumnType theDataType)
        {
            Name = theName;
            DataType = theDataType;
            Count = -1;
            Low = High = Mean = Sd = Double.NaN;
        }

        /// <summary>
        ///     The name of the column.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        ///     The type of column.
        /// </summary>
        public ColumnType DataType { get; set; }

        /// <summary>
        ///     The observed low in a dataset.
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        ///     The observed high in a dataset.
        /// </summary>
        public double High { get; set; }

        /// <summary>
        ///     The observed mean in a dataset.
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        ///     The observed standard deviation in a dataset.
        /// </summary>
        public double Sd { get; set; }

        /// <summary>
        ///     The observed count for a catagorical column.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     The index of this column in the dataset.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The classes for a catagorical type.
        /// </summary>
        public IList<String> Classes
        {
            get { return _classes; }
        }

        /// <summary>
        ///     Analyze the specified value.
        /// </summary>
        /// <param name="value">The value to analyze.</param>
        public void Analyze(string value)
        {
            switch (DataType)
            {
                case ColumnType.Continuous:
                    AnalyzeContinuous(value);
                    break;
                case ColumnType.Ordinal:
                    AnalyzeOrdinal(value);
                    break;
                case ColumnType.Nominal:
                    AnalyzeNominal(value);
                    break;
            }
        }

        /**
	 * Analyze a nominal value.
	 * @param value The value to analyze.
	 */

        private void AnalyzeNominal(String value)
        {
            if (!_classes.Contains(value))
            {
                _classes.Add(value);
            }
        }

        /**
	 * Analyze a nominal value.
	 * @param value The value to analyze.
	 */

        private void AnalyzeOrdinal(String value)
        {
            if (!_classes.Contains(value))
            {
                throw (new EncogError("You must predefine any ordinal values (in order). Undefined ordinal value: " +
                                      value));
            }
        }

        /// <summary>
        ///     Analyze a nominal value.
        /// </summary>
        /// <param name="value">The value to analyze.</param>
        private void AnalyzeContinuous(String value)
        {
            double d = Owner.Format.Parse(value);
            if (Count < 0)
            {
                Low = d;
                High = d;
                Mean = d;
                Sd = 0;
                Count = 1;
            }
            else
            {
                Mean = Mean + d;
                Low = Math.Min(Low, d);
                High = Math.Max(High, d);
                Count++;
            }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[");
            result.Append("ColumnDefinition:");
            result.Append(Name);
            result.Append("(");
            result.Append(DataType);
            result.Append(")");
            if (DataType == ColumnType.Continuous)
            {
                result.Append(";low=");
                result.Append(Format.FormatDouble(Low, 6));
                result.Append(",high=");
                result.Append(Format.FormatDouble(High, 6));
                result.Append(",mean=");
                result.Append(Format.FormatDouble(Mean, 6));
                result.Append(",sd=");
                result.Append(Format.FormatDouble(Sd, 6));
            }
            else
            {
                result.Append(";");
                result.Append(_classes);
            }
            result.Append("]");
            return result.ToString();
        }

        /**
	 * Define a class for a catagorical value.
	 * @param str The class to add.
	 */

        public void DefineClass(string str)
        {
            _classes.Add(str);
        }

        /// <summary>
        /// Define an array of classes for a catagorical value.
        /// </summary>
        /// <param name="str">The classes to add.</param>
        public void DefineClass(string[] str)
        {
            foreach (string s in str)
            {
                DefineClass(s);
            }
        }
    }
}
