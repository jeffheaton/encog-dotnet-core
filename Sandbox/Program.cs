using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.CL;
using Encog.Util;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
