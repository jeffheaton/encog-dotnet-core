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
using Encog.ML.Genetic.Crossover;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Mutate;
using Encog.ML.Genetic.Population;
using Encog.ML.Genetic.Species;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm. This is an abstract class. Other classes are
    /// provided by Encog use this base class to train neural networks or provide an
    /// answer to the traveling salesman problem.
    /// The genetic algorithm is also capable of using a thread pool to speed
    /// execution.
    /// </summary>
    ///
    public abstract class GeneticAlgorithm
    {
        /// <summary>
        /// The score calculation object.
        /// </summary>
        ///
        private ICalculateGenomeScore calculateScore;

        /// <summary>
        /// Set the score calculation object.
        /// </summary>
        public ICalculateGenomeScore CalculateScore
        {
            get { return calculateScore; }
            set { calculateScore = value; }
        }


        /// <summary>
        /// Set the comparator.
        /// </summary>
        public GenomeComparator Comparator { get; set; }


        /// <summary>
        /// Set the crossover object.
        /// </summary>
        public ICrossover Crossover { get; set; }


        /// <summary>
        /// Set the mating population percent.
        /// </summary>
        public double MatingPopulation { get; set; }


        /// <summary>
        /// Set the mutate object.
        /// </summary>
        public IMutate Mutate { get; set; }


        /// <summary>
        /// Set the mutation percent.
        /// </summary>
        public double MutationPercent { get; set; }


        /// <summary>
        /// Set the percent to mate.
        /// </summary>
        public double PercentToMate { get; set; }


        /// <summary>
        /// Set the population.
        /// </summary>
        public IPopulation Population { get; set; }

        /// <summary>
        /// Add a genome.
        /// </summary>
        ///
        /// <param name="species">The species to add.</param>
        /// <param name="genome">The genome to add.</param>
        public void AddSpeciesMember(ISpecies species,
                                     IGenome genome)
        {
            if (Comparator.IsBetterThan(genome.Score,
                                        species.BestScore))
            {
                species.BestScore = genome.Score;
                species.GensNoImprovement = 0;
                species.Leader = genome;
            }

            species.Members.Add(genome);
        }

        /// <summary>
        /// Calculate the score for this genome. The genome's score will be set.
        /// </summary>
        ///
        /// <param name="g">The genome to calculate for.</param>
        public void PerformCalculateScore(IGenome g)
        {
            if (g.Organism is MLContext)
            {
                ((MLContext) g.Organism).ClearContext();
            }
            double score = calculateScore.CalculateScore(g);
            g.Score = score;
        }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        public abstract void Iteration();
    }
}
