using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training
{
    /// <summary>
    /// The batch size. Specify 1 for pure online training. Specify 0 for pure batch
    /// training (complete training set in one batch). Otherwise specify the batch
    /// size for batch training.
    /// </summary>
    public interface IBatchSize
    {
        /// <summary>
        /// The batch size. Specify 1 for pure online training. Specify 0 for pure
	    /// batch training (complete training set in one batch). Otherwise specify
	    /// the batch size for batch training.
        /// </summary>
        int BatchSize { get; set; }


    }
}
