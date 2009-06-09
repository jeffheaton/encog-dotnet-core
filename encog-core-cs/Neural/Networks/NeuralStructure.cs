using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using log4net;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Holds "cached" information about the structure of the neural network. This is
    /// a very good performance boost since the neural network does not need to
    /// traverse itself each time a complete collection of layers or synapses is needed.
    /// </summary>
    public class NeuralStructure
    {
        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        private IList<ILayer> layers = new List<ILayer>();

        /// <summary>
        /// The synapses in this neural network.
        /// </summary>
        private IList<ISynapse> synapses = new List<ISynapse>();

        /// <summary>
        /// The neural network this class belongs to.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(NeuralStructure));

        /// <summary>
        /// Construct a structure object for the specified network.
        /// </summary>
        /// <param name="network">The network to construct a structure for.</param>
        public NeuralStructure(BasicNetwork network)
        {
            this.network = network;
        }

        /// <summary>
        /// Build the layer structure.
        /// </summary>
        private void FinalizeLayers()
        {
            this.layers.Clear();
            if (this.network.InputLayer != null)
            {
                GetLayers(this.layers, this.network.InputLayer);
            }
        }

        /// <summary>
        /// Build the synapse and layer structure.  This method should be called 
        /// after you are done adding layers to a network.
        /// </summary>
        public void FinalizeStructure()
        {
            FinalizeLayers();
            FinalizeSynapses();
        }

        /// <summary>
        /// Build the synapse structure.
        /// </summary>
        private void FinalizeSynapses()
        {
            this.synapses.Clear();
            foreach (ILayer layer in this.Layers)
            {
                foreach (ISynapse synapse in layer.Next)
                {
                    if (!this.synapses.Contains(synapse))
                        this.synapses.Add(synapse);
                }
            }

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
        /// <param name="result">The layer list. </param>
        /// <param name="layer">The current layer being processed.</param>
        private void GetLayers(IList<ILayer> result,
                 ILayer layer)
        {

            if (!result.Contains(layer))
            {
                result.Add(layer);
            }

            foreach (ISynapse synapse in layer.Next)
            {
                ILayer nextLayer = synapse.ToLayer;

                if (!result.Contains(nextLayer))
                {
                    GetLayers(result, nextLayer);
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
        public ICollection<ISynapse> GetPreviousSynapses(ILayer targetLayer)
        {

            ICollection<ISynapse> result = new HashSet<ISynapse>();

            foreach (ISynapse synapse in this.synapses)
            {
                if (synapse.ToLayer == targetLayer)
                {
                    result.Add(synapse);
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
    }

}
