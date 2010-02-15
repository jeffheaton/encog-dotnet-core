// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Util.MathUtil;
#if logging
using log4net;
#endif
namespace Encog.Util.Randomize
{
    /// <summary>
    /// A randomizer that attempts to create starting weight values that are
    /// conducive to back propagation training.
    ///
    /// From:
    ///  
    /// Neural Networks - A Comprehensive Foundation, Haykin, chapter 6.7
    /// </summary>
    public class FanInRandomizer : BasicRandomizer
    {

        /// <summary>
        /// Error message. Can't use fan-in on a single number.
        /// </summary>
        static String ERROR = "To use FanInRandomizer you must "
                + "present a Matrix or 2D array type value.";

        /// <summary>
        /// The lower bound.
        /// </summary>
        private double lowerBound;

        /// <summary>
        /// The upper bound. 
        /// </summary>
        private double upperBound;

        /// <summary>
        /// The default boundary.
        /// </summary>
        private static double DEFAULT_BOUNDARY = 2.4;

        /// <summary>
        /// Should the square root of the number of rows be used?
        /// </summary>
        private bool sqrt;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(FanInRandomizer));
#endif

        /// <summary>
        /// Create a fan-in randomizer with default values.
        /// </summary>
        public FanInRandomizer()
            : this(-DEFAULT_BOUNDARY, DEFAULT_BOUNDARY, false)
        {

        }

        /// <summary>
        /// Construct a fan-in randomizer along the specified boundary. The min will
        /// be -boundary and the max will be boundary.
        /// </summary>
        /// <param name="boundary">The boundary for the fan-in.</param>
        /// <param name="sqrt">Should the square root of the rows to be used in the
        /// calculation.</param>
        public FanInRandomizer(double boundary, bool sqrt)
            : this(-boundary, boundary, sqrt)
        {

        }


        /// <summary>
        /// Construct a fan-in randomizer. Use the specified bounds.
        /// </summary>
        /// <param name="aLowerBound">The lower bound.</param>
        /// <param name="anUpperBound">The upper bound.</param>
        /// <param name="sqrt">True if the square root of the rows should be used in the
        /// calculation.</param>
        public FanInRandomizer(double aLowerBound, double anUpperBound,
                 bool sqrt)
        {
            this.lowerBound = aLowerBound;
            this.upperBound = anUpperBound;
            this.sqrt = sqrt;
        }

        /// <summary>
        /// Calculate the fan-in value.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <returns>The fan-in value.</returns>
        private double CalculateValue(int rows)
        {
            double rowValue;

            if (this.sqrt)
            {
                rowValue = Math.Sqrt(rows);
            }
            else
            {
                rowValue = rows;
            }

            return (this.lowerBound / rowValue) + ThreadSafeRandom.NextDouble()
                    * ((this.upperBound - this.lowerBound) / rowValue);
        }

        /// <summary>
        /// Throw an error if this class is used improperly.
        /// </summary>
        private void CauseError()
        {
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(FanInRandomizer.ERROR);
            }
#endif
            throw new EncogError(FanInRandomizer.ERROR);
        }

        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        public override double Randomize(double d)
        {
            CauseError();
            return 0;
        }

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        public override void Randomize(double[] d)
        {
            CauseError();
        }

        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous 
        /// values may be used, or they may be discarded, depending on 
        /// the randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        public override void Randomize(double[][] d)
        {
            for (int row = 0; row < d.Length; row++)
            {
                for (int col = 0; col < d[0].Length; col++)
                {
                    d[row][col] = CalculateValue(d.Length);
                }
            }
        }

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="m">A matrix to randomize.</param>
        public override void Randomize(Matrix.Matrix m)
        {
            for (int row = 0; row < m.Rows; row++)
            {
                for (int col = 0; col < m.Cols; col++)
                {
                    m[row, col] = CalculateValue(m.Rows);
                }
            }
        }

    }

}
