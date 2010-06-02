using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// A ramp activation function. This function has a high and low threshold. If
    /// the high threshold is exceeded a fixed value is returned. Likewise, if the
    /// low value is exceeded another fixed value is returned.
    /// </summary>
    public class ActivationRamp : BasicActivationFunction
    {
        /// <summary>
        /// The high threshold.
        /// </summary>
        private double thresholdHigh = 1;

        /// <summary>
        /// The low threshold.
        /// </summary>
        private double thresholdLow = 0;

        /// <summary>
        /// The high value that will be used if the high threshold is exceeded.
        /// </summary>
        private double high = 1;

        /// <summary>
        /// The low value that will be used if the low threshold is exceeded.
        /// </summary>
        private double low = 0;

        /// <summary>
        /// Construct a ramp activation function. 
        /// </summary>
        /// <param name="thresholdHigh">The high threshold value.</param>
        /// <param name="thresholdLow">The low threshold value.</param>
        /// <param name="high">The high value, replaced if the high threshold is exceeded.</param>
        /// <param name="low">The low value, replaced if the low threshold is exceeded.</param>
        public ActivationRamp(double thresholdHigh,
                double thresholdLow, double high, double low)
        {
            this.thresholdHigh = thresholdHigh;
            this.thresholdLow = thresholdLow;
            this.high = high;
            this.low = low;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActivationRamp()
        {
        }

        /// <summary>
        /// Calculate the ramp value. 
        /// </summary>
        /// <param name="d">The array of values to calculate for.</param>
        public override void ActivationFunction(double[] d)
        {

            double slope = (this.thresholdHigh - this.thresholdLow) /
                (this.high - this.low);

            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] < this.thresholdLow)
                {
                    d[i] = this.low;
                }
                else if (d[i] > this.thresholdHigh)
                {
                    d[i] = this.high;
                }
                else
                {
                    d[i] = (slope * d[i]);
                }
            }

        }


        /// <summary>
        /// Clone the object. 
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override Object Clone()
        {
            return new ActivationRamp(this.thresholdHigh,
                    this.thresholdLow, this.high, this.low);
        }

        /// <summary>
        /// Calculate the derivative of this function. This will always be 1, as it
        /// is a linear function.
        /// </summary>
        /// <param name="d">The array of values to calculate over.</param>
        public override void DerivativeFunction(double[] d)
        {
            EncogArray.Fill(d, 1.0);
        }

        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
            set
            {
                this.high = value;
            }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
            set
            {
                this.low = low;
            }
        }

        /// <summary>
        /// The high threshold value.
        /// </summary>
        public double ThresholdHigh
        {
            get
            {
                return this.thresholdHigh;
            }
            set
            {
                this.thresholdHigh = value;
            }
        }

        /// <summary>
        /// The low threshold value.
        /// </summary>
        public double ThresholdLow
        {
            get
            {
                return this.thresholdLow;
            }
            set
            {
                this.thresholdLow = value;
            }
        }

        /// <summary>
        /// Returns true, as this activation function does have a derivative.
        /// </summary>
        public override bool HasDerivative
        {
            get
            {
                return true;
            }
        }

    }
}
