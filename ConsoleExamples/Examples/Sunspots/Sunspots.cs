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
using ConsoleExamples.Examples;
using Encog.Neural.Networks;
using Encog.Normalize;
using Encog.Neural.NeuralData;
using Encog.Neural.NeuralData.Temporal;
using Encog.Neural.Networks.Training;
using Encog.Normalize.Input;
using Encog.Normalize.Output;
using Encog.Normalize.Target;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Util;

namespace Encog.Examples.Sunspots
{
    public class Sunspots : IExample
    {
        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(Sunspots),
                    "sunspots",
                    "Predict Sunspot Activity",
                    "Predict sunspot activity using a Feedforward neural network.");
                return info;
            }
        }

        public double[] SUNSPOTS = {
            0.0262,  0.0575,  0.0837,  0.1203,  0.1883,  0.3033,  
            0.1517,  0.1046,  0.0523,  0.0418,  0.0157,  0.0000,  
            0.0000,  0.0105,  0.0575,  0.1412,  0.2458,  0.3295,  
            0.3138,  0.2040,  0.1464,  0.1360,  0.1151,  0.0575,  
            0.1098,  0.2092,  0.4079,  0.6381,  0.5387,  0.3818,  
            0.2458,  0.1831,  0.0575,  0.0262,  0.0837,  0.1778,  
            0.3661,  0.4236,  0.5805,  0.5282,  0.3818,  0.2092,  
            0.1046,  0.0837,  0.0262,  0.0575,  0.1151,  0.2092,  
            0.3138,  0.4231,  0.4362,  0.2495,  0.2500,  0.1606,  
            0.0638,  0.0502,  0.0534,  0.1700,  0.2489,  0.2824,  
            0.3290,  0.4493,  0.3201,  0.2359,  0.1904,  0.1093,  
            0.0596,  0.1977,  0.3651,  0.5549,  0.5272,  0.4268,  
            0.3478,  0.1820,  0.1600,  0.0366,  0.1036,  0.4838,  
            0.8075,  0.6585,  0.4435,  0.3562,  0.2014,  0.1192,  
            0.0534,  0.1260,  0.4336,  0.6904,  0.6846,  0.6177,  
            0.4702,  0.3483,  0.3138,  0.2453,  0.2144,  0.1114,  
            0.0837,  0.0335,  0.0214,  0.0356,  0.0758,  0.1778,  
            0.2354,  0.2254,  0.2484,  0.2207,  0.1470,  0.0528,  
            0.0424,  0.0131,  0.0000,  0.0073,  0.0262,  0.0638,  
            0.0727,  0.1851,  0.2395,  0.2150,  0.1574,  0.1250,  
            0.0816,  0.0345,  0.0209,  0.0094,  0.0445,  0.0868,  
            0.1898,  0.2594,  0.3358,  0.3504,  0.3708,  0.2500,  
            0.1438,  0.0445,  0.0690,  0.2976,  0.6354,  0.7233,  
            0.5397,  0.4482,  0.3379,  0.1919,  0.1266,  0.0560,  
            0.0785,  0.2097,  0.3216,  0.5152,  0.6522,  0.5036,  
            0.3483,  0.3373,  0.2829,  0.2040,  0.1077,  0.0350,  
            0.0225,  0.1187,  0.2866,  0.4906,  0.5010,  0.4038,  
            0.3091,  0.2301,  0.2458,  0.1595,  0.0853,  0.0382,  
            0.1966,  0.3870,  0.7270,  0.5816,  0.5314,  0.3462,  
            0.2338,  0.0889,  0.0591,  0.0649,  0.0178,  0.0314,  
            0.1689,  0.2840,  0.3122,  0.3332,  0.3321,  0.2730,  
            0.1328,  0.0685,  0.0356,  0.0330,  0.0371,  0.1862,  
            0.3818,  0.4451,  0.4079,  0.3347,  0.2186,  0.1370,  
            0.1396,  0.0633,  0.0497,  0.0141,  0.0262,  0.1276,  
            0.2197,  0.3321,  0.2814,  0.3243,  0.2537,  0.2296,  
            0.0973,  0.0298,  0.0188,  0.0073,  0.0502,  0.2479,  
            0.2986,  0.5434,  0.4215,  0.3326,  0.1966,  0.1365,  
            0.0743,  0.0303,  0.0873,  0.2317,  0.3342,  0.3609,  
            0.4069,  0.3394,  0.1867,  0.1109,  0.0581,  0.0298,  
            0.0455,  0.1888,  0.4168,  0.5983,  0.5732,  0.4644,  
            0.3546,  0.2484,  0.1600,  0.0853,  0.0502,  0.1736,  
            0.4843,  0.7929,  0.7128,  0.7045,  0.4388,  0.3630,  
            0.1647,  0.0727,  0.0230,  0.1987,  0.7411,  0.9947,  
            0.9665,  0.8316,  0.5873,  0.2819,  0.1961,  0.1459,  
            0.0534,  0.0790,  0.2458,  0.4906,  0.5539,  0.5518,  
            0.5465,  0.3483,  0.3603,  0.1987,  0.1804,  0.0811,  
            0.0659,  0.1428,  0.4838,  0.8127 
          };

        public const int STARTING_YEAR = 1700;
        public const int WINDOW_SIZE = 30;
        public const int TRAIN_START = WINDOW_SIZE;
        public const int TRAIN_END = 259;
        public const int EVALUATE_START = 260;
        public int EVALUATE_END;

        /// <summary>
        /// This really should be lowered, I am setting it to a level here that will
        /// train in under a minute.
        /// </summary>
        public const double MAX_ERROR = 0.05;

        private double[] normalizedSunspots;
        private double[] closedLoopSunspots;

        public Sunspots()
        {
            this.EVALUATE_END = SUNSPOTS.Length - 1;
        }

        public void NormalizeSunspots(double lo, double hi)
        {
            IInputField inField;

            // create arrays to hold the normalized sunspots
            normalizedSunspots = new double[SUNSPOTS.Length];
            closedLoopSunspots = new double[SUNSPOTS.Length];

            // normalize the sunspots
            DataNormalization norm = new DataNormalization();
            norm.Report = new NullStatusReportable();
            norm.AddInputField(inField = new InputFieldArray1D(true, SUNSPOTS));
            norm.AddOutputField(new OutputFieldRangeMapped(inField, lo, hi));
            norm.Storage = new NormalizationStorageArray1D(normalizedSunspots);
            norm.Process();

            for (int i = 0; i < SUNSPOTS.Length; i++)
                this.closedLoopSunspots[i] = this.normalizedSunspots[i];

        }

        public INeuralDataSet GenerateTraining()
        {
            TemporalNeuralDataSet result = new TemporalNeuralDataSet(WINDOW_SIZE, 1);

            TemporalDataDescription desc = new TemporalDataDescription(
                    TemporalDataDescription.Type.RAW, true, true);
            result.AddDescription(desc);

            for (int year = TRAIN_START; year < TRAIN_END; year++)
            {
                TemporalPoint point = new TemporalPoint(1);
                point.Sequence = year;
                point.Data[0] = this.normalizedSunspots[year];
                result.Points.Add(point);
            }

            result.Generate();

            return result;
        }

        public BasicNetwork CreateNetwork()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(WINDOW_SIZE));
            network.AddLayer(new BasicLayer(10));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public void Train(BasicNetwork network, INeuralDataSet training)
        {
            ITrain train = new ResilientPropagation(network, training);

            int epoch = 1;

            do
            {
                train.Iteration();
                app.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
                epoch++;
            } while (train.Error > MAX_ERROR);
        }

        public void Predict(BasicNetwork network)
        {


            app.WriteLine("Year\tActual\tPredict\tClosed Loop Predict");

            for (int year = EVALUATE_START; year < EVALUATE_END; year++)
            {
                // calculate based on actual data
                INeuralData input = new BasicNeuralData(WINDOW_SIZE);
                for (int i = 0; i < input.Count; i++)
                {
                    input[i] = this.normalizedSunspots[(year - WINDOW_SIZE) + i];
                }
                INeuralData output = network.Compute(input);
                double prediction = output[0];
                this.closedLoopSunspots[year] = prediction;

                // calculate "closed loop", based on predicted data
                for (int i = 0; i < input.Count; i++)
                {
                    input[i] = this.closedLoopSunspots[(year - WINDOW_SIZE) + i];
                }
                output = network.Compute(input);
                double closedLoopPrediction = output[0];

                // display
                app.WriteLine((STARTING_YEAR + year)
                        + "\t" + Format.FormatDouble(this.normalizedSunspots[year], 4)
                        + "\t" + Format.FormatDouble(prediction, 4)
                        + "\t" + Format.FormatDouble(closedLoopPrediction, 4)
                );

            }
        }


        public void Execute(IExampleInterface app)
        {
            this.app = app;
            NormalizeSunspots(0.1, 0.9);
            BasicNetwork network = CreateNetwork();
            INeuralDataSet training = GenerateTraining();
            Train(network, training);
            Predict(network);
        }
    }
}
