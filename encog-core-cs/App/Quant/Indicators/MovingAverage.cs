using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;

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
