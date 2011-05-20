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
using ConsoleExamples.Examples;
using Encog.Examples.Util;

namespace Encog.Examples.AnnealTSP
{
    public class SolveTSP: IExample
    {
        public const double START_TEMP = 10.0;
        public const double STOP_TEMP = 2.0;
        public const int CYCLES = 10;
        public const int CITIES = 50;
        public const int MAP_SIZE = 256;
        public const int MAX_SAME_SOLUTION = 25;
        private IExampleInterface app;


        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(SolveTSP),
                    "tsp-anneal",
                    "Annealing Traveling Salesman",
                    "Use simulated annealing to provide a solution for the traveling salesman problem (TSP).");
                return info;
            }
        }


        /// <summary>
        /// The simulated annealing algorithm to use.
        /// </summary>
        private TSPSimulatedAnnealing anneal;

        /// <summary>
        /// The cities to use.
        /// </summary>
        private City[] cities;

        /// <summary>
        /// Place the cities in random locations.
        /// </summary>
        private void InitCities()
        {
            Random rand = new Random();

            cities = new City[CITIES];
            for (int i = 0; i < cities.Length; i++)
            {
                int xPos = (int)(rand.NextDouble() * MAP_SIZE);
                int yPos = (int)(rand.NextDouble() * MAP_SIZE);

                cities[i] = new City(xPos, yPos);
            }
        }

        /// <summary>
        /// Create an initial path of cities.
        /// </summary>
        private void InitPath()
        {
            Random rand = new Random();
            bool[] taken = new bool[this.cities.Length];
            int[] path = new int[this.cities.Length];

            for (int i = 0; i < path.Length; i++)
            {
                taken[i] = false;
            }
            for (int i = 0; i < path.Length - 1; i++)
            {
                int icandidate;
                do
                {
                    icandidate = (int)(rand.NextDouble() * path.Length);
                } while (taken[icandidate]);
                path[i] = icandidate;
                taken[icandidate] = true;
                if (i == path.Length - 2)
                {
                    icandidate = 0;
                    while (taken[icandidate])
                    {
                        icandidate++;
                    }
                    path[i + 1] = icandidate;
                }
            }

            this.anneal.PutArray(path);
        }

        /// <summary>
        /// Display the cities in the final path.
        /// </summary>
        public void DisplaySolution()
        {
            int[] path = anneal.Array;
            for (int i = 0; i < path.Length; i++)
            {
                if (i != 0)
                {
                    app.Write(">");
                }
                app.Write("" + path[i]);
            }
            app.WriteLine("");
        }

        /// <summary>
        /// Setup and solve the TSP.
        /// </summary>
        public void Solve()
        {
            StringBuilder builder = new StringBuilder();

            InitCities();

            anneal = new TSPSimulatedAnnealing(cities, START_TEMP, STOP_TEMP,
                    CYCLES);

            InitPath();

            int sameSolutionCount = 0;
            int iteration = 1;
            double lastSolution = Double.MaxValue;

            while (sameSolutionCount < MAX_SAME_SOLUTION)
            {
                anneal.Iteration();

                double thisSolution = anneal.Score;

                builder.Length = 0;
                builder.Append("Iteration: ");
                builder.Append(iteration++);
                builder.Append(", Best Path Length = ");
                builder.Append(thisSolution);

                app.WriteLine(builder.ToString());

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

            app.WriteLine("Good solution found:");
            DisplaySolution();

        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            this.app = app;
            Solve();
        }
    }
}
