using System;
using Encog.ML.Data.Versatile.Columns;

namespace Encog.ML.Data.Versatile.Normalizers
{
    /// <summary>
    ///     Normalize an ordinal into a specific range. An ordinal is a string value that
    ///     has order. For example "first grade", "second grade", ... "freshman", ...,
    ///     "senior". These values are mapped to an increasing index.
    /// </summary>
    public class RangeOrdinal : INormalizer
    {
        /// <summary>
        ///     The high range of the normalized data.
        /// </summary>
        private readonly double _normalizedHigh;

        /// <summary>
        ///     The low range of the normalized data.
        /// </summary>
        private readonly double _normalizedLow;

        /// <summary>
        ///     Construct with normalized high and low.
        /// </summary>
        /// <param name="theNormalizedLow">The normalized low value.</param>
        /// <param name="theNormalizedHigh">The normalized high value.</param>
        public RangeOrdinal(double theNormalizedLow, double theNormalizedHigh)
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
        public int NormalizeColumn(ColumnDefinition colDef, String theValue,
            double[] outputData, int outputColumn)
        {
            // Find the index of the ordinal
            int v = colDef.Classes.IndexOf(theValue);
            if (v == -1)
            {
                throw new EncogError("Unknown ordinal: " + theValue);
            }

            double high = colDef.Classes.Count;
            double value = v;

            double result = (value/high)
                            *(_normalizedHigh - _normalizedLow)
                            + _normalizedLow;

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.
            if (Double.IsNaN(result))
            {
                result = ((_normalizedHigh - _normalizedLow)/2)
                         + _normalizedLow;
            }

            outputData[outputColumn] = result;

            return outputColumn + 1;
        }

        /// <inheritdoc />
        public int NormalizeColumn(ColumnDefinition colDef, double value,
            double[] outputData, int outputColumn)
        {
            throw new EncogError(
                "Can't ordinal range-normalize a continuous value: " + value);
        }

        /// <inheritdoc />
        public String DenormalizeColumn(ColumnDefinition colDef, IMLData data,
            int dataColumn)
        {
            double high = colDef.Classes.Count;
            double low = 0;

            double value = data[dataColumn];
            double result = ((low - high)*value - _normalizedHigh*low + high
                             *_normalizedLow)
                            /(_normalizedLow - _normalizedHigh);

            // typically caused by a number that should not have been normalized
            // (i.e. normalization or actual range is infinitely small.
            if (Double.IsNaN(result))
            {
                return colDef.Classes[0];
            }
            return colDef.Classes[(int) result];
        }
    }
}