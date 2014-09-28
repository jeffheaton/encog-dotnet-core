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
using Encog.Util;

namespace Encog.MathUtil
{
    /// <summary>
    /// Basic vector algebra operators.
    /// Vectors are represented as arrays of doubles.
    /// 
    /// This class was created to support the calculations 
    /// in the PSO algorithm.
    /// 
    /// This class is thread safe.
    /// 
    /// Contributed by:
    /// Geoffroy Noel
    /// https://github.com/goffer-looney 
    /// 
    /// </summary>
    public class VectorAlgebra
    {
        static Random rand = new Random();

        /// <summary>
        /// v1 = v1 + v2 
        /// </summary>
        /// <param name="v1">an array of doubles</param>
        /// <param name="v2">an array of doubles</param>
        public void Add(double[] v1, double[] v2)
        {
            for (int i = 0; i < v1.Length; i++)
            {
                v1[i] += v2[i];
            }
        }

        /// <summary>
        /// v1 = v1 - v2
        /// </summary>
        /// <param name="v1">an array of doubles</param>
        /// <param name="v2">an array of doubles</param>
        public void Sub(double[] v1, double[] v2)
        {
            for (int i = 0; i < v1.Length; i++)
            {
                v1[i] -= v2[i];
            }
        }

        /// <summary>
        /// v = -v 
        /// </summary>
        /// <param name="v">an array of doubles</param>
        public void Neg(double[] v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = -v[i];
            }
        }

        /// <summary>
        ///  v = k * U(0,1) * v
        /// 
        /// The components of the vector are multiplied 
        /// by k and a random number.
        /// A new random number is generated for each 
        /// component.    
        /// Thread-safety depends on Random.nextDouble()
        /// </summary>
        /// <param name="v">an array of doubles.</param>
        /// <param name="k">a scalar.</param>
        public void MulRand(double[] v, double k)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] *= k * rand.NextDouble();
            }
        }

        /// <summary>
        /// v = k * v
        ///
        /// The components of the vector are multiplied 
        /// by k.
        /// </summary>
        /// <param name="v">an array of doubles.</param>
        /// <param name="k">a scalar.</param>
        public void Mul(double[] v, double k)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] *= k;
            }
        }

        /// <summary>
        /// dst = src 
        /// 
        /// Copy a vector.
        /// </summary>
        /// <param name="dst">an array of doubles</param>
        /// <param name="src">an array of doubles</param>
        public void Copy(double[] dst, double[] src)
        {
            EngineArray.ArrayCopy(src, dst);
        }

        /// <summary>
        /// v = U(0, 0.1) 
        /// </summary>
        /// <param name="v">an array of doubles</param>
        public void Randomise(double[] v)
        {
            Randomise(v, 0.1);
        }

        /// <summary>
        /// v = U(-1, 1) * maxValue
        ///
        /// Randomise each component of a vector to 
        /// [-maxValue, maxValue].
        /// thread-safety depends on Random.nextDouble(). 
        /// </summary>
        /// <param name="v">an array of doubles</param>
        /// <param name="maxValue">The max value.</param>
        public void Randomise(double[] v, double maxValue)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = (2 * rand.NextDouble() - 1) * maxValue;
            }
        }

        /// <summary>
        /// For each components, reset their value to maxValue if 
        /// their absolute value exceeds it.
        /// </summary>
        /// <param name="v">an array of doubles</param>
        /// <param name="maxValue">if -1 this function does nothing</param>
        public void ClampComponents(double[] v, double maxValue)
        {
            if (maxValue != -1)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    if (v[i] > maxValue) v[i] = maxValue;
                    if (v[i] < -maxValue) v[i] = -maxValue;
                }
            }
        }

    }
}
