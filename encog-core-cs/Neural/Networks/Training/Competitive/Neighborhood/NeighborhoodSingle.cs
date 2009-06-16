using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    /// <summary>
    /// A very simple neighborhood function that will return 1.0 (full effect)
    /// for the winning neuron, and 0.0 (no change) for everything else.
    /// </summary>
    public class NeighborhoodSingle : INeighborhoodFunction
    {
        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        /// <param name="currentNeuron">The current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public double Function(int currentNeuron,
                 int bestNeuron)
        {
            if (currentNeuron == bestNeuron)
            {
                return 1.0;
            }
            else
            {
                return 0.0;
            }
        }

    }

}
