using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Util.CL;
using Encog.Util.CL.Kernels;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.NeuralData;
using Encog.Util.Banchmark;
using Encog.Util.Simple;
using Encog.Neural.Networks;
using System.Diagnostics;
using Encog.Util;

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
            
            // if there is ONLY a CPU OpenCL device, then use ONLY it
            if (Encog.Instance.CL.AreCPUsPresent && Encog.Instance.CL.Devices.Count==1)
            {
                train.NumThreads = -1; // NO non-CL threads
            }
            else if (Encog.Instance.CL.AreCPUsPresent && Encog.Instance.CL.Devices.Count > 1)
            {
                // if there are CPU OpenCL devices and OpenCL GPU's then disable the CPU's
                Encog.Instance.CL.DisableAllCPUs();
                train.NumThreads = 0;
            }

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
                    trainingSize, inputSize, outputSize, -1, 1);
                BasicNetwork network = EncogUtility.SimpleFeedForward(
                    training.InputSize, 6, 0, training.IdealSize, true);
                network.Reset();

                Console.WriteLine("Running non-OpenCL test.");
                long cpuTime = benchmarkCPU(network, training);
                Console.WriteLine("Non-OpenCL test took " + cpuTime + "ms.");
                Console.WriteLine("Starting OpenCL");
                Encog.Instance.InitCL();
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
