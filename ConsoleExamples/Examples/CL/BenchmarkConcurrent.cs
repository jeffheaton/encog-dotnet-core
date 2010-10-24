using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Neural.Networks.Training.Concurrent.Jobs;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Concurrent;
using Encog.Util.Banchmark;
using Encog.Neural.Networks;
using Encog.Util.Simple;
using Encog.Neural.Networks.Training.Strategy.End;
using System.Diagnostics;
using Encog.Util.Logging;

namespace Encog.Examples.CL
{
    public class BenchmarkConcurrent : IExample
    {
        public const int OUTPUT_SIZE = 2;
        public const int INPUT_SIZE = 10;
        public const int HIDDEN1 = 6;
        public const int HIDDEN2 = 0;
        public const int TRAINING_SIZE = 1000;
        public const int ITERATIONS = 1000;
        public const int JOBS = 50;

        /// <summary>
        /// Iterations per cycle.  Higher numbers load up OpenCL more, but too high may 
        /// timeout your GPU, if your OS has a timeout.  Do not set this higher than one
        /// unless you are using the MAX openCL ratio of 1.0, otherwise it is pointless, 
        /// and will throw an error.
        /// </summary>
        public const int ITERATIONS_PER = 1;

        public const double LOCAL_RATIO = 1.0;
        public const int GLOBAL_RATIO = 1;
        public const double SEGMENTATION_RATIO = 1.0;

        /// <summary>
        /// Max cores to use, 0=autodetect, -1=no CPU cores, other number is the # of cores.
        /// </summary>
        public const int MAX_CORES = 0;

        public IExampleInterface app;


        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(BenchmarkConcurrent),
                    "opencl-concurrent",
                    "Run concurrent CPU and OpenCL jobs.",
                    "Dump basic OpenCL info to console.");
                return info;
            }
        }


        public TrainingJob generateTrainingJob(ConcurrentTrainingManager manager)
        {
            INeuralDataSet training = RandomTrainingFactory.Generate(1000,
                    TRAINING_SIZE, INPUT_SIZE, OUTPUT_SIZE, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                    training.InputSize, HIDDEN1, HIDDEN2,
                    training.IdealSize, true);
            network.Reset();

            RPROPJob job = new RPROPJob(network, training, true, LOCAL_RATIO, GLOBAL_RATIO, SEGMENTATION_RATIO, ITERATIONS_PER);
            job.Strategies.Add(new EndIterationsStrategy(ITERATIONS));

            manager.AddTrainingJob(job);
            return job;

        }

        public int benchmark(bool splitCores)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ConcurrentTrainingManager manager = ConcurrentTrainingManager
                    .Instance;

            manager.Report = new ConsoleStatusReportable();
            manager.DetectPerformers(splitCores, 0);
            app.WriteLine("Device(s) in use:");
            app.WriteLine(manager.ToString());
            manager.ClearQueue();
            for (int i = 0; i < JOBS; i++)
                generateTrainingJob(manager);

            manager.Start();
            app.WriteLine("Manager has started.");
            manager.Join();
            app.WriteLine("Manager has stopped.");

            stopWatch.Stop();
            return (int)(stopWatch.ElapsedMilliseconds / 1000);
        }

        public void run()
        {
            Logging.StopConsoleLogging();
            app.WriteLine("* * * Performing CPU-Only Test * * *");
            int cpu = benchmark(false);
            app.WriteLine("CPU-only took: " + cpu + " seconds.");

            app.WriteLine();
            app.WriteLine("* * * Performing CPU-Only(split cores) Test * * *");
            int cpuSplit = benchmark(true);
            app.WriteLine("CPU-only(split cores took: " + cpuSplit
                    + " seconds.");
            Logging.StopConsoleLogging();
            EncogFramework.Instance.InitCL();
            app.WriteLine();

            app.WriteLine("* * * Performing OpenCL Test * * *");
            EncogFramework.Instance.InitCL();
            int gpu = benchmark(true);

            app.WriteLine("OpenCL took: " + gpu + " seconds.");
            app.WriteLine();
            app.WriteLine("Final times:");
            app.WriteLine("CPU-Only       : " + cpu + "ms");
            app.WriteLine("CPU-Split Cores: " + cpuSplit + "ms");
            app.WriteLine("CPU and OpenCL : " + gpu + "ms");
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            try
            {
                this.app = app;
                run();
                EncogFramework.Instance.Shutdown();
            }
            catch (Exception ex)
            {
                throw new EncogError(ex);
            }
        }

    }
}
