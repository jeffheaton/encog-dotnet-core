using System;
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers
{
    /// <summary>
    ///     A a range normalizer forces a value to fall in a specific range.
    /// </summary>
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