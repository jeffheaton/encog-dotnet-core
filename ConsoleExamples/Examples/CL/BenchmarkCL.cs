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
using Encog.Util.Logging;
using Encog.Engine.Network.Train.Prop;
using Encog.Neural.Networks.Training.Strategy.End;

namespace Encog.Examples.CL
{
    public class BenchmarkCL : IExample
    {

        public const int GLOBAL_SIZE = 200;
        public const int BENCHMARK_ITERATIONS = 100;
        public const double OPENCL_RATIO = 1.0;
        public const int ITERATIONS_PER_CYCLE = 1;
        public static OpenCLTrainingProfile profile;

        public static IExampleInterface app;

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

        public static long PerformBenchmarkCPU(BasicNetwork network, INeuralDataSet training)
        {
            ResilientPropagation train = new ResilientPropagation(network, training);
            EndIterationsStrategy stop;
            train.AddStrategy(stop = new EndIterationsStrategy(BENCHMARK_ITERATIONS));
            train.Iteration(); // warmup

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!stop.ShouldStop())
            {
                train.Iteration(ITERATIONS_PER_CYCLE);
            }
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        public static long PerformBenchmarkCL(BasicNetwork network, INeuralDataSet training)
        {
            profile = new OpenCLTrainingProfile(EncogFramework.Instance.CL.ChooseDevice());

            app.WriteLine("Using device: " + profile.Device.ToString());
            ResilientPropagation train = new ResilientPropagation(network,
                    training, profile);

            train.Iteration(); // warmup

            EndIterationsStrategy stop;

            train.AddStrategy(stop = new EndIterationsStrategy(BENCHMARK_ITERATIONS));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!stop.ShouldStop())
            {
                train.Iteration(ITERATIONS_PER_CYCLE);
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
                Logging.StopConsoleLogging();
                int outputSize = 2;
                int inputSize = 10;
                int trainingSize = 100000;

                BenchmarkCL.app = app;

                INeuralDataSet training = RandomTrainingFactory.Generate(1000,
                        trainingSize, inputSize, outputSize, -1, 1);
                BasicNetwork network = EncogUtility.SimpleFeedForward(training
                        .InputSize, 6, 0, training.IdealSize, true);
                network.Reset();

                app.WriteLine("Running non-OpenCL test.");
                long cpuTime = PerformBenchmarkCPU(network, training);
                app.WriteLine("Non-OpenCL test took " + cpuTime + "ms.");
                app.WriteLine();

                app.WriteLine("Starting OpenCL");
                EncogFramework.Instance.InitCL();

                int i = 0;
                app.WriteLine("OpenCL Devices: (Encog will use the first GPU, or CPU if no GPU's)");
                foreach (EncogCLDevice device in EncogFramework.Instance.CL
                        .Devices)
                {
                    app.WriteLine("Device " + i + ": " + device.ToString());
                    i++;
                }

                app.WriteLine("Running OpenCL test.");
                long clTime = PerformBenchmarkCL(network, training);
                app.WriteLine("OpenCL test took " + clTime + "ms.");
                app.WriteLine();

                app.WriteLine("ITERATIONS_PER_CYCLE: " + ITERATIONS_PER_CYCLE);

                app.WriteLine();
                app.WriteLine(profile.ToString());
                app.WriteLine();
                String percent = Format.FormatPercent((double)cpuTime
                        / (double)clTime);
                app.WriteLine("OpenCL Performed at " + percent
                        + " the speed of non-OpenCL");
                app.WriteLine("You will likely get better performance by tuning: ITERATIONS_PER_CYCLE, local ratio, global ratio & segmentation ratio.");

            }
            catch (EncogCLError ex)
            {
                app.WriteLine("Can't startup CL, make sure you have drivers loaded.");
                app.WriteLine(ex.ToString());
            }
            finally
            {
                EncogFramework.Instance.Shutdown();
            }
        }

    }
}
