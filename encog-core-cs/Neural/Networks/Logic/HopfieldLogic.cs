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
using Encog.Neural.Data;
using Encog.Matrix;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an Hopfield type network.  See HopfieldPattern
    /// for more information on this type of network.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class HopfieldLogic : ThermalLogic
    {
        /// <summary>
        /// Train the neural network for the specified pattern. The neural network
        /// can be trained for more than one pattern. To do this simply call the
        /// train method more than once.
        /// </summary>
        /// <param name="pattern">The pattern to train for.</param>
        public void AddPattern(INeuralData pattern)
        {

            // Create a row matrix from the input, convert boolean to bipolar
            Matrix.Matrix m2 = Matrix.Matrix.CreateRowMatrix(pattern.Data);
            // Transpose the matrix and multiply by the original input matrix
            Matrix.Matrix m1 = MatrixMath.Transpose(m2);
            Matrix.Matrix m3 = MatrixMath.Multiply(m1, m2);

            // matrix 3 should be square by now, so create an identity
            // matrix of the same size.
            Matrix.Matrix identity = MatrixMath.Identity(m3.Rows);

            // subtract the identity matrix
            Matrix.Matrix m4 = MatrixMath.Subtract(m3, identity);

            // now add the calculated matrix, for this pattern, to the
            // existing weight matrix.
            ConvertHopfieldMatrix(m4);
        }

        /// <summary>
        /// Update the Hopfield weights after training.
        /// </summary>
        /// <param name="delta">The amount to change the weights by.</param>
        private void ConvertHopfieldMatrix(Matrix.Matrix delta)
        {
            // add the new weight matrix to what is there already
            for (int row = 0; row < delta.Rows; row++)
            {
                for (int col = 0; col < delta.Rows; col++)
                {
                    this.ThermalSynapse.WeightMatrix.Add(
                            row, col, delta[row, col]);
                }
            }
        }

        /// <summary>
        /// Perform one Hopfield iteration.
        /// </summary>
        public void Run()
        {
            INeuralData temp = this.Compute(this.CurrentState, null);
            for (int i = 0; i < temp.Count; i++)
            {
                this.CurrentState.SetBoolean(i, temp[i] > 0);
            }
        }

        /// <summary>
        /// Run the network until it becomes stable and does not change from
        /// more runs.
        /// </summary>
        /// <param name="max">The maximum number of cycles to run before giving up.</param>
        /// <returns>The number of cycles that were run.</returns>
        public int RunUntilStable(int max)
        {
            bool done = false;
            String lastStateStr = this.CurrentState.ToString();
            String currentStateStr = this.CurrentState.ToString();

            int cycle = 0;
            do
            {
                Run();
                cycle++;

                lastStateStr = this.CurrentState.ToString();

                if (!currentStateStr.Equals(lastStateStr))
                {
                    if (cycle > max)
                        done = true;
                }
                else
                    done = true;

                currentStateStr = lastStateStr;

            } while (!done);

            return cycle;
        }
    }
}
