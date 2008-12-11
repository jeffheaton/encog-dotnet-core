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
using Encog.Neural.Persist;
using Encog.Neural.NeuralData;
using Encog.Util;
using Encog.Matrix;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// BasicNetwork: This class implements a neural
    /// network. This class works in conjunction the Layer classes. Layers
    /// are added to the BasicNetwork to specify the structure of the
    /// neural network.
    /// 
    /// The first layer added is the input layer, the final layer added is the output
    /// layer. Any layers added between these two layers are the hidden layers.
    /// </summary>
    [Serializable]
    public class BasicNetwork : INetwork, IEncogPersistedObject, ICloneable
    {

        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The description for this object.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }


        /// <summary>
        /// The input layer.
        /// </summary>
        private ILayer inputLayer;

        /// <summary>
        /// The output layer.
        /// </summary>
        private ILayer outputLayer;

        /// <summary>
        /// All of the layers in the neural network.
        /// </summary>
        private IList<ILayer> layers = new List<ILayer>();

        private String description;

        private String name;

        /// <summary>
        /// Construct an empty neural network.
        /// </summary>
        public BasicNetwork()
        {
        }

        /// <summary>
        /// Add a layer to the neural network. The first layer added is the input
        /// layer, the last layer added is the output layer.
        /// </summary>
        /// <param name="layer">The layer to be added.</param>
        public void AddLayer(ILayer layer)
        {
            // setup the forward and back pointer
            if (this.outputLayer != null)
            {
                layer.Previous = this.OutputLayer;
                this.outputLayer.Next = layer;
            }

            // update the inputLayer and outputLayer variables
            if (this.layers.Count == 0)
            {
                this.inputLayer = layer;
                this.outputLayer = layer;
            }
            else
            {
                this.outputLayer = layer;
            }

            // add the new layer to the list
            this.layers.Add(layer);
        }

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(INeuralDataSet data)
        {
            ErrorCalculation errorCalculation = new ErrorCalculation();

            foreach (INeuralDataPair pair in data)
            {
                Compute(pair.Input);
                errorCalculation.UpdateError(this.outputLayer.Fire.Data, pair.Ideal.Data);
            }
            return (errorCalculation.CalculateRMS());
        }

        /**
         * Calculate the total number of neurons in the network across all layers.
         * 
         * @return The neuron count.
         */
        public int CalculateNeuronCount()
        {
            int result = 0;
            foreach (ILayer layer in this.layers)
            {
                result += layer.NeuronCount;
            }
            return result;
        }

        /// <summary>
        /// Compute the output for a given input to the neural network.
        /// </summary>
        /// <param name="input">The input provide to the neural network.</param>
        /// <returns>The results from the output neurons.</returns>
        public INeuralData Compute(INeuralData input)
        {

            if (input.Count != this.inputLayer.NeuronCount)
            {
                throw new NeuralNetworkError(
                        "Size mismatch: Can't compute outputs for input size="
                                + input.Count + " for input layer size="
                                + this.inputLayer.NeuronCount);
            }

            foreach (ILayer layer in this.layers)
            {
                if (layer.IsInput())
                {
                    layer.Compute(input);
                }
                else if (layer.IsHidden())
                {
                    layer.Compute(null);
                }
            }

            return this.outputLayer.Fire;
        }


        /// <summary>
        /// Compare the two neural networks. For them to be equal they must be of the
        /// same structure, and have the same matrix values.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <returns>True if the two networks are equal.</returns>
        public bool Equals(BasicNetwork other)
        {
            IEnumerator<ILayer> otherLayers = other.Layers.GetEnumerator();

            foreach (ILayer layer in this.Layers)
            {
                otherLayers.MoveNext();
                ILayer otherLayer = otherLayers.Current;

                if (layer.NeuronCount != otherLayer.NeuronCount)
                {
                    return false;
                }

                // make sure they either both have or do not have
                // a weight matrix.
                if ((layer.WeightMatrix == null)
                        && (otherLayer.WeightMatrix != null))
                {
                    return false;
                }

                if ((layer.WeightMatrix != null)
                        && (otherLayer.WeightMatrix == null))
                {
                    return false;
                }

                // if they both have a matrix, then compare the matrices
                if ((layer.WeightMatrix != null)
                        && (otherLayer.WeightMatrix != null))
                {
                    if (!layer.WeightMatrix.Equals(otherLayer.WeightMatrix))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The number of hidden layers.
        /// </summary>
        public int HiddenLayerCount
        {
            get
            {
                return this.layers.Count - 2;
            }
        }

        /// <summary>
        /// Get the hidden layers.
        /// </summary>
        public ICollection<ILayer> HiddenLayers
        {
            get
            {
                ICollection<ILayer> result = new List<ILayer>();
                foreach (ILayer layer in this.layers)
                {
                    if (layer.IsHidden())
                    {
                        result.Add(layer);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Get the input layer.
        /// </summary>
        public ILayer InputLayer
        {
            get
            {
                return this.inputLayer;
            }
        }

        /// <summary>
        /// Get all of the layers.
        /// </summary>
        public IList<ILayer> Layers
        {
            get
            {
                return this.layers;
            }
        }

        /// <summary>
        /// Get the output layer.
        /// </summary>
        public ILayer OutputLayer
        {
            get
            {
                return this.outputLayer;
            }
        }

        /// <summary>
        /// Get the size of the weight matrix.
        /// </summary>
        public int WeightMatrixSize
        {
            get
            {
                int result = 0;
                foreach (ILayer layer in this.layers)
                {
                    result += layer.MatrixSize;
                }
                return result;
            }
        }

        /// <summary>
        /// Reset the weight matrix and the thresholds.
        /// </summary>
        public void Reset()
        {
            foreach (ILayer layer in this.layers)
            {
                layer.Reset();
            }
        }

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        /// <param name="input">The input patter to present to the neural network.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(INeuralData input)
        {

            INeuralData output = Compute(input);

            int win = 0;

            double biggest = Double.MinValue;
            for (int i = 0; i < output.Count; i++)
            {

                if (output[i] > biggest)
                {
                    biggest = output[i];
                    win = i;
                }
            }

            return win;
        }

        /// <summary>
        /// Return a clone of this neural network. Including structure, weights and
        /// threshold values.
        /// </summary>
        /// <returns>A cloned copy of the neural network.</returns>
        public Object Clone()
        {
            BasicNetwork result = CloneStructure();
            Double[] copy = MatrixCODEC.NetworkToArray(this);
            MatrixCODEC.ArrayToNetwork(copy, result);
            return result;
        }

        /// <summary>
        /// Return a clone of the structure of this neural network.
        /// </summary>
        /// <returns>A cloned copy of the structure of the neural network.</returns>
        public BasicNetwork CloneStructure()
        {
            BasicNetwork result = new BasicNetwork();

            foreach (ILayer layer in this.Layers)
            {
                ILayer clonedLayer = new FeedforwardLayer(layer.NeuronCount);
                result.AddLayer(clonedLayer);
            }

            return result;
        }

    }
}
