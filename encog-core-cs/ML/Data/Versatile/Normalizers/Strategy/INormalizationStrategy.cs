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
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers.Strategy
{
    /// <summary>
    /// Defines the interface to a normalization strategy.
    /// </summary>
    public interface INormalizationStrategy
    {
        /// <summary>
        /// Calculate how many elements a column will normalize into.
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <returns>The number of elements needed to normalize this column.</returns>
        int NormalizedSize(ColumnDefinition colDef, bool isInput);

        /// <summary>
        /// Normalize a column, with a string input. 
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outpuData">The output data.</param>
        /// <param name="outputColumn">The element to begin outputing to.</param>
        /// <returns>The new output element, advanced by the correct amount.</returns>
        int NormalizeColumn(ColumnDefinition colDef, bool isInput, string value,
                double[] outpuData, int outputColumn);

        
        /// <summary>
        /// Normalize a column, with a double input. 
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <param name="output">The value to normalize.</param>
        /// <param name="idx">The output data.</param>
        /// <returns>The new output element, advanced by the correct amount.</returns>
        string DenormalizeColumn(ColumnDefinition colDef, bool isInput, IMLData output,
                int idx);
        
        /// <summary>
        /// Normalize a column, with a double value. 
        /// </summary>
        /// <param name="colDef">The column definition.</param>
        /// <param name="isInput">True, if this is an input column.</param>
        /// <param name="value">The value to normalize.</param>
        /// <param name="outpuData">The output data.</param>
        /// <param name="outputColumn">The element to begin outputing to.</param>
        /// <returns>The new output element, advanced by the correct amount.</returns>
        int NormalizeColumn(ColumnDefinition colDef, bool isInput, double value,
                double[] outpuData, int outputColumn);
    }
}
