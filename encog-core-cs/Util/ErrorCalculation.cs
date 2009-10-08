// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// ErrorCalculation: An implementation of root mean square (RMS) error
    /// calculation. This class is used by nearly every neural network in this book
    /// to calculate error.
    /// </summary>
    public class ErrorCalculation
    {
        /// <summary>
        /// The current error level.
        /// </summary>
        private double globalError;

        /// <summary>
        /// The size of a training set.
        /// </summary>
        private int setSize;

        /// <summary>
        /// Returns the root mean square error for a complete training set.
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateRMS()
        {
            double err = Math.Sqrt(this.globalError / (this.setSize));
            return err;

        }

        /// <summary>
        /// Reset the error accumulation to zero.
        /// </summary>
        public void Reset()
        {
            this.globalError = 0;
            this.setSize = 0;
        }

        /// <summary>
        /// Called to update for each number that should be checked.
        /// </summary>
        /// <param name="actual">The actual number.</param>
        /// <param name="ideal">The ideal number.</param>
        public void UpdateError(double[] actual, double[] ideal)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                double delta = ideal[i] - actual[i];
                this.globalError += delta * delta;
            }
            this.setSize += ideal.Length;
        }
    }
}
