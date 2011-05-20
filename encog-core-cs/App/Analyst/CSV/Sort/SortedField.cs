using System;
using System.Text;

namespace Encog.App.Analyst.CSV.Sort
{
    /// <summary>
    /// Specifies how a field is to be sorted by SortCSV.
    /// </summary>
    ///
    public class SortedField
    {
        /// <summary>
        /// The index of the field.
        /// </summary>
        ///
        private int _index;

        /// <summary>
        /// The type of sort.
        /// </summary>
        ///
        private SortType _sortType;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="theIndexindex">The index of the sorted field.</param>
        /// <param name="t">The type of sort, the type of object.</param>
        /// <param name="theAscending">True, if this is an ascending sort.</param>
        public SortedField(int theIndexindex, SortType t,
                           bool theAscending)
        {
            _index = theIndexindex;
            Ascending = theAscending;
            _sortType = t;
        }


        /// <value>the index to set</value>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        /// <value>the sortType to set</value>
        public SortType SortType
        {
            get { return _sortType; }
            set { _sortType = value; }
        }


        /// <value>the ascending to set</value>
        public bool Ascending { get; set; }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" index=");
            result.Append(_index);
            result.Append(", type=");
            result.Append(_sortType);

            result.Append("]");
            return result.ToString();
        }
    }
}