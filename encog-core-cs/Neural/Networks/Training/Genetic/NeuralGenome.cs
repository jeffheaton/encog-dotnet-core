// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.MathUtil.Randomize;
using Encog.Solve.Genetic.Genes;
using Encog.Neural.Networks.Structure;
using Encog.Solve.Genetic.Genome;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genome that allows a feedforward neural
    /// network to be trained using a genetic algorithm. The chromosome for a feed
    /// forward neural network is the weight and threshold matrix.
    /// </summary>
    public class NeuralGenome : BasicGenome
    {
        /// <summary>
        /// The network chromosome.
        /// </summary>
        private Chromosome networkChromosome;

        /// <summary>
        /// Construct a neural network genome.
        /// </summary>
        /// <param name="nga">The neural genetic algorithm.</param>
        /// <param name="network">The network.</param>
        public NeuralGenome(NeuralGeneticAlgorithm nga, BasicNetwork network)
            : base(nga.Helper)
        {
            this.Organism = network;
            this.networkChromosome = new Chromosome();

            // create an array of "double genes"
            int size = network.Structure.CalculateSize();
            for (int i = 0; i < size; i++)
            {
                IGene gene = new DoubleGene();
                this.networkChromosome.Genes.Add(gene);
            }

            this.Chromosomes.Add(this.networkChromosome);

            Encode();
        }

        /// <summary>
        /// Decode the genome to a network.
        /// </summary>
        public override void Decode()
        {
            double[] net = new double[networkChromosome.Genes.Count];
            for (int i = 0; i < net.Length; i++)
            {
                DoubleGene gene = (DoubleGene)networkChromosome.Genes[i];
                net[i] = gene.Value;

            }
            NetworkCODEC.ArrayToNetwork(net, (BasicNetwork)Organism);

        }

        /// <summary>
        /// Encode the network to a genome.
        /// </summary>
        public override void Encode()
        {
            double[] net = NetworkCODEC.NetworkToArray((BasicNetwork)Organism);

            for (int i = 0; i < net.Length; i++)
            {
                ((DoubleGene)networkChromosome.Genes[i]).Value = net[i];
            }
        }
    }
}
