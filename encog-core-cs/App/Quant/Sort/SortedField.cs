using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Sort
{
    public class SortedField
    {
        public int Index { get; set; }
        public bool Ascending { get; set; }
        public SortType SortType { get; set; }

        public SortedField(int index, SortType t, bool ascending)
        {
            Index = index;
            Ascending = ascending;
            SortType = t;
        }
    }
}
