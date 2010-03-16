using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Neural.Networks.Synapse.NEAT
{
    /// <summary>
    /// Implements a link between two NEAT neurons.
    ///
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    
    public class NEATLink
    {
        /**
	 * The source neuron.
	 */
	[EGReference]
	private NEATNeuron fromNeuron;

	/**
	 * Is this link recurrent.
	 */
	private bool recurrent;

	/**
	 * The target neuron.
	 */
	[EGReference]
	private NEATNeuron toNeuron;
	
	/**
	 * The weight between the two neurons.
	 */
	private double weight;

	/**
	 * Construct a NEAT link.
	 * @param weight The weight between the two neurons.
	 * @param fromNeuron The source neuron.
	 * @param toNeuron The target neuron.
	 * @param recurrent Is this a recurrent link.
	 */
	public NEATLink(double weight, NEATNeuron fromNeuron,
			NEATNeuron toNeuron, bool recurrent) {
		this.weight = weight;
		this.fromNeuron = fromNeuron;
		this.toNeuron = toNeuron;
		this.recurrent = recurrent;
	}

	/**
	 * @return The source neuron.
	 */
	public NEATNeuron FromNeuron {
        get
        {
		return fromNeuron;
        }
	}

	/**
	 * @return The target neuron.
	 */
	public NEATNeuron ToNeuron {
        get
        {
		return toNeuron;
        }
	}

	/**
	 * @return The weight of the link.
	 */
	public double Weight {
        get
        {
            return weight;
        }
	}

	/**
	 * @return True if this is a recurrent link.
	 */
	public bool IsRecurrent {
        get
        {
            return recurrent;
        }
	}
    }
}
