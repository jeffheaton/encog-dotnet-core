using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using System.Runtime.InteropServices;

namespace Encog.Util.CL.Kernels
{
    public class KernelVectorAdd : EncogKernel
    {
        private ComputeKernel kernel;
        private ComputeCommandQueue commands;

        public KernelVectorAdd(ComputeContext context)
            : base(context, "Encog.Resources.KernelVectorAdd.txt")
        {
        }

        public double[] Add(double[] inputA, double[] inputB)
        {
            int count = inputA.Length;
            float[] arrA = new float[count];
            float[] arrB = new float[count];

            this.kernel = Program.CreateKernel("VectorAdd");
            this.commands = new ComputeCommandQueue(Context, Context.Devices[0], ComputeCommandQueueFlags.None);

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                arrA[i] = (float)inputA[i];
                arrB[i] = (float)inputB[i];
            }

            ComputeBuffer<float> a = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, arrA);
            ComputeBuffer<float> b = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, arrB);
            ComputeBuffer<float> c = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, arrA.Length);

            kernel.SetMemoryArgument(0, a);
            kernel.SetMemoryArgument(1, b);
            kernel.SetMemoryArgument(2, c);

            this.commands = new ComputeCommandQueue(Context, Context.Devices[0], ComputeCommandQueueFlags.None);

            ComputeEventList events = new ComputeEventList();

            commands.Execute(kernel, null, new long[] { count }, null, events);


            float[] arrC = commands.Read(c, false, 0, inputA.Length, events);
            commands.Finish();

            double[] result = new double[arrA.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = arrC[i];

            return result;
        }
    }
}
