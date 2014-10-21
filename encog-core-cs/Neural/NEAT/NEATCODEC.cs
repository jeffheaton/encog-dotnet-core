//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using Encog.ML.EA.Codec;
using Encog.ML;
using Encog.ML.EA.Genome;
using Encog.Neural.NEAT.Training;
using Encog.Engine.Network.Activation;
using Encog.ML.Genetic;

namespace Encog.Neural.NEAT
{
    /// <summary>
    /// This CODEC is used to create phenomes (NEATNetwork) objects using a genome
    /// (NEATGenome). Conversion is only one direction. You are allowed to transform
    /// a NEAT genome into a NEATNetwork, but you cannot transform a NEAT phenome
    /// back into a genome. The main reason is I have not found a great deal of need
    /// to go the other direction. If someone ever does find a need and creates a
    /// encode method, please consider contributing it to the Encog project. :)
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
    public class NEATCODEC : IGeneticCODEC
    {
        /// <inheritdoc/>
        public IMLMethod Decode(IGenome genome)
        {
            var neatGenome = (NEATGenome)genome;
            var pop = (NEATPopulation)neatGenome.Population;
            IList<NEATNeuronGene> neuronsChromosome = neatGenome.NeuronsChromosome;
            IList<NEATLinkGene> linksChromosome = neatGenome.LinksChromosome;

            if (neuronsChromosome[0].NeuronType != NEATNeuronType.Bias)
            {
                throw new NeuralNetworkError(
                        "The first neuron must be the bias neuron, this genome is invalid.");
            }

            var links = new List<NEATLink>();
            var afs = new IActivationFunction[neuronsChromosome.Count];

            for (int i = 0; i < afs.Length; i++)
            {
                afs[i] = neuronsChromosome[i].ActivationFunction;
            }

            IDictionary<long, int> lookup = new Dictionary<long, int>();
            for (int i = 0; i < neuronsChromosome.Count; i++)
            {
                NEATNeuronGene neuronGene = neuronsChromosome[i];
                lookup[neuronGene.Id] = i;
            }

            // loop over connections
            for (int i = 0; i < linksChromosome.Count; i++)
            {
                NEATLinkGene linkGene = linksChromosome[i];
                if (linkGene.Enabled)
                {
                    links.Add(new NEATLink(lookup[linkGene.FromNeuronId],
                            lookup[linkGene.ToNeuronId], linkGene.Weight));
                }

            }

            links.Sort();

            NEATNetwork network = new NEATNetwork(neatGenome.InputCount,
                    neatGenome.OutputCount, links, afs);

            network.ActivationCycles = pop.ActivationCycles;
            return network;
        }

        /// <summary>
        /// This method is not currently implemented. If you have need of it, and do
        /// implement a conversion from a NEAT phenotype to a genome, consider
        /// contribution to the Encog project.
        /// </summary>
        /// <param name="phenotype">Not used.</param>
        /// <returns>Not used.</returns>
        public IGenome Encode(IMLMethod phenotype)
        {
            throw new GeneticError(
                    "Encoding of a NEAT network is not supported.");
        }
    }
}
