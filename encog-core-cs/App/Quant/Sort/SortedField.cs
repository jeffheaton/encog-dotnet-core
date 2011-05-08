using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Sort
{
    /// <summary>
    /// Specifies how a field is to be sorted by SortCSV.
    /// </summary>
    public class SortedField
    {
        /// <summary>
        /// The index of the field.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// True, if the field is to be sorted ascending.
        /// </summary>
        public bool Ascending { get; set; }

        /// <summary>
        /// The type of sort.
        /// </summary>
        public SortType SortType { get; set; }

        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="index">The index of the sorted field.</param>
        /// <param name="t">The type of sort, the type of object.</param>
        /// <param name="ascending">True, if this is an ascending sort.</param>
        public SortedField(int index, SortType t, bool ascending)
        {
            Index = index;
            Ascending = ascending;
            SortType = t;
        }
    }
}
