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

namespace Encog.Neural.Networks.Training.PNN
{
    /// <summary>
    /// Search sigma's for a global minimum. First do a rough search, and then use
    /// the "Brent Method" to refine the search for an optimal sigma. This class uses
    /// the same sigma for each kernel. Multiple sigmas will be introduced in a later
    /// step.
    /// Some of the algorithms in this class are based on C++ code from:
    /// Advanced Algorithms for Neural Networks: A C++ Sourcebook by Timothy Masters
    /// John Wiley Sons Inc (Computers); April 3, 1995 ISBN: 0471105880
    /// </summary>
    ///
    public class GlobalMinimumSearch
    {
        /// <summary>
        /// The golden section.
        /// </summary>
        ///
        public const double Cgold = 0.3819660d;

        /// <summary>
        /// A gamma to the left(lower) of the best(middle) gamma.
        /// </summary>
        ///
        private double _x1;

        /// <summary>
        /// The middle(best) gamma.
        /// </summary>
        ///
        private double _x2;

        /// <summary>
        /// A gamma to the right(higher) of the middle(best) gamma.
        /// </summary>
        ///
        private double _x3;

        /// <summary>
        /// The value y1 is the error for x1.
        /// </summary>
        ///
        private double _y1;

        /// <summary>
        /// The value y2 is the error for x2. This is the best(middle) error.
        /// </summary>
        ///
        private double _y2;

        /// <summary>
        /// The value y3 is the error for x3.
        /// </summary>
        ///
        private double _y3;


        /// <value></value>
        public double X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }


        /// <value>Set X2, which is the middle(best) gamma.</value>
        public double X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }


        /// <value>X3, which is a gamma to the right(higher) of the middle(best)
        /// gamma.</value>
        public double X3
        {
            get { return _x3; }
            set { _x3 = value; }
        }


        /// <value>Set Y1, which is the value y1 is the error for x1.</value>
        public double Y1
        {
            get { return _y1; }
            set { _y1 = value; }
        }


        /// <value>Y2, which is the value y2 is the error for x2. This is the
        /// best(middle) error.</value>
        public double Y2
        {
            get { return _y2; }
            set { _y2 = value; }
        }


        /// <value>Set Y3, which is the value y3 is the error for x3.</value>
        public double Y3
        {
            get { return _y3; }
            set { _y3 = value; }
        }

        /// <summary>
        /// Use the "Brent Method" to find a better minimum.
        /// </summary>
        ///
        /// <param name="maxIterations">THe maximum number of iterations.</param>
        /// <param name="maxError">We can stop if we reach this error.</param>
        /// <param name="eps">The approximate machine precision.</param>
        /// <param name="tol">Brent's tolerance, must be >= sqrt( eps )</param>
        /// <param name="network">The network to obtain the error from.</param>
        /// <param name="y">The error at x2.</param>
        /// <returns>The best error.</returns>
        public double Brentmin(int maxIterations,
                               double maxError, double eps, double tol,
                               ICalculationCriteria network, double y)
        {
            double prevdist = 0.0d;
            double step = 0.0d;

            // xBest is the minimum function ordinate thus far.
            // also keep 2nd and 3rd
            double xbest = _x2;
            double x2ndBest = _x2;
            double x3rdBest = _x2;
            // Keep the minimum bracketed between xlow and xhigh.

            // Get the low and high from our previous "crude" search.
            double xlow = _x1;
            double xhigh = _x3;

            double fbest = y;
            double fsecbest = y;
            double fthirdbest = y;

            // Main loop.
            // We will go up to the specified number of iterations.
            // Hopefully we will "break out" long before that happens!
            for (int iter = 0; iter < maxIterations; iter++)
            {
                // Have we reached an acceptable error?
                if (fbest < maxError)
                {
                    break;
                }

                double xmid = 0.5d*(xlow + xhigh);
                double tol1 = tol*(Math.Abs(xbest) + eps);
                double tol2 = 2.0*tol1;

                // See if xlow is close relative to tol2,
                // Also, that that xbest is near the midpoint.
                if (Math.Abs(xbest - xmid) <= (tol2 - 0.5d*(xhigh - xlow)))
                {
                    break;
                }

                // Don't go to close to eps, the machine precision.
                if ((iter >= 2) && ((fthirdbest - fbest) < eps))
                {
                    break;
                }

                double xrecent = 0;

                // Try parabolic fit, if we moved far enough.
                if (Math.Abs(prevdist) > tol1)
                {
                    // Temps holders for the parabolic estimate
                    double t1 = (xbest - x2ndBest)*(fbest - fthirdbest);
                    double t2 = (xbest - x3rdBest)*(fbest - fsecbest);
                    double numer = (xbest - x3rdBest)*t2
                                   - (xbest - x2ndBest)*t1;
                    double denom = 2.0*(t1 - t2);
                    double testdist = prevdist;
                    prevdist = step;
                    // This is the parabolic estimate to min.
                    if (denom != 0.0d)
                    {
                        step = numer/denom;
                    }
                    else
                    {
                        // test failed.
                        step = 1.0e30d;
                    }

                    // If shrinking, and within bounds, then use the parabolic
                    // estimate.
                    if ((Math.Abs(step) < Math.Abs(0.5d*testdist))
                        && (step + xbest > xlow) && (step + xbest < xhigh))
                    {
                        xrecent = xbest + step;
                        // If very close to known bounds.
                        if ((xrecent - xlow < tol2) || (xhigh - xrecent < tol2))
                        {
                            if (xbest < xmid)
                            {
                                step = tol1;
                            }
                            else
                            {
                                step = -tol1;
                            }
                        }
                    }
                    else
                    {
                        // Parabolic estimate poor, so use golden section
                        prevdist = (xbest >= xmid) ? xlow - xbest : xhigh - xbest;
                        step = Cgold*prevdist;
                    }
                }
                else
                {
                    // prevdist did not exceed tol1: we did not move far
                    // enough
                    // to justify a parabolic fit. Use golden section.
                    prevdist = (xbest >= xmid) ? xlow - xbest : xhigh - xbest;
                    step = .3819660d*prevdist;
                }

                if (Math.Abs(step) >= tol1)
                {
                    xrecent = xbest + step; // another trial we must move a
                }
                else
                {
                    // decent distance.
                    if (step > 0.0)
                    {
                        xrecent = xbest + tol1;
                    }
                    else
                    {
                        xrecent = xbest - tol1;
                    }
                }

                /*
				 * At long last we have a trial point 'xrecent'. Evaluate the
				 * function.
				 */

                double frecent = network.CalcErrorWithSingleSigma(xrecent);

                if (frecent < 0.0d)
                {
                    break;
                }

                if (frecent <= fbest)
                {
                    // If we improved...
                    if (xrecent >= xbest)
                    {
                        xlow = xbest; // replacing the appropriate endpoint
                    }
                    else
                    {
                        xhigh = xbest;
                    }
                    x3rdBest = x2ndBest; // Update x and f values for best,
                    x2ndBest = xbest; // second and third best
                    xbest = xrecent;
                    fthirdbest = fsecbest;
                    fsecbest = fbest;
                    fbest = frecent;
                }
                else
                {
                    // We did not improve
                    if (xrecent < xbest)
                    {
                        xlow = xrecent; // replacing the appropriate endpoint
                    }
                    else
                    {
                        xhigh = xrecent;
                    }

                    if ((frecent <= fsecbest) || (x2ndBest == xbest))
                    {
                        x3rdBest = x2ndBest;

                        x2ndBest = xrecent;
                        fthirdbest = fsecbest;
                        fsecbest = frecent;
                    }
                    else if ((frecent <= fthirdbest) || (x3rdBest == xbest)
                             || (x3rdBest == x2ndBest))
                    {
                        x3rdBest = xrecent;
                        fthirdbest = frecent;
                    }
                }
            }

            // update the three sigmas.

            _x1 = xlow;
            _x2 = xbest;
            _x3 = xhigh;

            // return the best.
            return fbest;
        }

        /// <summary>
        /// Find the best common gamma. Use the same gamma for all kernels. This is a
        /// crude brute-force search. The range found should be refined using the
        /// "Brent Method", also provided in this class.
        /// </summary>
        ///
        /// <param name="low">The low gamma to begin the search with.</param>
        /// <param name="high">The high gamma to end the search with.</param>
        /// <param name="numberOfPoints">If you do set this to negative, set x2 and y2 to the correct values.</param>
        /// <param name="useLog">Should we progress "logarithmically" from low to high.</param>
        /// <param name="minError">We are done if the error is below this.</param>
        /// <param name="network">The network to evaluate.</param>
        public void FindBestRange(double low, double high,
                                  int numberOfPoints, bool useLog, double minError,
                                  ICalculationCriteria network)
        {
            int i, ibest;
            double x, y, rate, previous;
            bool firstPointKnown;

            // if the number of points is negative, then
            // we already know the first point. Don't recalculate it.
            if (numberOfPoints < 0)
            {
                numberOfPoints = -numberOfPoints;
                firstPointKnown = true;
            }
            else
            {
                firstPointKnown = false;
            }

            // Set the rate to go from high to low. We are either advancing
            // logarithmically, or linear.
            if (useLog)
            {
                rate = Math.Exp(Math.Log(high/low)/(numberOfPoints - 1));
            }
            else
            {
                rate = (high - low)/(numberOfPoints - 1);
            }

            // Start the search at the low.
            x = low;
            previous = 0.0d;
            ibest = -1;

            // keep track of if the error is getting worse.
            bool gettingWorse = false;

            // Try the specified number of points, between high and low.
            for (i = 0; i < numberOfPoints; i++)
            {
                // Determine the error. If the first point is known, then us y2 as
                // the error.
                if ((i > 0) || !firstPointKnown)
                {
                    y = network.CalcErrorWithSingleSigma(x);
                }
                else
                {
                    y = _y2;
                }

                // Have we found a new best candidate point?
                if ((i == 0) || (y < _y2))
                {
                    // yes, we found a new candidate point!
                    ibest = i;
                    _x2 = x;
                    _y2 = y;
                    _y1 = previous; // Function value to its left
                    gettingWorse = false; // Flag that min is not yet bounded
                }
                else if (i == (ibest + 1))
                {
                    // Things are getting worse!
                    // Might be the right neighbor of the best found.
                    _y3 = y;
                    gettingWorse = true;
                }

                // Track the left neighbour of the best.
                previous = y;

                // Is this good enough? Might be able to stop early
                if ((_y2 <= minError) && (ibest > 0) && gettingWorse)
                {
                    break;
                }

                // Decrease the rate either linearly or
                if (useLog)
                {
                    x *= rate;
                }
                else
                {
                    x += rate;
                }
            }

            /*
			 * At this point we have a minimum (within low,high) at (x2,y2). Compute
			 * x1 and x3, its neighbors. We already know y1 and y3 (unless the
			 * minimum is at an endpoint!).
			 */

            // We have now located a minimum! Yeah!!
            // Lets calculate the neighbors. x1 and x3, which are the sigmas.
            // We should already have y1 and y3 calculated, these are the errors,
            // and are expensive to recalculate.
            if (useLog)
            {
                _x1 = _x2/rate;
                _x3 = _x2*rate;
            }
            else
            {
                _x1 = _x2 - rate;
                _x3 = _x2 + rate;
            }

            // We are really done at this point. But for "extra credit", we check to
            // see if things were "getting worse".
            //
            // If NOT, and things were getting better, the user probably cropped the
            // gamma range a bit short. After all, it is hard to guess at a good
            // gamma range.
            //
            // To try and get the best common gamma that we can, we will actually
            // slip off the right-hand high-range and search for an even better
            // gamma.

            if (!gettingWorse)
            {
                // Search as far as needed! (endless loop)
                for (;;)
                {
                    // calculate at y3(the end point)
                    _y3 = network.CalcErrorWithSingleSigma(_x3);

                    // If we are not finding anything better, then stop!
                    // We are already outside the specified search range.
                    if (_y3 > _y2)
                    {
                        break;
                    }
                    if ((_y1 == _y2) && (_y2 == _y3))
                    {
                        break;
                    }

                    // Shift the points for the new range, as we have
                    // extended to the right.
                    _x1 = _x2;
                    _y1 = _y2;
                    _x2 = _x3;
                    _y2 = _y3;

                    // We want to step further each time. We can't search forever,
                    // and we are already outside of the area we were supposed to
                    // scan.
                    rate *= 3.0d;
                    if (useLog)
                    {
                        _x3 *= rate;
                    }
                    else
                    {
                        _x3 += rate;
                    }
                }
            }
                // We will also handle one more "bad situation", which results from a
                // bad gamma search range.
                //
                // What if the first gamma was tried, and that was the best it ever got?
                //
                // If this is the case, there MIGHT be better gammas to the left of the
                // search space. Lets try those.
            else if (ibest == 0)
            {
                // Search as far as needed! (endless loop)
                for (;;)
                {
                    // Calculate at y3(the begin point)
                    _y1 = network.CalcErrorWithSingleSigma(_x1);

                    if (_y1 < 0.0d)
                    {
                        return;
                    }

                    // If we are not finding anything better, then stop!
                    // We are already outside the specified search range.
                    if (_y1 > _y2)
                    {
                        break;
                    }
                    if ((_y1 == _y2) && (_y2 == _y3))
                    {
                        break;
                    }

                    // Shift the points for the new range, as we have
                    // extended to the left.
                    _x3 = _x2;
                    _y3 = _y2;
                    _x2 = _x1;
                    _y2 = _y1;

                    // We want to step further each time. We can't search forever,
                    // and we are already outside of the area we were supposed to
                    // scan.
                    rate *= 3.0d;
                    if (useLog)
                    {
                        _x1 /= rate;
                    }
                    else
                    {
                        _x1 -= rate;
                    }
                }
            }
            return;
        }
    }
}
