using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Normalize
{
    /// <summary>
    /// Normalization actions desired.
    /// </summary>
    public enum NormalizationDesired
    {
        /// <summary>
        /// Do not normalize the column, just allow it to pass through.  This allows 
        /// string fields to pass through as well.
        /// </summary>
        PassThrough,

        /// <summary>
        /// Normalize this column.
        /// </summary>
        Normalize,

        /// <summary>
        /// Ignore this column, do not include in the output.
        /// </summary>
        Ignore
    }
}
