// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a chromosome that allows a
    /// feedforward neural network to be trained using a genetic algorithm. The
    /// network is trained using training sets.
    /// 
    /// The chromosome for a feed forward neural network is the weight and threshold
    /// matrix.
    /// </summary>
    public class TrainingSetNeuralChromosome : NeuralChromosome
    {
        /// <summary>
        /// The constructor, takes a list of cities to set the initial "genes" to.
        /// </summary>
        /// <param name="genetic">The genetic algorithm used with this chromosome.</param>
        /// <param name="network">The neural network to train.</param>
        public TrainingSetNeuralChromosome(
                 TrainingSetNeuralGeneticAlgorithm genetic,
                 BasicNetwork network)
        {
            this.GeneticAlgorithm = genetic.Genetic;
            this.genetic = genetic;
            this.Network = network;

            InitGenes(network.WeightMatrixSize);
            UpdateGenes();
        }

        /// <summary>
        /// The genetic algorithm being used.
        /// </summary>
        private TrainingSetNeuralGeneticAlgorithm genetic;

        /// <summary>
        /// Calculate the cost for this chromosome.
        /// </summary>
        public override void CalculateCost()
        {
            // update the network with the new gene values
            UpdateNetwork();

            // update the cost with the new genes
            this.Cost = this.Network.CalculateError(this.genetic.Training);

        }

        /// <summary>
        /// Get all genes.
        /// </summary>
        public override double[] Genes
        {
            set
            {
                // copy the new genes
                base.Genes = value;

                CalculateCost();
            }

        }
    }

}
