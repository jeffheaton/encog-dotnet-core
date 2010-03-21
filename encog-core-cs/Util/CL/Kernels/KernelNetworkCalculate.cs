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
    public class KernelNetworkCalculate: EncogKernel
    {
        private float[] inputArray;
        private float[] outputArray;

        public KernelNetworkCalculate(ComputeContext context)
            : base(context, "Encog.Resources.KernelNetCalculate.txt")
        {
        }

        private void BuildInputArray(double[][] input)
        {
            inputArray = new float[input.Length * input[0].Length];
            int index = 0;
            for (int row = 0; row < input.Length; row++)
            {
                for (int col = 0; col < input[0].Length; col++)
                {
                    inputArray[index++] = (float)input[row][col];
                }
            }
        }

        public double[][] Calculate(FlatNetwork flat, INeuralDataSet input)
        {
            if (!(input is IIndexable))
            {
                throw new NeuralNetworkError("Neural network input must support IIndexable");
            }

            IIndexable indexable = (IIndexable)input;

            double[][] result = EncogArray.AllocateDouble2D((int)indexable.Count,(int)flat.OutputCount);

            return result;
        }

        public void Calculate(FlatNetwork flat, double[][] input, double[][] output)
        {
            if (input.Length != output.Length)
                throw new NeuralNetworkError("The input and output row count must match");

            if (input[0].Length != flat.InputCount)
                throw new NeuralNetworkError("The input column count must match the network input neuron count");

            if (output[0].Length != flat.OutputCount)
                throw new NeuralNetworkError("The output column count must match the network output neuron count");


            float[] outputArray = new float[flat.OutputCount*input.Length];
            float[] weightArray = new float[flat.Weights.Length];

            Random rand = new Random();
            
            for (int i = 0; i < flat.Weights.Length; i++)
                weightArray[i] = (float)flat.Weights[i];

            ComputeBuffer<float> a = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, inputArray);
            ComputeBuffer<float> b = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            ComputeBuffer<float> c = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, flat.LayerOutput.Length);

            ComputeBuffer<int> d = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            ComputeBuffer<int> e = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            ComputeBuffer<int> f = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

            ComputeKernel kernel = Program.CreateKernel("NetworkCalculate");

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

            ComputeCommandQueue commands = new ComputeCommandQueue(Context, Context.Devices[0], ComputeCommandQueueFlags.None);

            ComputeEventList events = new ComputeEventList();

            commands.Execute(kernel, null, new long[] { input.Length }, null, events);

            outputArray = commands.Read(c, true, 0, flat.LayerOutput.Length, events);            
        }
    }
}
