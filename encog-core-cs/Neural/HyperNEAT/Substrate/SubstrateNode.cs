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
    public class SubstrateNode
    {
        /// <summary>
        /// The ID of this node.
        /// </summary>
        private int id;

        /// <summary>
        /// The location of this node.
        /// </summary>
        private double[] location;

        /// <summary>
        /// Construct this node.
        /// </summary>
        /// <param name="theID">The ID.</param>
        /// <param name="size">The size.</param>
        public SubstrateNode(int theID, int size)
        {
            this.id = theID;
            this.location = new double[size];
        }

        /// <summary>
        /// The id.
        /// </summary>
        public int ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// The location.
        /// </summary>
        public double[] Location
        {
            get
            {
                return location;
            }
        }

        /// <summary>
        /// The number of dimensions in this node.
        /// </summary>
        public int Count
        {
            get
            {
                return this.location.Length;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[SubstrateNode: id=");
            result.Append(this.id);
            result.Append(", pos=");
            result.Append(location.ToString());
            result.Append("]");
            return result.ToString();
        }
    }
}
