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
using Encog.Solve.Anneal;
using Encog.Examples.Util;

namespace Encog.Examples.AnnealTSP
{
/// <summary>
    /// A subclass of the simulated annealing algorithm that is designed to work with the 
    /// traveling salesman problem.
    /// </summary>
    class TSPSimulatedAnnealing : SimulatedAnnealing<int>
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

            this.Temperature = startTemp;
            this.StartTemperature = startTemp;
            this.StopTemperature = stopTemp;
            this.Cycles = cycles;

            this.cities = cities;
            this.path = new int[this.cities.Length];
        }

        /// <summary>
        /// Determine the current error, which is the length of the best path
        /// through the cities.  We want to make this as low as we can.
        /// </summary>
        /// <returns>The length of the best path through the cities.</returns>
        public override double PerformScoreCalculation()
        {
            double cost = 0.0;
            for (int i = 0; i < this.cities.Length - 1; i++)
            {
                double dist = this.cities[this.path[i]]
                       .Proximity(this.cities[this.path[i + 1]]);
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
            int c1 = this.path[i % this.path.Length];
            int c2 = this.path[j % this.path.Length];
            return this.cities[c1].Proximity(this.cities[c2]);
        }

        /// <summary>
        /// Get the best solution as an array.  In the case of the TSP, this
        /// is the best path.
        /// </summary>
        /// <returns>The best path through the cities.</returns>
        public override int[] GetArray()
        {
            return this.path;
        }

        /// <summary>
        /// Save an array to the best solution.  
        /// </summary>
        /// <param name="array">A path through the cities.</param>
        public override void PutArray(int[] array)
        {
            this.path = array;
        }

        /// <summary>
        /// Randomize the cities according to the simulated annealing algorithm.
        /// </summary>
        public override void Randomize()
        {

            int length = this.path.Length;

            Random rand = new Random();

            // make adjustments to city order(annealing)
            for (int i = 0; i < this.Temperature; i++)
            {
                int index1 = (int)Math.Floor(length * rand.NextDouble());
                int index2 = (int)Math.Floor(length * rand.NextDouble());
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
                        int temp = this.path[index1 + 1];
                        this.path[index1 + 1] = this.path[index2];
                        this.path[index2] = temp;
                        index1++;
                    }
                }
            }

        }

        /// <summary>
        /// Create a new copy of the best path.
        /// </summary>
        /// <returns>A copy of the best path through the cities.</returns>
        public override int[] GetArrayCopy()
        {
            int[] result = new int[this.path.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = this.path[i];
            }

            return result;
        }
    }
}
