//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Flat;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Structure;
using Encog.Util;
using Encog.Util.CSV;
using Encog.Util.Simple;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// This class implements a neural network. This class works in conjunction the
    /// Layer classes. Layers are added to the BasicNetwork to specify the structure
    /// of the neural network.
    /// The first layer added is the input layer, the final layer added is the output
    /// layer. Any layers added between these two layers are the hidden layers.
    /// The network structure is stored in the structure member. It is important to
    /// call:
    /// network.getStructure().finalizeStructure();
    /// Once the neural network has been completely constructed.
    /// </summary>
    ///
    [Serializable]
    public class BasicNetwork : BasicML, IContainsFlat, IMLContext,
                                IMLRegression, IMLEncodable, IMLResettable, IMLClassification, IMLError
    {
        /// <summary>
        /// Tag used for the connection limit.
        /// </summary>
        ///
        public const String TagLimit = "CONNECTION_LIMIT";

        /// <summary>
        /// The default connection limit.
        /// </summary>
        ///
        public const double DefaultConnectionLimit = 0.0000000001d;

        /// <summary>
        /// The property for connection limit.
        /// </summary>
        ///
        public const String TagConnectionLimit = "connectionLimit";

        /// <summary>
        /// The property for begin training.
        /// </summary>
        ///
        public const String TagBeginTraining = "beginTraining";

        /// <summary>
        /// The property for context target offset.
        /// </summary>
        ///
        public const String TagContextTargetOffset = "contextTargetOffset";

        /// <summary>
        /// The property for context target size.
        /// </summary>
        ///
        public const String TagContextTargetSize = "contextTargetSize";

        /// <summary>
        /// The property for end training.
        /// </summary>
        ///
        public const String TagEndTraining = "endTraining";

        /// <summary>
        /// The property for has context.
        /// </summary>
        ///
        public const String TagHasContext = "hasContext";

        /// <summary>
        /// The property for layer counts.
        /// </summary>
        ///
        public const String TagLayerCounts = "layerCounts";

        /// <summary>
        /// The property for layer feed counts.
        /// </summary>
        ///
        public const String TagLayerFeedCounts = "layerFeedCounts";

        /// <summary>
        /// The property for layer index.
        /// </summary>
        ///
        public const String TagLayerIndex = "layerIndex";

        /// <summary>
        /// The property for weight index.
        /// </summary>
        ///
        public const String TagWeightIndex = "weightIndex";

        /// <summary>
        /// The property for bias activation.
        /// </summary>
        ///
        public const String TagBiasActivation = "biasActivation";

        /// <summary>
        /// The property for layer context count.
        /// </summary>
        ///
        public const String TagLayerContextCount = "layerContextCount";

        /// <summary>
        /// Holds the structure of the network. This keeps the network from having to
        /// constantly lookup layers and synapses.
        /// </summary>
        ///
        private readonly NeuralStructure _structure;

        /// <summary>
        /// Construct an empty neural network.
        /// </summary>
        ///
        public BasicNetwork()
        {
            _structure = new NeuralStructure(this);
        }

        /// <value>The layer count.</value>
        public int LayerCount
        {
            get
            {
                _structure.RequireFlat();
                return _structure.Flat.LayerCounts.Length;
            }
        }

        /// <value>Get the structure of the neural network. The structure allows you
        /// to quickly obtain synapses and layers without traversing the
        /// network.</value>
        public NeuralStructure Structure
        {
            get { return _structure; }
        }

        /// <summary>
        /// Sets the bias activation for every layer that supports bias. Make sure
        /// that the network structure has been finalized before calling this method.
        /// </summary>
        public double BiasActivation
        {
            set
            {
                // first, see what mode we are on. If the network has not been
                // finalized, set the layers
                if (_structure.Flat == null)
                {
                    foreach (ILayer layer  in  _structure.Layers)
                    {
                        if (layer.HasBias())
                        {
                            layer.BiasActivation = value;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < LayerCount; i++)
                    {
                        if (IsLayerBiased(i))
                        {
                            SetLayerBiasActivation(i, value);
                        }
                    }
                }
            }
        }

        #region ContainsFlat Members

        /// <inheritdoc/>
        public FlatNetwork Flat
        {
            get { return Structure.Flat; }
        }

        #endregion

        #region MLClassification Members

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            return Winner(input);
        }

        #endregion

        #region MLContext Members

        /// <summary>
        /// Clear any data from any context layers.
        /// </summary>
        ///
        public void ClearContext()
        {
            if (_structure.Flat != null)
            {
                _structure.Flat.ClearContext();
            }
        }

        #endregion

        #region MLEncodable Members

        /// <inheritdoc/>
        public void DecodeFromArray(double[] encoded)
        {
            _structure.RequireFlat();
            double[] weights = _structure.Flat.Weights;
            if (weights.Length != encoded.Length)
            {
                throw new NeuralNetworkError(
                    "Size mismatch, encoded array should be of length "
                    + weights.Length);
            }

            EngineArray.ArrayCopy(encoded, weights);
        }

        /// <inheritdoc/>
        public int EncodedArrayLength()
        {
            _structure.RequireFlat();
            return _structure.Flat.EncodeLength;
        }

        /// <inheritdoc/>
        public void EncodeToArray(double[] encoded)
        {
            _structure.RequireFlat();
            double[] weights = _structure.Flat.Weights;
            if (weights.Length != encoded.Length)
            {
                throw new NeuralNetworkError(
                    "Size mismatch, encoded array should be of length "
                    + weights.Length);
            }

            EngineArray.ArrayCopy(weights, encoded);
        }

        #endregion

        #region MLError Members

        /// <summary>
        /// Calculate the error for this neural network.
        /// </summary>
        ///
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        public double CalculateError(IMLDataSet data)
        {
            return EncogUtility.CalculateRegressionError(this, data);
        }

        #endregion

        #region MLRegression Members

        /// <summary>
        /// Compute the output for a given input to the neural network.
        /// </summary>
        ///
        /// <param name="input">The input to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        public IMLData Compute(IMLData input)
        {
            try
            {
				var output = new double[_structure.Flat.OutputCount];
                _structure.Flat.Compute(input, output);
				return new BasicMLData(output, false);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new NeuralNetworkError(
                    "Index exception: there was likely a mismatch between layer sizes, or the size of the input presented to the network.",
                    ex);
            }
        }

        /// <inheritdoc/>
        public virtual int InputCount
        {
            get
            {
                _structure.RequireFlat();
                return Structure.Flat.InputCount;
            }
        }

        /// <inheritdoc/>
        public virtual int OutputCount
        {
            get
            {
                _structure.RequireFlat();
                return Structure.Flat.OutputCount;
            }
        }

        #endregion

        /// <summary>
        /// Determines the randomizer used for resets. This will normally return a
        /// Nguyen-Widrow randomizer with a range between -1 and 1. If the network
        /// does not have an input, output or hidden layers, then Nguyen-Widrow
        /// cannot be used and a simple range randomize between -1 and 1 will be
        /// used. Range randomizer is also used if the activation function is not
        /// TANH, Sigmoid, or the Elliott equivalents. 
        /// </summary>
        private IRandomizer Randomizer
        {
            get
            {
                var useNwr = true;

                for (var i = 0; i < LayerCount; i++)
                {
                    var af = GetActivation(i);
                    if (af.GetType() != typeof(ActivationSigmoid)
                        && af.GetType() != typeof(ActivationTANH)
                        && af.GetType() != typeof(ActivationElliott)
                        && af.GetType() != typeof(ActivationElliottSymmetric))
                    {
                        useNwr = false;
                    }
                }

                if (LayerCount < 3)
                {
                    useNwr = false;
                }

                if (useNwr)
                {
                    return new NguyenWidrowRandomizer();
                }
                else
                {
                    return new RangeRandomizer(-1, 1);
                }
            }
        }

        #region MLResettable Members

        /// <summary>
        /// Determines the randomizer used for resets. This will normally return a
        /// Nguyen-Widrow randomizer with a range between -1 and 1. If the network
        /// does not have an input, output or hidden layers, then Nguyen-Widrow
        /// cannot be used and a simple range randomize between -1 and 1 will be
        /// used. Range randomizer is also used if the activation function is not
        /// TANH, Sigmoid, or the Elliott equivalents. 
        /// </summary>
        public void Reset()
        {
            Randomizer.Randomize(this);
        }

        /// <summary>
        /// Reset the weight matrix and the bias values. This will use a
        /// Nguyen-Widrow randomizer with a range between -1 and 1. If the network
        /// does not have an input, output or hidden layers, then Nguyen-Widrow
        /// cannot be used and a simple range randomize between -1 and 1 will be
        /// used.
        /// Use the specified seed.
        /// </summary>
        ///
        public void Reset(int seed)
        {
            Reset();
        }

        #endregion

        /// <summary>
        /// Add a layer to the neural network. If there are no layers added this
        /// layer will become the input layer. This function automatically updates
        /// both the input and output layer references.
        /// </summary>
        ///
        /// <param name="layer">The layer to be added to the network.</param>
        public void AddLayer(ILayer layer)
        {
            layer.Network = this;
            _structure.Layers.Add(layer);
        }

        /// <summary>
        /// Add to a weight.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="v">The value to add.</param>
        public void AddWeight(int fromLayer, int fromNeuron,
                              int toNeuron, double v)
        {
            double old = GetWeight(fromLayer, fromNeuron, toNeuron);
            SetWeight(fromLayer, fromNeuron, toNeuron, old + v);
        }

        /// <summary>
        /// Calculate the total number of neurons in the network across all layers.
        /// </summary>
        ///
        /// <returns>The neuron count.</returns>
        public int CalculateNeuronCount()
        {
            int result = 0;

            foreach (ILayer layer  in  _structure.Layers)
            {
                result += layer.NeuronCount;
            }
            return result;
        }

        /// <summary>
        /// Return a clone of this neural network. Including structure, weights and
        /// bias values. This is a deep copy.
        /// </summary>
        ///
        /// <returns>A cloned copy of the neural network.</returns>
        public Object Clone()
        {
            var result = (BasicNetwork) ObjectCloner.DeepCopy(this);
            return result;
        }

        /// <summary>
        /// Compute the output for this network.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public void Compute(double[] input, double[] output)
        {
            var input2 = new BasicMLData(input);
            IMLData output2 = Compute(input2);
			output2.CopyTo(output, 0, output2.Count);
        }


        /// <returns>The weights as a comma separated list.</returns>
        public String DumpWeights()
        {
            var result = new StringBuilder();
            NumberList.ToList(CSVFormat.EgFormat, result, _structure.Flat.Weights);
            return result.ToString();
        }

        /// <summary>
        /// Enable, or disable, a connection.
        /// </summary>
        ///
        /// <param name="fromLayer">The layer that contains the from neuron.</param>
        /// <param name="fromNeuron">The source neuron.</param>
        /// <param name="toNeuron">The target connection.</param>
        /// <param name="enable">True to enable, false to disable.</param>
        public void EnableConnection(int fromLayer,
                                     int fromNeuron, int toNeuron, bool enable)
        {
            double v = GetWeight(fromLayer, fromNeuron, toNeuron);

            if (enable)
            {
                if (!_structure.ConnectionLimited)
                {
                    return;
                }

                if (Math.Abs(v) < _structure.ConnectionLimit)
                {
                    SetWeight(fromLayer, fromNeuron, toNeuron,
                              RangeRandomizer.Randomize(-1, 1));
                }
            }
            else
            {
                if (!_structure.ConnectionLimited)
                {
                    SetProperty(TagLimit,
                                DefaultConnectionLimit);
                    _structure.UpdateProperties();
                }
                SetWeight(fromLayer, fromNeuron, toNeuron, 0);
            }
        }

        /// <summary>
        /// Compare the two neural networks. For them to be equal they must be of the
        /// same structure, and have the same matrix values.
        /// </summary>
        ///
        /// <param name="other">The other neural network.</param>
        /// <returns>True if the two networks are equal.</returns>
        public bool Equals(BasicNetwork other)
        {
            return Equals(other, EncogFramework.DefaultPrecision);
        }

        /// <summary>
        /// Determine if this neural network is equal to another. Equal neural
        /// networks have the same weight matrix and bias values, within a specified
        /// precision.
        /// </summary>
        ///
        /// <param name="other">The other neural network.</param>
        /// <param name="precision">The number of decimal places to compare to.</param>
        /// <returns>True if the two neural networks are equal.</returns>
        public bool Equals(BasicNetwork other, int precision)
        {
            return NetworkCODEC.Equals(this, other, precision);
        }

        /// <summary>
        /// Get the activation function for the specified layer.
        /// </summary>
        ///
        /// <param name="layer">The layer.</param>
        /// <returns>The activation function.</returns>
        public IActivationFunction GetActivation(int layer)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - layer - 1;
            return _structure.Flat.ActivationFunctions[layerNumber];
        }


        /// <summary>
        /// Get the bias activation for the specified layer.
        /// </summary>
        ///
        /// <param name="l">The layer.</param>
        /// <returns>The bias activation.</returns>
        public double GetLayerBiasActivation(int l)
        {
            if (!IsLayerBiased(l))
            {
                throw new NeuralNetworkError(
                    "Error, the specified layer does not have a bias: " + l);
            }

            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;

            int layerOutputIndex = _structure.Flat.LayerIndex[layerNumber];
            int count = _structure.Flat.LayerCounts[layerNumber];
            return _structure.Flat.LayerOutput[layerOutputIndex
                                              + count - 1];
        }


        /// <summary>
        /// Get the neuron count.
        /// </summary>
        ///
        /// <param name="l">The layer.</param>
        /// <returns>The neuron count.</returns>
        public int GetLayerNeuronCount(int l)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return _structure.Flat.LayerFeedCounts[layerNumber];
        }

        /// <summary>
        /// Get the layer output for the specified neuron.
        /// </summary>
        ///
        /// <param name="layer">The layer.</param>
        /// <param name="neuronNumber">The neuron number.</param>
        /// <returns>The output from the last call to compute.</returns>
        public double GetLayerOutput(int layer, int neuronNumber)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - layer - 1;
            int index = _structure.Flat.LayerIndex[layerNumber]
                        + neuronNumber;
            double[] output = _structure.Flat.LayerOutput;
            if (index >= output.Length)
            {
                throw new NeuralNetworkError("The layer index: " + index
                                             + " specifies an output index larger than the network has.");
            }
            return output[index];
        }

        /// <summary>
        /// Get the total (including bias and context) neuron cont for a layer.
        /// </summary>
        ///
        /// <param name="l">The layer.</param>
        /// <returns>The count.</returns>
        public int GetLayerTotalNeuronCount(int l)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return _structure.Flat.LayerCounts[layerNumber];
        }


        /// <summary>
        /// Get the weight between the two layers.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <returns>The weight value.</returns>
        public double GetWeight(int fromLayer, int fromNeuron,
                                int toNeuron)
        {
            _structure.RequireFlat();
            ValidateNeuron(fromLayer, fromNeuron);
            ValidateNeuron(fromLayer + 1, toNeuron);
            int fromLayerNumber = LayerCount - fromLayer - 1;
            int toLayerNumber = fromLayerNumber - 1;

            if (toLayerNumber < 0)
            {
                throw new NeuralNetworkError(
                    "The specified layer is not connected to another layer: "
                    + fromLayer);
            }

            int weightBaseIndex = _structure.Flat.WeightIndex[toLayerNumber];
            int count = _structure.Flat.LayerCounts[fromLayerNumber];
            int weightIndex = weightBaseIndex + fromNeuron
                              + (toNeuron*count);

            return _structure.Flat.Weights[weightIndex];
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        ///
        /// <returns>THe hash code.</returns>
        public override sealed int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determine if the specified connection is enabled.
        /// </summary>
        ///
        /// <param name="layer">The layer to check.</param>
        /// <param name="fromNeuron">The source neuron.</param>
        /// <param name="toNeuron">THe target neuron.</param>
        /// <returns>True, if the connection is enabled, false otherwise.</returns>
        public bool IsConnected(int layer, int fromNeuron,
                                int toNeuron)
        {
            /*
			 * if (!this.structure.isConnectionLimited()) { return true; } final
			 * double value = synapse.getMatrix().get(fromNeuron, toNeuron);
			 * 
			 * return (Math.abs(value) > this.structure.getConnectionLimit());
			 */
            return false;
        }

        /// <summary>
        /// Determine if the specified layer is biased.
        /// </summary>
        ///
        /// <param name="l">The layer number.</param>
        /// <returns>True, if the layer is biased.</returns>
        public bool IsLayerBiased(int l)
        {
            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return _structure.Flat.LayerCounts[layerNumber] != _structure.Flat.LayerFeedCounts[layerNumber];
        }


        /// <summary>
        /// Set the bias activation for the specified layer.
        /// </summary>
        ///
        /// <param name="l">The layer to use.</param>
        /// <param name="value_ren">The bias activation.</param>
        public void SetLayerBiasActivation(int l, double v)
        {
            if (!IsLayerBiased(l))
            {
                throw new NeuralNetworkError(
                    "Error, the specified layer does not have a bias: " + l);
            }

            _structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;

            int layerOutputIndex = _structure.Flat.LayerIndex[layerNumber];
            int count = _structure.Flat.LayerCounts[layerNumber];
            _structure.Flat.LayerOutput[layerOutputIndex + count - 1] = v;
        }

        /// <summary>
        /// Set the weight between the two specified neurons.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="v">The to value.</param>
        public void SetWeight(int fromLayer, int fromNeuron,
                              int toNeuron, double v)
        {
            _structure.RequireFlat();
            int fromLayerNumber = LayerCount - fromLayer - 1;
            int toLayerNumber = fromLayerNumber - 1;

            if (toLayerNumber < 0)
            {
                throw new NeuralNetworkError(
                    "The specified layer is not connected to another layer: "
                    + fromLayer);
            }

            int weightBaseIndex = _structure.Flat.WeightIndex[toLayerNumber];
            int count = _structure.Flat.LayerCounts[fromLayerNumber];
            int weightIndex = weightBaseIndex + fromNeuron
                              + (toNeuron*count);

            _structure.Flat.Weights[weightIndex] = v;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[BasicNetwork: Layers=");
            
            int layers = _structure.Flat==null ? _structure.Layers.Count : _structure.Flat.LayerCounts.Length;
            
            builder.Append(layers);
            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void UpdateProperties()
        {
            _structure.UpdateProperties();
        }

        /// <summary>
        /// Validate the the specified targetLayer and neuron are valid.
        /// </summary>
        ///
        /// <param name="targetLayer">The target layer.</param>
        /// <param name="neuron">The target neuron.</param>
        public void ValidateNeuron(int targetLayer, int neuron)
        {
            if ((targetLayer < 0) || (targetLayer >= LayerCount))
            {
                throw new NeuralNetworkError("Invalid layer count: " + targetLayer);
            }

            if ((neuron < 0) || (neuron >= GetLayerTotalNeuronCount(targetLayer)))
            {
                throw new NeuralNetworkError("Invalid neuron number: " + neuron);
            }
        }

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        ///
        /// <param name="input">The input patter to present to the neural network.</param>
        /// <returns>The winning neuron.</returns>
        public int Winner(IMLData input)
        {
            IMLData output = Compute(input);
            return EngineArray.MaxIndex(output);
        }
    }
}
