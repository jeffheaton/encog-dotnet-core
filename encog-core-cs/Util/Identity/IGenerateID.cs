using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.Identity
{
    /// <summary>
    /// Interface for ID generation.
    /// </summary>
    public interface IGenerateID
    {
        /// <summary>
        /// Generate an ID number.
        /// </summary>
        /// <returns>The ID number generated.</returns>
        long Generate();
    }
}
