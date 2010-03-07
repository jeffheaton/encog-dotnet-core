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
            EncogUtility.TrainConsole(network, trainingData, 10);
        }

        static void Main(string[] args)
        {
            try
            {
                //testCL();
                stress();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
