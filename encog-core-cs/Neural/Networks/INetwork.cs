using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Interface that defines a neural network.
    /// </summary>
    public interface INetwork : IEncogPersistedObject
    {
        /// <summary>
        /// Add a layer to the neural network. The first layer added is the input
        /// layer, the last layer added is the output layer. This layer is added with
        /// a weighted synapse.
        /// </summary>
        /// <param name="layer">The layer to be added.</param>
        void AddLayer(ILayer layer);


        /// <summary>
        /// Add a layer to the neural network. If there are no layers added this
        /// layer will become the input layer. This function automatically updates
        /// both the input and output layer references.
        /// </summary>
        /// <param name="layer">The layer to be added to the network.</param>
        /// <param name="type">What sort of synapse should connect this layer to the last.</param>
        void AddLayer(ILayer layer, SynapseType type);

        /// <summary>
        /// Calculate the error for this neural network. The error is calculated
        /// using root-mean-square(RMS).
        /// </summary>
        /// <param name="data">The training set.</param>
        /// <returns>The error percentage.</returns>
        double CalculateError(INeuralDataSet data);

        /// <summary>
        /// Calculate the total number of neurons in the network across all layers.
        /// </summary>
        /// <returns>The neuron count.</returns>
        int CalculateNeuronCount();

        /// <summary>
        /// Check that the input size is acceptable, if it does not match the input
        /// layer, then throw an error.
        /// </summary>
        /// <param name="input">The input data.</param>
        void CheckInputSize(INeuralData input);

        /// <summary>
        /// Used to compare one neural network to another, compare two layers.
        /// </summary>
        /// <param name="layerThis">The layer being compared.</param>
        /// <param name="layerOther">The other layer.</param>
        /// <param name="precision">The precision to use, how many decimal places.</param>
        /// <returns>Returns true if the two layers are the same.</returns>
        bool CompareLayer(ILayer layerThis, ILayer layerOther,
                 int precision);

        /// <summary>
        /// Compute the output for a given input to the neural network.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        INeuralData Compute(INeuralData input);

        /// <summary>
        /// Compute the output for a given input to the neural network. This method
        /// provides a parameter to specify an output holder to use. This holder
        /// allows propagation training to track the output from each layer. If you
        /// do not need this holder pass null, or use the other compare method.
        /// </summary>
        /// <param name="input">The input provide to the neural network.</param>
        /// <param name="useHolder">Allows a holder to be specified, this allows propagation
        /// training to check the output of each layer.</param>
        /// <returns>The results from the output neurons.</returns>
        INeuralData Compute(INeuralData input,
                 NeuralOutputHolder useHolder);

        /// <summary>
        /// Compare the two neural networks. For them to be equal they must be of the
        /// same structure, and have the same matrix values.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <returns>True if the two networks are equal.</returns>
        bool Equals(BasicNetwork other);

        /// <summary>
        /// Determine if this neural network is equal to another. Equal neural
        /// networks have the same weight matrix and threshold values, within a
        /// specified precision.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <param name="precision">The number of decimal places to compare to.</param>
        /// <returns>True if the two neural networks are equal.</returns>
        bool Equals(BasicNetwork other, int precision);


        /// <summary>
        /// Get the count for how many hidden layers are present.
        /// </summary>
        int HiddenLayerCount
        {
            get;
        }

        /// <summary>
        /// Get a collection of the hidden layers in the network.
        /// </summary>
        ICollection<ILayer> HiddenLayers
        {
            get;
        }

        /// <summary>
        /// Get the input layer.
        /// </summary>
        ILayer InputLayer
        {
            get;
            set;
        }

        /// <summary>
        /// Get the output layer.
        /// </summary>
        ILayer OutputLayer
        {
            get;
            set;
        }

        /// <summary>
        /// Get the structure of the neural network. The structure allows you
        /// to quickly obtain synapses and layers without traversing the
        /// network.
        /// </summary>
        NeuralStructure Structure
        {
            get;
        }

        /// <summary>
        /// The size of the matrix.
        /// </summary>
        int WeightMatrixSize
        {
            get;
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        /// <returns></returns>
        int GetHashCode();

        /// <summary>
        /// Called to cause the network to attempt to infer which layer should be the
        /// output layer.
        /// </summary>
        void InferOutputLayer();

        /// <summary>
        /// Determine if this layer is hidden.
        /// </summary>
        /// <param name="layer">The layer to evaluate.</param>
        /// <returns>True if this layer is a hidden layer.</returns>
        bool IsHidden(ILayer layer);

        /// <summary>
        /// Determine if this layer is the input layer.
        /// </summary>
        /// <param name="layer">The layer to evaluate.</param>
        /// <returns>True if this layer is the input layer.</returns>
        bool IsInput(ILayer layer);

        /// <summary>
        /// Determine if this layer is the output layer.
        /// </summary>
        /// <param name="layer">The layer to evaluate.</param>
        /// <returns>True if this layer is the output layer.</returns>
        bool IsOutput(ILayer layer);

        /// <summary>
        /// Reset the weight matrix and the thresholds.
        /// </summary>
        void Reset();

        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        String ToString();

        /// <summary>
        /// Determine the winner for the specified input. This is the number of the
        /// winning neuron.
        /// </summary>
        /// <param name="input">The input patter to present to the neural network.</param>
        /// <returns>The winning neuron.</returns>
        int Winner(INeuralData input);
    }
}
