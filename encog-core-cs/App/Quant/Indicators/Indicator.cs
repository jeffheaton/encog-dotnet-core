using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators
{

    /// <summary>
    /// An indicator, used by Encog.
    /// </summary>
    public abstract class Indicator : BaseCachedColumn
    {
        /// <summary>
        /// Construct the indicator.
        /// </summary>
        /// <param name="name">The indicator name.</param>
        /// <param name="input">Is this indicator used to predict?</param>
        /// <param name="output">Is this indicator what we are trying to predict.</param>
        public Indicator(String name, bool input, bool output)
            :base(name,input,output)
        {
        }

        /// <summary>
        /// Require a specific type of underlying data.
        /// </summary>
        /// <param name="data">The data available.</param>
        /// <param name="item">The type of data we are looking for.</param>
        public void Require(IDictionary<string, BaseCachedColumn> data, String item)
        {
            if (!data.ContainsKey(item))
            {
                throw new QuantError("To use this indicator, the underlying data must contain: " + item);
            }
        }

        /// <summary>
        /// The number of periods this indicator is for.
        /// </summary>
        public abstract int Periods { get; }

        /// <summary>
        /// The beginning index.
        /// </summary>
        public int BeginningIndex { get; set; }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int EndingIndex { get; set; }

        /// <summary>
        /// Calculate this indicator.
        /// </summary>
        /// <param name="data">The data available to this indicator.</param>
        /// <param name="length">The length of data to use.</param>
        public abstract void Calculate(IDictionary<String, BaseCachedColumn> data, int length);
    }
}
