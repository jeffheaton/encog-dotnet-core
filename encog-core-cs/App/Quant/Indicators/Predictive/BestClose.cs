using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Util;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators.Predictive
{
    /// <summary>
    /// Get the best close.
    /// </summary>
    public class BestClose : Indicator
    {
        /// <summary>
        /// The name of this indicator.
        /// </summary>
        public static readonly String NAME = "PredictBestClose";

        /// <summary>
        /// The number of periods this indicator is for.
        /// </summary>
        private int periods;

        /// <summary>
        /// The number of periods.
        /// </summary>
        public override int Periods
        {
            get
            {
                return periods;
            }
        }

        /// <summary>
        /// Construct the object. 
        /// </summary>
        /// <param name="periods">The number of periods.</param>
        /// <param name="output">True, if this indicator is to be predicted.</param>
        public BestClose(int periods, bool output)
            :base(NAME,false,output)
        {
            this.periods = periods;
            Output = output;
        }

        /// <summary>
        /// The name of this indicator.
        /// </summary>
        public string Name
        {
            get { return NAME; }
        }

        /// <summary>
        /// Calculate the indicator.
        /// </summary>
        /// <param name="data">The data available to the indicator.</param>
        /// <param name="length">The length of the data to calculate.</param>
        public override void Calculate(IDictionary<string, BaseCachedColumn> data, int length)
        {
            double[] close = data[FileData.CLOSE].Data;
            double[] output = this.Data;

            int stop = length - Periods;
            for (int i = 0; i < stop; i++)
            {
                double bestClose = Double.MinValue;
                for (int j = 1; j <= Periods; j++)
                {
                    bestClose = Math.Max(close[i + j], bestClose);
                }
                output[i] = bestClose;
            }

            for (int i = length - Periods; i < length; i++)
            {
                output[i] = 0;
            }

            this.BeginningIndex = 0;
            this.EndingIndex = length - Periods-1;
        }
    }
}
