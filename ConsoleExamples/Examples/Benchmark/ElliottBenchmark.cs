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
using Encog.Neural.Networks.Training.Propagation.Resilient;
using System.Diagnostics;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Layers;
using Encog.Util;
using Encog.Engine.Network.Activation;
using Encog.Util.Banchmark;

namespace Encog.Examples.Benchmark
{
    public class ElliottBenchmark : IExample
    {
        public const int INPUT_OUTPUT = 25;
        public const int HIDDEN = 5;
        public const int SAMPLE_SIZE = 50;
        public const double TARGET_ERROR = 0.01;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof(ElliottBenchmark),
                    "benchmark-elliott",
                    "Perform a benchmark of the Elliott activation function.",
                    "The Elliott activation function can be much faster than tan/sigmoid, this example measures that.");
                return info;
            }
        }

        public static int Evaluate(BasicNetwork network, IMLDataSet training)
        {
            ResilientPropagation rprop = new ResilientPropagation(network, training);
            int iterations = 0;

            for (; ; )
            {
                rprop.Iteration();                
                iterations++;
                if (rprop.Error < TARGET_ERROR)
                {
                    return iterations;
                }

                if (iterations > 1000)
                {
                    iterations = 0;
                    return -1;
                }
            }
        }

        public static void evaluateNetwork(BasicNetwork network, IMLDataSet training)
        {
            double total = 0;
            int seed = 0;
            int completed = 0;

            Stopwatch sw = new Stopwatch();

            sw.Start();
            while (completed < SAMPLE_SIZE)
            {
                new ConsistentRandomizer(-1, 1, seed).Randomize(network);
                int iter = Evaluate(network, training);
                if (iter == -1)
                {
                    seed++;
                }
                else
                {
                    total += iter;
                    seed++;
                    completed++;
                }
            }

            sw.Stop();


            Console.WriteLine(network.GetActivation(1).GetType().Name + ": time="
                    + Format.FormatInteger((int)sw.ElapsedMilliseconds)
                    + "ms, Avg Iterations: "
                    + Format.FormatInteger((int)(total / SAMPLE_SIZE)));

        }

        public static BasicNetwork createTANH()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, INPUT_OUTPUT));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, HIDDEN));
            network.AddLayer(new BasicLayer(new ActivationTANH(), false, INPUT_OUTPUT));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public static BasicNetwork createElliott()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, INPUT_OUTPUT));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), true, HIDDEN));
            network.AddLayer(new BasicLayer(new ActivationElliottSymmetric(), false, INPUT_OUTPUT));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }


        public void Execute(IExampleInterface app)
        {
            Console.WriteLine("Average iterations needed (lower is better)");

            IMLDataSet training = EncoderTrainingFactory.GenerateTraining(INPUT_OUTPUT, false, -1, 1);

            evaluateNetwork(createTANH(), training);
            evaluateNetwork(createElliott(), training);

            EncogFramework.Instance.Shutdown();
        }
    }
}
