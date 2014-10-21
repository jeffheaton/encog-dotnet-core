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
#if logging

#endif

namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// Implements a radial function based on the inverse multiquadric function.
    /// 
    /// Contributed to Encog By M.Dean and M.Fletcher
    /// University of Cambridge, Dept. of Physics, UK
    /// </summary>
    [Serializable]
    public class InverseMultiquadricFunction : BasicRBF
    {
        /// <summary>
        /// Create centered at zero, width 0, and peak 0.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        public InverseMultiquadricFunction(int dimensions)
        {
            Centers = new double[dimensions];
            Peak = 1.0;
            Width = 1.0;
        }

        /// <summary>
        /// Construct a multi-dimension Inverse-Multiquadric function with the
        /// specified peak, centers and widths. 
        /// </summary>
        /// <param name="peak">The peak for all dimensions.</param>
        /// <param name="center">The centers for each dimension.</param>
        /// <param name="width">The widths for each dimension.</param>
        public InverseMultiquadricFunction(double peak,
                                           double[] center, double width)
        {
            Centers = center;
            Peak = peak;
            Width = width;
        }

        /// <summary>
        /// Construct a single-dimension Inverse-Multiquadric function with the
        /// specified peak, centers and widths. 
        /// </summary>
        /// <param name="center">The peak for all dimensions.</param>
        /// <param name="peak">The centers for each dimension.</param>
        /// <param name="width">The widths for each dimension.</param>
        public InverseMultiquadricFunction(double center, double peak,
                                           double width)
        {
            Centers = new double[1];
            Centers[0] = center;
            Peak = peak;
            Width = width;
        }

        /// <summary>
        /// Calculate the output.
        /// </summary>
        /// <param name="x">Input value.</param>
        /// <returns>Output value.</returns>
        public override double Calculate(double[] x)
        {
            double value = 0;
            double[] center = Centers;
            double width = Width;

            for (int i = 0; i < center.Length; i++)
            {
				var inner = x[i] - center[i];
                value += inner * inner + (width*width);
            }
            return Peak/BoundMath.Sqrt(value);
        }
    }
}
