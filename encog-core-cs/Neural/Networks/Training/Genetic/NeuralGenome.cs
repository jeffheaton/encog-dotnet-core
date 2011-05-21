//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Genome;
using Encog.Neural.Networks.Structure;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genome that allows a feedforward neural network to be trained
    /// using a genetic algorithm. The chromosome for a feed forward neural network
    /// is the weight and bias matrix.
    /// </summary>
    ///
    public class NeuralGenome : BasicGenome
    {
        /// <summary>
        /// The chromosome.
        /// </summary>
        ///
        private readonly Chromosome _networkChromosome;

        /// <summary>
        /// Construct a neural genome.
        /// </summary>
        ///
        /// <param name="network">The network to use.</param>
        public NeuralGenome(BasicNetwork network)
        {
            Organism = network;

            _networkChromosome = new Chromosome();

            // create an array of "double genes"
            int size = network.Structure.CalculateSize();
            for (int i = 0; i < size; i++)
            {
                IGene gene = new DoubleGene();
                _networkChromosome.Genes.Add(gene);
            }

            Chromosomes.Add(_networkChromosome);

            Encode();
        }

        /// <summary>
        /// Decode the genomes into a neural network.
        /// </summary>
        ///
        public override sealed void Decode()
        {
            var net = new double[_networkChromosome.Genes.Count];
            for (int i = 0; i < net.Length; i++)
            {
                var gene = (DoubleGene) _networkChromosome.Genes[i];
                net[i] = gene.Value;
            }
            NetworkCODEC.ArrayToNetwork(net, (BasicNetwork) Organism);
        }

        /// <summary>
        /// Encode the neural network into genes.
        /// </summary>
        ///
        public override sealed void Encode()
        {
            double[] net = NetworkCODEC
                .NetworkToArray((BasicNetwork) Organism);

            for (int i = 0; i < net.Length; i++)
            {
                ((DoubleGene) _networkChromosome.GetGene(i)).Value = net[i];
            }
        }
    }
}
