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