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
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks;
using Encog.Neural.NeuralData;
using ConsoleExamples.Examples;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Data.Basic;
using Encog.Util.Simple;

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

        public double Evaluate(BasicNetwork network, INeuralDataSet training)
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
                BasicNetwork network, INeuralDataSet training)
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

            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT,
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
