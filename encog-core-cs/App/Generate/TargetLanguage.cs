using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Generate
{
    /// <summary>
    /// Specifies the target language for Encog code generation.
    /// </summary>
    public enum TargetLanguage
    {
        /// <summary>
        /// No code generation.
        /// </summary>
        NoGeneration,
        /// <summary>
        /// Generate using Java.
        /// </summary>
        Java,
        /// <summary>
        /// Generate using Javascript.
        /// </summary>
        JavaScript,
        /// <summary>
        /// Generate using C#.
        /// </summary>
        CSharp,
        /// <summary>
        /// Generate for MetaTrader 4 using MQL4.
        /// </summary>
        MQL4,
        /// <summary>
        /// Generate for NinjaTrader 7 using NinjaScript.
        /// </summary>
        NinjaScript

    }
}
