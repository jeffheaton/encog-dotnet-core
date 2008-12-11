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
using Encog.Neural.Networks.Layers;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Training.Backpropagation
{
    /// <summary>
    /// Backpropagation: This class implements a backpropagation training algorithm
    /// for feed forward neural networks. It is used in the same manner as any other
    /// training class that implements the Train interface.
    /// 
    /// Backpropagation is a common neural network training algorithm. It works by
    /// analyzing the error of the output of the neural network. Each neuron in the
    /// output layer's contribution, according to weight, to this error is
    /// determined. These weights are then adjusted to minimize this error. This
    /// process continues working its way backwards through the layers of the neural
    /// network.
    /// 
    /// This implementation of the backpropagation algorithm uses both momentum and a
    /// learning rate. The learning rate specifies the degree to which the weight
    /// matrixes will be modified through each iteration. The momentum specifies how
    /// much the previous learning iteration affects the current. To use no momentum
    /// at all specify zero.
    /// </summary>
    public class Backpropagation : ITrain
    {
        /// <summary>
        /// Get the current best neural network.
        /// </summary>
        public BasicNetwork TrainedNetwork
        {
            get
            {
                return this.network;
            }
        }


        /// <summary>
        /// Returns the root mean square error for a complete training set.
        /// </summary>

        public double Error
        {
            get
            {
                return this.error;
            }
        }



        /// <summary>
        /// The error from the last iteration.
        /// </summary>
        private double error;


        /// <summary>
        /// The learning rate. This is the degree to which the deltas will affect the
        /// current network.
        /// </summary>
        private double learnRate;


        /// <summary>
        /// The momentum, this is the degree to which the previous training cycle
        /// affects the current one.
        /// </summary>
        private double momentum;

        /// <summary>
        /// The network that is being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// A map between neural network layers and the corresponding
        /// BackpropagationLayer.
        /// </summary>
        private IDictionary<ILayer, BackpropagationLayer> layerMap = new Dictionary<ILayer, BackpropagationLayer>();

        private INeuralDataSet training;

        /// <summary>
        /// The rate at which the weight matrix will be adjusted based on
        /// learning.
        /// </summary>
        /// <param name="network">The neural network to be trained.</param>
        /// <param name="training">The training set.</param>
        /// <param name="learnRate">The learning rate, how fast to modify neural network values.</param>
        /// <param name="momentum">The momentum, how much to use the previous training iteration for the current.</param>
        public Backpropagation(BasicNetwork network,
                 INeuralDataSet training,
                 double learnRate, double momentum)
        {
            this.network = network;
            this.learnRate = learnRate;
            this.momentum = momentum;
            this.training = training;

            foreach (FeedforwardLayer layer in network.Layers)
            {
                BackpropagationLayer bpl = new BackpropagationLayer(this,
                       layer);
                this.layerMap.Add(layer, bpl);
            }
        }

        /// <summary>
        /// Calculate the error for the recognition just done.
        /// </summary>
        /// <param name="ideal">What the output neurons should have yielded.</param>
        public void CalcError(INeuralData ideal)
        {
            if (ideal.Count != this.network.OutputLayer.NeuronCount)
            {
                throw new NeuralNetworkError(
                        "Size mismatch: Can't calcError for ideal input size="
                                + ideal.Count + " for output layer size="
                                + this.network.OutputLayer.NeuronCount);
            }

            // clear out all previous error data
            foreach (ILayer layer in this.network.Layers)
            {
                GetBackpropagationLayer(layer).ClearError();
            }

            for (int i = this.network.Layers.Count - 1; i >= 0; i--)
            {
                ILayer layer = this.network.Layers[i];

                if (layer is FeedforwardLayer)
                {

                    if (layer.IsOutput())
                    {

                        GetBackpropagationLayer(layer).CalcError(ideal);
                    }
                    else
                    {
                        GetBackpropagationLayer(layer).CalcError();
                    }
                }
            }

        }

        /// <summary>
        /// Get the BackpropagationLayer that corresponds to the specified layer.
        /// </summary>
        /// <param name="layer">The specified layer.</param>
        /// <returns>The BackpropagationLayer that corresponds to the specified layer.</returns>
        public BackpropagationLayer GetBackpropagationLayer(
                 ILayer layer)
        {
            BackpropagationLayer result = this.layerMap[layer];

            if (result == null)
            {
                throw new NeuralNetworkError(
                        "Layer unknown to backpropagation trainer, was a layer added after training begain?");
            }

            return result;
        }





        /// <summary>
        /// Perform one iteration of training.
        /// </summary>
        public void Iteration()
        {

            foreach (INeuralDataPair pair in this.training)
            {
                this.network.Compute(pair.Input);
                CalcError(pair.Ideal);
            }
            Learn();

            this.error = this.network.CalculateError(this.training);
        }

        /// <summary>
        /// Modify the weight matrix and thresholds based on the last call to
        /// calcError.
        /// </summary>
        public void Learn()
        {

            foreach (FeedforwardLayer layer in this.network.Layers)
            {
                GetBackpropagationLayer(layer).Learn(this.learnRate, this.momentum);
            }

        }

    }
}
