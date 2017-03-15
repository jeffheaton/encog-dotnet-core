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
using System.Linq;
using System.Text;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Missing;
using Encog.ML.Data.Versatile.Normalizers.Strategy;
using Encog.Util.CSV;

namespace Encog.ML.Data.Versatile
{
    /// <summary>
    ///     This class is used to perform normalizations for methods trained with the
    ///     versatile dataset.
    /// </summary>
    [Serializable]
    public class NormalizationHelper
    {
        /// <summary>
        ///     The columns, from the source columns, used for input to the model.
        /// </summary>
        private readonly IList<ColumnDefinition> _inputColumns = new List<ColumnDefinition>();

        /// <summary>
        ///     The missing column handlers.
        /// </summary>
        private readonly IDictionary<ColumnDefinition, IMissingHandler> _missingHandlers =
            new Dictionary<ColumnDefinition, IMissingHandler>();

        /// <summary>
        ///     The columns, from the source columns, used for output from the model.
        /// </summary>
        private readonly IList<ColumnDefinition> _outputColumns = new List<ColumnDefinition>();

        /// <summary>
        ///     The source columns from the original file. These are then added to the
        ///     input and output columns.
        /// </summary>
        private readonly IList<ColumnDefinition> _sourceColumns = new List<ColumnDefinition>();

        /// <summary>
        ///     What to do with unknown values.
        /// </summary>
        private readonly IList<String> _unknownValues = new List<string>();

        /// <summary>
        ///     Default constructor;
        /// </summary>
        public NormalizationHelper()
        {
            Format = CSVFormat.English;
        }

        /// <summary>
        ///     The normalizaton strategy to use.
        /// </summary>
        public INormalizationStrategy NormStrategy { get; set; }

        /// <summary>
        ///     The CSV format to use.
        /// </summary>
        public CSVFormat Format { get; set; }

        /// <summary>
        ///     The source columns.
        /// </summary>
        public IList<ColumnDefinition> SourceColumns
        {
            get { return _sourceColumns; }
        }


        /// <summary>
        ///     The input columns.
        /// </summary>
        public IList<ColumnDefinition> InputColumns
        {
            get { return _inputColumns; }
        }


        /// <summary>
        ///     The output columns.
        /// </summary>
        public IList<ColumnDefinition> OutputColumns
        {
            get { return _outputColumns; }
        }

        /// <summary>
        ///     The unknown values.
        /// </summary>
        public IList<string> UnknownValues
        {
            get { return _unknownValues; }
        }

        /// <summary>
        ///     Add a source column. These define the raw input.
        /// </summary>
        /// <param name="def">The column definition.</param>
        public void AddSourceColumn(ColumnDefinition def)
        {
            _sourceColumns.Add(def);
            def.Owner = this;
        }

        /// <summary>
        ///     Define a source column. These define the raw input. Use this function if
        ///     you know the index of the column in a non-header file.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="index">The index of the column, needed for non-headered files.</param>
        /// <param name="colType">The column type.</param>
        /// <returns>The column definition</returns>
        public ColumnDefinition DefineSourceColumn(string name, int index,
            ColumnType colType)
        {
            var result = new ColumnDefinition(name, colType) {Index = index};
            AddSourceColumn(result);
            return result;
        }

        /// <inheritdoc />
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[NormalizationHelper:\n");
            foreach (ColumnDefinition colDef in _sourceColumns)
            {
                result.Append(colDef);
                result.Append("\n");
            }
            result.Append("]");
            return result.ToString();
        }

        /// <summary>
        ///     Clear the input/output columns, but not the source columns.
        /// </summary>
        public void ClearInputOutput()
        {
            _inputColumns.Clear();
            _outputColumns.Clear();
        }

        /// <summary>
        ///     Normalize a single input column.
        /// </summary>
        /// <param name="i">The column definition index (from the input columns).</param>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized result.</returns>
        public double[] NormalizeInputColumn(int i, String value)
        {
            ColumnDefinition colDef = _inputColumns[i];
            var result = new double[NormStrategy.NormalizedSize(colDef,
                true)];
            NormStrategy.NormalizeColumn(colDef, true, value, result, 0);
            return result;
        }

        /// <summary>
        ///     Normalize a single output column.
        /// </summary>
        /// <param name="i">The column definition index (from the output columns).</param>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized result.</returns>
        public double[] NormalizeOutputColumn(int i, string value)
        {
            ColumnDefinition colDef = _outputColumns[i];
            var result = new double[NormStrategy.NormalizedSize(colDef,
                false)];
            NormStrategy.NormalizeColumn(colDef, false, value, result, 0);
            return result;
        }

        /// <summary>
        ///     Calculate the number of elements the input will normalize to.
        /// </summary>
        /// <returns>The number of elements the input will normalize to.</returns>
        public int CalculateNormalizedInputCount()
        {
            return _inputColumns.Sum(colDef => NormStrategy.NormalizedSize(colDef, true));
        }

        /// <summary>
        ///     Calculate the number of elements the output will normalize to.
        /// </summary>
        /// <returns>The number of elements the output will normalize to.</returns>
        public int CalculateNormalizedOutputCount()
        {
            return _outputColumns.Sum(colDef => NormStrategy.NormalizedSize(colDef, false));
        }

        /// <summary>
        ///     Allocate a data item large enough to hold a single input vector.
        /// </summary>
        /// <returns>The data element.</returns>
        public IMLData AllocateInputVector()
        {
            return AllocateInputVector(1);
        }

        /// <summary>
        ///     Allocate a data item large enough to hold several input vectors. This is
        ///     normally used for timeslices.
        /// </summary>
        /// <param name="multiplier">How many input vectors.</param>
        /// <returns>The data element.</returns>
        public IMLData AllocateInputVector(int multiplier)
        {
            return new BasicMLData(CalculateNormalizedInputCount()*multiplier);
        }

        /// <summary>
        ///     Denormalize a complete output vector to an array of strings.
        /// </summary>
        /// <param name="output">The data vector to denorm, the source.</param>
        /// <returns>The denormalized vector.</returns>
        public String[] DenormalizeOutputVectorToString(IMLData output)
        {
            var result = new String[_outputColumns.Count];

            int idx = 0;
            for (int i = 0; i < _outputColumns.Count; i++)
            {
                ColumnDefinition colDef = _outputColumns[i];
                result[i] = NormStrategy.DenormalizeColumn(colDef, false,
                    output, idx);
                idx += NormStrategy.NormalizedSize(colDef, false);
            }

            return result;
        }

        /// <summary>
        ///     Define the string that signifies an unknown value (eg "?")
        /// </summary>
        /// <param name="str">The string for unknowns.</param>
        public void DefineUnknownValue(String str)
        {
            _unknownValues.Add(str);
        }

        /// <summary>
        ///     Normalize a single column to the input vector.
        /// </summary>
        /// <param name="colDef">The column to normalize.</param>
        /// <param name="outputColumn">The current position in the vector.</param>
        /// <param name="output">The vector to output to.</param>
        /// <param name="isInput">Is this an input column.</param>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The new current position in the vector.</returns>
        public int NormalizeToVector(ColumnDefinition colDef, int outputColumn,
            double[] output, bool isInput, String value)
        {
            IMissingHandler handler = null;

            if (_unknownValues.Contains(value))
            {
                if (!_missingHandlers.ContainsKey(colDef))
                {
                    throw new EncogError(
                        "Do not know how to process missing value \"" + value
                        + "\" in field: " + colDef.Name);
                }
                handler = _missingHandlers[colDef];
            }

            if (colDef.DataType == ColumnType.Continuous)
            {
                double d = ParseDouble(value);
                if (handler != null)
                {
                    d = handler.ProcessDouble(colDef);
                }
                return NormStrategy.NormalizeColumn(colDef, isInput, d,
                    output, outputColumn);
            }
            if (handler != null)
            {
                value = handler.ProcessString(colDef);
            }
            return NormStrategy.NormalizeColumn(colDef, isInput, value,
                output, outputColumn);
        }

        /// <summary>
        ///     Parse a double, using the correct formatter.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The double.</returns>
        public double ParseDouble(string str)
        {
            return Format.Parse(str);
        }

        /// <summary>
        ///     Define a missing value handler.
        /// </summary>
        /// <param name="colDef">The column this handler applies to.</param>
        /// <param name="handler">The handler.</param>
        public void DefineMissingHandler(ColumnDefinition colDef,
            IMissingHandler handler)
        {
            _missingHandlers[colDef] = handler;
            handler.Init(this);
        }

        /// <summary>
        ///     Normalize a string array to an input vector.
        /// </summary>
        /// <param name="line">The unnormalized string array.</param>
        /// <param name="data">The output data.</param>
        /// <param name="originalOrder">Should the output be forced into the original column order?</param>
        public void NormalizeInputVector(String[] line, double[] data,
            bool originalOrder)
        {
            int outputIndex = 0;
            int i = 0;
            foreach (ColumnDefinition colDef in _inputColumns)
            {
                int idx = originalOrder ? _sourceColumns.IndexOf(colDef) : i;
                outputIndex = NormalizeToVector(colDef, outputIndex, data, false,
                    line[idx]);
                i++;
            }
        }
    }
}
