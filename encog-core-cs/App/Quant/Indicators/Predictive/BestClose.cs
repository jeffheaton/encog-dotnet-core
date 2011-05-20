using System;
using System.Collections.Generic;
using Encog.App.Analyst.CSV.Basic;

namespace Encog.App.Quant.Indicators.Predictive
{
    /// <summary>
    /// Get the best close.
    /// </summary>
    ///
    public class BestClose : Indicator
    {
        /// <summary>
        /// The name of this indicator.
        /// </summary>
        ///
        public const String NAME = "PredictBestClose";

        /// <summary>
        /// The number of periods this indicator is for.
        /// </summary>
        ///
        private readonly int periods;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="thePeriods">The number of periods.</param>
        /// <param name="output">True, if this indicator is to be predicted.</param>
        public BestClose(int thePeriods, bool output) : base(NAME, false, output)
        {
            periods = thePeriods;
            Output = output;
        }


        /// <value>The number of periods.</value>
        public override int Periods
        {
            get { return periods; }
        }

        /// <summary>
        /// Calculate the indicator.
        /// </summary>
        ///
        /// <param name="data">The data available to the indicator.</param>
        /// <param name="length">The length available to the indicator.</param>
        public override sealed void Calculate(IDictionary<String, BaseCachedColumn> data,
                                              int length)
        {
            double[] close = data[FileData.CLOSE].Data;
            double[] output = Data;

            int stop = length - periods;
            for (int i = 0; i < stop; i++)
            {
                double bestClose = Double.MinValue;
                for (int j = 1; j <= periods; j++)
                {
                    bestClose = Math.Max(close[i + j], bestClose);
                }
                output[i] = bestClose;
            }

            for (int i_0 = length - periods; i_0 < length; i_0++)
            {
                output[i_0] = 0;
            }

            BeginningIndex = 0;
            EndingIndex = length - periods - 1;
        }
    }
}