using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.NEAT
{
    /// <summary>
    /// Tracks the split depth of NEAT links.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public struct SplitDepth
    {
        /// <summary>
        /// The depth.
        /// </summary>
        public int Depth;

        /// <summary>
        /// The value.
        /// </summary>
        public double Value;

        /// <summary>
        /// Construct a split depth.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="depth">The depth.</param>
        public SplitDepth(double value, int depth)
        {
            this.Value = value;
            this.Depth = depth;
        }
    }
}
