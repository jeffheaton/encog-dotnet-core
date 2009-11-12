using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Data
{
    /// <summary>
    /// Specifies that a data set can be accessed in random order via an index. This
    /// property is required for MPROP training. 
    /// </summary>
    public interface IIndexable : INeuralDataSet
    {
        /// <summary>
        /// The total number of records in the set.
        /// </summary>
        long Count
        {
            get;
        }

        /// <summary>
        /// Read an individual record, specified by index, in random order.
        /// </summary>
        /// <param name="index">The index to read.</param>
        /// <param name="pair">The pair that the record will be copied into.</param>
        void GetRecord(long index, INeuralDataPair pair);

        /// <summary>
        /// Opens an additional instance of this dataset.
        /// </summary>
        /// <returns>The new instance.</returns>
        IIndexable OpenAdditional();
    }
}
