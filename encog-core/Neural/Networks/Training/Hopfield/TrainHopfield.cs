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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data.Basic;
using Encog.Matrix;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Training.Hopfield
{
    /// <summary>
    /// Train the hopfield layer.
    /// </summary>
    public class TrainHopfield : ITrain
    {
        /// <summary>
        /// The training set to use.
        /// </summary>
        private INeuralDataSet trainingSet;

        /// <summary>
        /// The network to train.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// Construct a trainer.
        /// </summary>
        /// <param name="pattern">The pattern to train for.</param>
        /// <param name="network">The network to train.</param>
        public TrainHopfield(INeuralData pattern, BasicNetwork network)
        {
            this.network = network;
            this.trainingSet = new BasicNeuralDataSet();
            this.trainingSet.Add(new BasicNeuralDataPair(pattern));
        }

        /// <summary>
        /// Construct a trainer.
        /// </summary>
        /// <param name="trainingSet">The training set to use.</param>
        /// <param name="network">The network to train.</param>
        public TrainHopfield(INeuralDataSet trainingSet,
                 BasicNetwork network)
        {
            this.trainingSet = trainingSet;
            this.network = network;
        }

        /// <summary>
        /// Not really used, for hopfield.
        /// </summary>
        public double Error
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// The trained neural network.
        /// </summary>
        public BasicNetwork TrainedNetwork
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        public void Iteration()
        {
            foreach (ILayer layer in this.network.Layers)
            {
                if (layer is HopfieldLayer)
                {
                    foreach (INeuralDataPair pair in this.trainingSet)
                    {
                        TrainHopfieldLayer((HopfieldLayer)layer, pair.Input);
                    }
                }
            }
        }

        /// <summary>
        /// Train the neural network for the specified pattern. The neural network
        /// can be trained for more than one pattern. To do this simply call the
        /// train method more than once. 
        /// </summary>
        /// <param name="layer">The layer to train.</param>
        /// <param name="pattern">The pattern to train for.</param>
        public void TrainHopfieldLayer(HopfieldLayer layer,
                 INeuralData pattern)
        {
            if (pattern.Count != layer.WeightMatrix.Rows)
            {
                throw new NeuralNetworkError("Can't train a pattern of size "
                        + pattern.Count + " on a hopfield network of size "
                        + layer.WeightMatrix.Rows);
            }

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
            layer.WeightMatrix = MatrixMath.Add(layer.WeightMatrix, m4);
        }

    }
}
