using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Strategy.End
{
    /// <summary>
    /// This is a training strategy that can be used to end training.
    /// </summary>
    public interface IEndTrainingStrategy: IStrategy
    {
        /// <summary>
        /// Determine if training should stop.
        /// </summary>
        /// <returns>True if training should stop.</returns>
        bool ShouldStop();
    }
}
