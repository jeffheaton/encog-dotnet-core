using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Sort
{
    public class RowComparitor: IComparer<LoadedRow>
    {
        private SortCSV sort;

        public RowComparitor(SortCSV sort)
        {
            this.sort = sort;
        }

        public int Compare(LoadedRow x, LoadedRow y)
        {
            foreach (SortedField t in this.sort.SortOrder)
            {
                int index = t.Index;
                String xStr = x.Data[index];
                String yStr = y.Data[index];

                switch (t.SortType)
                {
                    case SortType.SortDecimal:
                        double xDouble = this.sort.Format.Parse(xStr);
                        double yDouble = this.sort.Format.Parse(yStr);
                        int c = xDouble.CompareTo(yDouble);
                        if (c != 0)
                        {
                            return c;
                        }
                        break;
                    case SortType.SortInteger:
                        int xInteger = int.Parse(xStr);
                        int yInteger = int.Parse(yStr);
                        int c2 = xInteger.CompareTo(yInteger);
                        if (c2 != 0)
                        {
                            return c2;
                        }
                        break;
                    case SortType.SortString:
                        int c3 = xStr.CompareTo(yStr);
                        if (c3 != 0)
                        {
                            return c3;
                        }
                        break;
                }
            }

            // failing all of this, they are equal
            return 0;
        }
    }
}
