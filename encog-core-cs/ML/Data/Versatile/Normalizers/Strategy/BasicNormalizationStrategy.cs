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
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers.Strategy
{
    /// <summary>
    ///     Provides a basic normalization strategy that will work with most models built into Encog.
    ///     This is often used as a starting point for building more customized models, as this
    ///     normalizer works mainly by using maps to define which normalizer to use for what
    ///     data type.
    /// </summary>
    [Serializable]
    public class BasicNormalizationStrategy : INormalizationStrategy
    {
        /// <summary>
        ///     Mapping to all of the input normalizers.
        /// </summary>
        private readonly IDictionary<ColumnType, INormalizer> _inputNormalizers =
            new Dictionary<ColumnType, INormalizer>();

        /// <summary>
        ///     Mapping to all of the output normalizers.
        /// </summary>
        private readonly IDictionary<ColumnType, INormalizer> _outputNormalizers =
            new Dictionary<ColumnType, INormalizer>();

        /// <summary>
        ///     Construct the basic normalization strategy.
        /// </summary>
        /// <param name="inputLow">The desired low to normalize input into.</param>
        /// <param name="inputHigh">The desired high to normalize input into.</param>
        /// <param name="outputLow">The desired low to normalize output into.</param>
        /// <param name="outputHigh">The desired high to normalize output into.</param>
        public BasicNormalizationStrategy(double inputLow, double inputHigh, double outputLow, double outputHigh)
        {
            AssignInputNormalizer(ColumnType.Continuous, new RangeNormalizer(inputLow, inputHigh));
            AssignInputNormalizer(ColumnType.Nominal, new OneOfNNormalizer(inputLow, inputHigh));
            AssignInputNormalizer(ColumnType.Ordinal, new RangeOrdinal(inputLow, inputHigh));

            AssignOutputNormalizer(ColumnType.Continuous, new RangeNormalizer(outputLow, outputHigh));
            AssignOutputNormalizer(ColumnType.Nominal, new OneOfNNormalizer(outputLow, outputHigh));
            AssignOutputNormalizer(ColumnType.Ordinal, new RangeOrdinal(outputLow, outputHigh));
        }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public BasicNormalizationStrategy()
        {
        }

        /// <summary>
        ///     The input normalizers.
        /// </summary>
        public IDictionary<ColumnType, INormalizer> InputNormalizers
        {
            get { return _inputNormalizers; }
        }

        /// <summary>
        ///     The output normalizers.
        /// </summary>
        public IDictionary<ColumnType, INormalizer> OutputNormalizers
        {
            get { return _outputNormalizers; }
        }

        /// <inheritdoc />
        public int NormalizedSize(ColumnDefinition colDef, bool isInput)
        {
            INormalizer norm = FindNormalizer(colDef, isInput);
            return norm.OutputSize(colDef);
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, bool isInput,
            String value, double[] outputData, int outputColumn)
        {
            INormalizer norm = FindNormalizer(colDef, isInput);
            return norm.NormalizeColumn(colDef, value, outputData, outputColumn);
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, bool isInput,
            double value, double[] outputData, int outputColumn)
        {
            INormalizer norm = FindNormalizer(colDef, isInput);
            return norm.NormalizeColumn(colDef, value, outputData, outputColumn);
        }

        /// <inheritdoc />
        public String DenormalizeColumn(ColumnDefinition colDef, bool isInput,
            IMLData data, int dataColumn)
        {
            INormalizer norm = FindNormalizer(colDef, isInput);
            return norm.DenormalizeColumn(colDef, data, dataColumn);
        }

        /// <summary>
        ///     Assign a normalizer to the specified column type for output.
        /// </summary>
        /// <param name="colType">The column type.</param>
        /// <param name="norm">The normalizer.</param>
        public void AssignInputNormalizer(ColumnType colType, INormalizer norm)
        {
            _inputNormalizers[colType] = norm;
        }

        /// <summary>
        ///     Assign a normalizer to the specified column type for output.
        /// </summary>
        /// <param name="colType">The column type.</param>
        /// <param name="norm">The normalizer.</param>
        public void AssignOutputNormalizer(ColumnType colType, INormalizer norm)
        {
            _outputNormalizers[colType] = norm;
        }

        /// <summary>
        ///     Find a normalizer for the specified column definition, and if it is input or output.
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True if the column is input.</param>
        /// <returns>The normalizer to use.</returns>
        private INormalizer FindNormalizer(ColumnDefinition colDef, bool isInput)
        {
            INormalizer norm = null;

            if (isInput)
            {
                if (_inputNormalizers.ContainsKey(colDef.DataType))
                {
                    norm = _inputNormalizers[colDef.DataType];
                }
            }
            else
            {
                if (_outputNormalizers.ContainsKey(colDef.DataType))
                {
                    norm = _outputNormalizers[colDef.DataType];
                }
            }

            if (norm == null)
            {
                throw new EncogError("No normalizer defined for input=" + isInput + ", type=" + colDef.DataType);
            }
            return norm;
        }
    }
}
