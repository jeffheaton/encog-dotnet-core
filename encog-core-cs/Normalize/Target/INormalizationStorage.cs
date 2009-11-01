using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Target
{
    /// <summary>
    /// Defines a means by which normalized data can be stored.
    /// </summary>
    public interface INormalizationStorage
    {
        /// <summary>
        /// Open the storage.
        /// </summary>
        void Close();

        /// <summary>
        /// Close the storage.
        /// </summary>
        void Open();

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        void Write(double[] data, int inputCount);
    }
}
