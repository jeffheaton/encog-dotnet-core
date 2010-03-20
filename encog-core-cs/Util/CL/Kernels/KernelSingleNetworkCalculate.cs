using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Persist.Location;
using Encog.Neural.Networks.Flat;

namespace Encog.Util.CL.Kernels
{
    public class KernelSingleNetworkCalculate
    {
        private String cl;
        private ComputeContext context;
        private ComputeProgram program;

        public KernelSingleNetworkCalculate(ComputeContext context, String sourceName)
        {
            ResourcePersistence resource = new ResourcePersistence(sourceName);
            this.context = context;
            this.cl = resource.LoadString();
        }

        public void compile()
        {
            this.program = new ComputeProgram(this.context, new string[] { this.cl });
            program.Build(null, null, null, IntPtr.Zero);
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

            ComputeBuffer<float> a = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, inputArray);
            ComputeBuffer<float> b = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, weightArray);
            ComputeBuffer<float> c = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, flat.LayerOutput.Length);

            ComputeBuffer<int> d = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerIndex);
            ComputeBuffer<int> e = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.LayerCounts);
            ComputeBuffer<int> f = new ComputeBuffer<int>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, flat.WeightIndex);

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
    }
}
