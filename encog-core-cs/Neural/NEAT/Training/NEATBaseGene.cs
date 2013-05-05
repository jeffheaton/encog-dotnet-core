using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NEAT.Training
{
    /// <summary>
    /// Defines a base class for NEAT genes. A neat gene holds instructions on how to
    /// create either a neuron or a link. The NEATLinkGene and NEATLinkNeuron classes
    /// extend NEATBaseGene to provide this specific functionality.
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
    public class NEATBaseGene : IComparable<NEATBaseGene>
    {
        /// <summary>
        /// ID of this gene, -1 for unassigned.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Innovation ID, -1 for unassigned.
        /// </summary>
        public long InnovationId { get; set; }

        /// <summary>
        /// Construct the base gene.
        /// </summary>
        public NEATBaseGene()
        {
            Id = -1;
            InnovationId = -1;
        }

        /// <inheritdoc/>
        public int CompareTo(NEATBaseGene o)
        {
            return ((int)(InnovationId - o.InnovationId));
        }
    }
}
