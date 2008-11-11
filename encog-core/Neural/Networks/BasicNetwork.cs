using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Persist;
using Encog.Neural.NeuralData;
using Encog.Util;
using Encog.Matrix;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Networks
{
    [Serializable]
    public class BasicNetwork : INetwork, IEncogPersistedObject, ICloneable
    {

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

        public int HiddenLayerCount
        {
            get
            {
                return this.layers.Count - 2;
            }
        }

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

        public ILayer InputLayer
        {
            get
            {
                return this.inputLayer;
            }
        }

        public IList<ILayer> Layers
        {
            get
            {
                return this.layers;
            }
        }


        public ILayer OutputLayer
        {
            get
            {
                return this.outputLayer;
            }
        }

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

            double biggest = Double.MaxValue;
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
