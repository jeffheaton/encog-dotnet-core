//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.Util;

namespace Encog.Neural.Networks.Training.PNN
{
    /// <summary>
    /// This class determines optimal values for multiple sigmas in a PNN kernel.
    /// This is done using a CJ (conjugate gradient) method.
    /// Some of the algorithms in this class are based on C++ code from:
    /// Advanced Algorithms for Neural Networks: A C++ Sourcebook by Timothy Masters
    /// John Wiley Sons Inc (Computers); April 3, 1995 ISBN: 0471105880
    /// </summary>
    ///
    public class DeriveMinimum
    {
        /// <summary>
        /// Derive the minimum, using a conjugate gradient method.
        /// </summary>
        ///
        /// <param name="maxIterations">The max iterations.</param>
        /// <param name="maxError">Stop at this error rate.</param>
        /// <param name="eps">The machine's precision.</param>
        /// <param name="tol">The convergence tolerance.</param>
        /// <param name="network">The network to get the error from.</param>
        /// <param name="n">The number of variables.</param>
        /// <param name="x">The independent variable.</param>
        /// <param name="ystart">The start for y.</param>
        /// <param name="bs">Work vector, must have n elements.</param>
        /// <param name="direc">Work vector, must have n elements.</param>
        /// <param name="g">Work vector, must have n elements.</param>
        /// <param name="h">Work vector, must have n elements.</param>
        /// <param name="deriv2">Work vector, must have n elements.</param>
        /// <returns>The best error.</returns>
        public double Calculate(int maxIterations, double maxError,
                                double eps, double tol,
                                CalculationCriteria network, int n, double[] x,
                                double ystart, double[] bs, double[] direc,
                                double[] g, double[] h, double[] deriv2)
        {
            double prevBest, toler, gam, improvement;
            var globalMinimum = new GlobalMinimumSearch();

            double fbest = network.CalcErrorWithMultipleSigma(x, direc, deriv2,
                                                              true);
            prevBest = 1.0e30d;
            for (int i = 0; i < n; i++)
            {
                direc[i] = -direc[i];
            }

            EngineArray.ArrayCopy(direc, g);
            EngineArray.ArrayCopy(direc, h);

            int convergenceCounter = 0;
            int poorCJ = 0;

            // Main loop
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                if (fbest < maxError)
                {
                    break;
                }

                // Check for convergence
                if (prevBest <= 1.0d)
                {
                    toler = tol;
                }
                else
                {
                    toler = tol*prevBest;
                }

                // Stop if there is little improvement
                if ((prevBest - fbest) <= toler)
                {
                    if (++convergenceCounter >= 3)
                    {
                        break;
                    }
                }
                else
                {
                    convergenceCounter = 0;
                }

                double dot1 = 0;
                double dot2 = 0;
                double dlen = 0;

                dot1 = dot2 = dlen = 0.0d;
                double high = 1.0e-4d;
                for (int i_0 = 0; i_0 < n; i_0++)
                {
                    bs[i_0] = x[i_0];
                    if (deriv2[i_0] > high)
                    {
                        high = deriv2[i_0];
                    }
                    dot1 += direc[i_0]*g[i_0]; // Directional first derivative
                    dot2 += direc[i_0]*direc[i_0]*deriv2[i_0]; // and second
                    dlen += direc[i_0]*direc[i_0]; // Length of search vector
                }

                dlen = Math.Sqrt(dlen);

                double scale;

                if (Math.Abs(dot2) < EncogFramework.DEFAULT_DOUBLE_EQUAL)
                {
                    scale = 0;
                }
                else
                {
                    scale = dot1/dot2;
                }
                high = 1.5d/high;
                if (high < 1.0e-4d)
                {
                    high = 1.0e-4d;
                }

                if (scale < 0.0d)
                {
                    scale = high;
                }
                else if (scale < 0.1d*high)
                {
                    scale = 0.1d*high;
                }
                else if (scale > 10.0d*high)
                {
                    scale = 10.0d*high;
                }

                prevBest = fbest;
                globalMinimum.Y2 = fbest;

                globalMinimum.FindBestRange(0.0d, 2.0d*scale, -3, false, maxError,
                                            network);

                if (globalMinimum.Y2 < maxError)
                {
                    if (globalMinimum.Y2 < fbest)
                    {
                        for (int i_1 = 0; i_1 < n; i_1++)
                        {
                            x[i_1] = bs[i_1] + globalMinimum.Y2*direc[i_1];
                            if (x[i_1] < 1.0e-10d)
                            {
                                x[i_1] = 1.0e-10d;
                            }
                        }
                        fbest = globalMinimum.Y2;
                    }
                    else
                    {
                        for (int i_2 = 0; i_2 < n; i_2++)
                        {
                            x[i_2] = bs[i_2];
                        }
                    }
                    break;
                }

                if (convergenceCounter > 0)
                {
                    fbest = globalMinimum.Brentmin(20, maxError, eps, 1.0e-7d,
                                                   network, globalMinimum.Y2);
                }
                else
                {
                    fbest = globalMinimum.Brentmin(10, maxError, 1.0e-6d, 1.0e-5d,
                                                   network, globalMinimum.Y2);
                }

                for (int i_3 = 0; i_3 < n; i_3++)
                {
                    x[i_3] = bs[i_3] + globalMinimum.X2*direc[i_3];
                    if (x[i_3] < 1.0e-10d)
                    {
                        x[i_3] = 1.0e-10d;
                    }
                }

                improvement = (prevBest - fbest)/prevBest;

                if (fbest < maxError)
                {
                    break;
                }

                for (int i_4 = 0; i_4 < n; i_4++)
                {
                    direc[i_4] = -direc[i_4]; // negative gradient
                }

                gam = Gamma(n, g, direc);

                if (gam < 0.0d)
                {
                    gam = 0.0d;
                }

                if (gam > 10.0d)
                {
                    gam = 10.0d;
                }

                if (improvement < 0.001d)
                {
                    ++poorCJ;
                }
                else
                {
                    poorCJ = 0;
                }

                if (poorCJ >= 2)
                {
                    if (gam > 1.0d)
                    {
                        gam = 1.0d;
                    }
                }

                if (poorCJ >= 6)
                {
                    poorCJ = 0;
                    gam = 0.0d;
                }

                FindNewDir(n, gam, g, h, direc);
            }

            return fbest;
        }

        /// <summary>
        /// Find gamma.
        /// </summary>
        ///
        /// <param name="n">The number of variables.</param>
        /// <param name="gam">The gamma value.</param>
        /// <param name="g">The "g" value, used for CJ algorithm.</param>
        /// <param name="h">The "h" value, used for CJ algorithm.</param>
        /// <param name="grad">The gradients.</param>
        private void FindNewDir(int n, double gam, double[] g,
                                double[] h, double[] grad)
        {
            int i;

            for (i = 0; i < n; i++)
            {
                g[i] = grad[i];
                grad[i] = h[i] = g[i] + gam*h[i];
            }
        }

        /// <summary>
        /// Find correction for next iteration.
        /// </summary>
        ///
        /// <param name="n">The number of variables.</param>
        /// <param name="g">The "g" value, used for CJ algorithm.</param>
        /// <param name="grad">The gradients.</param>
        /// <returns>The correction for the next iteration.</returns>
        private double Gamma(int n, double[] g, double[] grad)
        {
            int i;
            double denom, numer;

            numer = denom = 0.0d;

            for (i = 0; i < n; i++)
            {
                denom += g[i]*g[i];
                numer += (grad[i] - g[i])*grad[i]; // Grad is neg gradient
            }

            if (denom == 0.0d)
            {
                return 0.0d;
            }
            else
            {
                return numer/denom;
            }
        }
    }
}
