using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.App.Quant.Util;
using Encog.App.Quant.Basic;

namespace Encog.App.Quant.Indicators.Predictive
{
    public class BestClose : Indicator
    {
        public static readonly String NAME = "PredictBestClose";

        private int periods;

        public override int Periods
        {
            get
            {
                return periods;
            }
        }

        public BestClose(int periods, bool output)
            :base(NAME,false,output)
        {
            this.periods = periods;
            Output = output;
        }


        public string Name
        {
            get { return NAME; }
        }

        public override void Calculate(IDictionary<string, BaseColumn> data, int length)
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
