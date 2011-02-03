using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators.Predictive
{
    public class BestReturn: Indicator
    {
        public static readonly String NAME = "PredictBestReturn";

        private int periods;

        public override int Periods
        {
            get
            {
                return periods;
            }
        }

        public BestReturn(int periods, bool output)
            :base(NAME,false,output)
        {
            this.periods = periods;
            Output = output;
        }


        public string Name
        {
            get { return NAME; }
        }

        public override void Calculate(IDictionary<string, BaseCachedColumn> data, int length)
        {
            double[] close = data[FileData.CLOSE].Data;
            double[] output = this.Data;

            int stop = length - Periods;
            for (int i = 0; i < stop; i++)
            {
                double bestReturn = Double.MinValue;
                double baseClose = close[i];
                for (int j = 1; j <= Periods; j++)
                {
                    double newClose = close[i + j];
                    double rtn = (newClose - baseClose) / baseClose;
                    bestReturn = Math.Max(rtn, bestReturn);
                }
                output[i] = bestReturn;
            }

            for (int i = length - Periods; i < length; i++)
            {
                output[i] = 0;
            }

            this.BeginningIndex = 0;
            this.EndingIndex = length - Periods - 1;
        }
    }
}
