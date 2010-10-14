using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Strategy
{
    public interface IEndTrainingStrategy: IStrategy
    {
        /// <summary>
        /// Determine if training should stop.
        /// </summary>
        /// <returns>True if training should stop.</returns>
        bool ShouldStop();
    }
}
