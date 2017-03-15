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
    ///     Normalize to one-of-n for nominal values. For example, "one", "two", "three"
    ///     becomes 1,0,0 and 0,1,0 and 0,0,1 etc. Assuming 0 and 1 were the min/max.
    /// </summary>
    [Serializable]
    public class OneOfNNormalizer : INormalizer
    {
        /// <summary>
        ///     The normalized high.
        /// </summary>
        private readonly double _normalizedHigh;

        /// <summary>
        ///     The normalized low.
        /// </summary>
        private readonly double _normalizedLow;

        /// <summary>
        ///     Construct the normalizer.
        /// </summary>
        /// <param name="theNormalizedLow">The normalized low.</param>
        /// <param name="theNormalizedHigh">The normalized high.</param>
        public OneOfNNormalizer(double theNormalizedLow, double theNormalizedHigh)
        {
            _normalizedLow = theNormalizedLow;
            _normalizedHigh = theNormalizedHigh;
        }

        /// <inheritdoc />
        public int OutputSize(ColumnDefinition colDef)
        {
            return colDef.Classes.Count;
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, String value,
            double[] outputData, int outputColumn)
        {
            for (int i = 0; i < colDef.Classes.Count; i++)
            {
                double d = _normalizedLow;

                if (colDef.Classes[i].Equals(value))
                {
                    d = _normalizedHigh;
                }

                outputData[outputColumn + i] = d;
            }
            return outputColumn + colDef.Classes.Count;
        }

        /// <inheritdoc />
        public String DenormalizeColumn(ColumnDefinition colDef, IMLData data,
            int dataColumn)
        {
            double bestValue = Double.NegativeInfinity;
            int bestIndex = 0;

            for (int i = 0; i < data.Count; i++)
            {
                double d = data[dataColumn + i];
                if (d > bestValue)
                {
                    bestValue = d;
                    bestIndex = i;
                }
            }

            return colDef.Classes[bestIndex];
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, double value,
            double[] outputData, int outputColumn)
        {
            throw new EncogError(
                "Can't use a one-of-n normalizer on a continuous value: "
                + value);
        }
    }
}
