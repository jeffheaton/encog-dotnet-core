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
using Encog.Util;
using Encog.Util.Logging;

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
                                ICalculationCriteria network, int n, double[] x,
                                double ystart, double[] bs, double[] direc,
                                double[] g, double[] h, double[] deriv2)
        {
            var globalMinimum = new GlobalMinimumSearch();

            double fbest = network.CalcErrorWithMultipleSigma(x, direc, deriv2,
                                                              true);
            double prevBest = 1.0e30d;
            for (int i = 0; i < n; i++)
            {
                direc[i] = -direc[i];
            }

            EngineArray.ArrayCopy(direc, g);
            EngineArray.ArrayCopy(direc, h);

            int convergenceCounter = 0;
            int poorCj = 0;

            // Main loop
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                if (fbest < maxError)
                {
                    break;
                }

                EncogLogging.Log(EncogLogging.LevelInfo,
                    "Beginning internal Iteration #" + iteration + ", currentError=" + fbest + ",target=" + maxError); 

                // Check for convergence
                double toler;
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

                double dot2 = 0;
                double dlen = 0;
                double dot1 = dot2 = dlen = 0.0d;
                double high = 1.0e-4d;
                for (int i= 0; i < n; i++)
                {
                    bs[i] = x[i];
                    if (deriv2[i] > high)
                    {
                        high = deriv2[i];
                    }
                    dot1 += direc[i]*g[i]; // Directional first derivative
                    dot2 += direc[i]*direc[i]*deriv2[i]; // and second
                    dlen += direc[i]*direc[i]; // Length of search vector
                }

                double scale;

                if (Math.Abs(dot2) < EncogFramework.DefaultDoubleEqual)
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
                        for (int i = 0; i < n; i++)
                        {
                            x[i] = bs[i] + globalMinimum.Y2*direc[i];
                            if (x[i] < 1.0e-10d)
                            {
                                x[i] = 1.0e-10d;
                            }
                        }
                        fbest = globalMinimum.Y2;
                    }
                    else
                    {
                        for (int i = 0; i < n; i++)
                        {
                            x[i] = bs[i];
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

                for (int i = 0; i < n; i++)
                {
                    x[i] = bs[i] + globalMinimum.X2*direc[i];
                    if (x[i] < 1.0e-10d)
                    {
                        x[i] = 1.0e-10d;
                    }
                }

                double improvement = (prevBest - fbest)/prevBest;

                if (fbest < maxError)
                {
                    break;
                }

                for (int i = 0; i < n; i++)
                {
                    direc[i] = -direc[i]; // negative gradient
                }

                double gam = Gamma(n, g, direc);

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
                    ++poorCj;
                }
                else
                {
                    poorCj = 0;
                }

                if (poorCj >= 2)
                {
                    if (gam > 1.0d)
                    {
                        gam = 1.0d;
                    }
                }

                if (poorCj >= 6)
                {
                    poorCj = 0;
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
        private static void FindNewDir(int n, double gam, double[] g,
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
        private static double Gamma(int n, double[] g, double[] grad)
        {
            int i;
            double denom;

            double numer = denom = 0.0d;

            for (i = 0; i < n; i++)
            {
                denom += g[i]*g[i];
                numer += (grad[i] - g[i])*grad[i]; // Grad is neg gradient
            }

            if (denom == 0.0d)
            {
                return 0.0d;
            }
            return numer/denom;
        }
    }
}
