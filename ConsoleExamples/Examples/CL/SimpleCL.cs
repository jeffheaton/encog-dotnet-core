using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleExamples.Examples;
using Encog.Util.CL;
using Encog.Util.CL.Kernels;

namespace Encog.Examples.CL
{
    public class SimpleCL: IExample
    {
        public static ExampleInfo Info
        {
            get
            {
                ExampleInfo info = new ExampleInfo(
                    typeof(SimpleCL),
                    "opencl-simple",
                    "Simple OpenCL example.",
                    "Simple OpenCL example that does a vector add.");
                return info;
            }
        }


        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Not used.</param>
        public void Execute(IExampleInterface app)
        {
            try
            {
                Encog.Instance.InitCL();

                EncogCLDevice device = Encog.Instance.CL.Devices[0];
                KernelVectorAdd k = Encog.Instance.CL.Devices[0].Platform.VectorAdd;
                k.Compile();

                double[] a = { 1, 2, 3, 4 };
                double[] b = { 5, 6, 7, 8 };
                double[] c = k.Add(device, a, b);

                for (int i = 0; i < a.Length; i++)
                {
                    Console.WriteLine(a[i] + " + " + b[i] + " = " + c[i]);
                }
            }
            catch (EncogCLError ex)
            {
                Console.WriteLine("Can't startup CL, make sure you have drivers loaded.");
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
