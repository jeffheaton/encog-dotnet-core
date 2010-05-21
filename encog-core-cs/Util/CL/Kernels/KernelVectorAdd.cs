using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using System.Runtime.InteropServices;

namespace Encog.Util.CL.Kernels
{
    /// <summary>
    /// A very simple kernel, used to add a vector.  Not actually used by Encog, 
    /// it is a simple test case to verify that OpenCL is working.
    /// </summary>
    public class KernelVectorAdd : EncogKernel
    {
        /// <summary>
        /// Construct a simple kernal to add two vectors.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public KernelVectorAdd(ComputeContext context)
            : base(context, "Encog.Resources.KernelVectorAdd.txt", "VectorAdd")
        {
        }

        /// <summary>
        /// Perform the addition.
        /// </summary>
        /// <param name="device">The OpenCL device to use.</param>
        /// <param name="inputA">The first vector to add.</param>
        /// <param name="inputB">The second vector to add.</param>
        /// <returns>The result of the addition.</returns>
        public double[] Add(EncogCLDevice device, double[] inputA, double[] inputB)
        {
            PrepareKernel();

            int count = inputA.Length;
            float[] arrA = new float[count];
            float[] arrB = new float[count];

            for (int i = 0; i < count; i++)
            {
                arrA[i] = (float)inputA[i];
                arrB[i] = (float)inputB[i];
            }

            ComputeBuffer<float> a = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, arrA);
            ComputeBuffer<float> b = new ComputeBuffer<float>(Context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, arrB);
            ComputeBuffer<float> c = new ComputeBuffer<float>(Context, ComputeMemoryFlags.WriteOnly, arrA.Length);

            Kernel.SetMemoryArgument(0, a);
            Kernel.SetMemoryArgument(1, b);
            Kernel.SetMemoryArgument(2, c);

            ComputeEventList events = new ComputeEventList();

            device.Commands.Execute(Kernel, null, new long[] { count }, null, events);


            float[] arrC = device.Commands.Read(c, false, 0, inputA.Length, events);
            device.Commands.Finish();

            double[] result = new double[arrA.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = arrC[i];

            return result;
        }
    }
}
