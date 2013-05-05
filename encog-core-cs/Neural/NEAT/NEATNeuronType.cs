using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// The types of neurons supported by NEAT.
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
    public enum NEATNeuronType
    {
        /// <summary>
        /// Each NEAT network has one bias neuron.
        /// </summary>
        Bias,

        /// <summary>
        /// Hidden neurons are between the input and output.
        /// </summary>
        Hidden,

        /// <summary>
        /// Input neurons receive input, they are never altered during evolution.
        /// </summary>
        Input,

        /// <summary>
        /// Not really a neuron type, as you will never see one of these in the
        /// network. However, it is used to mark an innovation as not affecting a
        //// neuron type, but rather a link.
        /// </summary>
        None,

        /// <summary>
        /// Output neurons provide output, they are never altered during evolution.
        /// </summary>
        Output
    }
}
