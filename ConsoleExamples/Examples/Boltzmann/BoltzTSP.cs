// Encog(tm) Artificial Intelligence Framework v2.3: C# Examples
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
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Networks;
using ConsoleExamples.Examples;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Pattern;

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

        public bool IsValidTour(BiPolarNeuralData data)
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

        public double LengthOfTour(BiPolarNeuralData data)
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

        String DisplayTour(BiPolarNeuralData data)
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

        public void CalculateWeights(BasicNetwork network)
        {
            int n1, n2, n3, n4;
            int i, j;
            int predN3, succN3;
            double weight;

            BoltzmannLogic logic = (BoltzmannLogic)network.Logic;

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
                            logic.ThermalSynapse.WeightMatrix[i, j] = weight;
                        }
                    }
                    logic.ThermalLayer.BiasWeights[i] = (-gamma / 2);
                }
            }
        }


        public void Execute(IExampleInterface app)
        {
            this.app = app;
            BoltzmannPattern pattern = new BoltzmannPattern();
            pattern.InputNeurons = NEURON_COUNT;
            BasicNetwork network = pattern.Generate();
            BoltzmannLogic logic = (BoltzmannLogic)network.Logic;

            CreateCities();
            CalculateWeights(network);

            logic.Temperature = 100;
            do
            {
                logic.EstablishEquilibrium();
                app.WriteLine(logic.Temperature + " : " + DisplayTour(logic.CurrentState));
                logic.DecreaseTemperature(0.99);
            } while (!IsValidTour(logic.CurrentState));

            app.WriteLine("Final Length: " + this.LengthOfTour(logic.CurrentState));
        }
    }
}
