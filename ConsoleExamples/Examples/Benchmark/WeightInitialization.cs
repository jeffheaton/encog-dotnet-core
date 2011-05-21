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
using Encog.MathUtil.Randomize;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using ConsoleExamples.Examples;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Simple;
using Encog.Mathutil.Randomize;

namespace Encog.Examples.Benchmark
{
    /// <summary>
    /// There are several ways to init the weights in an Encog neural network. This
    /// example benhmarks each of the methods that Encog offers. A simple neural
    /// network is created for the XOR operator and is trained a number of times with
    /// each of the randomizers. The score for each randomizer is display, the score
    /// is the average amount of error improvement, higher is better.
    /// </summary>
    public class WeightInitialization : IExample
    {
        public const int SAMPLE_SIZE = 1000;
        public const int ITERATIONS = 50;

        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT ={
            new double[2] { 0.0, 0.0 },
            new double[2] { 1.0, 0.0 },
			new double[2] { 0.0, 1.0 },
            new double[2] { 1.0, 1.0 } };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {                                              
            new double[1] { 0.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 0.0 } };

        public double Evaluate(BasicNetwork network, IMLDataSet training)
        {
            ResilientPropagation rprop = new ResilientPropagation(network, training);
            double startingError = network.CalculateError(training);
            for (int i = 0; i < ITERATIONS; i++)
            {
                rprop.Iteration();
            }
            double finalError = network.CalculateError(training);
            return startingError - finalError;
        }

        public double EvaluateRandomizer(IRandomizer randomizer,
                BasicNetwork network, IMLDataSet training)
        {
            double total = 0;
            for (int i = 0; i < SAMPLE_SIZE; i++)
            {
                randomizer.Randomize(network);
                total += Evaluate(network, training);
            }
            return total / SAMPLE_SIZE;
        }

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(WeightInitialization),
                    "weight-init",
                    "weight Initializers",
                    "Show performance of different weight init methods.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            RangeRandomizer rangeRandom = new RangeRandomizer(-1, 1);
            NguyenWidrowRandomizer nwrRandom = new NguyenWidrowRandomizer(-1, 1);
            FanInRandomizer fanRandom = new FanInRandomizer();
            GaussianRandomizer gaussianRandom = new GaussianRandomizer(0, 1);

            BasicMLDataSet training = new BasicMLDataSet(XOR_INPUT,
                    XOR_IDEAL);
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 10, 0, 1, true);

            Console.WriteLine("Range random: "
                    + EvaluateRandomizer(rangeRandom, network, training));
            Console.WriteLine("Nguyen-Widrow: "
                    + EvaluateRandomizer(nwrRandom, network, training));
            Console.WriteLine("Fan-In: "
                    + EvaluateRandomizer(fanRandom, network, training));
            Console.WriteLine("Gaussian: "
                    + EvaluateRandomizer(gaussianRandom, network, training));
        }
    }
}
