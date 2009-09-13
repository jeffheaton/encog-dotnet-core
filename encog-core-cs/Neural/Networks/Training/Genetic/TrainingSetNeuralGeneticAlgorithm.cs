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
using Encog.Util.Randomize;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Networks.Training.Genetic
{

    /// <summary>
    /// Implements a genetic algorithm that allows a neural network to be trained
    /// using a genetic algorithm. This algorithm is for a neural network. The neural
    /// network is trained using training sets.
    /// </summary>
    public class TrainingSetNeuralGeneticAlgorithm : NeuralGeneticAlgorithm
    {
        /// <summary>
        /// Construct a training object.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="randomizer">The randomizer to use to create new networks.</param>
        /// <param name="training">The training set.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="mutationPercent">The mutation percent.</param>
        /// <param name="percentToMate">The percent to mate.</param>
        public TrainingSetNeuralGeneticAlgorithm(BasicNetwork network,
                 IRandomizer randomizer, INeuralDataSet training,
                 int populationSize, double mutationPercent,
                 double percentToMate)
            : base()
        {

            this.Genetic.MutationPercent = mutationPercent;
            this.Genetic.MatingPopulation = percentToMate * 2;
            this.Genetic.PopulationSize = populationSize;
            this.Genetic.PercentToMate = percentToMate;
            
            this.Training = training;

            this.Genetic.Chromosomes =
                    new TrainingSetNeuralChromosome[this.Genetic.PopulationSize];
            for (int i = 0; i < this.Genetic.Chromosomes.Length; i++)
            {
                BasicNetwork chromosomeNetwork = (BasicNetwork)network
                       .Clone();
                randomizer.Randomize(chromosomeNetwork);

                TrainingSetNeuralChromosome c =
                   new TrainingSetNeuralChromosome(
                       this, chromosomeNetwork);
                this.Genetic.Chromosomes[i] = c;
            }
            this.Genetic.SortChromosomes();
        }
    }
}
