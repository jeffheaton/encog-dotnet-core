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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Training.Genetic;
using Encog.MathUtil.Randomize;

namespace Sandbox
{
    class Program
    {
        public static void testCL()
        {
            Encog.Encog e = Encog.Encog.Instance;
            e.InitGPU();
            EncogKernel k = new EncogKernel(e.GPU, "Encog.Resources.KernelVectorAdd.txt");
            e.GPU.Compile(k);
            CLPayload payload = new CLPayload();
            EncogCLVector a = payload.CreateInputVector(10);
            EncogCLVector b = payload.CreateInputVector(10);
            EncogCLVector c = payload.CreateOutputVector(10);
            a.Array[0] = 5;
            b.Array[0] = 6;
            k.Execute(e.GPU.Adapters[0], payload);
            Console.WriteLine(c.Array[0]);
        }

        public static void stress()
        {
            INeuralDataSet trainingData = RandomTrainingFactory.Generate(100000, 100, 50, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(trainingData.InputSize, 50, 50, trainingData.IdealSize, true);
            EncogUtility.TrainDialog(network, trainingData);
            //EncogUtility.TrainConsole(network, trainingData, 10);
        }

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


        public static void genetic()
        {
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(2));
            network.AddLayer(new BasicLayer(3));
            network.AddLayer(new BasicLayer(1));
            network.Structure.FinalizeStructure();
            network.Reset();

            INeuralDataSet trainingSet = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);

            ICalculateScore score = new TrainingSetScore(trainingSet);
            // train the neural network
            ITrain train = new NeuralGeneticAlgorithm(
                network, new RangeRandomizer(-1, 1), score, 5000, 0.1, 0.25);

            int epoch = 1;

            do
            {
                train.Iteration();
                Console.WriteLine("Epoch #" + epoch + " Error:" + train.Error);
                epoch++;
            } while ((epoch < 5000) && (train.Error > 0.001));

        }

        static void Main(string[] args)
        {
            //try
            {
                //testCL();
                //stress();
                genetic();
            }
            /*catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }*/

        }
    }
}
