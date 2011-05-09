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
using Encog.Solve.Genetic.Genome;
using Encog.Solve.Genetic.Crossover;
using Encog.Solve.Genetic.Mutate;
using Encog.Solve.Genetic.Population;
using Encog.MathUtil;
using Encog.Neural.Networks;
using Encog.Solve.Genetic.Species;
using Encog.Util.Concurrency;

namespace Encog.Solve.Genetic
{
    /// <summary>
    /// Implements a genetic algorithm. This is an abstract class. Other classes are
    /// provided by Encog use this base class to train neural networks or
    /// provide an answer to the traveling salesman problem.
    ///
    /// The genetic algorithm is also capable of using a thread pool to speed
    /// execution.
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// Threadpool timeout.
        /// </summary>
        public const int TIMEOUT = 120;

        /// <summary>
        /// The score calculation object.
        /// </summary>
        public ICalculateGenomeScore CalculateScore { get; set; }

        /// <summary>
        /// Compare two genomes.
        /// </summary>
        public GenomeComparator Comparator { get; set; }

        /// <summary>
        /// Perform crossovers.
        /// </summary>
        public ICrossover Crossover { get; set; }

        /// <summary>
        /// The mating population.
        /// </summary>
        public double MatingPopulation { get; set; }

        /// <summary>
        /// Used to mutate offspring.
        /// </summary>
        private IMutate mutate;

        /// <summary>
        /// The percent that should mutate.
        /// </summary>
        public double MutationPercent { get; set; }

        /// <summary>
        /// What percent should be chosen to mate. They will choose partners from the
        /// entire mating population.
        /// </summary>
        public double PercentToMate { get; set; }

        /// <summary>
        /// The population.
        /// </summary>
        public IPopulation Population { get; set; }

        /// <summary>
        /// Perform a score calculation for the specified genome.
        /// </summary>
        /// <param name="g">The genome to calculate for.</param>
        public void PerformScoreCalculation(IGenome g)
        {
            if (g.Organism is IContextClearable)
            {
                ((IContextClearable)g.Organism).ClearContext();
            }
            double score = CalculateScore.CalculateScore(g);
            g.Score = score;
        }

        /// <summary>
        /// The mutation object.
        /// </summary>
        public IMutate Mutate
        {
            get
            {
                return mutate;
            }
            set
            {
                this.mutate = value;
            }
        }



        /// <summary>
        /// Perform one generation.
        /// </summary>
        public virtual void Iteration()
        {

            int countToMate = (int)(Population.Genomes.Count * PercentToMate);
            int offspringCount = countToMate * 2;
            int offspringIndex = Population.Genomes.Count - offspringCount;
            int matingPopulationSize = (int)(Population.Genomes.Count * MatingPopulation);

            TaskGroup group = EngineConcurrency.Instance
                    .CreateTaskGroup();

            // mate and form the next generation
            for (int i = 0; i < countToMate; i++)
            {
                IGenome mother = Population.Genomes[i];
                int fatherInt = (int)(ThreadSafeRandom.NextDouble() * matingPopulationSize);
                IGenome father = Population.Genomes[fatherInt];
                IGenome child1 = Population.Genomes[offspringIndex];
                IGenome child2 = Population.Genomes[
                        offspringIndex + 1];

                MateWorker worker = new MateWorker(mother, father, child1,
                        child2);

                EngineConcurrency.Instance.ProcessTask(worker, group);

                offspringIndex += 2;
            }

            group.WaitForComplete();

            // sort the next generation
            Population.Sort();
        }

        /// <summary>
        /// Add a genome to a species.
        /// </summary>
        /// <param name="species">Tge soecues to add to.</param>
        /// <param name="genome">The genome to add.</param>
        public void AddSpeciesMember(ISpecies species, IGenome genome)
        {

            if (this.Comparator.IsBetterThan(genome.Score, species.BestScore))
            {
                species.BestScore = genome.Score;
                species.GensNoImprovement = 0;
                species.Leader = genome;
            }

            species.Members.Add(genome);

        }
    }
}
