// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.NeuralData;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Matrix;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Layers
{
    /// <summary>
    /// HopfieldLayer: This class implements a Hopfield neural network. A Hopfield
    /// neural network is fully connected and consists of a single layer. Hopfield
    /// neural networks are usually used for pattern recognition.
    /// </summary>
    [Serializable]
    public class HopfieldLayer : BasicLayer
    {
        /// <summary>
        /// Construct a hopfield layer of the specified size.
        /// </summary>
        /// <param name="size">The number of neurons in this layer.</param>
        public HopfieldLayer(int size)
            : base(size)
        {
            this.Fire = new BiPolarNeuralData(size);
            this.WeightMatrix = new Matrix.Matrix(size, size);
        }

        /// <summary>
        /// Present a pattern to the neural network and receive the result.
        /// </summary>
        /// <param name="pattern">The pattern to be presented to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        public override INeuralData Compute(INeuralData pattern)
        {
            // convert the input pattern into a matrix with a single row.
            // also convert the boolean values to bipolar(-1=false, 1=true)
            Matrix.Matrix inputMatrix = Matrix.Matrix.CreateRowMatrix(pattern.Data);

            // Process each value in the pattern
            for (int col = 0; col < pattern.Count; col++)
            {
                Matrix.Matrix columnMatrix = this.WeightMatrix.GetCol(col);
                columnMatrix = MatrixMath.Transpose(columnMatrix);

                // The output for this input element is the dot product of the
                // input matrix and one column from the weight matrix.
                double dotProduct = MatrixMath.DotProduct(inputMatrix,
                       columnMatrix);

                // Convert the dot product to either true or false.
                if (dotProduct > 0)
                {
                    this.Fire[col] = 1;
                }
                else
                {
                    this.Fire[col] = -1;
                }
            }

            return this.Fire;
        }

        /// <summary>
        /// The output pattern from this layer.
        /// </summary>
        public new BiPolarNeuralData Fire
        {
            get
            {
                return (BiPolarNeuralData)base.Fire;
            }
            set
            {
                base.Fire = value;
            }
        }
    }
}
