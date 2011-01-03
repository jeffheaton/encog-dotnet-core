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
        private EncogNormalize norm;
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

        public EncogNormalize Norm
        {
            get
            {
                return this.norm;
            }
        }

        public BasicMLIndicator(BasicNetwork method, EncogNormalize norm, ProcessIndicators indicators)
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
