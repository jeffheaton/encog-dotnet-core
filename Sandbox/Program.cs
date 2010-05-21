using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CL;
using Encog.Util;
using Encog.Neural.NeuralData;
using Encog.Util.Banchmark;
using Encog.Neural.Networks;
using Encog.Util.Simple;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks.Flat;
using Encog.Neural.Data.Basic;
using Cloo;
using Encog.Util.CL.Kernels;
using Encog.Neural.Networks.Training;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Training.NEAT;
using Encog.Persist;
using System.IO;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Sandbox
{
    class Program
    {

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


        public static void XORNEAT()
        {
            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            ICalculateScore score = new TrainingSetScore(trainingSet);
            // train the neural network
            ActivationStep step = new ActivationStep();
            step.Center = 0.5;

            NEATTraining train = new NEATTraining(
                    score, 2, 1, 100);
            train.OutputActivationFunction = step;

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
                epoch++;
            } while ((train.Error > 0.001));

            BasicNetwork network = train.Network;

            EncogPersistedCollection encog = new EncogPersistedCollection("d:\\test.eg",FileMode.Create);
            //encog.Add("test", network);
            encog.Add("test2", train.Population);
        }

        static void simple()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, true);
            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);
            ResilientPropagation train = new ResilientPropagation(network, training);

            do
            {
                train.Iteration();
                Console.WriteLine("Train error: " + train.Error);
            } while (train.Error > .001);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static void train()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, false);
            FlatNetwork flat = new FlatNetwork(network);
            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);
            Encog.Encog.Instance.InitCL();
            TrainFlatNetworkMulti train = new TrainFlatNetworkMulti(flat, training, 1.0);
            for (int i = 0; i < 50; i++)
            {
                train.Iteration();
                Console.WriteLine("Netwk error: " + flat.CalculateError(training));
                Console.WriteLine("Train error: " + train.Error);
            }

            train.Iteration();
            flat.CalculateError(training);


            Console.WriteLine("Done");
        }

        public static void train2()
        {
            int outputSize = 2;
            int inputSize = 10;
            int trainingSize = 100000;

            INeuralDataSet training = RandomTrainingFactory.Generate(
                trainingSize, inputSize, outputSize, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                training.InputSize, 6, 0, training.IdealSize, true);
            network.Reset();
            FlatNetwork flat = new FlatNetwork(network);
            
            Encog.Encog.Instance.InitCL();

            long start = Environment.TickCount;
            TrainFlatNetworkMulti train = new TrainFlatNetworkMulti(flat, training, 1.0);
            train.NumThreads = 1;
            for (int i = 0; i < 50; i++)
            {
                train.Iteration();
                Console.WriteLine("Train error: " + train.Error);
                Console.WriteLine("Netwk error: " + flat.CalculateError(training));
            }
            long stop = Environment.TickCount;

            Console.WriteLine("GPU Time:" + train.CLTimePerIteration);
            Console.WriteLine("CPU Time:" + train.CPUTimePerIteration);
            Console.WriteLine("Ratio:" + train.CalculatedCLRatio);
            Console.WriteLine("Done:" + (stop-start));
            Console.WriteLine("Stop");
        }

        public static void testSimpleCL()
        {
            Encog.Encog.Instance.InitCL();

            EncogCLDevice device = Encog.Encog.Instance.CL.Devices[0];
            KernelVectorAdd k = Encog.Encog.Instance.CL.Devices[0].Platform.VectorAdd;
            k.Compile();

            double[] a = { 1, 2, 3, 4 };
            double[] b = { 5, 6, 7, 8 };
            double[] c = k.Add(device, a, b);

            for (int i = 0; i < a.Length; i++)
            {
                Console.WriteLine(a[i] + " + " + b[i] + " = " + c[i]);
            }
        }

        static void testFlatten()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, true);
            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);
            ResilientPropagation train = new ResilientPropagation(network, training);
            long start = Environment.TickCount;
            for(int i=0;i<1000;i++)
            {
                train.Iteration();
                Console.WriteLine(i + " Train error: " + train.Error);
            } 
            long stop = Environment.TickCount;
            Console.WriteLine("Done:" + (stop - start));
        }

        static void Main(string[] args)
        {
            //try
            {
                ///testCL();
                //stress();
                //benchmark();
                //testBuffer();
                //train2();
                //XORNEAT();
                testFlatten();
            }
            //catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Done");

        }
    }
}
