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
using Encog.ML.Data.Specific;
using Encog.Neural.Networks;
using ConsoleExamples.Examples;
using Encog.Neural.Thermal;
using Encog.Neural.Pattern;

namespace Encog.Examples.Boltzmann
{
    public class BoltzTSP:IExample
    {

        public const int NUM_CITIES = 10;
        public const int NEURON_COUNT = NUM_CITIES * NUM_CITIES;

        private double gamma = 7;
        private double[][] distance;

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(BoltzTSP),
                    "tsp-boltzmann",
                    "Boltzmann Machine for the Traveling Salesman (TSP)",
                    "Use a Boltzmann machine to provide a solution for the Traveling Salesman Problem (TSP).");
                return info;
            }
        }

        public double Sqr(double x)
        {
            return x * x;
        }

        public void CreateCities()
        {
            double x1, x2, y1, y2;
            double alpha1, alpha2;

            this.distance = new double[NUM_CITIES][];

            for (int n1 = 0; n1 < NUM_CITIES; n1++)
            {
                this.distance[n1] = new double[NUM_CITIES];
                for (int n2 = 0; n2 < NUM_CITIES; n2++)
                {
                    alpha1 = ((double)n1 / NUM_CITIES) * 2 * Math.PI;
                    alpha2 = ((double)n2 / NUM_CITIES) * 2 * Math.PI;
                    x1 = Math.Cos(alpha1);
                    y1 = Math.Sin(alpha1);
                    x2 = Math.Cos(alpha2);
                    y2 = Math.Sin(alpha2);
                    distance[n1][n2] = Math.Sqrt(Sqr(x1 - x2) + Sqr(y1 - y2));
                }
            }
        }

        public bool IsValidTour(BiPolarMLData data)
        {
            int cities, stops;

            for (int n1 = 0; n1 < NUM_CITIES; n1++)
            {
                cities = 0;
                stops = 0;
                for (int n2 = 0; n2 < NUM_CITIES; n2++)
                {
                    if (data.GetBoolean(n1 * NUM_CITIES + n2))
                    {
                        if (++cities > 1)
                            return false;
                    }
                    if (data.GetBoolean(n2 * NUM_CITIES + n1))
                    {
                        if (++stops > 1)
                            return false;
                    }
                }
                if ((cities != 1) || (stops != 1))
                    return false;
            }
            return true;
        }

        public double LengthOfTour(BiPolarMLData data)
        {
            double result;
            int n1, n2, n3;

            result = 0;
            for (n1 = 0; n1 < NUM_CITIES; n1++)
            {
                for (n2 = 0; n2 < NUM_CITIES; n2++)
                {
                    if (data.GetBoolean(((n1) % NUM_CITIES) * NUM_CITIES + n2))
                        break;
                }
                for (n3 = 0; n3 < NUM_CITIES; n3++)
                {
                    if (data.GetBoolean(((n1 + 1) % NUM_CITIES) * NUM_CITIES + n3))
                        break;
                }
                result += distance[n2][n3];
            }
            return result;
        }

        String DisplayTour(BiPolarMLData data)
        {
            StringBuilder result = new StringBuilder();

            int n1, n2;
            bool first;

            for (n1 = 0; n1 < NUM_CITIES; n1++)
            {
                first = true;
                result.Append("[");
                for (n2 = 0; n2 < NUM_CITIES; n2++)
                {
                    if (data.GetBoolean(n1 * NUM_CITIES + n2))
                    {
                        if (first)
                        {
                            first = false;
                            result.Append(n2);
                        }
                        else
                        {
                            result.Append(", " + n2);
                        }
                    }
                }
                result.Append("]");
                if (n1 != NUM_CITIES - 1)
                {
                    result.Append(" -> ");
                }
            }
            return result.ToString();
        }

        public void CalculateWeights(BoltzmannMachine network)
        {
            int n1, n2, n3, n4;
            int i, j;
            int predN3, succN3;
            double weight;

            for (n1 = 0; n1 < NUM_CITIES; n1++)
            {
                for (n2 = 0; n2 < NUM_CITIES; n2++)
                {
                    i = n1 * NUM_CITIES + n2;
                    for (n3 = 0; n3 < NUM_CITIES; n3++)
                    {
                        for (n4 = 0; n4 < NUM_CITIES; n4++)
                        {
                            j = n3 * NUM_CITIES + n4;
                            weight = 0;
                            if (i != j)
                            {
                                predN3 = (n3 == 0 ? NUM_CITIES - 1 : n3 - 1);
                                succN3 = (n3 == NUM_CITIES - 1 ? 0 : n3 + 1);
                                if ((n1 == n3) || (n2 == n4))
                                    weight = -gamma;
                                else if ((n1 == predN3) || (n1 == succN3))
                                    weight = -distance[n2][n4];
                            }
                            network.SetWeight(i,j, weight);
                        }
                    }
                    network.Threshold[i] = (-gamma / 2);
                }
            }
        }


        public void Execute(IExampleInterface app)
        {
            this.app = app;
            BoltzmannPattern pattern = new BoltzmannPattern();
            pattern.InputNeurons = NEURON_COUNT;
            BoltzmannMachine network = (BoltzmannMachine)pattern.Generate();

            CreateCities();
            CalculateWeights(network);

            network.Temperature = 100;
            do
            {
                network.EstablishEquilibrium();
                app.WriteLine(network.Temperature + " : " + DisplayTour(network.CurrentState));
                network.DecreaseTemperature(0.99);
            } while (!IsValidTour(network.CurrentState));

            app.WriteLine("Final Length: " + this.LengthOfTour(network.CurrentState));
        }
    }
}
