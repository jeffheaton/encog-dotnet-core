// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
using Encog.Solve.Genetic;
using Encog.Util.Randomize;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm that allows a feedforward neural network to be
    /// trained using a genetic algorithm. This algorithm is for a feed forward
    /// neural network.
    /// 
    /// This class is somewhat undefined. If you wish to train the neural network 
    /// using training sets, you should use the TrainingSetNeuralGeneticAlgorithm 
    /// class. If you wish to use a cost function to train the neural network, 
    /// then implement a subclass of this one that properly calculates the cost.
    /// </summary>
    public class NeuralGeneticAlgorithm : BasicTraining
    {
        /// <summary>
        /// Very simple class that implements a genetic algorithm.
        /// </summary>
        public class NeuralGeneticAlgorithmHelper : GeneticAlgorithm<double>
        {
            /// <summary>
            /// The error from the last iteration.
            /// </summary>
            public double Error
            {
                get
                {
                    return this.Chromosomes[0].Score;
                }
            }
        }

        /// <summary>
        /// Simple helper class that implements the required methods to 
        /// implement a genetic algorithm.
        /// </summary>
        private NeuralGeneticAlgorithmHelper genetic;

        /// <summary>
        /// The score calculation object.
        /// </summary>
        private ICalculateScore calculateScore;

        /// <summary>
        /// Construct a neural genetic algorithm. 
        /// </summary>
        /// <param name="network">The network to base this on.</param>
        /// <param name="randomizer">The randomizer used to create this initial population.</param>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="mutationPercent">The percent of offspring to mutate.</param>
        /// <param name="percentToMate">The percent of the population allowed to mate.</param>
        public NeuralGeneticAlgorithm(BasicNetwork network,
                IRandomizer randomizer,
                ICalculateScore calculateScore,
                int populationSize, double mutationPercent,
                double percentToMate)
        {
            this.genetic = new NeuralGeneticAlgorithmHelper();
            this.genetic.ShouldMinimize = calculateScore.ShouldMinimize;
            this.calculateScore = calculateScore;
            this.genetic.MutationPercent = mutationPercent;
            this.genetic.MatingPopulation = percentToMate * 2;
            this.genetic.PopulationSize = populationSize;
            this.genetic.PercentToMate = percentToMate;

            this.genetic.Chromosomes =
                    new NeuralChromosome[this.genetic.PopulationSize];
            for (int i = 0; i < this.genetic.Chromosomes.Length; i++)
            {
                BasicNetwork chromosomeNetwork = (BasicNetwork)network.Clone();
                randomizer.Randomize(chromosomeNetwork);

                NeuralChromosome c =
                    new NeuralChromosome(
                        this, chromosomeNetwork);
                this.genetic.Chromosomes[i] = c;
            }
            this.genetic.SortChromosomes();
            this.genetic.DefineCutLength();
        }

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(NeuralGeneticAlgorithm));
#endif



        /// <summary>
        /// Perform one training iteration.
        /// </summary>
       
        public override void Iteration()
        {
#if logging
            if (logger.IsInfoEnabled)
            {
                logger.Info("Performing Genetic iteration.");
            }
#endif
            PreIteration();
            genetic.Iteration();
            Error = this.genetic.Error;
            PostIteration();
        }


        /// <summary>
        /// The network being trained.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                NeuralChromosome c = (NeuralChromosome)this.genetic.Chromosomes[0];
                c.UpdateNetwork();
                return c.Network;
            }
        }

        /// <summary>
        /// The genetic algorithm.
        /// </summary>
        public NeuralGeneticAlgorithmHelper Genetic
        {
            get
            {
                return this.genetic;
            }
        }

        /// <summary>
        /// The object used to calculate the score.
        /// </summary>
        public ICalculateScore CalculateScore
        {
            get
            {
                return this.calculateScore;
            }
        }
    }
}
