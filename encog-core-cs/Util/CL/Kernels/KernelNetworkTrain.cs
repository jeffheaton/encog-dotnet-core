using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Neural.Networks.Flat;
using Encog.Neural;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;

namespace Encog.Util.CL.Kernels
{
    public class KernelNetworkTrain : EncogKernel
    {

        private float[] outputArray;

        public KernelNetworkTrain(ComputeContext context)
            : base(context, "Encog.Resources.KernelNetTrain.txt")
        {
        }

        public void Train(FlatNetwork flat, INeuralDataSet input, int high, int low)
        {
            if (!(input is IIndexable))
            {
                throw new NeuralNetworkError("Neural network input must support IIndexable");
            }

            IIndexable indexable = (IIndexable)input;

            double[][] result = EncogArray.AllocateDouble2D((int)indexable.Count, (int)flat.OutputCount);

            float[] inputArray = new float[indexable.Count * flat.InputCount];
            float[] idealArray = new float[indexable.Count * flat.OutputCount];

            int inputIndex = 0;
            int idealIndex = 0;

            foreach (INeuralDataPair pair in input)
            {
                for (int col = 0; col < flat.InputCount; col++)
                {
                    inputArray[inputIndex++] = (float)pair.Input.Data[col];
                }

                for (int col = 0; col < flat.OutputCount; col++)
                {
                    idealArray[idealIndex++] = (float)pair.Ideal.Data[col];
                }
            }

            Calculate(flat, (int)indexable.Count , inputArray, idealArray);
        }


        public double Calculate(FlatNetwork flat, int length, float[] input, float[] idealArray)
        {
            int totalOutputLength = flat.LayerOutput.Length * length;
            float[] weightArray = new float[flat.Weights.Length];

            for (int i = 0; i < flat.Weights.Length; i++)
                weightArray[i] = (float)flat.Weights[i];

            ComputeBuffer<float> a = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, input);
            ComputeBuffer<float> b = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            ComputeBuffer<float> c = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, totalOutputLength);

            ComputeBuffer<int> d = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            ComputeBuffer<int> e = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            ComputeBuffer<int> f = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

            ComputeBuffer<float> gradientBuffer = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, 1);

            ComputeKernel kernel = Program.CreateKernel("NetworkTrain");

            kernel.SetValueArgument<int>(0, flat.InputCount);
            kernel.SetValueArgument<int>(1, flat.OutputCount);
            kernel.SetValueArgument<int>(2, flat.LayerCounts.Length);
            kernel.SetValueArgument<int>(3, flat.LayerOutput.Length);
            kernel.SetMemoryArgument(4, gradientBuffer);

            kernel.SetMemoryArgument(5, d);
            kernel.SetMemoryArgument(6, e);
            kernel.SetMemoryArgument(7, f);

            kernel.SetMemoryArgument(8, a);
            kernel.SetMemoryArgument(9, b);
            kernel.SetMemoryArgument(10, c);

            ComputeCommandQueue commands = new ComputeCommandQueue(Context, Context.Devices[0], ComputeCommandQueueFlags.None);

            ComputeEventList events = new ComputeEventList();

            commands.Execute(kernel, null, new long[] { length }, null, events);
  
            outputArray = commands.Read(gradientBuffer, true, 0, 1, events);
            return outputArray[0];
        }
    }
}
