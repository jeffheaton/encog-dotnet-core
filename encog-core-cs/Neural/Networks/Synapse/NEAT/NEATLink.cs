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
        /// <summary>
        /// The source neuron.
        /// </summary>
        [EGReference]
        private NEATNeuron fromNeuron;

        /// <summary>
        /// Is this link recurrent.
        /// </summary>
        [EGAttribute]
        private bool recurrent;

        /// <summary>
        /// The target neuron.
        /// </summary>
        [EGReference]
        private NEATNeuron toNeuron;

        /// <summary>
        /// The weight between the two neurons.
        /// </summary>
        [EGAttribute]
        private double weight;

        
        /// <summary>
        /// Construct a NEAT link. 
        /// </summary>
        /// <param name="weight">The weight between the two neurons.</param>
        /// <param name="fromNeuron">The source neuron.</param>
        /// <param name="toNeuron">The target neuron.</param>
        /// <param name="recurrent">Is this a recurrent link.</param>
        public NEATLink(double weight, NEATNeuron fromNeuron,
                NEATNeuron toNeuron, bool recurrent)
        {
            this.weight = weight;
            this.fromNeuron = fromNeuron;
            this.toNeuron = toNeuron;
            this.recurrent = recurrent;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NEATLink()
        {
        }

        /// <summary>
        /// The source neuron.
        /// </summary>
        public NEATNeuron FromNeuron
        {
            get
            {
                return fromNeuron;
            }
        }

        /// <summary>
        /// The target neuron.
        /// </summary>
        public NEATNeuron ToNeuron
        {
            get
            {
                return toNeuron;
            }
        }

        /// <summary>
        /// The weight of the link.
        /// </summary>
        public double Weight
        {
            get
            {
                return weight;
            }
        }

        /// <summary>
        /// True if this is a recurrent link.
        /// </summary>
        public bool IsRecurrent
        {
            get
            {
                return recurrent;
            }
        }
    }
}
