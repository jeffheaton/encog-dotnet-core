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

namespace Encog.MathUtil
{
    /// <summary>
    /// Math functions used by Encog.
    /// </summary>
    public class EncogMath
    {
        /// <summary>
        /// Calculate sqrt(a^2 + b^2) without under/overflow.
        /// </summary>
        /// <param name="a">The a value.</param>
        /// <param name="b">The b value.</param>
        /// <returns>The result.</returns>
        public static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b/a;
                r = Math.Abs(a)*Math.Sqrt(1 + r*r);
            }
            else if (b != 0)
            {
                r = a/b;
                r = Math.Abs(b)*Math.Sqrt(1 + r*r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }


        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="deg">Degrees</param>
        /// <returns>Radians</returns>
        public static double Deg2Rad(double deg)
        {
            return deg*(Math.PI/180.0);
        }


        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="rad">Radians.</param>
        /// <returns>Degrees.</returns>
        public static double Rad2Deg(double rad)
        {
            return rad*(180.0/Math.PI);
        }

        /// <summary>
        /// Compute the factorial (n!) for p.
        /// </summary>
        /// <param name="p">The number to compute the factorial for.</param>
        /// <returns>The factorial.</returns>
        public static double Factorial(int p)
        {
            double result = 1.0;

            for (int i = 1; i <= p; i++)
            {
                result *= (double)i;
            }

            return result;
        }

        internal static int Sign(double p)
        {
            if (Math.Abs(p) < EncogFramework.DefaultDoubleEqual)
            {
                return 0;
            }
            if (p > 0)
            {
                return 1;
            }
            return -1;
        }
    }
}
