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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// This class is a thin layer on top of the existing random class.  The main difference is 
    /// that it provides the NextGaussian function that is based on Java's Random class. All of Encog's
    /// random number generation is based on this class.  To create other random number generators, 
    /// that Encog can use, extend this class.
    /// 
    /// 
    /// This uses the polar method of G. E. P. Box, M. E. Muller, and G. Marsaglia, as described 
    /// by Donald E. Knuth in The Art of Computer Programming, Volume 2: Seminumerical Algorithms, 
    /// section 3.4.1, subsection C, algorithm P. Note that it generates two independent values at the 
    /// cost of only one call to Math.log and one call to Math.sqrt.
    /// </summary>
    [Serializable]
    public class EncogRandom : Random
    {
        /// <summary>
        /// Do we have a next Gaussian.
        /// </summary>
        private bool haveNextNextGaussian = false;
        private double nextNextGaussian = 0;

        /// <summary>
        /// Initializes a new instance of the Random class, using a time-dependent default seed value.
        /// </summary>
        public EncogRandom()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Random class, using the specified seed value.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public EncogRandom(Int32 seed)
            : base(seed)
        {
        }

        /// <summary>
        /// Returns the next pseudorandom, Gaussian ("normally") distributed double value with 
        /// mean 0.0 and standard deviation 1.0 from this random number generator's sequence.
        /// </summary>
        /// <returns>The random number.</returns>
        public double NextGaussian()
        {
            if (haveNextNextGaussian)
            {
                haveNextNextGaussian = false;
                return nextNextGaussian;
            }
            else
            {
                double v1, v2, s;
                do
                {
                    v1 = 2 * NextDouble() - 1;   // between -1.0 and 1.0
                    v2 = 2 * NextDouble() - 1;   // between -1.0 and 1.0
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1 || s == 0);
                double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
                nextNextGaussian = v2 * multiplier;
                haveNextNextGaussian = true;
                return v1 * multiplier;
            }
        }
    }
}
