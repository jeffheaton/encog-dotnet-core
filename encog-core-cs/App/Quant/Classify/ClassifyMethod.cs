using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Classify
{
    /// <summary>
    /// The classification method to use.
    /// </summary>
    public enum ClassifyMethod
    {
        /// <summary>
        /// Use the "one-of" classification method.
        /// </summary>
        OneOf,

        /// <summary>
        /// Use the equilateral classification method.
        /// </summary>
        Equilateral,

        /// <summary>
        /// Use a single-field classification method.
        /// </summary>
        SingleField
    }
}
