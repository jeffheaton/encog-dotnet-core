using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML
{
    /// <summary>
    /// This interface defines the fact that a class, or object, is having the
    /// ability to generate an Encog factory code from the objects instanciated
    /// state.
    /// </summary>
    public interface IMLFactory
    {
        /// <summary>
        /// The Encog factory type code.
        /// </summary>
        string FactoryType { get; }

        /// <summary>
        /// The Encog architecture code.
        /// </summary>
        string FactoryArchitecture { get; }
    }
}
