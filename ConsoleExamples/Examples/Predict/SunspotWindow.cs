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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Neural.Networks;
using Encog.ML.Data;
using Encog.Util.Arrayutil;
using Encog.Util;
using Encog.ML.Data.Temporal;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.ML.Data.Basic;

namespace Encog.Examples.Predict
{
    public class SunspotWindow : IExample
    {
        public const int StartingYear = 1700;
        public const int WindowSize = 30;
        public const int TrainStart = WindowSize;
        public const int TrainEnd = 259;
        public const int EvaluateStart = 260;

        public const double MaxError = 0.01;

        public static double[] Sunspots = {
                                              0.0262, 0.0575, 0.0837, 0.1203, 0.1883, 0.3033,
                                              0.1517, 0.1046, 0.0523, 0.0418, 0.0157, 0.0000,
                                              0.0000, 0.0105, 0.0575, 0.1412, 0.2458, 0.3295,
                                              0.3138, 0.2040, 0.1464, 0.1360, 0.1151, 0.0575,
                                              0.1098, 0.2092, 0.4079, 0.6381, 0.5387, 0.3818,
                                              0.2458, 0.1831, 0.0575, 0.0262, 0.0837, 0.1778,
                                              0.3661, 0.4236, 0.5805, 0.5282, 0.3818, 0.2092,
                                              0.1046, 0.0837, 0.0262, 0.0575, 0.1151, 0.2092,
                                              0.3138, 0.4231, 0.4362, 0.2495, 0.2500, 0.1606,
                                              0.0638, 0.0502, 0.0534, 0.1700, 0.2489, 0.2824,
                                              0.3290, 0.4493, 0.3201, 0.2359, 0.1904, 0.1093,
                                              0.0596, 0.1977, 0.3651, 0.5549, 0.5272, 0.4268,
                                              0.3478, 0.1820, 0.1600, 0.0366, 0.1036, 0.4838,
                                              0.8075, 0.6585, 0.4435, 0.3562, 0.2014, 0.1192,
                                              0.0534, 0.1260, 0.4336, 0.6904, 0.6846, 0.6177,
                                              0.4702, 0.3483, 0.3138, 0.2453, 0.2144, 0.1114,
                                              0.0837, 0.0335, 0.0214, 0.0356, 0.0758, 0.1778,
                                              0.2354, 0.2254, 0.2484, 0.2207, 0.1470, 0.0528,
                                              0.0424, 0.0131, 0.0000, 0.0073, 0.0262, 0.0638,
                                              0.0727, 0.1851, 0.2395, 0.2150, 0.1574, 0.1250,
                                              0.0816, 0.0345, 0.0209, 0.0094, 0.0445, 0.0868,
                                              0.1898, 0.2594, 0.3358, 0.3504, 0.3708, 0.2500,
                                              0.1438, 0.0445, 0.0690, 0.2976, 0.6354, 0.7233,
                                              0.5397, 0.4482, 0.3379, 0.1919, 0.1266, 0.0560,
                                              0.0785, 0.2097, 0.3216, 0.5152, 0.6522, 0.5036,
                                              0.3483, 0.3373, 0.2829, 0.2040, 0.1077, 0.0350,
                                              0.0225, 0.1187, 0.2866, 0.4906, 0.5010, 0.4038,
                                              0.3091, 0.2301, 0.2458, 0.1595, 0.0853, 0.0382,
                                              0.1966, 0.3870, 0.7270, 0.5816, 0.5314, 0.3462,
                                              0.2338, 0.0889, 0.0591, 0.0649, 0.0178, 0.0314,
                                              0.1689, 0.2840, 0.3122, 0.3332, 0.3321, 0.2730,
                                              0.1328, 0.0685, 0.0356, 0.0330, 0.0371, 0.1862,
                                              0.3818, 0.4451, 0.4079, 0.3347, 0.2186, 0.1370,
                                              0.1396, 0.0633, 0.0497, 0.0141, 0.0262, 0.1276,
                                              0.2197, 0.3321, 0.2814, 0.3243, 0.2537, 0.2296,
                                              0.0973, 0.0298, 0.0188, 0.0073, 0.0502, 0.2479,
                                              0.2986, 0.5434, 0.4215, 0.3326, 0.1966, 0.1365,
                                              0.0743, 0.0303, 0.0873, 0.2317, 0.3342, 0.3609,
                                              0.4069, 0.3394, 0.1867, 0.1109, 0.0581, 0.0298,
                                              0.0455, 0.1888, 0.4168, 0.5983, 0.5732, 0.4644,
                                              0.3546, 0.2484, 0.1600, 0.0853, 0.0502, 0.1736,
                                              0.4843, 0.7929, 0.7128, 0.7045, 0.4388, 0.3630,
                                              0.1647, 0.0727, 0.0230, 0.1987, 0.7411, 0.9947,
                                              0.9665, 0.8316, 0.5873, 0.2819, 0.1961, 0.1459,
                                              0.0534, 0.0790, 0.2458, 0.4906, 0.5539, 0.5518,
                                              0.5465, 0.3483, 0.3603, 0.1987, 0.1804, 0.0811,
                                              0.0659, 0.1428, 0.4838, 0.8127
                                          };

        public static int EvaluateEnd = Sunspots.Length - 1;

        private double[] _closedLoopSunspots;
        private double[] _normalizedSunspots;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(SunspotWindow),
                    "sunwindow",
                    "Predict sunspots w/Window.",
                    "Use a sliding window to predict sunspots.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            NormalizeSunspots(0.1, 0.9);
            BasicNetwork network = CreateNetwork();
            IMLDataSet training = GenerateTraining();
            Train(network, training);
            Predict(network);
        }

        #endregion

        public void NormalizeSunspots(double lo, double hi)
        {
            var norm = new NormalizeArray { NormalizedHigh = hi, NormalizedLow = lo };

            // create arrays to hold the normalized sunspots
            _normalizedSunspots = norm.Process(Sunspots);
            _closedLoopSunspots = EngineArray.ArrayCopy(_normalizedSunspots);
        }


        public IMLDataSet GenerateTraining()
        {


            var result = new TemporalMLDataSet(WindowSize, 1);

            var desc = new TemporalDataDescription(
                TemporalDataDescription.Type.Raw, true, true);
            result.AddDescription(desc);

            for (int year = TrainStart; year < TrainEnd; year++)
            {
                var point = new TemporalPoint(1) { Sequence = year };
                point.Data[0] = _normalizedSunspots[year];
                result.Points.Add(point);
            }

            result.Generate();

            return result;
        }

        public BasicNetwork CreateNetwork()
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(WindowSize));
            network.AddLayer(new BasicLayer(10));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public void Train(BasicNetwork network, IMLDataSet training)
        {
            ITrain train = new ResilientPropagation(network, training);

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine(@"Epoch #" + epoch + @" Error:" + train.Error);
                epoch++;
            } while (train.Error > MaxError);
        }

        public void Predict(BasicNetwork network)
        {
            Console.WriteLine(@"Year    Actual  Predict Closed Loop Predict");

            for (int year = EvaluateStart; year < EvaluateEnd; year++)
            {
                // calculate based on actual data
                var input = new BasicMLData(WindowSize);
                for (var i = 0; i < input.Count; i++)
                {
                    input[i] = _normalizedSunspots[(year - WindowSize) + i];
                }
                IMLData output = network.Compute(input);
                double prediction = output[0];
                _closedLoopSunspots[year] = prediction;

                // calculate "closed loop", based on predicted data
                for (var i = 0; i < input.Count; i++)
                {
                    input[i] = _closedLoopSunspots[(year - WindowSize) + i];
                }
                output = network.Compute(input);
                double closedLoopPrediction = output[0];

                // display
                Console.WriteLine((StartingYear + year)
                                  + @"  " + Format.FormatDouble(_normalizedSunspots[year], 2)
                                  + @"  " + Format.FormatDouble(prediction, 2)
                                  + @"  " + Format.FormatDouble(closedLoopPrediction, 2));
            }
        }

    }
}
