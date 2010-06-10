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
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Training.Propagation;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Training.Propagation.Manhattan;

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

            EncogUtility.TrainToError(train,trainingSet,0.01);

            BasicNetwork network = train.Network;
            network.ClearContext();
            EncogUtility.Evaluate(network, trainingSet);

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

        public static void benchmarkCL()
        {
            int outputSize = 1;
            int inputSize = 10;
            int trainingSize = 10000;

            INeuralDataSet training = RandomTrainingFactory.Generate(
                trainingSize, inputSize, outputSize, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                training.InputSize, 6, 0, training.IdealSize, true);
            network.Reset();
            
            Encog.Encog.Instance.InitCL();

            ResilientPropagation train = new ResilientPropagation(network, training);
            //Propagation train = new Backpropagation(network, training, 0.000007, 0.0);
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
            Console.WriteLine("Done:" + (stop-start));
            Console.WriteLine("Stop");
            if( Encog.Encog.Instance.CL!=null )
                Console.WriteLine(Encog.Encog.Instance.CL.ToString());
        }

        public static void testSimpleCL()
        {
            try
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
            catch (EncogCLError ex)
            {
                Console.WriteLine("Can't startup CL, make sure you have drivers loaded.");
                Console.WriteLine(ex.ToString());
            }
        }

        static void testFlatten()
        {
            //Encog.Encog.Instance.InitCL();
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, true);
            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);
            ResilientPropagation train = new ResilientPropagation(network, training);
            train.AttemptFlatten = true;

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
                //benchmarkCL();
                //testSimpleCL();
                //XORNEAT();
                testFlatten();
                //test();
                //test();
            }
            //catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Done");

        }
    }
}
