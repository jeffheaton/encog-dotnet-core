using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Implements a NEAT link gene. This describes a way in which two neurons are
    /// linked.
    ///
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    ///
    /// -----------------------------------------------------------------------------
    /// http://www.cs.ucf.edu/~kstanley/ Encog's NEAT implementation was drawn from
    /// the following three Journal Articles. For more complete BibTeX sources, see
    /// NEATNetwork.java.
    /// 
    /// Evolving Neural Networks Through Augmenting Topologies
    /// 
    /// Generating Large-Scale Neural Networks Through Discovering Geometric
    /// Regularities
    /// 
    /// Automatic feature selection in neuroevolution
    /// </summary>
    [Serializable]
    public class NEATLinkGene : NEATBaseGene
    {
        /// <summary>
        /// The from neuron id.
        /// </summary>
        public long FromNeuronID { get; set; }

        /// <summary>
        /// The to neuron id.
        /// </summary>
        public long ToNeuronID { get; set; }

        /// <summary>
        /// The weight of this link.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Is this gene enabled?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Default constructor, used mainly for persistence.
        /// </summary>
        public NEATLinkGene()
        {

        }

        /// <summary>
        /// Construct a NEAT link gene.
        /// </summary>
        /// <param name="fromNeuronID">The source neuron.</param>
        /// <param name="toNeuronID">The target neuron.</param>
        /// <param name="enabled">Is this link enabled.</param>
        /// <param name="innovationID">The innovation id.</param>
        /// <param name="weight">The weight.</param>
        public NEATLinkGene(long fromNeuronID, long toNeuronID,
                bool enabled, long innovationID,
                double weight)
            : this()
        {
            FromNeuronID = fromNeuronID;
            ToNeuronID = toNeuronID;
            Enabled = enabled;
            InnovationId = innovationID;
            Weight = weight;
        }

        public NEATLinkGene(NEATLinkGene other)
        {
            Copy(other);
        }

        /// <summary>
        /// Copy from another gene.
        /// </summary>
        /// <param name="gene">The other gene.</param>
        public void Copy(NEATLinkGene gene)
        {
            NEATLinkGene other = gene;
            Enabled = other.Enabled;
            FromNeuronID = other.FromNeuronID;
            ToNeuronID = other.ToNeuronID;
            InnovationId = other.InnovationId;
            this.Weight = other.Weight;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[NEATLinkGene:innov=");
            result.Append(InnovationId);
            result.Append(",enabled=");
            result.Append(Enabled);
            result.Append(",from=");
            result.Append(this.FromNeuronID);
            result.Append(",to=");
            result.Append(this.ToNeuronID);
            result.Append("]");
            return result.ToString();
        }

    }
}
