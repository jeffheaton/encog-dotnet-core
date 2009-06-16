using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.MathUtil.RBF;

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    /// <summary>
    /// A neighborhood function based on the Gaussian function.
    /// </summary>
    public class NeighborhoodGaussian : INeighborhoodFunction
    {
        /// <summary>
        /// The radial basis function (RBF) to use to calculate the training falloff
        /// from the best neuron.
        /// </summary>
        private IRadialBasisFunction radial;

        /// <summary>
        /// Construct the neighborhood function with the specified radial function.
        /// Generally this will be a Gaussian function but any RBF should do.
        /// </summary>
        /// <param name="radial">The radial basis function to use.</param>
        public NeighborhoodGaussian(IRadialBasisFunction radial)
        {
            this.radial = radial;
        }

        /// <summary>
        /// Determine how much the current neuron should be affected by training
        /// based on its proximity to the winning neuron.
        /// </summary>
        /// <param name="currentNeuron">The current neuron being evaluated.</param>
        /// <param name="bestNeuron">The winning neuron.</param>
        /// <returns>The ratio for this neuron's adjustment.</returns>
        public double Function(int currentNeuron, int bestNeuron)
        {
            return this.radial.Calculate(currentNeuron - bestNeuron);
        }

    }

}
