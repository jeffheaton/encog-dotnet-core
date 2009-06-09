using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data
{
    /// <summary>
    /// Neural data, basically an array of values.
    /// </summary>
    public interface INeuralData: ICloneable
    {
        /// <summary>
        /// Get or set the specified index.
        /// </summary>
        /// <param name="x">The index to access.</param>
        /// <returns></returns>
        double this[int x]
        {
            get;
            set;
        }

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        double[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// How many elements in this data structure.
        /// </summary>
        int Count
        {
            get;
        }
    }
}
