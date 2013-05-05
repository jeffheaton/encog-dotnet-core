using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.HyperNEAT.Substrate
{
    /// <summary>
    /// A substrate node. A node has a coordinate in an n-dimension space that
    /// matches the dimension count of the substrate.
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
    public class SubstrateLink
    {
        /// <summary>
        /// The source.
        /// </summary>
        private SubstrateNode source;

        /// <summary>
        /// The target.
        /// </summary>
        private SubstrateNode target;

        /// <summary>
        /// Construct the link.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        public SubstrateLink(SubstrateNode source, SubstrateNode target)
        {
            this.source = source;
            this.target = target;
        }

        /// <summary>
        /// The source.
        /// </summary>
        public SubstrateNode Source
        {
            get { return source; }
        }

        /// <summary>
        /// The target.
        /// </summary>
        public SubstrateNode Target
        {
            get
            {
                return target;
            }
        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[SubstrateLink: source=");
            result.Append(this.Source.ToString());
            result.Append(",target=");
            result.Append(this.Target.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}
