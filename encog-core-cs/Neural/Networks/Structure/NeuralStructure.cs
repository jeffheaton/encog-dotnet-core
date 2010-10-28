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

#if logging
using log4net;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.MathUtil;
using Encog.MathUtil.Matrices;
using Encog.Engine.Network.Flat;
using Encog.Engine.Util;
using Encog.Engine.Network.Activation;



namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Holds "cached" information about the structure of the neural network. This is
    /// a very good performance boost since the neural network does not need to
    /// traverse itself each time a complete collection of layers or synapses is
    /// needed.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class NeuralStructure
    {

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(NeuralStructure));
#endif

        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        private List<ILayer> layers = new List<ILayer>();

        /// <summary>
        /// The synapses in this neural network.
        /// </summary>
        private List<ISynapse> synapses = new List<ISynapse>();

        /// <summary>
        /// The neural network this class belongs to.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The next ID to be assigned to a layer.
        /// </summary>
        private int nextID = 1;

        private double connectionLimit;
        private bool connectionLimited;

        /// <summary>
        /// The flattened form of the network.
        /// </summary>
        [NonSerialized]
        private FlatNetwork flat;

        /// <summary>
        /// What type of update is needed to the flat network.
        /// </summary>
        [NonSerialized]
        private FlatUpdateNeeded flatUpdate;


        /// <summary>
        /// Construct a structure object for the specified network. 
        /// </summary>
        /// <param name="network">The network to construct a structure for.</param>
        public NeuralStructure(BasicNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        /// Assign an ID to every layer that does not already have one.
        /// </summary>
        public void AssignID()
        {
            foreach (ILayer layer in this.layers)
            {
                AssignID(layer);
            }
            Sort();
        }

        /// <summary>
        /// Assign an ID to the specified layer. 
        /// </summary>
        /// <param name="layer">The layer to get an ID assigned.</param>
        public void AssignID(ILayer layer)
        {
            if (layer.ID == -1)
            {
                layer.ID = GetNextID();
            }
        }

        /// <summary>
        /// Calculate the size that an array should be to hold all of the weights
        /// and bias values. 
        /// </summary>
        /// <returns>The size of the calculated array.</returns>
        public int CalculateSize()
        {
            int size = 0;

            // first determine size from matrixes
            foreach (ISynapse synapse
                    in this.network.Structure.Synapses)
            {
                size += synapse.MatrixSize;
            }

            // determine size from bias values
            foreach (ILayer layer in this.network.Structure.Layers)
            {
                if (layer.HasBias)
                {
                    size += layer.NeuronCount;
                }
            }
            return size;
        }


        /// <summary>
        /// Determine if the network contains a layer of the specified type. 
        /// </summary>
        /// <param name="type">The layer type we are looking for.</param>
        /// <returns>True if this layer type is present.</returns>
        public bool ContainsLayerType(Type type)
        {
            foreach (ILayer layer in this.layers)
            {
                if (layer.GetType().IsInstanceOfType(type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Build the layer structure.
        /// </summary>
        private void FinalizeLayers()
        {

            this.layers.Clear();

            foreach (ILayer layer in this.network.LayerTags.Values)
            {
                GetLayers(layer);
            }

            // make sure that the current ID is not going to cause a repeat
            foreach (ILayer layer in this.layers)
            {
                if (layer.ID >= this.nextID)
                {
                    this.nextID = layer.ID + 1;
                }
            }

            Sort();
        }

        /// <summary>
        /// Build the synapse and layer structure. This method should be called after
        /// you are done adding layers to a network, or change the network's logic
        /// property.
        /// </summary>
        public void FinalizeStructure()
        {
            FinalizeLayers();
            FinalizeSynapses();
            FinalizeLimit();
            this.layers.Sort();
            AssignID();
            this.network.Logic.Init(this.network);
            EnforceLimit();
            Flatten();
        }

        /// <summary>
        /// Build the synapse structure.
        /// </summary>
        private void FinalizeSynapses()
        {

            this.synapses.Clear();

            foreach (ILayer layer in Layers)
            {
                foreach (ISynapse synapse in layer.Next)
                {
                    this.synapses.Add(synapse);
                }
            }
        }

        /// <summary>
        /// Find the specified synapse, throw an error if it is required. 
        /// </summary>
        /// <param name="fromLayer">The from layer.</param>
        /// <param name="toLayer">The to layer.</param>
        /// <param name="required">Is this required?</param>
        /// <returns>The synapse, if it exists, otherwise null.</returns>
        public ISynapse FindSynapse(ILayer fromLayer, ILayer toLayer,
                 bool required)
        {
            ISynapse result = null;
            foreach (ISynapse synapse in Synapses)
            {
                if ((synapse.FromLayer == fromLayer)
                        && (synapse.ToLayer == toLayer))
                {
                    result = synapse;
                    break;
                }
            }

            if (required && (result == null))
            {
                String str =
               "This operation requires a network with a synapse between the "
                       + NameLayer(fromLayer)
                       + " layer to the "
                       + NameLayer(toLayer) + " layer.";
#if logging
                if (NeuralStructure.logger.IsErrorEnabled)
                {
                    NeuralStructure.logger.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }

            return result;
        }

        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        public IList<ILayer> Layers
        {
            get
            {
                return this.layers;
            }
        }

        /// <summary>
        /// Called to help build the layer structure. 
        /// </summary>
        /// <param name="layer">The current layer being processed.</param>
        private void GetLayers(ILayer layer)
        {

            if (!this.layers.Contains(layer))
            {
                this.layers.Add(layer);
            }

            foreach (ISynapse synapse in layer.Next)
            {
                ILayer nextLayer = synapse.ToLayer;

                if (!this.layers.Contains(nextLayer))
                {
                    GetLayers(nextLayer);
                }
            }
        }

        /// <summary>
        /// The network this structure belongs to.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Get the next layer id. 
        /// </summary>
        /// <returns>The next layer id.</returns>
        public int GetNextID()
        {
            return this.nextID++;
        }

        /// <summary>
        /// Get the previous layers from the specified layer. 
        /// </summary>
        /// <param name="targetLayer">The target layer.</param>
        /// <returns>The previous layers.</returns>
        public ICollection<ILayer> GetPreviousLayers(ILayer targetLayer)
        {
            ICollection<ILayer> result = new List<ILayer>();
            foreach (ILayer layer in this.Layers)
            {
                foreach (ISynapse synapse in layer.Next)
                {
                    if (synapse.ToLayer == targetLayer)
                    {
                        if (!result.Contains(synapse.FromLayer))
                        {
                            result.Add(synapse.FromLayer);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get the previous synapses. 
        /// </summary>
        /// <param name="targetLayer">The layer to get the previous layers from.</param>
        /// <returns>A collection of synapses.</returns>
        public IList<ISynapse> GetPreviousSynapses(ILayer targetLayer)
        {

            IList<ISynapse> result = new List<ISynapse>();

            foreach (ISynapse synapse in this.synapses)
            {
                if (synapse.ToLayer == targetLayer)
                {
                    if (!result.Contains(synapse))
                    {
                        result.Add(synapse);
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// All synapses in the neural network.
        /// </summary>
        public IList<ISynapse> Synapses
        {
            get
            {
                return this.synapses;
            }
        }

        /// <summary>
        /// Obtain a name for the specified layer. 
        /// </summary>
        /// <param name="layer">The layer to name.</param>
        /// <returns>The name of this layer.</returns>
        public IList<String> NameLayer(ILayer layer)
        {
            IList<String> result = new List<String>();

            foreach (KeyValuePair<String, ILayer> entry in this.network.LayerTags)
            {
                if (entry.Value == layer)
                {
                    result.Add(entry.Key);
                }
            }

            return result;
        }

        /// <summary>
        /// Sort the layers and synapses.
        /// </summary>
        public void Sort()
        {
            this.layers.Sort(new LayerComparator(this));
            this.synapses.Sort(new SynapseComparator(this));
        }

        /// <summary>
        /// The current connection limit, any connections below this limit 
        /// are considered disabled.
        /// </summary>
        public double ConnectionLimit
        {
            get
            {
                return this.connectionLimit;
            }
        }

        /// <summary>
        /// True if connection limiting is being used.  If false, all layers 
        /// are fully connected.
        /// </summary>
        public bool IsConnectionLimited
        {
            get
            {
                return this.connectionLimited;
            }
        }

        /// <summary>
        /// Read the connection limit value from the network properties and 
        /// finalize the connection limiting.
        /// </summary>
        private void FinalizeLimit()
        {
            // see if there is a connection limit imposed
            String limit = this.network.GetPropertyString(BasicNetwork.TAG_LIMIT);
            if (limit != null)
            {
                try
                {
                    this.connectionLimited = true;
                    this.connectionLimit = double.Parse(limit);
                }
                catch (Exception)
                {
                    throw new NeuralNetworkError(
                            "Invalid property("
                            + BasicNetwork.TAG_LIMIT
                            + "):"
                            + limit);
                }
            }
            else
            {
                this.connectionLimited = false;
                this.connectionLimit = 0;
            }
        }

        /// <summary>
        /// Enforce the connection limit.  Any connections below the limit are removed.
        /// </summary>
        public void EnforceLimit()
        {
            if (!this.connectionLimited)
                return;

            foreach (ISynapse synapse in this.synapses)
            {
                Matrix matrix = synapse.WeightMatrix;
                if (matrix != null)
                {
                    for (int row = 0; row < matrix.Rows; row++)
                    {
                        for (int col = 0; col < matrix.Cols; col++)
                        {
                            double value = matrix[row, col];
                            if (Math.Abs(value) < this.connectionLimit)
                            {
                                matrix[row, col] = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is this network recurrent?  A recurrent network has context layers.
        /// </summary>
        /// <returns>True if this network is recurrent.</returns>
        public bool IsRecurrent()
        {
            foreach (ILayer layer in this.layers)
            {
                if (layer is ContextLayer)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Create the flat neural network.
        /// </summary>
        public void Flatten()
        {
            bool isRBF = false;
            IDictionary<ILayer, FlatLayer> regular2flat = new Dictionary<ILayer, FlatLayer>();
            IList<ObjectPair<ILayer, ILayer>> contexts = new List<ObjectPair<ILayer, ILayer>>();
            this.flat = null;

            ValidateForFlat val = new ValidateForFlat();

            if (val.IsValid(this.network) == null)
            {
                if (this.layers.Count == 3
                        && this.layers[1] is RadialBasisFunctionLayer)
                {
                    RadialBasisFunctionLayer rbf = (RadialBasisFunctionLayer)this.layers[1];
                    this.flat = new FlatNetworkRBF(this.network.InputCount,
                            rbf.NeuronCount, this.network.OutputCount,
                            rbf.RadialBasisFunction);
                    FlattenWeights();
                    this.flatUpdate = FlatUpdateNeeded.None;
                    return;
                }

                FlatLayer[] flatLayers = new FlatLayer[CountNonContext()];

                int index = flatLayers.Length - 1;
                foreach (ILayer layer in this.layers)
                {

                    if (layer is ContextLayer)
                    {
                        ISynapse inboundSynapse = network.Structure
                                .FindPreviousSynapseByLayerType(layer,
                                        typeof(BasicLayer));
                        ISynapse outboundSynapse = network
                                .Structure
                                .FindNextSynapseByLayerType(layer, typeof(BasicLayer));

                        if (inboundSynapse == null)
                            throw new NeuralNetworkError(
                                    "Context layer must be connected to by one BasicLayer.");

                        if (outboundSynapse == null)
                            throw new NeuralNetworkError(
                                    "Context layer must connect to by one BasicLayer.");

                        ILayer inbound = inboundSynapse.FromLayer;
                        ILayer outbound = outboundSynapse.ToLayer;

                        contexts
                                .Add(new ObjectPair<ILayer, ILayer>(inbound, outbound));
                    }
                    else
                    {
                        double bias = this.FindNextBias(layer);

                        IActivationFunction activationType;
                        double[] param = new double[1];

                        if (layer.ActivationFunction == null)
                        {
                            activationType = new ActivationLinear();
                            param = new double[1];
                            param[0] = 1;
                        }
                        else
                        {
                            activationType = layer.ActivationFunction;
                            param = layer.ActivationFunction.Params;
                        }

                        FlatLayer flatLayer = new FlatLayer(activationType, layer
                                .NeuronCount, bias, param);

                        regular2flat[layer] = flatLayer;
                        flatLayers[index--] = flatLayer;
                    }
                }

                // now link up the context layers
                foreach (ObjectPair<ILayer, ILayer> context in contexts)
                {
                    ILayer layer = context.B;
                    ISynapse synapse = this.network
                            .Structure
                            .FindPreviousSynapseByLayerType(layer, typeof(BasicLayer));
                    FlatLayer from = regular2flat[context.A];
                    FlatLayer to = regular2flat[synapse.FromLayer];
                    to.ContextFedBy = from;
                }

                this.flat = new FlatNetwork(flatLayers);

                if (isRBF)
                {
                    this.flat.EndTraining = flatLayers.Length - 1;
                }

                FlattenWeights();

                if (this.IsConnectionLimited)
                {

                }

                this.flatUpdate = FlatUpdateNeeded.None;
            }
            else
                this.flatUpdate = FlatUpdateNeeded.Never;
        }

        /// <summary>
        /// Flatten the weights of a neural network.
        /// </summary>
        public void FlattenWeights()
        {
            if (this.flat != null)
            {
                this.flatUpdate = FlatUpdateNeeded.Flatten;

                double[] targetWeights = this.flat.Weights;
                double[] sourceWeights = NetworkCODEC.NetworkToArray(this.network);

                EngineArray.ArrayCopy(sourceWeights, targetWeights);
                this.flatUpdate = FlatUpdateNeeded.None;

                // handle limited connection networks
                if (this.connectionLimited)
                {
                    this.flat.ConnectionLimit = this.connectionLimit;
                }
                else
                {
                    this.flat.ClearConnectionLimit();
                }
            }
        }

        /// <summary>
        /// The type of flat update that is needed.
        /// </summary>
        public FlatUpdateNeeded FlatUpdate
        {
            get
            {
                return flatUpdate;
            }
            set
            {
                flatUpdate = value;
            }
        }

        /// <summary>
        /// The flat network.
        /// </summary>
        public FlatNetwork Flat
        {
            get
            {
                return flat;
            }
        }

        /// <summary>
        /// Update the flat network.  Either flatten or unflatten as needed.
        /// </summary>
        public void UpdateFlatNetwork()
        {

            // if flatUpdate is null, the network was likely just loaded from a  serialized file
            if (this.flatUpdate == null)
            {
                FlattenWeights();
                this.flatUpdate = FlatUpdateNeeded.None;
            }

            switch (this.flatUpdate)
            {

                case FlatUpdateNeeded.Flatten:
                    FlattenWeights();
                    break;

                case FlatUpdateNeeded.Unflatten:
                    UnflattenWeights();
                    break;

                case FlatUpdateNeeded.None:
                case FlatUpdateNeeded.Never:
                    return;
            }

            this.flatUpdate = FlatUpdateNeeded.None;
        }

        /// <summary>
        /// Find the next bias layer for a given layer.
        /// </summary>
        /// <param name="layer">The layer to search from.</param>
        /// <returns>The layer bias.</returns>
        private double FindNextBias(ILayer layer)
        {
            double bias = FlatNetwork.NO_BIAS_ACTIVATION;

            if (layer.Next.Count > 0)
            {
                ISynapse synapse = network.Structure
                        .FindNextSynapseByLayerType(layer, typeof(BasicLayer));
                if (synapse != null)
                {
                    ILayer nextLayer = synapse.ToLayer;
                    if (nextLayer.HasBias)
                        bias = nextLayer.BiasActivation;
                }
            }
            return bias;
        }

        /// <summary>
        /// Unflatten the weights, copy the flat network weights to the
        /// neural network weight matrixes.
        /// </summary>
        public void UnflattenWeights()
        {
            double[] sourceWeights = flat.Weights;
            NetworkCODEC.ArrayToNetwork(sourceWeights, network);
            this.flatUpdate = FlatUpdateNeeded.None;
        }

        /// <summary>
        /// Count the non-context layers in a network.
        /// </summary>
        /// <returns>The number of layers that are not contextual.</returns>
        private int CountNonContext()
        {
            int result = 0;

            foreach (ILayer layer in this.Layers)
            {
                if (layer.GetType() != typeof(ContextLayer))
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Find the previous synapse of a layer type.
        /// </summary>
        /// <param name="layer">The layer to search from.</param>
        /// <param name="type">The layer type.</param>
        /// <returns></returns>

        public ISynapse FindPreviousSynapseByLayerType(ILayer layer,
                Type type)
        {
            foreach (ISynapse synapse in GetPreviousSynapses(layer))
            {
                if (synapse.FromLayer.GetType() == type)
                    return synapse;
            }
            return null;
        }

        /// <summary>
        /// Find the next synapse of a layer type.
        /// </summary>
        /// <param name="layer">The layer to search from.</param>
        /// <param name="type">The layer type.</param>
        /// <returns></returns>
        public ISynapse FindNextSynapseByLayerType(ILayer layer,
                Type type)
        {
            foreach (ISynapse synapse in layer.Next)
            {
                if (synapse.ToLayer.GetType() == type)
                    return synapse;
            }
            return null;
        }

    }
}
