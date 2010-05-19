// Encog(tm) Artificial Intelligence Framework v2.3
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Synapse;
using Encog.MathUtil;
using Encog.Neural.NeuralData;
using Encog.Neural.Data;
using System.Runtime.Serialization;
using Encog.Neural.Networks.Layers;
using Encog.Persist;
using Encog.Persist.Persistors;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Logic;
using Encog.Neural.Networks.Structure;

#if logging
using log4net;
using Encog.Util;
#endif

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
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicNetwork : INetwork, IContextClearable
    {

        /// <summary>
        /// The input layer.
        /// </summary>
        public const String TAG_INPUT = "INPUT";

        /// <summary>
        /// The output layer.
        /// </summary>
        public const String TAG_OUTPUT = "OUTPUT";

        /// <summary>
        /// Tag used for the connection limit.
        /// </summary>
        public const String TAG_LIMIT = "CONNECTION_LIMIT";

        /// <summary>
        /// The default connection limit.
        /// </summary>
        public const String DEFAULT_CONNECTION_LIMIT = "0.0000000001";


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
        /// This class tells the network how to calculate the output for each of the layers.
        /// </summary>
        private INeuralLogic logic;

        /// <summary>
        /// Properties about the neural network.  Some NeuralLogic classes require certain properties 
        /// to be set.
        /// </summary>
        private IDictionary<String, String> properties = new Dictionary<String, String>();

        /// <summary>
        /// A set of tags to identify special layers.
        /// </summary>
        private IDictionary<String, ILayer> layerTags = new Dictionary<String, ILayer>();

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif

        /// <summary>
        /// Construct an empty neural network.
        /// </summary>
        public BasicNetwork()
        {
            this.structure = new NeuralStructure(this);
            this.logic = new SimpleRecurrentLogic();
        }

        /// <summary>
        /// Construct a basic network with the specified logic.
        /// </summary>
        /// <param name="logic">The logic to use with this network.</param>
        public BasicNetwork(INeuralLogic logic)
        {
            this.structure = new NeuralStructure(this);
            this.logic = logic;
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
            if (this.layerTags.Count == 0)
            {
                this.TagLayer(BasicNetwork.TAG_INPUT, layer);
                this.TagLayer(BasicNetwork.TAG_OUTPUT, layer);
            }
            else
            {
                // add the layer to any previous layers
                ILayer outputLayer = this.GetLayer(BasicNetwork.TAG_OUTPUT);
                outputLayer.AddNext(layer, type);
                this.TagLayer(BasicNetwork.TAG_OUTPUT, layer);
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
            ClearContext();
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
        /// Return a clone of this neural network. Including structure, weights and
        /// threshold values.
        /// </summary>
        /// <returns>A cloned copy of the neural network.</returns>
        public Object Clone()
        {
            return ObjectCloner.DeepCopy(this);
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
            try
            {
                return logic.Compute(input, useHolder);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new NeuralNetworkError(
                        "Index exception: there was likely a mismatch between layer sizes, or the size of the input presented to the network.",
                        ex);
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
            return Equals(other,
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
            return NetworkCODEC.Equals(this, other, precision);
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
#if logging
                    if (BasicNetwork.logger.IsDebugEnabled)
                    {
                        BasicNetwork.logger.Debug("Recurrent layer from: " + input);
                    }
#endif
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
#if logging
                        if (BasicNetwork.logger.IsDebugEnabled)
                        {
                            BasicNetwork.logger.Debug("Recurrent layer to: " + input);
                        }
#endif
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
        /// Reset the weight matrix and the thresholds. This will use a Nguyen-Widrow
        /// randomizer with a range between -1 and 1. If the network does not have an
        /// input, output or hidden layers, then Nguyen-Widrow cannot be used and a
        /// simple range randomize between -1 and 1 will be used.
        /// </summary>
        public void Reset()
        {
            ILayer inputLayer = GetLayer(BasicNetwork.TAG_INPUT);
            ILayer outputLayer = GetLayer(BasicNetwork.TAG_OUTPUT);

            if ( this.structure.Layers.Count<3 
                || inputLayer == null 
                || outputLayer == null)
                (new RangeRandomizer(-1, 1)).Randomize(this);
            else
                (new NguyenWidrowRandomizer(-1, 1)).Randomize(this);
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

        /// <summary>
        /// The neural logic to use.
        /// </summary>
        public INeuralLogic Logic
        {
            get
            {
                return logic;
            }
            set
            {
                this.logic = value;
            }
        }

        /// <summary>
        /// Set a network property as a string.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The string value.</param>
        public void SetProperty(String name, String value)
        {
            this.properties[name] = value;
        }

        /// <summary>
        /// Set a network property as long string.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="l">The long value.</param>
        public void SetProperty(String name, long l)
        {
            this.properties[name] = "" + l;
        }

        /// <summary>
        /// Set a network property as a double.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="d">The double value.</param>
        public void SetProperty(String name, double d)
        {
            this.properties[name] = "" + d;
        }

        /// <summary>
        /// The network properties, a collection of name-value pairs.
        /// </summary>
        public IDictionary<String, String> Properties
        {
            get
            {
                return properties;
            }
        }

        /// <summary>
        /// Get the property as a string.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a string.</returns>
        public String GetPropertyString(String name)
        {
            if (this.properties.ContainsKey(name))
                return (String)this.properties[name];
            else
                return null;
        }

        /// <summary>
        /// Get a network property as a long.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a long.</returns>
        public long GetPropertyLong(String name)
        {
            return long.Parse(this.properties[name]);
        }

        /// <summary>
        /// Get a network property as a double.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The property as a double.</returns>
        public double GetPropertyDouble(String name)
        {
            return double.Parse(this.properties[name]);
        }

        /// <summary>
        /// Get the layer tags.
        /// </summary>
        public IDictionary<String, ILayer> LayerTags
        {
            get
            {
                return layerTags;
            }
        }

        /// <summary>
        /// Tag a layer.
        /// </summary>
        /// <param name="tag">The name of the tag.</param>
        /// <param name="layer">The layer to be tagged.</param>
        public void TagLayer(String tag, ILayer layer)
        {
            this.layerTags[tag] = layer;
        }

        /// <summary>
        /// Clear all network tags.
        /// </summary>
        public void ClearLayerTags()
        {
            this.layerTags.Clear();
        }

        /// <summary>
        /// Get a layer using a tag name.
        /// </summary>
        /// <param name="tag">The tag name.</param>
        /// <returns>The layer.</returns>
        public ILayer GetLayer(String tag)
        {
            if (!this.layerTags.ContainsKey(tag))
                return null;
            else
                return this.layerTags[tag];
        }

        /// <summary>
        /// Get a list of all of the tags on a specific layer.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        /// <returns>A collection of the layer tags.</returns>
        public ICollection<String> GetTags(ILayer layer)
        {
            ICollection<String> result = new List<String>();

            foreach (String key in this.layerTags.Keys)
            {
                ILayer l = this.layerTags[key];
                if (l == layer)
                {
                    result.Add(key);
                }
            }

            return result;
        }

        /// <summary>
        /// Clear any data from any context layers.
        /// </summary>
        public void ClearContext()
        {
            foreach (ILayer layer in this.structure.Layers)
            {
                if (layer is IContextClearable)
                {
                    ((IContextClearable)layer).ClearContext();
                }
            }

            foreach (ISynapse synapse in this.structure.Synapses)
            {
                if (synapse is IContextClearable)
                {
                    ((IContextClearable)synapse).ClearContext();
                }
            }

        }

        public bool IsConnected(ISynapse synapse, int fromNeuron, int toNeuron)
        {
            if (!this.structure.IsConnectionLimited)
                return true;
            double value = synapse.WeightMatrix[fromNeuron, toNeuron];

            return (Math.Abs(value) > this.structure.ConnectionLimit);
        }

        public void EnableConnection(ISynapse synapse, int fromNeuron, int toNeuron, bool enable)
        {
            if (synapse.WeightMatrix == null)
            {
                throw new NeuralNetworkError("Can't enable/disable connection on a synapse that does not have a weight matrix.");
            }

            double value = synapse.WeightMatrix[fromNeuron, toNeuron];

            if (enable)
            {
                if (!this.structure.IsConnectionLimited)
                    return;

                if (Math.Abs(value) < this.structure.ConnectionLimit)
                    synapse.WeightMatrix[fromNeuron, toNeuron] = RangeRandomizer.Randomize(-1, 1);
            }
            else
            {
                if (!this.structure.IsConnectionLimited)
                {
                    this.Properties[BasicNetwork.TAG_LIMIT] = BasicNetwork.DEFAULT_CONNECTION_LIMIT;
                    this.structure.FinalizeStructure();
                }
                synapse.WeightMatrix[fromNeuron, toNeuron] = 0;
            }
        }

        /// <summary>
        /// Set the bias activation on all layers.
        /// </summary>
        public double BiasActivation
        {
            set
            {
                foreach (ILayer layer in Structure.Layers)
                {
                    layer.BiasActivation = value;
                }
            }
        }
    }
}
