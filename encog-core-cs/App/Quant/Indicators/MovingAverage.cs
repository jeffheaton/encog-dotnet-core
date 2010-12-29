using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Indicators
{
    public class MovingAverage: Indicator
    {
        public static readonly String NAME = "MovAvg";

        public override int Periods 
        {
            get
            {
                return this.periods;
            }
        }

        private int periods;

        public MovingAverage(int periods, bool output) :
            base( NAME , false, output )
        {
            this.periods = periods;
            this.Output = output;
        }
     
        public override void Calculate(IDictionary<string, BaseColumn> data, int length)
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

            int outIdx = 0;
            do
            {
                periodTotal += close[i++];
                double t = periodTotal;
                periodTotal -= close[trailingIdx++];
                output[outIdx++] = t / Periods;
            } while (i < close.Length);
        }
    }
}
