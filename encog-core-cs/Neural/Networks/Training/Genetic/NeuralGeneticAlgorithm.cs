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
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.ML.Genetic;
using Encog.ML.Genetic.Crossover;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Mutate;
using Encog.ML.Genetic.Population;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util.Logging;

namespace Encog.Neural.Networks.Training.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm that allows a feedforward or simple recurrent
    /// neural network to be trained using a genetic algorithm.
    /// There are essentially two ways you can make use of this class.
    /// Either way, you will need a score object. The score object tells the genetic
    /// algorithm how well suited a neural network is.
    /// If you would like to use genetic algorithms with a training set you should
    /// make use TrainingSetScore class. This score object uses a training set to
    /// score your neural network.
    /// If you would like to be more abstract, and not use a training set, you can
    /// create your own implementation of the CalculateScore method. This class can
    /// then score the networks any way that you like.
    /// </summary>
    ///
    public class NeuralGeneticAlgorithm : BasicTraining
    {
        /// <summary>
        /// Simple helper class that implements the required methods to implement a
        /// genetic algorithm.
        /// </summary>
        ///
        private NeuralGeneticAlgorithmHelper _genetic;

        /// <summary>
        /// Construct a neural genetic algorithm.
        /// </summary>
        ///
        /// <param name="network">The network to base this on.</param>
        /// <param name="randomizer">The randomizer used to create this initial population.</param>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="populationSize">The population size.</param>
        /// <param name="mutationPercent">The percent of offspring to mutate.</param>
        /// <param name="percentToMate">The percent of the population allowed to mate.</param>
        public NeuralGeneticAlgorithm(BasicNetwork network,
                                      IRandomizer randomizer, ICalculateScore calculateScore,
                                      int populationSize, double mutationPercent,
                                      double percentToMate) : base(TrainingImplementationType.Iterative)
        {
            _genetic = new NeuralGeneticAlgorithmHelper();
            _genetic.CalculateScore = new GeneticScoreAdapter(calculateScore);
            IPopulation population = new BasicPopulation(populationSize);
            Genetic.MutationPercent = mutationPercent;
            Genetic.MatingPopulation = percentToMate*2;
            Genetic.PercentToMate = percentToMate;
            Genetic.Crossover = new Splice(network.Structure.CalculateSize()/3);
            Genetic.Mutate = new MutatePerturb(4.0d);
            Genetic.Population = population;
            for (int i = 0; i < population.PopulationSize; i++)
            {
                var chromosomeNetwork = (BasicNetwork) (network
                                                           .Clone());
                randomizer.Randomize(chromosomeNetwork);

                var genome = new NeuralGenome(chromosomeNetwork);
                genome.GA = Genetic;
                Genetic.PerformCalculateScore(genome);
                Genetic.Population.Add(genome);
            }
            population.Sort();
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Set the genetic helper class.
        /// </summary>
        public NeuralGeneticAlgorithmHelper Genetic
        {
            get { return _genetic; }
            set { _genetic = value; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return Genetic.Method; }
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            EncogLogging.Log(EncogLogging.LEVEL_INFO,
                             "Performing Genetic iteration.");
            PreIteration();
            Genetic.Iteration();
            Error = Genetic.Error;
            PostIteration();
        }

        /// <inheritdoc/>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override sealed void Resume(TrainingContinuation state)
        {
        }

        #region Nested type: NeuralGeneticAlgorithmHelper

        /// <summary>
        /// Very simple class that implements a genetic algorithm.
        /// </summary>
        ///
        public class NeuralGeneticAlgorithmHelper : BasicGeneticAlgorithm
        {
            /// <value>The error from the last iteration.</value>
            public double Error
            {
                get
                {
                    IGenome genome = Population.Best;
                    return genome.Score;
                }
            }


            /// <summary>
            /// Get the current best neural network.
            /// </summary>
            public IMLMethod Method
            {
                get
                {
                    IGenome genome = Population.Best;
                    return (BasicNetwork) genome.Organism;
                }
            }
        }

        #endregion
    }
}
