using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT innovation. This lets NEAT track what changes it has
    /// previously tried with a neural network.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/
    /// Encog's NEAT implementation was drawn from the following three Journal
    /// Articles. For more complete BibTeX sources, see NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    [Serializable]
    public class NEATInnovation
    {
        /// <summary>
        /// The neuron id.
        /// </summary>
        public long NeuronID { get; set; }

        /// <summary>
        /// The innovation id.
        /// </summary>
        public long InnovationID { get; set; }


        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        public NEATInnovation()
        {

        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NeatInnovation:");
            result.Append("id=");
            result.Append(this.InnovationID);
            result.Append(",neuron=");
            result.Append(this.NeuronID);
            result.Append("]");
            return result.ToString();
        }

    }
}
