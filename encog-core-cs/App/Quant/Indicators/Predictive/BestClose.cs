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
                return Periods;
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
            throw new NotImplementedException();
        }
    }
}
