using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Error
{
    /// <summary>
    /// Selects the error calculation mode for Encog.
    /// </summary>
    public enum ErrorCalculationMode
    {
        /// <summary>
        /// Root mean square error.
        /// </summary>
        RMS,

        /// <summary>
        /// Mean square error.
        /// </summary>
        MSE,

        /// <summary>
        /// Used for QuickProp, an exaggerated error function. 
        /// Fahlman suggests using a function that exaggerates the difference the larger the error is 
        /// in a non-linear fashion.
        /// </summary>
        ARCTAN
    }
}
