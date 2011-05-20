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
using Encog.Neural.Flat;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Util.Simple;
using Encog.MathUtil.Randomize;
using Encog.ML.Data.Basic;
using Encog.Neural.Flat.Train.Prop;
using Encog.Util.CSV;

namespace Encog.Examples.XOR
{
    public class XORDisplay : IExample
    {
        public const int ITERATIONS = 10;

        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XORInput = {
                                                new double[2] {0.0, 0.0},
                                                new double[2] {1.0, 0.0},
                                                new double[2] {0.0, 1.0},
                                                new double[2] {1.0, 1.0}
                                            };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XORIdeal = {
                                                new double[1] {0.0},
                                                new double[1] {1.0},
                                                new double[1] {1.0},
                                                new double[1] {0.0}
                                            };

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(XORDisplay),
                    "xor-display",
                    "Train XOR and display training deltas.",
                    "This example allows you to peer into the training process.  Useful for debugging.");
                return info;
            }
        }

        #region IExample Members

        public static void DisplayWeights(FlatNetwork network)
        {
            Console.WriteLine(@"Weights:"
                    + FormatArray(network.Weights));
        }

        public static void Evaluate(FlatNetwork network, MLDataSet trainingSet)
        {
            double[] output = new double[1];
            foreach (MLDataPair pair in trainingSet)
            {
                network.Compute(pair.Input.Data, output);
                Console.WriteLine(pair.Input.Data[0] + @"," + pair.Input[1]
                        + @", actual=" + output[0] + @",ideal=" + pair.Ideal[0]);
            }

        }

        public static FlatNetwork CreateNetwork()
        {
            BasicNetwork network = EncogUtility
                    .SimpleFeedForward(2, 4, 0, 1, false);
            IRandomizer randomizer = new ConsistentRandomizer(-1, 1);
            randomizer.Randomize(network);
            return (FlatNetwork)network.Structure.Flat.Clone();
        }

        public static String FormatArray(double[] a)
        {
            StringBuilder builder = new StringBuilder();
            NumberList.ToList(CSVFormat.ENGLISH,5,builder,a);
            return builder.ToString();
        }


        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="app">Holds arguments and other info.</param>
        public void Execute(IExampleInterface app)
        {
            MLDataSet trainingSet = new BasicMLDataSet(XORInput, XORIdeal);

            FlatNetwork network = CreateNetwork();

            Console.WriteLine(@"Starting Weights:");
            DisplayWeights(network);
            Evaluate(network, trainingSet);

            TrainFlatNetworkResilient train = new TrainFlatNetworkResilient(
                    network, trainingSet);

            for (int iteration = 1; iteration <= ITERATIONS; iteration++)
            {
                train.Iteration();

                Console.WriteLine();
                Console.WriteLine(@"*** Iteration #" + iteration);
                Console.WriteLine(@"Error: " + train.Error);
                Evaluate(network, trainingSet);

                Console.WriteLine(@"LastGrad:"
                        + FormatArray(train.LastGradient));
                Console.WriteLine(@"Updates :"
                        + FormatArray(train.UpdateValues));

                DisplayWeights(network);
            }
        }

        #endregion
    }
}
