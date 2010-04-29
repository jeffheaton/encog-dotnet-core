using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Generally, you will not want to use this randomizer as a pure neural network
    /// randomizer. More on this later in the description.
    /// 
    /// Generate random numbers that fall within a Gaussian curve. The mean
    /// represents the center of the curve, and the standard deviation helps
    /// determine the length of the curve on each side.
    /// 
    /// This randomizer is used mainly for special cases where I want to generate
    /// random numbers in a Gaussian range. For a pure neural network initilizer, it
    /// leaves much to be desired. Typically this randomizer provides the worst
    /// performance of any Encog randomizer.
    /// 
    /// Uses the "Box Muller" method.
    /// http://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
    /// 
    /// Ported from C++ version provided by Everett F. Carter Jr., 1994
    /// </summary>
    public class GaussianRandomizer : BasicRandomizer
    {
        /// <summary>
        /// The y2 value.
        /// </summary>
        private double y2;

        /// <summary>
        /// Should we use the last value.
        /// </summary>
        private bool useLast = false;

        /// <summary>
        /// The mean.
        /// </summary>
        private double mean;

        /// <summary>
        /// The standard deviation.
        /// </summary>
        private double standardDeviation;
        
        /// <summary>
        /// Compute a Gaussian random number. 
        /// </summary>
        /// <param name="m">The mean.</param>
        /// <param name="s">The standard deviation.</param>
        /// <returns>The random number.</returns>
        public double BoxMuller(double m, double s)
        {
            double x1, x2, w, y1;

            // use value from previous call
            if (useLast)
            {
                y1 = y2;
                useLast = false;
            }
            else
            {
                do
                {
                    x1 = 2.0 * ThreadSafeRandom.NextDouble() - 1.0;
                    x2 = 2.0 * ThreadSafeRandom.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;
                useLast = true;
            }

            return (m + y1 * s);
        }
        
        /// <summary>
        /// Construct a Gaussian randomizer.  The mean, the standard deviation. 
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="standardDeviation">The standard deviation.</param>
        public GaussianRandomizer(double mean, double standardDeviation)
        {
            this.mean = mean;
            this.standardDeviation = standardDeviation;
        }

        /// <summary>
        /// Generate a random number.
        /// </summary>
        /// <param name="d">The input value, not used.</param>
        /// <returns>The random number.</returns>
        public override double Randomize(double d)
        {
            return BoxMuller(this.mean, this.standardDeviation);
        }

    }
}
