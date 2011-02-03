using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.App.Quant.Normalize;

namespace Encog.App.Quant.Indicators.ML
{
    public class BasicMLIndicator
    {
        private BasicNetwork method;
        private NormalizeCSV norm;
        private ProcessIndicators indicators;

        public ProcessIndicators IndicatorsUsed
        {
            get
            {
                return this.indicators;
            }
        }

        public BasicNetwork Method
        {
            get
            {
                return method;
            }
        }

        public NormalizeCSV Norm
        {
            get
            {
                return this.norm;
            }
        }

        public BasicMLIndicator(BasicNetwork method, NormalizeCSV norm, ProcessIndicators indicators)
        {
            this.method = method;
            this.norm = norm;
        }

        public bool Calculate(double[] inputData, double[] outputData)
        {
            //this.indicators.Calculate(inputData,d);
            return false;
        }
    }
}
