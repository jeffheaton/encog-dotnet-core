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
    ///     A a range normalizer forces a value to fall in a specific range.
    /// </summary>
    [Serializable]
    public class RangeNormalizer : INormalizer
    {
        /// <summary>
        ///     The normalized high value.
        /// </summary>
        private readonly double _normalizedHigh;

        /// <summary>
        ///     The normalized low value.
        /// </summary>
        private readonly double _normalizedLow;

        /// <summary>
        ///     Construct the range normalizer.
        /// </summary>
        /// <param name="theNormalizedLow">The normalized low value.</param>
        /// <param name="theNormalizedHigh">The normalized high value.</param>
        public RangeNormalizer(double theNormalizedLow, double theNormalizedHigh)
        {
            _normalizedLow = theNormalizedLow;
            _normalizedHigh = theNormalizedHigh;
        }

        /// <inheritdoc />
        public int OutputSize(ColumnDefinition colDef)
        {
            return 1;
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, String value,
            double[] outputData, int outputColumn)
        {
            throw new EncogError("Can't range-normalize a string value: " + value);
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, double value,
            double[] outputData, int outputColumn)
        {
            double result = ((value - colDef.Low)/(colDef.High - colDef.Low))
                            *(_normalizedHigh - _normalizedLow)
                            + _normalizedLow;

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.
            if (Double.IsNaN(result))
            {
                result = ((_normalizedHigh - _normalizedLow)/2) + _normalizedLow;
            }

            outputData[outputColumn] = result;

            return outputColumn + 1;
        }

        /// <inheritdoc />
        public String DenormalizeColumn(ColumnDefinition colDef, IMLData data,
            int dataColumn)
        {
            double value = data[dataColumn];
            double result = ((colDef.Low - colDef.High)*value
                             - _normalizedHigh*colDef.Low + colDef.High
                             *_normalizedLow)
                            /(_normalizedLow - _normalizedHigh);

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.
            if (Double.IsNaN(result))
            {
                return "" + (((_normalizedHigh - _normalizedLow)/2) + _normalizedLow);
            }
            return "" + result;
        }
    }
}
