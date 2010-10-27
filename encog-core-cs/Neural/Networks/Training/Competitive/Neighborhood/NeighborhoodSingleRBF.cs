using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.RBF;

namespace Encog.Neural.Networks.Training.Competitive.Neighborhood
{
    public class NeighborhoodSingleRBF: INeighborhoodFunction
    {
        	/**
	 * The radial basis function (RBF) to use to calculate the training falloff
	 * from the best neuron.
	 */
	private IRadialBasisFunction radial;

    public double Radius { get; set; }

	/**
	 * Construct the neighborhood function with the specified radial function.
	 * Generally this will be a Gaussian function but any RBF should do.
	 * 
	 * @param radial
	 *            The radial basis function to use.
	 */
	public NeighborhoodSingleRBF(IRadialBasisFunction radial) {
		this.radial = radial;
	}

	/**
	 * Determine how much the current neuron should be affected by training
	 * based on its proximity to the winning neuron.
	 * 
	 * @param currentNeuron
	 *            THe current neuron being evaluated.
	 * @param bestNeuron
	 *            The winning neuron.
	 * @return The ratio for this neuron's adjustment.
	 */
	public double Function( int currentNeuron,  int bestNeuron) {
		double[] d = new double[1];
		d[0] = currentNeuron - bestNeuron;
		return this.radial.Calculate(d);
	}

    }
}
