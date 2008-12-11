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
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// TrainingSetNeuralGeneticAlgorithm: Implements a genetic algorithm 
    /// that allows a feedforward neural network to be trained using a 
    /// genetic algorithm.  This algorithm is for a feed forward neural 
    /// network.  The neural network is trained using training sets.
    /// </summary>
    public class TrainingSetNeuralGeneticAlgorithm : NeuralGeneticAlgorithm
    {
        /// <summary>
        /// The training set's input array.
        /// </summary>
        public INeuralDataSet Training
        {
            get
            {
                return this.training;
            }
        }

        /// <summary>
        /// The RMS error for the best chromosome.
        /// </summary>
        public new double Error
        {
            get
            {
                BasicNetwork network = this.TrainedNetwork;
                return network.CalculateError(this.training);
            }
        }

        /// <summary>
        /// The training set to use.
        /// </summary>
        protected INeuralDataSet training;

        /// <summary>
        /// Construct a genetic algorithm for a neural network that uses training sets.
        /// </summary>
        /// <param name="network">The neural network.</param>
        /// <param name="reset">Should each neural network be reset to random values.</param>
        /// <param name="training">The training set.</param>
        /// <param name="populationSize">The initial population size.</param>
        /// <param name="mutationPercent">The mutation percentage.</param>
        /// <param name="percentToMate">The percentage of the population allowed to mate.</param>
        public TrainingSetNeuralGeneticAlgorithm(BasicNetwork network,
                 bool reset, INeuralDataSet training, int populationSize,
                 double mutationPercent, double percentToMate)
        {

            this.MutationPercent = mutationPercent;
            this.MatingPopulation = percentToMate * 2;
            this.PopulationSize = populationSize;
            this.PercentToMate = percentToMate;

            this.training = training;

            this.Chromosomes = new TrainingSetNeuralChromosome[this.PopulationSize];
            for (int i = 0; i < this.Chromosomes.Length; i++)
            {
                BasicNetwork chromosomeNetwork = (BasicNetwork)network
                       .Clone();
                if (reset)
                {
                    chromosomeNetwork.Reset();
                }

                TrainingSetNeuralChromosome c = new TrainingSetNeuralChromosome(
                       this, chromosomeNetwork);
                c.UpdateGenes();
                SetChromosome(i, c);
            }
            SortChromosomes();
        }
    }
}
