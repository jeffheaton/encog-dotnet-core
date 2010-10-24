using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Engine.Network.Flat;
using Encog.Neural.Networks;
using Encog.Engine.Network.Train.Prop;
using Encog.Util.Simple;
using Encog.MathUtil.Randomize;
using Encog.Util.Logging;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Engine.Opencl;
using Encog.Engine.Util;
using Encog.Util.CSV;

namespace Encog.Examples.CL
{
    public class CompareCL : IExample
    {
        public const int ITERATIONS = 10;
        /// <summary>
        /// Input for the XOR function.
        /// </summary>
        public static double[][] XOR_INPUT ={
            new double[2] { 0.0, 0.0 },
            new double[2] { 1.0, 0.0 },
			new double[2] { 0.0, 1.0 },
            new double[2] { 1.0, 1.0 } };

        /// <summary>
        /// Ideal output for the XOR function.
        /// </summary>
        public static double[][] XOR_IDEAL = {                                              
            new double[1] { 0.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 1.0 }, 
            new double[1] { 0.0 } };

        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(CompareCL),
                    "opencl-compare",
                    "Compare OpenCL training.",
                    "Compare OpenCL training to non-OpenCL training.");
                return info;
            }
        }

        private IExampleInterface app;

        private String FormatArray(double[] array)
        {
            StringBuilder result = new StringBuilder();
            NumberList.ToList(CSVFormat.EG_FORMAT, result, array);
            return result.ToString();
        }

        public void displayWeights(FlatNetwork networkCPU,
        FlatNetwork networkGPU)
        {
            app.WriteLine("CPU-Weights:"
                    + FormatArray(networkCPU.Weights));
            app.WriteLine("GPU-Weights:"
                    + FormatArray(networkGPU.Weights));
        }

        public FlatNetwork createNetwork()
        {
            BasicNetwork network = EncogUtility
                    .SimpleFeedForward(2, 4, 0, 1, false);
            IRandomizer randomizer = new ConsistentRandomizer(-1, 1);
            randomizer.Randomize(network);
            return (FlatNetwork)network.Structure.Flat.Clone();
        }


        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            this.app = app;
            Logging.StopConsoleLogging();
            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            FlatNetwork networkCPU = createNetwork();
            FlatNetwork networkGPU = createNetwork();

            app.WriteLine("Starting Weights:");
            displayWeights(networkCPU, networkGPU);

            EncogFramework.Instance.InitCL();
            EncogCLDevice device = EncogFramework.Instance.CL.ChooseDevice();
            OpenCLTrainingProfile profile = new OpenCLTrainingProfile(device);

            TrainFlatNetworkResilient trainCPU = new TrainFlatNetworkResilient(
                    networkCPU, trainingSet);
            TrainFlatNetworkOpenCL trainGPU = new TrainFlatNetworkOpenCL(
                    networkGPU, trainingSet, profile);

            trainGPU.LearnRPROP();

            for (int iteration = 1; iteration <= ITERATIONS; iteration++)
            {
                trainCPU.Iteration();
                trainGPU.Iteration();

                app.WriteLine();
                app.WriteLine("*** Iteration #" + iteration);
                app.WriteLine("CPU-Error: " + trainCPU.Error);
                app.WriteLine("GPU-Error: " + trainGPU.Error);
                app.WriteLine("CPU-LastGrad:"
                        + FormatArray(trainCPU.LastGradient));
                app.WriteLine("GPU-LastGrad:"
                        + FormatArray(trainGPU.LastGradient));
                app.WriteLine("CPU-Updates :"
                        + FormatArray(trainCPU.UpdateValues));
                app.WriteLine("GPU-Updates :"
                        + FormatArray(trainGPU.UpdateValues));
                displayWeights(networkCPU, networkGPU);
            }
        }
    }
}
