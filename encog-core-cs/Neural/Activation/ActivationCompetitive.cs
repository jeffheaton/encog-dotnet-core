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
using Encog.Persist;
using Encog.Persist.Persistors;

namespace Encog.Neural.Activation
{
    /// <summary>
    /// An activation function that only allows a specified number, usually one,  
    /// of the out-bound connection to win.  These connections will share in the 
    /// sum of the output, whereas the other neurons will recieve zero.
    /// 
    ///  This activation function can be useful for "winner take all" layers.
    /// </summary>
    public class ActivationCompetitive : BasicActivationFunction
    {
        /// <summary>
        /// How many winning neurons are allowed.
        /// </summary>
        private int maxWinners = 1;

        /// <summary>
        /// Create a competitive activation function with the specified maximum
        /// number of winners.
        /// </summary>
        /// <param name="winners">The maximum number of winners that this function supports.</param>
        public ActivationCompetitive(int winners)
        {
            this.maxWinners = winners;
        }

        /// <summary>
        /// Create a competitive activation function with one winner allowed.
        /// </summary>
        public ActivationCompetitive()
            : this(1)
        {
        }

        /// <summary>
        /// Calculate the activation function.
        /// </summary>
        /// <param name="d">The input.</param>
        public override void ActivationFunction(double[] d)
        {
            bool[] winners = new bool[d.Length];
            double sumWinners = 0;

            // find the desired number of winners
            for (int i = 0; i < this.maxWinners; i++)
            {
                double maxFound = Double.MinValue;
                int winner = -1;

                // find one winner
                for (int j = 0; j < d.Length; j++)
                {
                    if (!winners[j] && d[j] > maxFound)
                    {
                        winner = j;
                        maxFound = d[j];
                    }
                }
                sumWinners += maxFound;
                winners[winner] = true;
            }

            // adjust weights for winners and non-winners
            for (int i = 0; i < d.Length; i++)
            {
                if (winners[i])
                {
                    d[i] = d[i] / sumWinners;
                }
                else
                {
                    d[i] = 0.0;
                }
            }
        }

        /// <summary>
        /// Implements the activation function.  The array is modified according
        /// to the activation function being used.  See the class description
        /// for more specific information on this type of activation function.
        /// </summary>
        /// <param name="d">The input array to the activation function.</param>
        public override void DerivativeFunction(double[] d)
        {
            throw new NeuralNetworkError(
                    "Can't use the competitive activation function "
                            + "where a derivative is required.");

        }

        /// <summary>
        /// False, indication that no derivative is available for htis
        /// function.
        /// </summary>
        public override bool HasDerivative
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The maximum number of winners this function supports.
        /// </summary>
        public int MaxWinners
        {
            get
            {
                return this.maxWinners;
            }
        }
    }
}
