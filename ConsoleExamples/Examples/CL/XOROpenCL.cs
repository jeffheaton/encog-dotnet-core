using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Util.Logging;
using Encog.Neural.Networks;
using Encog.Util.Simple;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Engine.Opencl;
using Encog.Engine.Network.Train.Prop;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Strategy;
using Encog.Engine.Util;

namespace Encog.Examples.CL
{
    public class XOROpenCL : IExample
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
                    typeof(XOROpenCL),
                    "opencl-xor",
                    "Simple OpenCL XOR Example.",
                    "Simple OpenCL example learns XOR.");
                return info;
            }
        }


        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            Logging.StopConsoleLogging();
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 4, 0, 1, false);
            network.Reset();

            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            EncogFramework.Instance.InitCL();

            // train the neural network
            EncogCLDevice device = EncogFramework.Instance.CL.ChooseDevice();
            OpenCLTrainingProfile profile = new OpenCLTrainingProfile(device);
            Propagation train = new ResilientPropagation(network, trainingSet, profile);

            // reset if improve is less than 1% over 50 cycles
            train.AddStrategy(new RequiredImprovementStrategy(5));

            EncogUtility.TrainToError(train, trainingSet, 0.01);

            // test the neural network

            EncogUtility.Evaluate(network, trainingSet);
            Console.WriteLine("Neural Network Results:");

            Console.WriteLine("OpenCL device used: " + profile.Device.ToString());

        }
    }
}
