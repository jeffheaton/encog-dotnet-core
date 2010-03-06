using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
