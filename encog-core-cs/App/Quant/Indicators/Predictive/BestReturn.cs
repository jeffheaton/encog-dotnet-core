using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Indicators.Predictive
{
    public class BestReturn: BestClose
    {
        public int Periods { get; set; }

        public BestReturn(int periods, bool output) :base(periods,output)
        {
        }        

        public string Name
        {
            get { return "PredictBestReturn"; }
        }
    }
}
