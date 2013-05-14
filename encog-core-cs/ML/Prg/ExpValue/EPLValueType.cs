using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Prg.ExpValue
{
    /// <summary>
    /// The type of value.
    /// </summary>
    public enum EPLValueType
    {
        /// <summary>
        /// Floating point.
        /// </summary>
        floatingType,
        /// <summary>
        /// String
        /// </summary>
        stringType,
        /// <summary>
        /// boolean
        /// </summary>
        booleanType,
        /// <summary>
        /// Integer.
        /// </summary>
        intType,
        /// <summary>
        /// Enumeration
        /// </summary>
        enumType,
        /// <summary>
        /// Unknown.
        /// </summary>
        unknown
    }
}
