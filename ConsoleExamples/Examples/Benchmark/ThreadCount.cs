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
using System.Diagnostics;
using Encog.Neural.Networks.Layers;
using Encog.Util.Banchmark;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Examples.Benchmark
{
    public class ThreadCount : IExample
    {
        public const int INPUT_COUNT = 40;
        public const int HIDDEN_COUNT = 60;
        public const int OUTPUT_COUNT = 20;

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(ThreadCount),
                    "threadcount",
                    "Evaluate Thread Count Performance",
                    "Compare Encog performance at various thread counts.");
                return info;
            }
        }

        public void Perform(int thread)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(INPUT_COUNT));
            network.AddLayer(new BasicLayer(HIDDEN_COUNT));
            network.AddLayer(new BasicLayer(OUTPUT_COUNT));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet training = RandomTrainingFactory.Generate(50000,
                    INPUT_COUNT, OUTPUT_COUNT, -1, 1);

            ResilientPropagation rprop = new ResilientPropagation(network, training);
            rprop.NumThreads = thread;
            for (int i = 0; i < 5; i++)
            {
                rprop.Iteration();
            }
            stopwatch.Stop();
            Console.WriteLine("Result with " + thread + " was " + stopwatch.ElapsedMilliseconds + "ms");
        }

        public void Execute(IExampleInterface app)
        {
            for (int i = 1; i < 16; i++)
            {
                Perform(i);
            }
        }
    }
}
