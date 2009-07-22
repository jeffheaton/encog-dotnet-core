// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using log4net;
using Encog.Neural.Networks.Synapse;
using Encog.Util;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using System.Runtime.Serialization;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using Encog.Persist.Persistors;
using Encog.Util.Randomize;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// This class implements a neural network. This class works in conjunction the
    /// Layer classes. Layers are added to the BasicNetwork to specify the structure
    /// of the neural network.
    /// 
    /// The first layer added is the input layer, the  layer added is the output
    /// layer. Any layers added between these two layers are the hidden layers.
    /// 
    /// The network structure is stored in the structure member. It is important to
    /// call:
    /// 
    /// network.getStructure().FinalizeStructure();
    ///
    /// Once the neural network has been completely constructed.
    /// </summary>
    [Serializable]
    public class BasicNetwork : INetwork
    {
        /// <summary>
        /// The input layer.
        /// </summary>
        private ILayer inputLayer;

        /// <summary>
        /// The output layer.
        /// </summary>
        private ILayer outputLayer;

        /// <summary>
        /// The description of this object.
        /// </summary>
        private String description;

        /// <summary>
        /// The name of this object.
        /// </summary>
        private String name;

        /// <summary>
        /// Holds the structure of the network. This keeps the network from having to
        /// constantly lookup layers and synapses.
        /// </summary>
        private NeuralStructure structure;

        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));

        /// <summary>
        /// Construct an empty neural network.
        /// </summary>
        public BasicNetwork()
        {
            this.structure = new NeuralStructure(this);
        }

        /// <summary>
        /// Add a layer to the neural network. The first layer added is the input
        /// layer, the last layer added is the output layer. This layer is added with
        /// a weighted synapse.
        /// </summary>
        /// <param name="layer">The layer to be added.</param>
        public void AddLayer(ILayer layer)
        {
            AddLayer(layer, SynapseType.Weighted);
        }

        /// <summary>
        /// Add a layer to the neural network. If there are no layers added this
        /// layer will become the input layer. This function automatically updates
        /// both the input and output layer references.
        /// </summary>
        /// <param name="layer">The layer to be added to the network.</param>
        /// <param name="type">What sort of synapse should connect this layer to the last.</param>
        public void AddLayer(ILayer layer, SynapseType type)
        {

            // is this the first layer? If so, mark as the input layer.
            if (this.inputLayer == null)
            {
                this.outputLayer = layer;
                this.inputLayer = layer;
            }
            else
            {
                // add the layer to any previous layers
                this.outputLayer.AddNext(layer, type);
                this.outputLayer = layer;
            }
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
                INeuralData actual = Compute(pair.Input);
                errorCalculation.UpdateError(actual.Data, pair.Ideal.Data);
            }
            return errorCalculation.CalculateRMS();
        }

        /// <summary>
        /// Calculate the total number of neurons in the network across all layers.
        /// </summary>
        /// <returns>The neuron count.</returns>
        public int CalculateNeuronCount()
        {
            int result = 0;
            foreach (ILayer layer in this.structure.Layers)
            {
                result += layer.NeuronCount;
            }
            return result;
        }

        /// <summary>
        /// Check that the input size is acceptable, if it does not match
        /// the input layer, then throw an error.
        /// </summary>
        /// <param name="input">The input data.</param>
        public void CheckInputSize(INeuralData input)
        {
            if (input.Count != this.inputLayer.NeuronCount)
            {

                String str =
                   "Size mismatch: Can't compute outputs for input size="
                       + input.Count
                       + " for input layer size="
                       + this.inputLayer.NeuronCount;

                if (BasicNetwork.logger.IsErrorEnabled)
                {
                    BasicNetwork.logger.Error(str);
                }

                throw new NeuralNetworkError(str);
            }
        }

        /// <summary>
        /// Return a clone of this neural network. Including structure, weights and
        /// threshold values.
        /// </summary>
        /// <returns>A cloned copy of the neural network.</returns>
        public Object Clone()
        {
            BasicNetwork result = new BasicNetwork();
            ILayer input = CloneLayer(this.inputLayer, result);
            result.InputLayer = input;
            result.Structure.FinalizeStructure();
            result.InferOutputLayer();
            return result;
        }

        /// <summary>
        /// Clone an individual layer, called internally by clone.
        /// </summary>
        /// <param name="layer">The layer to be cloned.</param>
        /// <param name="network">The new network being created.</param>
        /// <returns>The cloned layer.</returns>
        private ILayer CloneLayer(ILayer layer, BasicNetwork network)
        {
            ILayer newLayer = (ILayer)layer.Clone();

            if (layer == OutputLayer)
            {
                network.OutputLayer = newLayer;
            }

            foreach (ISynapse synapse in layer.Next )
            {
                ISynapse newSynapse = (ISynapse)synapse.Clone();
                newSynapse.FromLayer = layer;
                if (synapse.ToLayer != null)
                {
                    ILayer to = CloneLayer(synapse.ToLayer, network);
                    newSynapse.ToLayer = to;
                }
                newLayer.Next.Add(newSynapse);

            }
            return newLayer;
        }

        /// <summary>
        /// Used to compare one neural network to another, compare two layers.
        /// </summary>
        /// <param name="layerThis">The layer being compared.</param>
        /// <param name="layerOther">The other layer.</param>
        /// <param name="precision">The precision to use, how many decimal places.</param>
        /// <returns>Returns true if the two layers are the same.</returns>
        public bool CompareLayer(ILayer layerThis, ILayer layerOther,
                 int precision)
        {
            IEnumerator<ISynapse> iteratorOther = layerOther.Next.GetEnumerator();

            foreach (ISynapse synapseThis in layerThis.Next)
            {
                if (!iteratorOther.MoveNext())
                {
                    return false;
                }
                ISynapse synapseOther = iteratorOther.Current;
                if (!synapseThis.WeightMatrix.equals(synapseOther.WeightMatrix,
                        precision))
                {
                    return false;
                }
                if (synapseThis.ToLayer != null)
                {
                    if (synapseOther.ToLayer == null)
                    {
                        return false;
                    }
                    if (!CompareLayer(synapseThis.ToLayer, synapseOther
                            .ToLayer, precision))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Compute the output for a given input to the neural network.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        public INeuralData Compute(INeuralData input)
        {
            return Compute(input, null);
        }

        /// <summary>
        /// Compute the output for a given input to the neural network. This method
        /// provides a parameter to specify an output holder to use.  This holder
        /// allows propagation training to track the output from each layer.
        /// If you do not need this holder pass null, or use the other 
        /// compare method.
        /// </summary>
        /// <param name="input">The input provide to the neural network.</param>
        /// <param name="useHolder">Allows a holder to be specified, this allows
        /// propagation training to check the output of each layer.</param>
        /// <returns>The results from the output neurons.</returns>
        public INeuralData Compute(INeuralData input,
                 NeuralOutputHolder useHolder)
        {
            NeuralOutputHolder holder;

            if (BasicNetwork.logger.IsDebugEnabled)
            {
                BasicNetwork.logger.Debug("Pattern {} presented to neural network" + input);
            }

            if (useHolder == null)
            {
                holder = new NeuralOutputHolder();
            }
            else
            {
                holder = useHolder;
            }

            CheckInputSize(input);
            Compute(holder, this.inputLayer, input, null);
            return holder.Output;

        }

        /// <summary>
        /// Internal computation method for a single layer.  This is called, 
        /// as the neural network processes.
        /// </summary>
        /// <param name="holder">The output holder.</param>
        /// <param name="layer">The layer to process.</param>
        /// <param name="input">The input to this layer.</param>
        /// <param name="source">The source synapse.</param>
        private void Compute(NeuralOutputHolder holder, ILayer layer,
                 INeuralData input, ISynapse source)
        {

            if (BasicNetwork.logger.IsDebugEnabled)
            {
                BasicNetwork.logger.Debug("Processing layer: " + layer + ", input= " + input);
            }

            HandleRecurrentInput(layer, input, source);

            foreach (ISynapse synapse in layer.Next)
            {
                if (!holder.Result.ContainsKey(synapse))
                {
                    if (BasicNetwork.logger.IsDebugEnabled)
                    {
                        BasicNetwork.logger.Debug("Processing synapse: " + synapse);
                    }
                    INeuralData pattern = synapse.Compute(input);
                    pattern = synapse.ToLayer.Compute(pattern);
                    synapse.ToLayer.Process(pattern);
                    holder.Result.Add(synapse, input);
                    Compute(holder, synapse.ToLayer, pattern, synapse);

                    // Is this the output from the entire network?
                    if (synapse.ToLayer == this.outputLayer)
                    {
                        holder.Output = pattern;
                    }
                }
            }
        }

        /// <summary>
        /// Create a persistor for this object.
        /// </summary>
        /// <returns>The newly created persistor.</returns>
        public virtual IPersistor CreatePersistor()
        {
            return new BasicNetworkPersistor();
        }

        /// <summary>
        /// Compare the two neural networks. For them to be equal they must be of the
        /// same structure, and have the same matrix values.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <returns>True if the two networks are equal.</returns>
        public bool Equals(BasicNetwork other)
        {
            return CompareLayer(this.inputLayer, other.InputLayer,
                    Encog.DEFAULT_PRECISION);
        }

        /// <summary>
        /// Determine if this neural network is equal to another.  Equal neural
        /// networks have the same weight matrix and threshold values, within
        /// a specified precision.
        /// </summary>
        /// <param name="other">The other neural network.</param>
        /// <param name="precision">The number of decimal places to compare to.</param>
        /// <returns>True if the two neural networks are equal.</returns>
        public bool Equals(BasicNetwork other, int precision)
        {
            return CompareLayer(this.inputLayer, other.InputLayer, precision);
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
        /// Get the count for how many hidden layers are present.
        /// </summary>
        public int HiddenLayerCount
        {
            get
            {
                return HiddenLayers.Count;
            }
        }

        /// <summary>
        /// Get a collection of the hidden layers in the network.
        /// </summary>
        public ICollection<ILayer> HiddenLayers
        {
            get
            {
                ICollection<ILayer> result = new List<ILayer>();

                foreach (ILayer layer in this.structure.Layers)
                {
                    if (IsHidden(layer))
                    {
                        if (!result.Contains(layer))
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
            set
            {
                this.inputLayer = value;
            }
        }

        /// <summary>
        /// The name.
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
        /// Get the output layer.
        /// </summary>
        public ILayer OutputLayer
        {
            get
            {
                return this.outputLayer;
            }
            set
            {
                this.outputLayer = value;
            }
        }

        /// <summary>
        /// Get the structure of the neural network.  The structure 
        /// allows you to quickly obtain synapses and layers without 
        /// traversing the network.
        /// </summary>
        public NeuralStructure Structure
        {
            get
            {
                return this.structure;
            }
        }

        /// <summary>
        /// The size of the matrix.
        /// </summary>
        public int WeightMatrixSize
        {
            get
            {
                int result = 0;
                foreach (ISynapse synapse in this.structure.Synapses)
                {
                    result += synapse.MatrixSize;
                }
                return result;
            }
        }

        /// <summary>
        /// Handle recurrent layers.  See if there are any recurrent layers before
        /// the specified layer that must affect the input.
        /// </summary>
        /// <param name="layer">The layer being processed, see if there are any recurrent
        /// connections to this.</param>
        /// <param name="input">The input to the layer, will be modified with the result
        /// from any recurrent layers.</param>
        /// <param name="source">The source synapse.</param>
        private void HandleRecurrentInput(ILayer layer,
                 INeuralData input, ISynapse source)
        {
            foreach (ISynapse synapse
                    in this.structure.GetPreviousSynapses(layer))
            {
                if (synapse != source)
                {
                    if (BasicNetwork.logger.IsDebugEnabled)
                    {
                        BasicNetwork.logger.Debug("Recurrent layer from: " + input);
                    }
                    INeuralData recurrentInput = synapse.FromLayer
                           .Recur();

                    if (recurrentInput != null)
                    {
                        INeuralData recurrentOutput = synapse
                               .Compute(recurrentInput);

                        for (int i = 0; i < input.Count; i++)
                        {
                            input[i] = input[i]
                                    + recurrentOutput[i];
                        }

                        if (BasicNetwork.logger.IsDebugEnabled)
                        {
                            BasicNetwork.logger.Debug("Recurrent layer to: " + input);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate a hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Called to cause the network to attempt to infer which layer should be
        /// the output layer.
        /// </summary>
        public void InferOutputLayer()
        {
            // set the output layer to null, if we can figure it out it will be set
            // to something else
            this.outputLayer = null;

            // if we do not know the input layer, then there is no way to infer the
            // output layer
            if (this.InputLayer == null)
            {
                return;
            }

            this.outputLayer = InferOutputLayer(this.inputLayer);
        }

        /// <summary>
        /// Internal method that allows the use of recurrsion to determine
        /// the output layer.
        /// </summary>
        /// <param name="layer">The layer currently being evaluated.</param>
        /// <returns>The potential output layer.</returns>
        private ILayer InferOutputLayer(ILayer layer)
        {
            foreach (ISynapse synapse in layer.Next)
            {
                if (synapse.IsTeachable && !synapse.IsSelfConnected)
                {
                    return InferOutputLayer(synapse.ToLayer);
                }
            }

            return layer;
        }

        /// <summary>
        /// Determine if this layer is hidden.
        /// </summary>
        /// <param name="layer">The layer to evaluate.</param>
        /// <returns>True if this layer is a hidden layer.</returns>
        public bool IsHidden(ILayer layer)
        {
            return !IsInput(layer) && !IsOutput(layer);
        }

        /// <summary>
        /// Determine if this layer is the input layer.
        /// </summary>
        /// <param name="layer">The layer to evaluate.</param>
        /// <returns>True if this layer is the input layer.</returns>
        public bool IsInput(ILayer layer)
        {
            return this.inputLayer == layer;
        }

        /// <summary>
        /// Determine if this layer is the output layer.
        /// </summary>
        /// <param name="layer">The layer to evaluate.</param>
        /// <returns>True if this layer is the output layer.</returns>
        public bool IsOutput(ILayer layer)
        {
            return this.outputLayer == layer;
        }

        /// <summary>
        /// Reset the weight matrix and the thresholds.
        /// </summary>
        public void Reset()
        {
            (new RangeRandomizer(-1, 1)).Randomize(this);
        }


        /// <summary>
        /// Convert this object to a string.
        /// </summary>
        /// <returns>The object as a string.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[BasicNetwork: Layers=");
            int layers = this.structure.Layers.Count;
            builder.Append(layers);
            builder.Append("]");
            return builder.ToString();
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
            return DetermineWinner(output);
        }

        /// <summary>
        /// Determine which member of the output is the winning neuron.
        /// </summary>
        /// <param name="output">The output from the neural network.</param>
        /// <returns>The winning neuron.</returns>
        public static int DetermineWinner(INeuralData output)
        {

            int win = 0;

            double biggest = double.MinValue;
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
    }

}
