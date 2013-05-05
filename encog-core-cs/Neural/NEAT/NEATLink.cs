using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// Implements a link between two NEAT neurons.
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
    /// </summary>
    [Serializable]
    public class NEATLink : IComparable<NEATLink>
    {
        /// <summary>
        /// The source neuron.
        /// </summary>
        public int FromNeuron { get; set; }

        /// <summary>
        /// The target neuron.
        /// </summary>
        public int ToNeuron { get; set; }

        /// <summary>
        /// The weight.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Construct a NEAT link. 
        /// </summary>
        /// <param name="theFromNeuron">The from neuron.</param>
        /// <param name="theToNeuron">The to neuron.</param>
        /// <param name="theWeight">The weight.</param>
        public NEATLink(int theFromNeuron, int theToNeuron,
                double theWeight)
        {
            FromNeuron = theFromNeuron;
            ToNeuron = theToNeuron;
            Weight = theWeight;
        }

        /// <inheritdoc/>
        public int CompareTo(NEATLink other)
        {
            int result = FromNeuron - other.FromNeuron;
            if (result != 0)
            {
                return result;
            }

            return ToNeuron - other.ToNeuron;
        }

        /// <inheritdoc/>
        public override bool Equals(Object other)
        {
            if (other == null)
            {
                return false;
            }
            if (other == this)
            {
                return true;
            }
            if (!(other is NEATLink))
            {
                return false;
            }
            NEATLink otherMyClass = (NEATLink)other;
            return CompareTo(otherMyClass) == 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NEATLink: fromNeuron=");
            result.Append(this.FromNeuron);
            result.Append(", toNeuron=");
            result.Append(this.ToNeuron);
            result.Append("]");
            return result.ToString();
        }
    }
}
