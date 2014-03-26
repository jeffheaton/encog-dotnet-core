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
using System.Diagnostics;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Activation;
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Util;

namespace Encog.Examples.Benchmark
{
    public class SimpleBenchmark : IExample
    {
        public const int ROW_COUNT = 100000;
        public const int INPUT_COUNT = 10;
        public const int OUTPUT_COUNT = 1;
        public const int HIDDEN_COUNT = 20;
        public const int ITERATIONS = 10;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (SimpleBenchmark),
                    "benchmark-simple",
                    "Perform a simple Encog benchmark.",
                    "Train an XOR for backprop for a number of iterations.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            // initialize input and output values
            double[][] input = Generate(ROW_COUNT, INPUT_COUNT);
            double[][] output = Generate(ROW_COUNT, OUTPUT_COUNT);

            for (int i = 0; i < 10; i++)
            {
                long time = BenchmarkEncog(input, output);
                Console.WriteLine("Regular: {0}ms", time);
            }
        }

        #endregion

        public static long BenchmarkEncog(double[][] input, double[][] output)
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true,
                                            input[0].Length));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true,
                                            HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false,
                                            output[0].Length));
            network.Structure.FinalizeStructure();
            network.Reset(23); // constant seed for repeatable testing

            IMLDataSet trainingSet = new BasicMLDataSet(input, output);

            // train the neural network
            IMLTrain train = new Backpropagation(network, trainingSet, 0.7, 0.7);

            var sw = new Stopwatch();
            sw.Start();
            // run epoch of learning procedure
            for (int i = 0; i < ITERATIONS; i++)
            {
                train.Iteration();
            }
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }


        private static double[][] Generate(int rows, int columns)
        {
            double[][] result = EngineArray.AllocateDouble2D(rows, columns);
			var rand = new Random(42); // same value every time for a benchmark

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i][j] = rand.NextDouble();
                }
            }

            return result;
        }
    }
}
