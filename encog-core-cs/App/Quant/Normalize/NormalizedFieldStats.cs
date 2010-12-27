using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Normalize
{
    public class NormalizedFieldStats
    {
        public double ActualHigh { get; set; }
        public double ActualLow { get; set; }
        public double NormalizedHigh { get; set; }
        public double NormalizedLow { get; set; }
        public NormalizationDesired Action { get; set; }


        public NormalizedFieldStats(NormalizationDesired action) : this(action,0,0,0,0)
        {
        }

        public NormalizedFieldStats(NormalizationDesired action, double ahigh, double alow, double nhigh, double nlow)
        {
            this.Action = action;
            this.ActualHigh = ahigh;
            this.ActualLow = alow;
            this.NormalizedHigh = nhigh;
            this.NormalizedLow = nlow;
        }

        public NormalizedFieldStats(double normalizedHigh, double normalizedLow)
        {
            this.NormalizedHigh = normalizedHigh;
            this.NormalizedLow = normalizedLow;
            this.ActualHigh = Double.MinValue;
            this.ActualLow = Double.MaxValue;
            this.Action = NormalizationDesired.Normalize;
        }

        public NormalizedFieldStats()
            : this(1,-1)
        {
        }

        public void MakePassThrough()
        {
            this.NormalizedHigh = 0;
            this.NormalizedLow = 0;
            this.ActualHigh = 0;
            this.ActualLow = 0;
            this.Action = NormalizationDesired.PassThrough;
        }

        public void Analyze(double d)
        {
            this.ActualHigh = Math.Max(this.ActualHigh, d);
            this.ActualLow = Math.Min(this.ActualLow, d);
        }
    
	public double Normalize(double value) {
		return ((value - ActualLow) 
				/ (ActualHigh - NormalizedLow))
				* (NormalizedHigh - NormalizedLow) + NormalizedLow;
	}
	
	public double DeNormalize(double value) {
		double result = ((ActualLow - ActualHigh) * value - NormalizedHigh
                * ActualLow + ActualHigh * NormalizedLow)
                / (NormalizedLow - NormalizedHigh);
		return result;
	}

    }
}
