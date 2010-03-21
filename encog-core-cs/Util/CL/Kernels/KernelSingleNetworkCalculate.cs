using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Persist.Location;
using Encog.Neural.Networks.Flat;

namespace Encog.Util.CL.Kernels
{
    public class KernelSingleNetworkCalculate: EncogKernel
    {
        public KernelSingleNetworkCalculate(ComputeContext context)
            : base(context, "Encog.Resources.KernelSingleNetCalculate.txt")
        {
        }

        public void Calculate(FlatNetwork flat, double[] input, double[] output)
        {
            float[] inputArray = new float[flat.InputCount];
            float[] outputArray = new float[flat.OutputCount];
            float[] weightArray = new float[flat.Weights.Length];

            Random rand = new Random();

            for (int i = 0; i < inputArray.Length; i++)
                inputArray[i] = (float)input[i];
            for (int i = 0; i < flat.Weights.Length; i++)
                weightArray[i] = (float)flat.Weights[i];

            ComputeBuffer<float> a = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, inputArray);
            ComputeBuffer<float> b = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            ComputeBuffer<float> c = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, flat.LayerOutput.Length);

            ComputeBuffer<int> d = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            ComputeBuffer<int> e = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            ComputeBuffer<int> f = new ComputeBuffer<int>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

            ComputeKernel kernel = Program.CreateKernel("SingleNetworkCalculate");

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

            commands.Execute(kernel, null, new long[] { 1 }, null, events);

            outputArray = commands.Read(c, true, 0, flat.LayerOutput.Length, events);

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = outputArray[i];
            }
        }
    }
}
