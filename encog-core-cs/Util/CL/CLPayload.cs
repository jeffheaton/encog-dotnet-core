using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.CL
{
    public class CLPayload
    {
        private List<EncogCLVector> input = new List<EncogCLVector>();
        private List<EncogCLVector> output = new List<EncogCLVector>();

        public List<EncogCLVector> InputParams
        {
            get
            {
                return input;
            }
        }

        public List<EncogCLVector> OutputParams
        {
            get
            {
                return output;
            }
        }

        public EncogCLVector CreateInputVector(int length)
        {
            EncogCLVector result = new EncogCLVector(length,false);
            input.Add(result);
            return result;
        }

        public EncogCLVector CreateOutputVector(int length)
        {
            EncogCLVector result = new EncogCLVector(length,true);
            output.Add(result);
            return result;
        }
    }
}
