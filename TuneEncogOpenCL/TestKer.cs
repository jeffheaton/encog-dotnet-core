using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;

namespace TuneEncogOpenCL
{
    public class TestKer
    {
        private static string kernelSource = @"
kernel void VectorAdd(
    global read_only float* a,
    global read_only float* b,
    global write_only float* c )
{
    int index = get_global_id(0);
    c[index] = a[index] + b[index];
}
";

        public static void test()
        {
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, IntPtr.Zero);

            int count = 10;
            float[] arrA = new float[count];
            float[] arrB = new float[count];
            float[] arrC = new float[count];

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                arrA[i] = (float)(rand.NextDouble() * 100);
                arrB[i] = (float)(rand.NextDouble() * 100);
            }

            ComputeBuffer<float> a = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, arrA);
            ComputeBuffer<float> b = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.CopyHostPointer, arrB);
            ComputeBuffer<float> c = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, arrC.Length);

            ComputeProgram program = new ComputeProgram(context, new string[] { kernelSource });
            program.Build(null, null, null, IntPtr.Zero);

            ComputeKernel kernel = program.CreateKernel("VectorAdd");
            kernel.SetMemoryArgument(0, a);
            kernel.SetMemoryArgument(1, b);
            kernel.SetMemoryArgument(2, c);

            ComputeCommandQueue commands = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);

            ComputeEventList events = new ComputeEventList();

            commands.Execute(kernel, null, new long[] { count }, null, events);

            arrC = commands.Read(c, true, 0, count, events);

            for (int i = 0; i < count; i++)
                Console.WriteLine("{0} + {1} = {2}", arrA[i], arrB[i], arrC[i]);
        } 
    }
}
