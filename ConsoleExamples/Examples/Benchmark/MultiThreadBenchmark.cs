//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
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
using ConsoleExamples.Examples;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Banchmark;

namespace Encog.Examples.Benchmark
{
    public class MultiThreadBenchmark : IExample
    {
        public const int INPUT_COUNT = 40;
        public const int HIDDEN_COUNT = 60;
        public const int OUTPUT_COUNT = 20;

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                var info = new ExampleInfo(
                    typeof (MultiThreadBenchmark),
                    "multibench",
                    "Multithreading Benchmark",
                    "See the effects that multithreading has on performance.");
                return info;
            }
        }

        #region IExample Members

        public void Execute(IExampleInterface app)
        {
            this.app = app;

            BasicNetwork network = generateNetwork();
            IMLDataSet data = generateTraining();

            double rprop = EvaluateRPROP(network, data);
            double mprop = EvaluateMPROP(network, data);
            double factor = rprop/mprop;
            Console.WriteLine("Factor improvement:" + factor);
        }

        #endregion

        public BasicNetwork generateNetwork()
        {
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public IMLDataSet generateTraining()
        {
            IMLDataSet training = RandomTrainingFactory.Generate(1000, 50000,
                                                                 INPUT_COUNT, OUTPUT_COUNT, -1, 1);
            return training;
        }

        public double EvaluateRPROP(BasicNetwork network, IMLDataSet data)
        {
            var train = new ResilientPropagation(network, data);
            train.ThreadCount = 1;
            long start = DateTime.Now.Ticks;
            Console.WriteLine(@"Training 20 Iterations with RPROP");
            for (int i = 1; i <= 20; i++)
            {
                train.Iteration();
                Console.WriteLine("Iteration #" + i + " Error:" + train.Error);
            }
            //train.FinishTraining();
            long stop = DateTime.Now.Ticks;
            double diff = new TimeSpan(stop - start).Seconds;
            Console.WriteLine("RPROP Result:" + diff + " seconds.");
            Console.WriteLine("Final RPROP error: " + network.CalculateError(data));
            return diff;
        }

        public double EvaluateMPROP(BasicNetwork network, IMLDataSet data)
        {
            var train = new ResilientPropagation(network, data);
            long start = DateTime.Now.Ticks;
            Console.WriteLine(@"Training 20 Iterations with MPROP");
            for (int i = 1; i <= 20; i++)
            {
                train.Iteration();
                Console.WriteLine("Iteration #" + i + " Error:" + train.Error);
            }
            //train.finishTraining();
            long stop = DateTime.Now.Ticks;
            double diff = new TimeSpan(stop - start).Seconds;
            Console.WriteLine("MPROP Result:" + diff + " seconds.");
            Console.WriteLine("Final MPROP error: " + network.CalculateError(data));
            return diff;
        }
    }
}
