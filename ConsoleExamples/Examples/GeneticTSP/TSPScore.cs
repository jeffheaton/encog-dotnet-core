using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Solve.Genetic.Genome;
using Encog.Examples.Util;

namespace Encog.Examples.GeneticTSP
{
    public class TSPScore : ICalculateGenomeScore
    {
        private City[] cities;

        public TSPScore(City[] cities)
        {
            this.cities = cities;
        }


        public double CalculateScore(IGenome genome)
        {
            double result = 0.0;

            int[] path = (int[])genome.Organism;

            for (int i = 0; i < cities.Length - 1; i++)
            {
                City city1 = cities[path[i]];
                City city2 = cities[path[i + 1]];

                double dist = city1.Proximity(city2);
                result += dist;
            }

            return result;
        }

        public bool ShouldMinimize
        {
            get
            {
                return true;
            }
        }
    }
}
