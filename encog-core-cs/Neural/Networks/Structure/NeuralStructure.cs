using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.Neural.Flat;
using Encog.Neural.Networks.Layers;
using Encog.Util.CSV;

namespace Encog.Neural.Networks.Structure
{
    /// <summary>
    /// Holds "cached" information about the structure of the neural network. This is
    /// a very good performance boost since the neural network does not need to
    /// traverse itself each time a complete collection of layers or synapses is
    /// needed.
    /// </summary>
    ///
    [Serializable]
    public class NeuralStructure
    {
        /// <summary>
        /// The layers in this neural network.
        /// </summary>
        ///
        private readonly IList<Layer> layers;

        /// <summary>
        /// The neural network this class belongs to.
        /// </summary>
        ///
        private readonly BasicNetwork network;

        /// <summary>
        /// The limit, below which a connection is treated as zero.
        /// </summary>
        ///
        private double connectionLimit;

        /// <summary>
        /// Are connections limited?
        /// </summary>
        ///
        private bool connectionLimited;

        /// <summary>
        /// The flattened form of the network.
        /// </summary>
        ///
        private FlatNetwork flat;

        /// <summary>
        /// Construct a structure object for the specified network.
        /// </summary>
        ///
        /// <param name="network_0">The network to construct a structure for.</param>
        public NeuralStructure(BasicNetwork network_0)
        {
            layers = new List<Layer>();
            network = network_0;
        }


        /// <value>The connection limit.</value>
        public double ConnectionLimit
        {
            /// <returns>The connection limit.</returns>
            get { return connectionLimit; }
        }


        /// <summary>
        /// Set the flat network.
        /// </summary>
        ///
        /// <value>The flat network.</value>
        public FlatNetwork Flat
        {
            /// <returns>The flat network.</returns>
            get
            {
                RequireFlat();
                return flat;
            }
            /// <summary>
            /// Set the flat network.
            /// </summary>
            ///
            /// <param name="flat_0">The flat network.</param>
            set { flat = value; }
        }


        /// <value>The layers in this neural network.</value>
        public IList<Layer> Layers
        {
            /// <returns>The layers in this neural network.</returns>
            get { return layers; }
        }


        /// <value>The network this structure belongs to.</value>
        public BasicNetwork Network
        {
            /// <returns>The network this structure belongs to.</returns>
            get { return network; }
        }


        /// <value>True if this is not a fully connected feedforward network.</value>
        public bool ConnectionLimited
        {
            /// <returns>True if this is not a fully connected feedforward network.</returns>
            get { return connectionLimited; }
        }

        /// <summary>
        /// Calculate the size that an array should be to hold all of the weights and
        /// bias values.
        /// </summary>
        ///
        /// <returns>The size of the calculated array.</returns>
        public int CalculateSize()
        {
            return NetworkCODEC.NetworkSize(network);
        }

        /// <summary>
        /// Enforce that all connections are above the connection limit. Any
        /// connections below this limit will be severed.
        /// </summary>
        ///
        public void EnforceLimit()
        {
            if (!connectionLimited)
            {
                return;
            }

            double[] weights = flat.Weights;

            for (int i = 0; i < weights.Length; i++)
            {
                if (Math.Abs(weights[i]) < connectionLimit)
                {
                    weights[i] = 0;
                }
            }
        }

        /// <summary>
        /// Parse/finalize the limit value for connections.
        /// </summary>
        ///
        private void FinalizeLimit()
        {
            // see if there is a connection limit imposed
            String limit = network
                .GetPropertyString(BasicNetwork.TAG_LIMIT);
            if (limit != null)
            {
                try
                {
                    connectionLimited = true;
                    connectionLimit = CSVFormat.EG_FORMAT.Parse(limit);
                }
                catch (FormatException e)
                {
                    throw new NeuralNetworkError("Invalid property("
                                                 + BasicNetwork.TAG_LIMIT + "):" + limit);
                }
            }
            else
            {
                connectionLimited = false;
                connectionLimit = 0;
            }
        }

        /// <summary>
        /// Build the synapse and layer structure. This method should be called after
        /// you are done adding layers to a network, or change the network's logic
        /// property.
        /// </summary>
        ///
        public void FinalizeStructure()
        {
            if (layers.Count < 2)
            {
                throw new NeuralNetworkError(
                    "There must be at least two layers before the structure is finalized.");
            }

            var flatLayers = new FlatLayer[layers.Count];

            for (int i = 0; i < layers.Count; i++)
            {
                var layer = (BasicLayer) layers[i];
                if (layer.Activation == null)
                {
                    layer.Activation = new ActivationLinear();
                }

                flatLayers[i] = layer;
            }

            flat = new FlatNetwork(flatLayers);

            FinalizeLimit();
            layers.Clear();
            EnforceLimit();
        }


        /// <summary>
        /// Throw an error if there is no flat network.
        /// </summary>
        ///
        public void RequireFlat()
        {
            if (flat == null)
            {
                throw new NeuralNetworkError(
                    "Must call finalizeStructure before using this network.");
            }
        }

        /// <summary>
        /// Update any properties from the property map.
        /// </summary>
        ///
        public void UpdateProperties()
        {
            if (network.Properties.ContainsKey(BasicNetwork.TAG_LIMIT))
            {
                connectionLimit = network
                    .GetPropertyDouble(BasicNetwork.TAG_LIMIT);
                connectionLimited = true;
            }
            else
            {
                connectionLimited = false;
                connectionLimit = 0;
            }

            if (flat != null)
            {
                flat.ConnectionLimit = connectionLimit;
            }
        }
    }
}