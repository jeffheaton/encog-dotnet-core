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
using Encog.Neural.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// This interface defines all necessary methods for a neural network layer.
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// Compute the output for this layer.
        /// </summary>
        /// <param name="pattern">The input pattern.</param>
        /// <returns>The output from this layer.</returns>
        INeuralData Compute(INeuralData pattern);

        /// <summary>
        /// The next layer.
        /// </summary>
        ILayer Next
        {
            get;
            set;
        }

        /// <summary>
        /// The previous layer.
        /// </summary>
        ILayer Previous
        {
            get;
            set;
        }

        /// <summary>
        /// The output pattern.
        /// </summary>
        INeuralData Fire
        {
            get;
            set;
        }

        /// <summary>
        /// How many neurons in this layer.
        /// </summary>
        int NeuronCount
        {
            get;
        }

        /// <summary>
        /// The weight matrix for this layer.
        /// </summary>
        Matrix.Matrix WeightMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// The size of the matrix.  Rows times columns.
        /// </summary>
        int MatrixSize
        {
            get;
        }

        /// <summary>
        /// Is this an input layer.
        /// </summary>
        /// <returns>True if this is an input layer.</returns>
        bool IsInput();

        /// <summary>
        /// Is this a hidden layer.
        /// </summary>
        /// <returns>True if this is a hidden layer.</returns>
        bool IsHidden();



        /// <summary>
        /// Reset the weight matrix to random values.
        /// </summary>
        void Reset();


        /// <summary>
        /// Is this an output layer.
        /// </summary>
        /// <returns>True if this is an output layer.</returns>
        bool IsOutput();

        /// <summary>
        /// Determine if this layer has a matrix.
        /// </summary>
        /// <returns>True if this layer has a matrix.</returns>
        bool HasMatrix();
    }
}
