//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using Encog.Examples.Util;
using Encog.ML.Genetic.Genome;
using Encog.Neural.Networks.Training;
using Encog.ML;

namespace Encog.Examples.GeneticTSP
{
    public class TSPScore : ICalculateScore
    {
        private readonly City[] cities;

        public TSPScore(City[] cities)
        {
            this.cities = cities;
        }

        #region ICalculateGenomeScore Members

        public double CalculateScore(IMLMethod phenotype)
        {
            double result = 0.0;
            IntegerArrayGenome genome = (IntegerArrayGenome)phenotype;
            int[] path = ((IntegerArrayGenome)genome).Data;

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
            get { return true; }
        }

        #endregion

        /// <inheritdoc/>
        public bool RequireSingleThreaded
        {
            get { return false; }
        }
    }
}
