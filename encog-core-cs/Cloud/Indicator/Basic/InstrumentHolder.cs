using System;
using System.Collections.Generic;
using System.Linq;
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
        private IDictionary<long, string> data = new Dictionary<long, string>();

        /// <summary>
        /// The sorted data.
        /// </summary>
        private ICollection<long> sorted = new SortedSet<long>();

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
            bool result;
            StringBuilder str = new StringBuilder();

            for (int i = starting; i < data.Length; i++)
            {
                if (i > starting)
                {
                    str.Append(',');
                }
                str.Append(data[i]);
            }

            result = !this.data.ContainsKey(when);
            this.sorted.Add(when);
            this.data[when] = str.ToString();
            return result;
        }

        /// <summary>
        /// The data.
        /// </summary>
        public IDictionary<long, string> Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Sorted keys.
        /// </summary>
        public ICollection<long> Sorted
        {
            get
            {
                return sorted;
            }
        }
    }
}
