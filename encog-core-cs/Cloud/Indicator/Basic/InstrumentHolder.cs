using System;
using System.Collections.Generic;
using System.Text;

namespace Encog.Cloud.Indicator.Basic
{
    /// <summary>
    /// Used to hold instruments, i.e. ticker symbols of securities.
    /// Also holds financial data downloaded by ticker symbol.
    /// </summary>
    public class InstrumentHolder
    {
        /// <summary>
        /// The downloaded financial data.
        /// </summary>
        private readonly IDictionary<long, string> _data = new Dictionary<long, string>();

        /// <summary>
        /// The sorted data.
        /// </summary>
        private readonly ICollection<long> _sorted = new SortedSet<long>();

        /// <summary>
        /// The data.
        /// </summary>
        public IDictionary<long, string> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Sorted keys.
        /// </summary>
        public ICollection<long> Sorted
        {
            get { return _sorted; }
        }

        /// <summary>
        /// Record one piece of data. Data with the same time stamp.
        /// </summary>
        /// <param name="when">The time the data occurred.</param>
        /// <param name="starting">Where should we start from when storing, index into data.
        /// Allows unimportant "leading data" to be discarded without creating a new
        /// array.</param>
        /// <param name="data">The financial data.</param>
        /// <returns>True, if the data did not exist already.</returns>
        public bool Record(long when, int starting, String[] data)
        {
            var str = new StringBuilder();

            for (int i = starting; i < data.Length; i++)
            {
                if (i > starting)
                {
                    str.Append(',');
                }
                str.Append(data[i]);
            }

            bool result = !_data.ContainsKey(when);
            _sorted.Add(when);
            this._data[when] = str.ToString();
            return result;
        }
    }
}