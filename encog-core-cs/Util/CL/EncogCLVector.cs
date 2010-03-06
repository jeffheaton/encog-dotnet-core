using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;

namespace Encog.Util.CL
{
    public class EncogCLVector
    {
        public bool IsOutput { get; set; }
        public float[] Array { get; set; }
        public ComputeBuffer<float> Buffer { get; set; }

        public EncogCLVector(int length, bool output)
        {
            IsOutput = output;
            Array = new float[length];
        }
    }
}
