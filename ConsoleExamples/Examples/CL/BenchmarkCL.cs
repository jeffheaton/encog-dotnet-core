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
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.NeuralData;
using Encog.Util.Banchmark;
using Encog.Util.Simple;
using Encog.Neural.Networks;
using System.Diagnostics;
using Encog.Util;
using Encog.Engine.Opencl;
using Encog.Engine.Util;

namespace Encog.Examples.CL
{
    public class BenchmarkCL : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(BenchmarkCL),
                    "opencl-benchmark",
                    "Simple OpenCL benchmark.",
                    "Simple OpenCL example that benchmarks OpenCL processing in Encog.");
                return info;
            }
        }

        public long benchmarkCPU(BasicNetwork network, INeuralDataSet training)
        {
            ResilientPropagation train = new ResilientPropagation(network, training);

            train.Iteration();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                train.Iteration();
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public long benchmarkCL(BasicNetwork network, INeuralDataSet training)
        {
            ResilientPropagation train = new ResilientPropagation(network, training);
            

            train.Iteration();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 100; i++)
            {
                train.Iteration();
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }


        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            try
            {
                int outputSize = 2;
                int inputSize = 10;
                int trainingSize = 100000;

                INeuralDataSet training = RandomTrainingFactory.Generate(
                    1000,trainingSize, inputSize, outputSize, -1, 1);
                BasicNetwork network = EncogUtility.SimpleFeedForward(
                    training.InputSize, 6, 0, training.IdealSize, true);
                network.Reset();

                Console.WriteLine("Running non-OpenCL test.");
                long cpuTime = benchmarkCPU(network, training);
                Console.WriteLine("Non-OpenCL test took " + cpuTime + "ms.");
                Console.WriteLine("Starting OpenCL");
                EncogFramework.Instance.InitCL();
                Console.WriteLine("Running OpenCL test.");
                long clTime = benchmarkCL(network, training);
                Console.WriteLine("OpenCL test took " + clTime + "ms.");

                double diff = ((double)cpuTime - (double)clTime);
                String percent = Format.FormatPercent((double)cpuTime/(double)clTime);
                Console.WriteLine("OpenCL Performed at " + percent + " the speed of non-OpenCL");
            }
            catch (EncogCLError ex)
            {
                Console.WriteLine("Can't startup CL, make sure you have drivers loaded.");
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
