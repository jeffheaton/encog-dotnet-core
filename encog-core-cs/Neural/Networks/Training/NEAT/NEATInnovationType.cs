using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.NEAT
{
    /// <summary>
    /// The type of NEAT innovation.
    /// 
    /// NeuroEvolution of Augmenting Topologies (NEAT) is a genetic algorithm for the
    /// generation of evolving artificial neural networks. It was developed by Ken
    /// Stanley while at The University of Texas at Austin.
    /// 
    /// http://www.cs.ucf.edu/~kstanley/
    /// </summary>
    public enum NEATInnovationType
    {
        /// <summary>
        /// A new link.
        /// </summary>
        NewLink,
        /// <summary>
        /// A new neuron.
        /// </summary>
        NewNeuron
    }
}
