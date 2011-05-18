using System;
using System.Collections.Generic;
using Encog.App.Analyst.CSV.Basic;
using Encog.App.Quant;

namespace Encog.App.Analyst.CSV.Sort
{
    /// <summary>
    /// Used to compare two LoadedRow objects.
    /// </summary>
    ///
    public class RowComparator : IComparer<LoadedRow>
    {
        /// <summary>
        /// The owner object.
        /// </summary>
        ///
        private readonly SortCSV sort;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="owner">The owner.</param>
        public RowComparator(SortCSV owner)
        {
            sort = owner;
        }

        #region IComparer<LoadedRow> Members

        /// <summary>
        /// Compare two LoadedRow objects.
        /// </summary>
        ///
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>0 if the same, &lt;0 x is less, &gt;0 y is less.</returns>
        public int Compare(LoadedRow x, LoadedRow y)
        {
            foreach (SortedField t  in  sort.SortOrder)
            {
                int index = t.Index;
                String xStr = x.Data[index];
                String yStr = y.Data[index];

                switch (t.SortType)
                {
                    case SortType.SortDecimal:
                        double xDouble = sort.InputFormat.Parse(xStr);
                        double yDouble = sort.InputFormat.Parse(yStr);
                        int c = xDouble.CompareTo(yDouble);
                        if (c != 0)
                        {
                            return c;
                        }
                        break;
                    case SortType.SortInteger:
                        int xInteger = Int32.Parse(xStr);
                        int yInteger = Int32.Parse(yStr);
                        int c2 = xInteger - yInteger;
                        if (c2 != 0)
                        {
                            return c2;
                        }
                        break;
                    case SortType.SortString:
                        int c3 = String.CompareOrdinal(xStr, yStr);
                        if (c3 != 0)
                        {
                            return c3;
                        }
                        break;
                    default:
                        throw new QuantError("Unknown sort method: " + t.SortType);
                }
            }

            // failing all of this, they are equal
            return 0;
        }

        #endregion
    }
}