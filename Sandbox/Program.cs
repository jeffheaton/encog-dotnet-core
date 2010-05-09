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

        private static string kernelSource = @"
kernel void SingleNetworkCalculate(
    global read_only int inputSize,
    global read_only int outputSize,
    global read_only int layerCount,
    global read_only int neuronCount,
    global read_only int *layerIndex,
    global read_only int *layerCounts,
    global read_only int *weightIndex,
    global read_only float* input,
    global read_only float* weights,
    global write_only float *layerOutput
    )
{
    int sourceIndex = neuronCount - inputSize;

    for(int i=0;i<inputSize;i++)
        layerOutput[sourceIndex+i] = input[i];

    for (int currentLayer = layerCount - 1; currentLayer > 0; currentLayer--)
    {
      int inputIndex = layerIndex[currentLayer];
      int outputIndex = layerIndex[currentLayer - 1];
      int inputSize = layerCounts[currentLayer];
      int outputSize = layerCounts[currentLayer - 1];
      int index = weightIndex[currentLayer - 1];

      for (int i = 0; i < outputSize; i++)
      {
        layerOutput[i + outputIndex] = weights[index++];
      }

      for (int x = 0; x < outputSize; x++)
      {
        float sum = 0;
        for (int y = 0; y < inputSize; y++)
        {
          float value = layerOutput[inputIndex + y];
          value = -1 + (2 / (1 + exp(-2 * value)));
          sum += weights[index++] * value;
        }
        
        layerOutput[outputIndex + x] += sum;

        layerOutput[outputIndex + x] = layerOutput[outputIndex + x];
        
      }
    }
}
";

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

        public static void stress()
        {
            INeuralDataSet trainingData = RandomTrainingFactory.Generate(100000, 100, 50, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(trainingData.InputSize, 50, 50, trainingData.IdealSize, true);
            EncogUtility.TrainDialog(network, trainingData);
            //EncogUtility.TrainConsole(network, trainingData, 10);
        }

        public static void CalculateGPU(FlatNetwork flat, double[] input, double[] output)
        {
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, IntPtr.Zero);

            float[] inputArray = new float[flat.InputCount];
            float[] outputArray = new float[flat.OutputCount];
            float[] weightArray = new float[flat.Weights.Length];

            Random rand = new Random();

            for (int i = 0; i < inputArray.Length; i++)
                inputArray[i] = (float)input[i];
            for (int i = 0; i < flat.Weights.Length; i++)
                weightArray[i] = (float)flat.Weights[i];

            ComputeBuffer<float> a = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, inputArray);
            ComputeBuffer<float> b = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            ComputeBuffer<float> c = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, flat.LayerOutput.Length);

            ComputeBuffer<int> d = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            ComputeBuffer<int> e = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            ComputeBuffer<int> f = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

            ComputeProgram program = new ComputeProgram(context, new string[] { kernelSource });
            program.Build(null, null, null, IntPtr.Zero);

            ComputeKernel kernel = program.CreateKernel("SingleNetworkCalculate");

            kernel.SetValueArgument<int>(0, flat.InputCount);
            kernel.SetValueArgument<int>(1, flat.OutputCount);
            kernel.SetValueArgument<int>(2, flat.LayerCounts.Length);
            kernel.SetValueArgument<int>(3, flat.LayerOutput.Length);

            kernel.SetMemoryArgument(4, d);
            kernel.SetMemoryArgument(5, e);
            kernel.SetMemoryArgument(6, f);

            kernel.SetMemoryArgument(7, a);
            kernel.SetMemoryArgument(8, b);
            kernel.SetMemoryArgument(9, c);

            ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

            ComputeEventList events = new ComputeEventList();

            commands.Execute(kernel, null, new long[] { 1 }, null, events);

            outputArray = commands.Read(c, true, 0, flat.LayerOutput.Length, events);

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = outputArray[i];
            }
        }

        public static void linear()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, true);
            FlatNetwork flat = new FlatNetwork(network);
            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);
            TrainFlatNetwork train = new TrainFlatNetwork(flat, training);
            for (int i = 0; i < 100; i++)
            {
                train.Iteration();
                Console.WriteLine(train.Error);
            }

            /*ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, IntPtr.Zero);

            KernelSingleNetworkCalculate k = new KernelSingleNetworkCalculate(context, "Encog.Resources.KernelSingleNetCalculate.txt");
            k.compile();*/

            Encog.Encog.Instance.InitGPU();

            Console.WriteLine( "Error1:" + flat.CalculateError(training) );
            Console.WriteLine( "Error2:" + flat.CalculateErrorGPU(training) );

            Console.WriteLine("Done");
        }


        public static void benchmark()
        {
            INeuralDataSet training = RandomTrainingFactory.Generate(50000, 10, 10, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(10, 6, 0, 10, true);
            network.Reset();
            FlatNetwork flat = new FlatNetwork(network);
            Encog.Encog.Instance.InitGPU();
            long start;
            long stop;

            start = Environment.TickCount;
            double error = network.CalculateError(training);
            stop = Environment.TickCount;
            Console.WriteLine("Error1:" + error + ",Time:" + (stop-start) );

            start = Environment.TickCount;
            error = flat.CalculateError(training);
            stop = Environment.TickCount;
            Console.WriteLine("Error2:" + error + ",Time:" + (stop - start));
            
            start = Environment.TickCount;
            flat.CalculateErrorGPU(training);
            stop = Environment.TickCount;
            Console.WriteLine("Error3:" + error + ",Time:" + (stop - start));

            Console.WriteLine("Done");
        }

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

        public static void train()
        {
            BasicNetwork network = EncogUtility.SimpleFeedForward(2, 3, 0, 1, false);
            FlatNetwork flat = new FlatNetwork(network);
            BasicNeuralDataSet training = new BasicNeuralDataSet(XOR_INPUT, XOR_IDEAL);
            Encog.Encog.Instance.InitGPU();
            TrainFlatNetworkMulti train = new TrainFlatNetworkMulti(flat, training);
            for (int i = 0; i < 50; i++)
            {
                train.Iteration();
                Console.WriteLine("Train error: " + train.Error);
                Console.WriteLine("Netwk error: " + flat.CalculateError(training));
            }

            train.Iteration();
            flat.CalculateError(training);


            Console.WriteLine("Done");
        }

        public static void train2()
        {
            int outputSize = 2;
            int inputSize = 10;
            int trainingSize = 100;

            INeuralDataSet training = RandomTrainingFactory.Generate(
                trainingSize, inputSize, outputSize, -1, 1);
            BasicNetwork network = EncogUtility.SimpleFeedForward(
                training.InputSize, 6, 0, training.IdealSize, true);
            network.Reset();
            FlatNetwork flat = new FlatNetwork(network);
            //Encog.Encog.Instance.InitGPU();

            long start = Environment.TickCount;
            TrainFlatNetworkMulti train = new TrainFlatNetworkMulti(flat, training);
            train.NumThreads = 1;
            for (int i = 0; i < 50; i++)
            {
                train.Iteration();
                Console.WriteLine("Train error: " + train.Error);
                Console.WriteLine("Netwk error: " + flat.CalculateError(training));
            }
            long stop = Environment.TickCount;

            Console.WriteLine("Done:" + (stop-start));
            Console.WriteLine("Stop");
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

        static void Main(string[] args)
        {
            //try
            {
                ///testCL();
                //stress();
                //benchmark();
                //testBuffer();
                train2();
                //XORNEAT();
                //simple();
            }
            //catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Done");

        }
    }
}
