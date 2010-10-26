using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Determines the type of flat network needed.
    /// </summary>
    public enum FlatUpdateNeeded
    {
        /// <summary>
        /// No update needed.
        /// </summary>
        None,
        /// <summary>
        /// The object network needs to be copied to the flat network.
        /// </summary>
        Flatten,
        /// <summary>
        /// The flat network needs to be copied to the object network.
        /// </summary>
        Unflatten,
        /// <summary>
        /// This network is not using a flat network.
        /// </summary>
        Never
    }
}
