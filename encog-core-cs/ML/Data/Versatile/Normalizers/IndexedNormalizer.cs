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
    ///     Normalize ordinal/nominal values to a single value that is simply the index
    ///     of the class in the list. For example, "one", "two", "three" normalizes to
    ///     0,1,2.
    /// </summary>
    public class IndexedNormalizer : INormalizer
    {
        /// <inheritdoc />
        public int OutputSize(ColumnDefinition colDef)
        {
            return 1;
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, String value,
            double[] outputData, int outputColumn)
        {
            if (!colDef.Classes.Contains(value))
            {
                throw new EncogError("Undefined value: " + value);
            }

            outputData[outputColumn] = colDef.Classes.IndexOf(value);
            return outputColumn + 1;
        }

        /// <inheritdoc />
        public String DenormalizeColumn(ColumnDefinition colDef, IMLData data,
            int dataColumn)
        {
            return colDef.Classes[(int) data[dataColumn]];
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, double value,
            double[] outputData, int outputColumn)
        {
            throw new EncogError(
                "Can't use an indexed normalizer on a continuous value: "
                + value);
        }
    }
}
