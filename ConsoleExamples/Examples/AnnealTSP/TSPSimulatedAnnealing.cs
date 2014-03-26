//
// Encog(tm) Core v3.2 - .Net Version
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
using Encog.Examples.Util;
using Encog.ML.Anneal;

namespace Encog.Examples.AnnealTSP
{
    /// <summary>
    /// A subclass of the simulated annealing algorithm that is designed to work with the 
    /// traveling salesman problem.
    /// </summary>
    internal class TSPSimulatedAnnealing : SimulatedAnnealing<int>
    {
        /// <summary>
        /// The cities to use.
        /// </summary>
        protected City[] cities;

        /// <summary>
        /// The current path through the cities.
        /// </summary>
        protected int[] path;

        /// <summary>
        /// Construct the traveling salesman simulated annealing class.
        /// </summary>
        /// <param name="cities">The cities to use.</param>
        /// <param name="startTemp">The starting temperature per iteration.</param>
        /// <param name="stopTemp">The stopping temperature per iteration.</param>
        /// <param name="cycles">The number of cycles per iteration.</param>
        public TSPSimulatedAnnealing(City[] cities, double startTemp,
                                     double stopTemp, int cycles)
        {
            Temperature = startTemp;
            StartTemperature = startTemp;
            StopTemperature = stopTemp;
            Cycles = cycles;

            this.cities = cities;
            path = new int[this.cities.Length];
        }

        /// <summary>
        /// Get the best solution as an array.  In the case of the TSP, this
        /// is the best path.
        /// </summary>
        /// <returns>The best path through the cities.</returns>
        public override int[] Array
        {
            get { return path; }
        }

        /// <summary>
        /// Create a new copy of the best path.
        /// </summary>
        /// <returns>A copy of the best path through the cities.</returns>
        public override int[] ArrayCopy
        {
            get
            {
                var result = new int[path.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = path[i];
                }

                return result;
            }
        }

        /// <summary>
        /// Determine the current error, which is the length of the best path
        /// through the cities.  We want to make this as low as we can.
        /// </summary>
        /// <returns>The length of the best path through the cities.</returns>
        public override double PerformCalculateScore()
        {
            double cost = 0.0;
            for (int i = 0; i < cities.Length - 1; i++)
            {
                double dist = cities[path[i]]
                    .Proximity(cities[path[i + 1]]);
                cost += dist;
            }
            return cost;
        }


        /// <summary>
        /// Called to get the distance between two cities.
        /// </summary>
        /// <param name="i">The first city.</param>
        /// <param name="j">The second city.</param>
        /// <returns>The distance between the two cities.</returns>
        public double Distance(int i, int j)
        {
            int c1 = path[i%path.Length];
            int c2 = path[j%path.Length];
            return cities[c1].Proximity(cities[c2]);
        }

        /// <summary>
        /// Save an array to the best solution.  
        /// </summary>
        /// <param name="array">A path through the cities.</param>
        public override void PutArray(int[] array)
        {
            path = array;
        }

        /// <summary>
        /// Randomize the cities according to the simulated annealing algorithm.
        /// </summary>
        public override void Randomize()
        {
            int length = path.Length;

            var rand = new Random();

            // make adjustments to city order(annealing)
            for (int i = 0; i < Temperature; i++)
            {
                var index1 = (int) Math.Floor(length*rand.NextDouble());
                var index2 = (int) Math.Floor(length*rand.NextDouble());
                double d = Distance(index1, index1 + 1) + Distance(index2, index2 + 1)
                           - Distance(index1, index2) - Distance(index1 + 1, index2 + 1);
                if (d > 0)
                {
                    // sort index1 and index2 if needed
                    if (index2 < index1)
                    {
                        int temp = index1;
                        index1 = index2;
                        index2 = temp;
                    }
                    for (; index2 > index1; index2--)
                    {
                        int temp = path[index1 + 1];
                        path[index1 + 1] = path[index2];
                        path[index2] = temp;
                        index1++;
                    }
                }
            }
        }
    }
}
