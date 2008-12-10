// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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

using Encog.Neural.Genetic;

namespace Encog.Neural.Networks.Training.Genetic
{
    class TrainingSetNeuralChromosome : NeuralChromosome
    {
        /// <summary>
        /// The genes that make up this chromosome.
        /// </summary>
        public override double[] Genes
        {
            get
            {
                return base.Genes;
            }

            set
            {
                // copy the new genes
                base.Genes = value;

                CalculateCost();
            }
        }


        /// <summary>
        /// The constructor sets up to train the neural network with a GA.
        /// </summary>
        /// <param name="genetic">The genetic algorithm to use.</param>
        /// <param name="network">The neural network to use.</param>
        public TrainingSetNeuralChromosome(
                 TrainingSetNeuralGeneticAlgorithm genetic,
                 BasicNetwork network)
        {
            this.GA =  genetic;
            this.Network = network;

            InitGenes(network.WeightMatrixSize);
            UpdateGenes();
        }

        /// <summary>
        /// Calculate the cost for this chromosome.
        /// </summary>
        override public void CalculateCost()
        {
            // update the network with the new gene values
            this.UpdateNetwork();

            // update the cost with the new genes
            this.Cost  = this.Network.CalculateError(this.getTrainingSetNeuralGeneticAlgorithm().Training);

        }

        /// <summary>
        /// Get the genetic algorithm used for this chromosome.
        /// </summary>
        /// <returns>The genetic algorithm used with this chromosome.</returns>
        public TrainingSetNeuralGeneticAlgorithm getTrainingSetNeuralGeneticAlgorithm()
        {
            return (TrainingSetNeuralGeneticAlgorithm)this.GA;
        }
    }
}
