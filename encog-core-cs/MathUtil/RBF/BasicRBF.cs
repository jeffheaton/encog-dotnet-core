using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Engine.Network.RBF;

namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// Implements the basic features needed for an RBF.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class BasicRBF : IRadialBasisFunction
    {
        /// <summary>
        /// The centers.
        /// </summary>
        private double[] center;

        /// <summary>
        /// The peak.
        /// </summary>
        private double peak;

        /// <summary>
        /// The width.
        /// </summary>
        private double width;

        /// <summary>
        /// The centers.
        /// </summary>
        public double[] Centers
        {
            get
            {
                return this.center;
            }
            set
            {
                this.center = value;
            }
        }

        /// <summary>
        /// The number of dimensions.
        /// </summary>
        public int Dimensions
        {
            get
            {
                return this.center.Length;
            }
        }

        /// <summary>
        /// The peak.
        /// </summary>
        public double Peak
        {
            get
            {
                return this.peak;
            }
            set
            {
                this.peak = value;
            }
        }

        /// <summary>
        /// The width.
        /// </summary>
        public double Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }


        /// <summary>
        /// Calculate the output of the RBF.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <returns>The output value.</returns>
        public abstract double Calculate(double[] x);

    }
}
