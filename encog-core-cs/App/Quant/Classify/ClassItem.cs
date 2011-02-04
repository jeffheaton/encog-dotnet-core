using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Classify
{
    public class ClassItem
    {
        public String Name { get; set; }
        public int Index { get; set; }

        public ClassItem(String name, int index)
        {
            Name = name;
            Index = index;
        }
    }
}
