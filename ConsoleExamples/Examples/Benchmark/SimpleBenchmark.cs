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
using Encog.ML.Data.Basic;
using Encog.Util.Banchmark;
using ConsoleExamples.Examples;
using Encog.Engine;
using System.Diagnostics;
using Encog.Neural.Flat;
using Encog.Neural.Flat.Train.Prop;

namespace Encog.Examples.Benchmark
{
    public class SimpleBenchmark : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(SimpleBenchmark),
                    "benchmark-simple",
                    "Perform a simple Encog benchmark.",
                    "Train an XOR for backprop for a number of iterations.");
                return info;
            }
        }

        public void Execute(IExampleInterface app)
        {
            FlatNetwork network = new FlatNetwork(2, 2, 0, 1, false);

            //double[] a = new double[2];
            //double[] b = new double[1];


            double[][] input = new double[4][] {
											new double[] {0, 0},
											new double[] {0, 1},
											new double[] {1, 0},
											new double[] {1, 1}
										};
            double[][] output = new double[4][] {
											 new double[] {0},
											 new double[] {1},
											 new double[] {1},
											 new double[] {0}
										 };

            BasicMLDataSet trainingSet = new BasicMLDataSet(input, output);

            TrainFlatNetworkBackPropagation train = new TrainFlatNetworkBackPropagation(network, trainingSet, 0.7, 0.7);


            Stopwatch sw = new Stopwatch();
            sw.Start();
            // run epoch of learning procedure
            for (int i = 0; i < 10000; i++)
            {
                train.Iteration();
            }
            sw.Stop();

            Console.WriteLine("Flat Network Time:" + sw.ElapsedMilliseconds);
        }
    }
}
