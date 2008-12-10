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
using Encog.Neural.Networks;

namespace Encog.Matrix
{

    /// <summary>
    /// MatrixCODEC: The matrix CODEC can encode or decode a matrix
    /// to/from an array of doubles.  This is very useful when the 
    /// neural network must be looked at as an array of doubles for
    /// genetic algorithms and simulated annealing.
    /// </summary>
    public class MatrixCODEC
    {
        /// <summary>
        /// Use an array to populate the memory of the neural network.
        /// </summary>
        /// <param name="array">An array of doubles.</param>
        /// <param name="network">The network to encode.</param>
        public static void ArrayToNetwork(double[] array,
                 BasicNetwork network)
        {

            // copy data to array
            int index = 0;

            foreach (ILayer layer in network.Layers)
            {

                // now the weight matrix(if it exists)
                if (layer.Next != null)
                {
                    index = layer.WeightMatrix.FromPackedArray(array, index);
                }
            }
        }

        /// <summary>
        /// Convert to an array. This is used with some training algorithms that
        /// require that the "memory" of the neuron(the weight and threshold values)
        /// be expressed as a linear array. 
        /// </summary>
        /// <param name="network">The network to encode.</param>
        /// <returns>The memory of the neuron.</returns>
        public static double[] NetworkToArray(BasicNetwork network)
        {
            int size = 0;

            // first determine size
            foreach (ILayer layer in network.Layers)
            {
                // count the size of the weight matrix
                if (layer.HasMatrix())
                {
                    size += layer.MatrixSize;
                }
            }

            // allocate an array to hold
            double[] result = new Double[size];

            // copy data to array
            int index = 0;

            foreach (ILayer layer in network.Layers)
            {

                // now the weight matrix(if it exists)
                if (layer.Next != null)
                {

                    double[] matrix = layer.WeightMatrix.ToPackedArray();
                    for (int i = 0; i < matrix.Length; i++)
                    {
                        result[index++] = matrix[i];
                    }
                }
            }

            return result;
        }

    }
}
