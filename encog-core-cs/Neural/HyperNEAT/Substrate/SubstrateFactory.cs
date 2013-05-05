using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.HyperNEAT.Substrate
{
    /// <summary>
    /// Produce substrates for various topologies. Currently provides the sandwich
    /// topology. You can create any topology you wish, this is simply a convienance
    /// method.
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
    public class SubstrateFactory
    {
        /// <summary>
        /// Create a sandwich substrate. A sandwich has an input layer connected
	    /// directly to an output layer, both are square.
        /// </summary>
        /// <param name="inputEdgeSize">The input edge size.</param>
        /// <param name="outputEdgeSize">The output edge size.</param>
        /// <returns>The substrate.</returns>
        public static Substrate factorSandwichSubstrate(int inputEdgeSize,
                int outputEdgeSize)
        {
            Substrate result = new Substrate(3);

            double inputTick = 2.0 / inputEdgeSize;
            double outputTick = 2.0 / inputEdgeSize;
            double inputOrig = -1.0 + (inputTick / 2.0);
            double outputOrig = -1.0 + (inputTick / 2.0);

            // create the input layer

            for (int row = 0; row < inputEdgeSize; row++)
            {
                for (int col = 0; col < inputEdgeSize; col++)
                {
                    SubstrateNode inputNode = result.CreateInputNode();
                    inputNode.Location[0] = -1;
                    inputNode.Location[1] = inputOrig + (row * inputTick);
                    inputNode.Location[2] = inputOrig + (col * inputTick);
                }
            }

            // create the output layer (and connect to input layer)

            for (int orow = 0; orow < outputEdgeSize; orow++)
            {
                for (int ocol = 0; ocol < outputEdgeSize; ocol++)
                {
                    SubstrateNode outputNode = result.CreateOutputNode();
                    outputNode.Location[0] = 1;
                    outputNode.Location[1] = outputOrig + (orow * outputTick);
                    outputNode.Location[2] = outputOrig + (ocol * outputTick);

                    // link this output node to every input node
                    foreach (SubstrateNode inputNode in result.InputNodes)
                    {
                        result.CreateLink(inputNode, outputNode);
                    }
                }
            }

            return result;
        }
    }
}
