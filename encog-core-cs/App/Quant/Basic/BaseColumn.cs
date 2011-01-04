using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Basic
{
    public class BaseColumn
    {
        private double[] data;

        public String Name { get; set; }
        public double[] Data { get { return data; } }

        public BaseColumn(String name, bool input, bool output)
        {
            this.Name = name;
            this.Input = input;
            this.Output = output;
            this.Ignore = false;
        }

        public bool Output { get; set; }
        public bool Input { get; set; }
        public bool Ignore { get; set; }

        public void Allocate(int length)
        {
            this.data = new double[length];
        }
    }
}
