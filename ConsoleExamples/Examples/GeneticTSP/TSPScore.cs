//
// Encog(tm) Console Examples v3.0 - .Net Version
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Examples.Util;
using Encog.ML.Genetic.Genome;

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
