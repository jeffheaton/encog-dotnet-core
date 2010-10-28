using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.MathUtil.RBF
{
    /// <summary>
    /// Multi-dimensional Mexican Hat, or Ricker wavelet, function.
    /// 
    /// It is usually only referred to as the "Mexican hat" in the Americas, due to
    /// cultural association with the "sombrero". In technical nomenclature this function
    /// is known as the Ricker wavelet, where it is frequently employed to model
    /// seismic data.
    /// 
    /// http://en.wikipedia.org/wiki/Mexican_Hat_Function
    /// </summary>
    public class MexicanHatFunction : BasicRBF
    {
        /// <summary>
        /// Create centered at zero, width 0, and peak 0.
        /// </summary>
        /// <param name="dimensions"></param>
        public MexicanHatFunction(int dimensions)
        {
            this.Centers = new double[dimensions];
            this.Peak = 1.0;
            this.Width = 1.0;
        }
        
        /// <summary>
        /// Construct a multi-dimension Mexican hat function with the specified peak,
        /// centers and widths. 
        /// </summary>
        /// <param name="peak">The peak for all dimensions.</param>
        /// <param name="center">The centers for each dimension.</param>
        /// <param name="width">The widths for each dimension.</param>
        public MexicanHatFunction(double peak, double[] center,
                double width)
        {
            this.Centers = center;
            this.Peak = peak;
            this.Width = width;
        }


        /// <summary>
        /// Construct a single-dimension Mexican hat function with the specified peak,
        /// centers and widths. 
        /// </summary>
        /// <param name="center">The peak for all dimensions.</param>
        /// <param name="peak">The centers for each dimension.</param>
        /// <param name="width">The widths for each dimension.</param>
        public MexicanHatFunction(double center, double peak,
                double width)
        {
            this.Centers = new double[1];
            this.Centers[0] = center;
            this.Peak = peak;
            this.Width = width;
        }


        /// <summary>
        /// Calculate the output.
        /// </summary>
        /// <param name="x">Input value.</param>
        /// <returns>Output value.</returns>
        public override double Calculate(double[] x)
        {

            double[] center = Centers;

            // calculate the "norm", but don't take square root
            // don't square because we are just going to square it
            double norm = 0;
            for (int i = 0; i < center.Length; i++)
            {
                norm += Math.Pow(x[i] - center[i], 2);
            }

            // calculate the value

            return this.Peak * (1 - norm) * Math.Exp(-norm / 2);
        }

    }
}
