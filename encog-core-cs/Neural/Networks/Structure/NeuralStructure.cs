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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.MathUtil;

#if logging
using log4net;
#endif

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
        /// and threshold values. 
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

            // determine size from threshold values
            foreach (ILayer layer in this.network.Structure.Layers)
            {
                if (layer.HasThreshold)
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

            AssignID();
            this.network.Logic.Init(this.network);
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
    }
}
