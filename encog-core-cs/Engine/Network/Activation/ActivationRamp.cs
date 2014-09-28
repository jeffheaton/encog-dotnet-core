//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.Engine.Network.Activation
{
    /// <summary>
    /// A ramp activation function. This function has a high and low threshold. If
    /// the high threshold is exceeded a fixed value is returned. Likewise, if the
    /// low value is exceeded another fixed value is returned.
    /// </summary>
    [Serializable]
    public class ActivationRamp : IActivationFunction
    {
        /// <summary>
        /// The ramp high threshold parameter.
        /// </summary>
        ///
        public const int ParamRampHighThreshold = 0;

        /// <summary>
        /// The ramp low threshold parameter.
        /// </summary>
        ///
        public const int ParamRampLowThreshold = 1;

        /// <summary>
        /// The ramp high parameter.
        /// </summary>
        ///
        public const int ParamRampHigh = 2;

        /// <summary>
        /// The ramp low parameter.
        /// </summary>
        ///
        public const int ParamRampLow = 3;

        /// <summary>
        /// The parameters.
        /// </summary>
        ///
        private readonly double[] _paras;

        /// <summary>
        /// Construct a ramp activation function.
        /// </summary>
        ///
        /// <param name="thresholdHigh">The high threshold value.</param>
        /// <param name="thresholdLow">The low threshold value.</param>
        /// <param name="high">The high value, replaced if the high threshold is exceeded.</param>
        /// <param name="low">The low value, replaced if the low threshold is exceeded.</param>
        public ActivationRamp(double thresholdHigh,
                              double thresholdLow, double high, double low)
        {
            _paras = new double[4];
            _paras[ParamRampHighThreshold] = thresholdHigh;
            _paras[ParamRampLowThreshold] = thresholdLow;
            _paras[ParamRampHigh] = high;
            _paras[ParamRampLow] = low;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public ActivationRamp()
            : this(1, 0, 1, 0)
        {
        }


        /// <summary>
        /// Clone the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            return new ActivationRamp(
                _paras[ParamRampHighThreshold],
                _paras[ParamRampLowThreshold],
                _paras[ParamRampHigh],
                _paras[ParamRampLow]);
        }


        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get { return _paras[ParamRampHigh]; }
            set { _paras[ParamRampHigh] = value; }
        }


        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get { return _paras[ParamRampLow]; }
            set { _paras[ParamRampLow] = value; }
        }


        /// <summary>
        /// Set the threshold high.
        /// </summary>
        public double ThresholdHigh
        {
            get { return _paras[ParamRampHighThreshold]; }
            set { _paras[ParamRampHighThreshold] = value; }
        }


        /// <summary>
        /// The threshold low.
        /// </summary>
        public double ThresholdLow
        {
            get { return _paras[ParamRampLowThreshold]; }
            set { _paras[ParamRampLowThreshold] = value; }
        }


        /// <summary>
        /// True, as this function does have a derivative.
        /// </summary>
        /// <returns>True, as this function does have a derivative.</returns>
        public virtual bool HasDerivative
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public virtual void ActivationFunction(double[] x, int start,
                                               int size)
        {
            double slope = (_paras[ParamRampHighThreshold] - _paras[ParamRampLowThreshold])
                           /(_paras[ParamRampHigh] - _paras[ParamRampLow]);

            for (int i = start; i < start + size; i++)
            {
                if (x[i] < _paras[ParamRampLowThreshold])
                {
                    x[i] = _paras[ParamRampLow];
                }
                else if (x[i] > _paras[ParamRampHighThreshold])
                {
                    x[i] = _paras[ParamRampHigh];
                }
                else
                {
                    x[i] = (slope*x[i]);
                }
            }
        }

        /// <inheritdoc />
        public virtual double DerivativeFunction(double b, double a)
        {
            return 1.0d;
        }

        /// <inheritdoc />
        public virtual String[] ParamNames
        {
            get
            {
                String[] result = {
                                      "thresholdHigh", "thresholdLow", "high",
                                      "low"
                                  };
                return result;
            }
        }


        /// <inheritdoc />
        public virtual double[] Params
        {
            get { return _paras; }
        }
    }
}
