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

namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// Implements the basic features needed for an RBF.
    /// </summary>
    [Serializable]
    public abstract class BasicRBF : IRadialBasisFunction
    {
        /// <summary>
        /// The centers.
        /// </summary>
        private double[] _center;

        /// <summary>
        /// The peak.
        /// </summary>
        private double _peak;

        /// <summary>
        /// The width.
        /// </summary>
        private double _width;

        /// <summary>
        /// The centers.
        /// </summary>
        public double[] Centers
        {
            get { return _center; }
            set { _center = value; }
        }

        /// <summary>
        /// The number of dimensions.
        /// </summary>
        public int Dimensions
        {
            get { return _center.Length; }
        }

        /// <summary>
        /// The peak.
        /// </summary>
        public double Peak
        {
            get { return _peak; }
            set { _peak = value; }
        }

        /// <summary>
        /// The width.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }


        /// <summary>
        /// Calculate the output of the RBF.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The output value.</returns>
        public abstract double Calculate(double[] x);
    }
}
