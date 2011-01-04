using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators
{
    public abstract class Indicator: BaseColumn
    {
        public Indicator(String name, bool input, bool output)
            :base(name,input,output)
        {
        }

        public void Require(IDictionary<string, BaseColumn> data, String item)
        {
            if (!data.ContainsKey(item))
            {
                throw new QuantError("To use this indicator, the underlying data must contain: " + item);
            }
        }

        public abstract int Periods { get; }

        public int BeginningIndex { get; set; }
        public int EndingIndex { get; set; }

        public abstract void Calculate(IDictionary<String,BaseColumn> data, int length);
    }
}
