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
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Layers;
using Encog.Util.Banchmark;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util.Logging;

namespace Encog.Examples.MultiBench
{
    public class MultiThreadBenchmark:IExample
    {
        public const int INPUT_COUNT = 40;
        public const int HIDDEN_COUNT = 60;
        public const int OUTPUT_COUNT = 20;

        private IExampleInterface app;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(MultiThreadBenchmark),
                    "multibench",
                    "Multithreading Benchmark",
                    "See the effects that multithreading has on performance.");
                return info;
            }
        }


        public BasicNetwork generateNetwork()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(MultiThreadBenchmark.INPUT_COUNT));
            network.AddLayer(new BasicLayer(MultiThreadBenchmark.HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(MultiThreadBenchmark.OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        public MLDataSet generateTraining()
        {
            MLDataSet training = RandomTrainingFactory.Generate(1000,50000,
                    INPUT_COUNT, OUTPUT_COUNT, -1, 1);
            return training;
        }

        public double evaluateRPROP(BasicNetwork network, MLDataSet data)
        {

            ResilientPropagation train = new ResilientPropagation(network, data);
            long start = DateTime.Now.Ticks;
            Console.WriteLine("Training 20 Iterations with RPROP");
            for (int i = 1; i <= 1; i++)
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

        public double evaluateMPROP(BasicNetwork network, MLDataSet data)
        {

            ResilientPropagation train = new ResilientPropagation(network, data);
            long start = DateTime.Now.Ticks;
            Console.WriteLine("Training 20 Iterations with MPROP");
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
        public void Execute(IExampleInterface app)
        {
            this.app = app;
            Logging.StopConsoleLogging();
            BasicNetwork network = generateNetwork();
            MLDataSet data = generateTraining();

            double rprop = evaluateRPROP(network, data);
            double mprop = evaluateMPROP(network, data);
            double factor = rprop / mprop;
            Console.WriteLine("Factor improvement:" + factor);
        }
    }
}
