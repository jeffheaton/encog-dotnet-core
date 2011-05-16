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
    public class BasicNetwork : BasicML, ContainsFlat, MLContext,
                                MLRegression, MLEncodable, MLResettable, MLClassification, MLError
    {
        /// <summary>
        /// Tag used for the connection limit.
        /// </summary>
        ///
        public const String TAG_LIMIT = "CONNECTION_LIMIT";

        /// <summary>
        /// The default connection limit.
        /// </summary>
        ///
        public const double DEFAULT_CONNECTION_LIMIT = 0.0000000001d;

        /// <summary>
        /// Serial id for this class.
        /// </summary>
        ///
        private const long serialVersionUID = -136440631687066461L;

        /// <summary>
        /// The property for connection limit.
        /// </summary>
        ///
        public const String TAG_CONNECTION_LIMIT = "connectionLimit";

        /// <summary>
        /// The property for begin training.
        /// </summary>
        ///
        public const String TAG_BEGIN_TRAINING = "beginTraining";

        /// <summary>
        /// The property for context target offset.
        /// </summary>
        ///
        public const String TAG_CONTEXT_TARGET_OFFSET = "contextTargetOffset";

        /// <summary>
        /// The property for context target size.
        /// </summary>
        ///
        public const String TAG_CONTEXT_TARGET_SIZE = "contextTargetSize";

        /// <summary>
        /// The property for end training.
        /// </summary>
        ///
        public const String TAG_END_TRAINING = "endTraining";

        /// <summary>
        /// The property for has context.
        /// </summary>
        ///
        public const String TAG_HAS_CONTEXT = "hasContext";

        /// <summary>
        /// The property for layer counts.
        /// </summary>
        ///
        public const String TAG_LAYER_COUNTS = "layerCounts";

        /// <summary>
        /// The property for layer feed counts.
        /// </summary>
        ///
        public const String TAG_LAYER_FEED_COUNTS = "layerFeedCounts";

        /// <summary>
        /// The property for layer index.
        /// </summary>
        ///
        public const String TAG_LAYER_INDEX = "layerIndex";

        /// <summary>
        /// The property for weight index.
        /// </summary>
        ///
        public const String TAG_WEIGHT_INDEX = "weightIndex";

        /// <summary>
        /// The property for bias activation.
        /// </summary>
        ///
        public const String TAG_BIAS_ACTIVATION = "biasActivation";

        /// <summary>
        /// The property for layer context count.
        /// </summary>
        ///
        public const String TAG_LAYER_CONTEXT_COUNT = "layerContextCount";

        /// <summary>
        /// Holds the structure of the network. This keeps the network from having to
        /// constantly lookup layers and synapses.
        /// </summary>
        ///
        private readonly NeuralStructure structure;

        /// <summary>
        /// Construct an empty neural network.
        /// </summary>
        ///
        public BasicNetwork()
        {
            structure = new NeuralStructure(this);
        }

        /// <value>The layer count.</value>
        public int LayerCount
        {
            /// <returns>The layer count.</returns>
            get
            {
                structure.RequireFlat();
                return structure.Flat.LayerCounts.Length;
            }
        }

        /// <value>Get the structure of the neural network. The structure allows you
        /// to quickly obtain synapses and layers without traversing the
        /// network.</value>
        public NeuralStructure Structure
        {
            /// <returns>Get the structure of the neural network. The structure allows you
            /// to quickly obtain synapses and layers without traversing the
            /// network.</returns>
            get { return structure; }
        }

        /// <summary>
        /// Sets the bias activation for every layer that supports bias. Make sure
        /// that the network structure has been finalized before calling this method.
        /// </summary>
        ///
        /// <value>THe new activation.</value>
        public double BiasActivation
        {
            /// <summary>
            /// Sets the bias activation for every layer that supports bias. Make sure
            /// that the network structure has been finalized before calling this method.
            /// </summary>
            ///
            /// <param name="activation">THe new activation.</param>
            set
            {
                // first, see what mode we are on. If the network has not been
                // finalized, set the layers
                if (structure.Flat == null)
                {
                    foreach (Layer layer  in  structure.Layers)
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

        /// <summary>
        /// 
        /// </summary>
        ///
        public FlatNetwork Flat
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return Structure.Flat; }
        }

        #endregion

        #region MLClassification Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public int Classify(MLData input)
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
            if (structure.Flat != null)
            {
                structure.Flat.ClearContext();
            }
        }

        #endregion

        #region MLEncodable Members

        /// <summary>
        /// 
        /// </summary>
        ///
        public void DecodeFromArray(double[] encoded)
        {
            structure.RequireFlat();
            double[] weights = structure.Flat.Weights;
            if (weights.Length != encoded.Length)
            {
                throw new NeuralNetworkError(
                    "Size mismatch, encoded array should be of length "
                    + weights.Length);
            }

            EngineArray.ArrayCopy(encoded, weights);
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public int EncodedArrayLength()
        {
            structure.RequireFlat();
            return structure.Flat.EncodeLength;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void EncodeToArray(double[] encoded)
        {
            structure.RequireFlat();
            double[] weights = structure.Flat.Weights;
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
        public double CalculateError(MLDataSet data)
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
        public MLData Compute(MLData input)
        {
            try
            {
                MLData result = new BasicMLData(structure.Flat.OutputCount);
                structure.Flat.Compute(input.Data, result.Data);
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new NeuralNetworkError(
                    "Index exception: there was likely a mismatch between layer sizes, or the size of the input presented to the network.",
                    ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual int InputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get
            {
                structure.RequireFlat();
                return Structure.Flat.InputCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public virtual int OutputCount
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get
            {
                structure.RequireFlat();
                return Structure.Flat.OutputCount;
            }
        }

        #endregion

        #region MLResettable Members

        /// <summary>
        /// Reset the weight matrix and the bias values. This will use a
        /// Nguyen-Widrow randomizer with a range between -1 and 1. If the network
        /// does not have an input, output or hidden layers, then Nguyen-Widrow
        /// cannot be used and a simple range randomize between -1 and 1 will be
        /// used.
        /// </summary>
        ///
        public void Reset()
        {
            if (LayerCount < 3)
            {
                (new RangeRandomizer(-1, 1)).Randomize(this);
            }
            else
            {
                (new NguyenWidrowRandomizer(-1, 1)).Randomize(this);
            }
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
        public void AddLayer(Layer layer)
        {
            layer.Network = this;
            structure.Layers.Add(layer);
        }

        /// <summary>
        /// Add to a weight.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="value">The value to add.</param>
        public void AddWeight(int fromLayer, int fromNeuron,
                              int toNeuron, double value_ren)
        {
            double old = GetWeight(fromLayer, fromNeuron, toNeuron);
            SetWeight(fromLayer, fromNeuron, toNeuron, old + value_ren);
        }

        /// <summary>
        /// Calculate the total number of neurons in the network across all layers.
        /// </summary>
        ///
        /// <returns>The neuron count.</returns>
        public int CalculateNeuronCount()
        {
            int result = 0;

            foreach (Layer layer  in  structure.Layers)
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
            MLData output2 = Compute(input2);
            EngineArray.ArrayCopy(output2.Data, output);
        }


        /// <returns>The weights as a comma separated list.</returns>
        public String DumpWeights()
        {
            var result = new StringBuilder();
            NumberList.ToList(CSVFormat.EG_FORMAT, result, structure.Flat.Weights);
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
            double value_ren = GetWeight(fromLayer, fromNeuron, toNeuron);

            if (enable)
            {
                if (!structure.ConnectionLimited)
                {
                    return;
                }

                if (Math.Abs(value_ren) < structure.ConnectionLimit)
                {
                    SetWeight(fromLayer, fromNeuron, toNeuron,
                              RangeRandomizer.Randomize(-1, 1));
                }
            }
            else
            {
                if (!structure.ConnectionLimited)
                {
                    SetProperty(TAG_LIMIT,
                                DEFAULT_CONNECTION_LIMIT);
                    structure.UpdateProperties();
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
            return Equals(other, EncogFramework.DEFAULT_PRECISION);
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
            structure.RequireFlat();
            int layerNumber = LayerCount - layer - 1;
            return structure.Flat.ActivationFunctions[layerNumber];
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

            structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;

            int layerOutputIndex = structure.Flat.LayerIndex[layerNumber];
            int count = structure.Flat.LayerCounts[layerNumber];
            return structure.Flat.LayerOutput[layerOutputIndex
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
            structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return structure.Flat.LayerFeedCounts[layerNumber];
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
            structure.RequireFlat();
            int layerNumber = LayerCount - layer - 1;
            int index = structure.Flat.LayerIndex[layerNumber]
                        + neuronNumber;
            double[] output = structure.Flat.LayerOutput;
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
            structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return structure.Flat.LayerCounts[layerNumber];
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
            structure.RequireFlat();
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

            int weightBaseIndex = structure.Flat.WeightIndex[toLayerNumber];
            int count = structure.Flat.LayerCounts[fromLayerNumber];
            int weightIndex = weightBaseIndex + fromNeuron
                              + (toNeuron*count);

            return structure.Flat.Weights[weightIndex];
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
            structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;
            return structure.Flat.LayerCounts[layerNumber] != structure.Flat.LayerFeedCounts[layerNumber];
        }


        /// <summary>
        /// Set the bias activation for the specified layer.
        /// </summary>
        ///
        /// <param name="l">The layer to use.</param>
        /// <param name="value">The bias activation.</param>
        public void SetLayerBiasActivation(int l, double value_ren)
        {
            if (!IsLayerBiased(l))
            {
                throw new NeuralNetworkError(
                    "Error, the specified layer does not have a bias: " + l);
            }

            structure.RequireFlat();
            int layerNumber = LayerCount - l - 1;

            int layerOutputIndex = structure.Flat.LayerIndex[layerNumber];
            int count = structure.Flat.LayerCounts[layerNumber];
            structure.Flat.LayerOutput[layerOutputIndex + count - 1] = value_ren;
        }

        /// <summary>
        /// Set the weight between the two specified neurons.
        /// </summary>
        ///
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="fromNeuron">The from neuron.</param>
        /// <param name="toNeuron">The to neuron.</param>
        /// <param name="value">The to value.</param>
        public void SetWeight(int fromLayer, int fromNeuron,
                              int toNeuron, double value_ren)
        {
            structure.RequireFlat();
            int fromLayerNumber = LayerCount - fromLayer - 1;
            int toLayerNumber = fromLayerNumber - 1;

            if (toLayerNumber < 0)
            {
                throw new NeuralNetworkError(
                    "The specified layer is not connected to another layer: "
                    + fromLayer);
            }

            int weightBaseIndex = structure.Flat.WeightIndex[toLayerNumber];
            int count = structure.Flat.LayerCounts[fromLayerNumber];
            int weightIndex = weightBaseIndex + fromNeuron
                              + (toNeuron*count);

            structure.Flat.Weights[weightIndex] = value_ren;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[BasicNetwork: Layers=");
            int layers = structure.Layers.Count;
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
            structure.UpdateProperties();
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
        public int Winner(MLData input)
        {
            MLData output = Compute(input);
            return EngineArray.MaxIndex(output.Data);
        }
    }
}