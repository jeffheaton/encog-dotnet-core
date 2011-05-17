using System;
using System.Collections.Generic;
using Encog.App.Analyst.CSV.Basic;

namespace Encog.App.Quant.Indicators
{
    /// <summary>
    /// An indicator, used by Encog.
    /// </summary>
    ///
    public abstract class Indicator : BaseCachedColumn
    {
        /// <summary>
        /// Construct the indicator.
        /// </summary>
        ///
        /// <param name="name">The indicator name.</param>
        /// <param name="input">Is this indicator used to predict?</param>
        /// <param name="output">Is this indicator what we are trying to predict.</param>
        public Indicator(String name, bool input,
                         bool output) : base(name, input, output)
        {
        }


        /// <value>the beginningIndex to set</value>
        public int BeginningIndex { /// <returns>the beginningIndex</returns>
            get; /// <param name="theBeginningIndex">the beginningIndex to set</param>
            set; }


        /// <value>the endingIndex to set.</value>
        public int EndingIndex { /// <returns>the endingIndex</returns>
            get; /// <param name="theEndingIndex">the endingIndex to set.</param>
            set; }


        /// <value>The number of periods this indicator is for.</value>
        public abstract int Periods { /// <returns>The number of periods this indicator is for.</returns>
            get; }

        /// <summary>
        /// Calculate this indicator.
        /// </summary>
        ///
        /// <param name="data">The data available to this indicator.</param>
        /// <param name="length">The length of data to use.</param>
        public abstract void Calculate(IDictionary<String, BaseCachedColumn> data,
                                       int length);


        /// <summary>
        /// Require a specific type of underlying data.
        /// </summary>
        ///
        /// <param name="theData">The data available.</param>
        /// <param name="item">The type of data we are looking for.</param>
        public void Require(IDictionary<String, BaseCachedColumn> theData,
                            String item)
        {
            if (!theData.ContainsKey(item))
            {
                throw new QuantError(
                    "To use this indicator, the underlying data must contain: "
                    + item);
            }
        }
    }
}