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
using Encog.Examples.Util;
using ConsoleExamples.Examples;
using Encog.MathUtil;
using Encog.ML.Genetic;
using Encog.ML.Genetic.Genome;
using Encog.ML.Genetic.Population;
using Encog.ML.Genetic.Genes;
using Encog.ML.Genetic.Crossover;
using Encog.ML.Genetic.Mutate;

namespace Encog.Examples.GeneticTSP
{
    public class GeneticSolveTSP : IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(GeneticSolveTSP),
                    "tsp-genetic",
                    "Genetic Algorithm Traveling Salesman",
                    "Use a Genetic Algorithm to provide a solution for the traveling salesman problem (TSP).");
                return info;
            }
        }



        public const int CITIES = 50;
        public const int POPULATION_SIZE = 1000;
        public const double MUTATION_PERCENT = 0.1;
        public const double PERCENT_TO_MATE = 0.24;
        public const double MATING_POPULATION_PERCENT = 0.5;
        public const int CUT_LENGTH = CITIES / 5;
        public const int MAP_SIZE = 256;
        public const int MAX_SAME_SOLUTION = 25;

        private GeneticAlgorithm genetic;
        private City[] cities;

        /**
         * Place the cities in random locations.
         */
        private void initCities()
        {
            cities = new City[CITIES];
            for (int i = 0; i < cities.Length; i++)
            {
                int xPos = (int)(ThreadSafeRandom.NextDouble() * MAP_SIZE);
                int yPos = (int)(ThreadSafeRandom.NextDouble() * MAP_SIZE);

                cities[i] = new City(xPos, yPos);
            }
        }

        private void initPopulation(GeneticAlgorithm ga)
        {
            ICalculateGenomeScore score = new TSPScore(cities);
            ga.CalculateScore = score;
            IPopulation population = new BasicPopulation(POPULATION_SIZE);
            ga.Population = population;

            for (int i = 0; i < POPULATION_SIZE; i++)
            {

                TSPGenome genome = new TSPGenome(ga, cities);
                ga.Population.Genomes.Add(genome);
                ga.PerformCalculateScore(genome);
            }
            population.Sort();
        }


        /**
         * Display the cities in the final path.
         */
        public void displaySolution()
        {

            bool first = true;

            foreach (IGene gene in genetic.Population.Best.Chromosomes[0].Genes)
            {
                if (!first)
                    Console.Write(">");
                Console.Write("" + ((IntegerGene)gene).Value);
                first = false;
            }

            Console.WriteLine(@"");
        }



        /// <summary>
        /// Setup and solve the TSP.
        /// </summary>
        public void Execute(IExampleInterface app)
        {
            this.app = app;

            StringBuilder builder = new StringBuilder();

            initCities();

            genetic = new BasicGeneticAlgorithm();

            initPopulation(genetic);
            genetic.MutationPercent = MUTATION_PERCENT;
            genetic.PercentToMate = PERCENT_TO_MATE;
            genetic.MatingPopulation = MATING_POPULATION_PERCENT;
            genetic.Crossover = new SpliceNoRepeat(CITIES / 3);
            genetic.Mutate = new MutateShuffle();

            int sameSolutionCount = 0;
            int iteration = 1;
            double lastSolution = Double.MaxValue;

            while (sameSolutionCount < MAX_SAME_SOLUTION)
            {
                genetic.Iteration();

                double thisSolution = genetic.Population.Best.Score;

                builder.Length = 0;
                builder.Append("Iteration: ");
                builder.Append(iteration++);
                builder.Append(", Best Path Length = ");
                builder.Append(thisSolution);

                Console.WriteLine(builder.ToString());

                if (Math.Abs(lastSolution - thisSolution) < 1.0)
                {
                    sameSolutionCount++;
                }
                else
                {
                    sameSolutionCount = 0;
                }

                lastSolution = thisSolution;
            }

            Console.WriteLine(@"Good solution found:");
            displaySolution();

        }
    }
}
