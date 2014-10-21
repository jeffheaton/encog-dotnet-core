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
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.Util.Concurrency;
using Encog.ML.EA.Train;
using Encog.ML.EA.Population;
using Encog.Neural.Networks.Training;
using Encog.ML.EA.Sort;
using Encog.ML.EA.Species;
using Encog.ML.Genetic.Mutate;
using Encog.ML.Genetic.Crossover;
using Encog.ML.EA.Genome;
using Encog.Util.Logging;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm that allows an MLMethod that is encodable
    /// (MLEncodable) to be trained. It works well with both BasicNetwork and
    /// FreeformNetwork class, as well as any MLEncodable class.
    /// 
    /// There are essentially two ways you can make use of this class.
    /// 
    /// Either way, you will need a score object. The score object tells the genetic
    /// algorithm how well suited a neural network is.
    /// 
    /// If you would like to use genetic algorithms with a training set you should
    /// make use TrainingSetScore class. This score object uses a training set to
    /// score your neural network.
    /// 
    /// If you would like to be more abstract, and not use a training set, you can
    /// create your own implementation of the CalculateScore method. This class can
    /// then score the networks any way that you like.
    /// </summary>
    public class MLMethodGeneticAlgorithm : BasicTraining, IMultiThreadable
    {
        /// <summary>
        /// Very simple class that implements a genetic algorithm.
        /// </summary>
        public class MLMethodGeneticAlgorithmHelper : TrainEA
        {

            /// <summary>
            /// Construct the helper. 
            /// </summary>
            /// <param name="thePopulation">The population.</param>
            /// <param name="theScoreFunction">The score function.</param>
            public MLMethodGeneticAlgorithmHelper(IPopulation thePopulation,
                    ICalculateScore theScoreFunction)
                : base(thePopulation, theScoreFunction)
            {

            }
        }

        /// <summary>
        /// Simple helper class that implements the required methods to implement a
        /// genetic algorithm.
        /// </summary>
        private MLMethodGeneticAlgorithmHelper genetic;

        /// <summary>
        /// Construct a method genetic algorithm. 
        /// </summary>
        /// <param name="phenotypeFactory">The phenotype factory.</param>
        /// <param name="calculateScore">The score calculation object.</param>
        /// <param name="populationSize">The population size.</param>
        public MLMethodGeneticAlgorithm(MLMethodGenomeFactory.CreateMethod phenotypeFactory,
                ICalculateScore calculateScore, int populationSize)
            : base(TrainingImplementationType.Iterative)
        {
            // create the population
            IPopulation population = new BasicPopulation(populationSize, null);
            population.GenomeFactory = new MLMethodGenomeFactory(phenotypeFactory,
                    population);

            ISpecies defaultSpecies = population.CreateSpecies();

            for (int i = 0; i < population.PopulationSize; i++)
            {
                IMLEncodable chromosomeNetwork = (IMLEncodable)phenotypeFactory();
                MLMethodGenome genome = new MLMethodGenome(chromosomeNetwork);
                defaultSpecies.Add(genome);
            }
            defaultSpecies.Leader = defaultSpecies.Members[0];



            // create the trainer
            this.genetic = new MLMethodGeneticAlgorithmHelper(population,
                    calculateScore);
            this.genetic.CODEC = new MLEncodableCODEC();

            IGenomeComparer comp = null;
            if (calculateScore.ShouldMinimize)
            {
                comp = new MinimizeScoreComp();
            }
            else
            {
                comp = new MaximizeScoreComp();
            }
            this.genetic.BestComparer = comp;
            this.genetic.SelectionComparer = comp;

            // create the operators
            int s = Math
                    .Max(defaultSpecies.Members[0].Size / 5, 1);
            Genetic.Population = population;

            this.genetic.AddOperation(0.9, new Splice(s));
            this.genetic.AddOperation(0.1, new MutatePerturb(1.0));
        }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The genetic algorithm implementation.
        /// </summary>
        public MLMethodGeneticAlgorithmHelper Genetic
        {
            get
            {
                return this.genetic;
            }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get
            {
                IGenome best = this.genetic.BestGenome;
                return this.genetic.CODEC.Decode(best);
            }
        }
        /// <inheritdoc/>
        public int ThreadCount
        {
            get
            {
                return this.genetic.ThreadCount;
            }
            set
            {
                this.genetic.ThreadCount = value;
            }
        }

        /// <inheritdoc/>
        public override void Iteration()
        {

            EncogLogging.Log(EncogLogging.LevelInfo,
                    "Performing Genetic iteration.");
            PreIteration();
            Error = Genetic.Error;
            Genetic.Iteration();
            Error = Genetic.Error;
            PostIteration();
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {

        }
    }
}
