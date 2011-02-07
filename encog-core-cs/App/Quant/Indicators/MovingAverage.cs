using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators
{
    /// <summary>
    /// A simple moving average.
    /// </summary>
    public class MovingAverage: Indicator
    {
        /// <summary>
        /// The name of this indicator.
        /// </summary>
        public static readonly String NAME = "MovAvg";

        /// <summary>
        /// The number of periods in this indicator.
        /// </summary>
        public override int Periods 
        {
            get
            {
                return this.periods;
            }
        }

        /// <summary>
        /// The number of periods in this indicator.
        /// </summary>
        private int periods;

        /// <summary>
        /// Construct this object.
        /// </summary>
        /// <param name="periods">The number of periods in this indicator.</param>
        /// <param name="output">True, if this indicator is predicted.</param>
        public MovingAverage(int periods, bool output) :
            base( NAME , false, output )
        {
            this.periods = periods;
            this.Output = output;
        }

        /// <summary>
        /// Calculate this indicator.
        /// </summary>
        /// <param name="data">The data to use.</param>
        /// <param name="length">The length to calculate over.</param>
        public override void Calculate(IDictionary<string, BaseCachedColumn> data, int length)
        {
            Require(data, FileData.CLOSE);
            
            double[] close = data[FileData.CLOSE].Data;
            double[] output = this.Data;

            int lookbackTotal = (Periods - 1);
            
            int start = lookbackTotal;
            if (start > (Periods-1))
            {
                return;
            }

            double periodTotal = 0;
            int trailingIdx = start - lookbackTotal;
            int i = trailingIdx;
            if (Periods > 1)
            {
                while (i < start)
                    periodTotal += close[i++];
            }

            int outIdx = Periods-1;
            do
            {
                periodTotal += close[i++];
                double t = periodTotal;
                periodTotal -= close[trailingIdx++];
                output[outIdx++] = t / Periods;
            } while (i < close.Length);

            BeginningIndex = Periods - 1;
            EndingIndex = output.Length - 1;

            for ( i = 0; i < Periods-1; i++)
            {
                output[i] = 0;
            }
        }
    }
}
