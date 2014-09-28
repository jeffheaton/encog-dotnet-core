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
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers
{
    /// <summary>
    /// The normalizer interface defines how to normalize a column.  The source of the
    /// normalization can be either string or double.
    /// </summary>
    public interface INormalizer
    {
        /// <summary>
        /// Determine the normalized size of the specified column.
        /// </summary>
        /// <param name="colDef">The column to check.</param>
        /// <returns>The size of the column normalized.</returns>
        int OutputSize(ColumnDefinition colDef);

        /// <summary>
        ///  Normalize a column from a string. The output will go to an array, starting at outputColumn. 
        /// </summary>
        /// <param name="colDef">The column that is being normalized.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outputData">The array to output to.</param>
        /// <param name="outputIndex">The index to start at in outputData.</param>
        /// <returns>The new index (in outputData) that we've moved to.</returns>
        int NormalizeColumn(ColumnDefinition colDef, String value,
                double[] outputData, int outputIndex);

        /// <summary>
        /// Normalize a column from a double. The output will go to an array, starting at outputColumn. 
        /// </summary>
        /// <param name="colDef">The column that is being normalized.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outputData">The array to output to.</param>
        /// <param name="outputIndex">The index to start at in outputData.</param>
        /// <returns>The new index (in outputData) that we've moved to.</returns>
        int NormalizeColumn(ColumnDefinition colDef, double value,
                double[] outputData, int outputIndex);

        /// <summary>
        /// Denormalize a value. 
        /// </summary>
        /// <param name="colDef">The column to denormalize.</param>
        /// <param name="data">The data to denormalize.</param>
        /// <param name="dataIndex">The starting location inside data.</param>
        /// <returns>The denormalized value.</returns>
        String DenormalizeColumn(ColumnDefinition colDef, IMLData data,
                int dataIndex);
    }
}
