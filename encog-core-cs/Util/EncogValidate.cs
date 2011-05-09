// Encog(tm) Artificial Intelligence Framework v2.5
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

using Encog.ML.Data;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural;

namespace Encog.Util
{
    /// <summary>
    /// Designed to perform validations after an array ove
    /// </summary>
    public class EncogValidate
    {
        /// <summary>
        /// Validate that the neural network would work with the specified training set.
        /// </summary>
        /// <param name="network">The neural network that is to be evaluated.</param>
        /// <param name="training">The training set that we should evaluate </param>
        public static void ValidateNetworkForTraining(BasicNetwork network, MLDataSet training)
        {
            ILayer inputLayer = network.GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            if (inputLayer == null)
            {
                throw new NeuralNetworkError("This operation requires that the neural network have an input layer.");
            }

            if (outputLayer == null)
            {
                throw new NeuralNetworkError("This operation requires that the neural network have an output layer.");
            }

            if (inputLayer.NeuronCount != training.InputSize )
            {
                throw new NeuralNetworkError("The input layer size of "
                        + inputLayer.NeuronCount
                        + " must match the training input size of "
                        + training.InputSize + ".");
            }

            if (training.IdealSize > 0 &&
                outputLayer.NeuronCount != training.IdealSize )
            {
                throw new NeuralNetworkError("The output layer size of "
                        + inputLayer.NeuronCount
                        + " must match the training input size of "
                        + training.IdealSize + ".");
            }
        }
    }
}
