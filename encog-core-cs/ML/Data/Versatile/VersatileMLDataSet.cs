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
using Encog.MathUtil.Randomize.Generate;
using Encog.ML.Data.Versatile.Columns;
using Encog.ML.Data.Versatile.Division;
using Encog.ML.Data.Versatile.Normalizers.Strategy;
using Encog.ML.Data.Versatile.Sources;
using Encog.Util;

namespace Encog.ML.Data.Versatile
{
    /// <summary>
    ///     The versatile dataset supports several advanced features. 1. it can directly
    ///     read and normalize from a CSV file. 2. It supports virtual time-boxing for
    ///     time series data (the data is NOT expanded in memory). 3. It can easily be
    ///     segmented into smaller datasets.
    /// </summary>
    public class VersatileMLDataSet : MatrixMLDataSet
    {
        /// <summary>
        ///     The source that data is being pulled from.
        /// </summary>
        private readonly IVersatileDataSource _source;

        /// <summary>
        ///     The number of rows that were analyzed.
        /// </summary>
        private int _analyzedRows;

        /// <summary>
        ///     Construct the data source.
        /// </summary>
        /// <param name="theSource">The data source.</param>
        public VersatileMLDataSet(IVersatileDataSource theSource) : this()
        {
            _source = theSource;
        }

        /// <summary>
        ///     The normalization helper.
        /// </summary>
        public NormalizationHelper NormHelper { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VersatileMLDataSet()
        {
            NormHelper = new NormalizationHelper();
        }

        /// <summary>
        ///     Find the index of a column.
        /// </summary>
        /// <param name="colDef">The column.</param>
        /// <returns>The column index.</returns>
        private int FindIndex(ColumnDefinition colDef)
        {
            if (colDef.Index != -1)
            {
                return colDef.Index;
            }

            int index = _source.ColumnIndex(colDef.Name);
            colDef.Index = index;

            if (index == -1)
            {
                throw new EncogError("Can't find column");
            }

            return index;
        }

        /// <summary>
        ///     Analyze the input and determine max, min, mean, etc.
        /// </summary>
        public void Analyze()
        {
            String[] line;

            // Collect initial stats: sums (for means), highs, lows.
            _source.Rewind();
            int c = 0;
            while ((line = _source.ReadLine()) != null)
            {
                c++;
                foreach (ColumnDefinition colDef in NormHelper.SourceColumns)
                {
                    int index = FindIndex(colDef);
                    String value = line[index];
                    colDef.Analyze(value);
                }
            }
            _analyzedRows = c;

            // Calculate the means, and reset for sd calc.
            foreach (ColumnDefinition colDef in NormHelper.SourceColumns)
            {
                // Only calculate mean/sd for continuous columns.
                if (colDef.DataType == ColumnType.Continuous)
                {
                    colDef.Mean = colDef.Mean/colDef.Count;
                    colDef.Sd = 0;
                }
            }

            // Sum the standard deviation
            _source.Rewind();
            while ((line = _source.ReadLine()) != null)
            {
                for (int i = 0; i < NormHelper.SourceColumns.Count; i++)
                {
                    ColumnDefinition colDef = NormHelper.SourceColumns[i];
                    String value = line[i];
                    if (colDef.DataType == ColumnType.Continuous)
                    {
                        double d = NormHelper.ParseDouble(value);
                        d = colDef.Mean - d;
                        d = d*d;
                        colDef.Sd = colDef.Sd + d;
                    }
                }
            }

            // Calculate the standard deviations.
            foreach (ColumnDefinition colDef in NormHelper.SourceColumns)
            {
                // Only calculate sd for continuous columns.
                if (colDef.DataType == ColumnType.Continuous)
                {
                    colDef.Sd = Math.Sqrt(colDef.Sd/colDef.Count);
                }
            }
        }

        /// <summary>
        ///     Normalize the data set, and allocate memory to hold it.
        /// </summary>
        public void Normalize()
        {
            INormalizationStrategy strat = NormHelper.NormStrategy;

            if (strat == null)
            {
                throw new EncogError(
                    "Please choose a model type first, with selectMethod.");
            }

            int normalizedInputColumns = NormHelper
                .CalculateNormalizedInputCount();
            int normalizedOutputColumns = NormHelper
                .CalculateNormalizedOutputCount();

            int normalizedColumns = normalizedInputColumns
                                    + normalizedOutputColumns;
            CalculatedIdealSize = normalizedOutputColumns;
            CalculatedInputSize = normalizedInputColumns;

            Data = EngineArray.AllocateDouble2D(_analyzedRows, normalizedColumns);

            _source.Rewind();
            String[] line;
            int row = 0;
            while ((line = _source.ReadLine()) != null)
            {
                int column = 0;
                foreach (ColumnDefinition colDef in NormHelper.InputColumns)
                {
                    int index = FindIndex(colDef);
                    string value = line[index];

                    column = NormHelper.NormalizeToVector(colDef, column,
                        Data[row], true, value);
                }

                foreach (ColumnDefinition colDef in NormHelper.OutputColumns)
                {
                    int index = FindIndex(colDef);
                    string value = line[index];

                    column = NormHelper.NormalizeToVector(colDef, column,
                        Data[row], false, value);
                }
                row++;
            }
        }

        /// <summary>
        ///     Define a source column. Used when the file does not contain headings.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="index">The index of the column.</param>
        /// <param name="colType">The column type.</param>
        /// <returns>The column definition.</returns>
        public ColumnDefinition DefineSourceColumn(String name, int index,
            ColumnType colType)
        {
            return NormHelper.DefineSourceColumn(name, index, colType);
        }

        /// <summary>
        ///     Divide, and optionally shuffle, the dataset.
        /// </summary>
        /// <param name="dataDivisionList">The desired divisions.</param>
        /// <param name="shuffle">True, if we should shuffle.</param>
        /// <param name="rnd">Random number generator, often with a specific seed.</param>
        public void Divide(IList<DataDivision> dataDivisionList, bool shuffle,
            IGenerateRandom rnd)
        {
            if (Data == null)
            {
                throw new EncogError(
                    "Can't divide, data has not yet been generated/normalized.");
            }

            var divide = new PerformDataDivision(shuffle, rnd);
            divide.Perform(dataDivisionList, this, CalculatedInputSize,
                CalculatedIdealSize);
        }

        /// <summary>
        ///     Define an output column.
        /// </summary>
        /// <param name="col">The output column.</param>
        public void DefineOutput(ColumnDefinition col)
        {
            NormHelper.OutputColumns.Add(col);
        }

        /// <summary>
        ///     Define an input column.
        /// </summary>
        /// <param name="col">The input column.</param>
        public void DefineInput(ColumnDefinition col)
        {
            NormHelper.InputColumns.Add(col);
        }

        /// <summary>
        ///     Define a single column as an output column, all others as inputs.
        /// </summary>
        /// <param name="outputColumn">The output column.</param>
        public void DefineSingleOutputOthersInput(ColumnDefinition outputColumn)
        {
            NormHelper.ClearInputOutput();

            foreach (ColumnDefinition colDef in NormHelper.SourceColumns)
            {
                if (colDef == outputColumn)
                {
                    DefineOutput(colDef);
                }
                else if (colDef.DataType != ColumnType.Ignore)
                {
                    DefineInput(colDef);
                }
            }
        }

        /// <summary>
        ///     Define a source column.
        /// </summary>
        /// <param name="name">The name of the source column.</param>
        /// <param name="colType">The column type.</param>
        /// <returns>The column definition.</returns>
        public ColumnDefinition DefineSourceColumn(string name, ColumnType colType)
        {
            return NormHelper.DefineSourceColumn(name, -1, colType);
        }
    }
}
