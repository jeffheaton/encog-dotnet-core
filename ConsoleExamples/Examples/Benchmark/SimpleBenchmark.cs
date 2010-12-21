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
using Encog.Util.Banchmark;
using ConsoleExamples.Examples;
using Encog.Engine;
using Encog.Engine.Network.Flat;
using Encog.Neural.Data.Basic;
using Encog.Engine.Network.Train.Prop;
using System.Diagnostics;

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

            BasicNeuralDataSet trainingSet = new BasicNeuralDataSet(input, output);

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
