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

namespace Encog.Examples.CL
{
    public class TuneCL : IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(TuneCL),
                    "opencl-tune",
                    "Simple OpenCL Tune.",
                    "Simple OpenCL example that gives stats on OpenCL processing in Encog to allow the CPU/GPU to be balanced.");
                return info;
            }
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

                Encog.Instance.InitCL();

                ResilientPropagation train = new ResilientPropagation(network, training);
                
                train.NumThreads = 0;
                train.Iteration();

                long start = Environment.TickCount;
                for (int i = 0; i < 100; i++)
                {
                    train.Iteration();
                    Console.WriteLine("Train error: " + train.Error);
                }
                long stop = Environment.TickCount;

                Console.WriteLine("GPU Time:" + train.FlatTraining.CLTimePerIteration);
                Console.WriteLine("CPU Time:" + train.FlatTraining.CPUTimePerIteration);
                Console.WriteLine("Ratio:" + train.FlatTraining.CalculatedCLRatio);
                Console.WriteLine("This ratio should be used as the \"enforced ratio\" to balance workloads.");
                Console.WriteLine("Done:" + (stop - start));
                Console.WriteLine("Stop");
                if (Encog.Instance.CL != null)
                    Console.WriteLine(Encog.Instance.CL.ToString());
            }
            catch (EncogCLError ex)
            {
                Console.WriteLine("Can't startup CL, make sure you have drivers loaded.");
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
