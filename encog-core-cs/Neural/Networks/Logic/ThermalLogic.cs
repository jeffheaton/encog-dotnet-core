using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for thermal networks.  Functions as a base 
    /// class for BoltzmannLogic and HopfieldLogic.
    /// </summary>
    [Serializable]
    public class ThermalLogic : SimpleRecurrentLogic
    {
        /// <summary>
        /// The thermal layer that is to be used.
        /// </summary>
        private ILayer thermalLayer;

        /// <summary>
        /// The thermal layer's single self-connected synapse.
        /// </summary>
        private ISynapse thermalSynapse;

        /// <summary>
        /// The current state of the thermal network.
        /// </summary>
        private BiPolarNeuralData currentState;

        /// <summary>
        /// Get the neuron count for the network.
        /// </summary>
        public int NeuronCount
        {
            get
            {
                return this.thermalLayer.NeuronCount;
            }
        }

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ThermalLogic));

        /// <summary>
        /// Calculate the current energy for the network.  The 
        /// network will seek to lower this value.
        /// </summary>
        /// <returns>The energy value.</returns>
        public double CalculateEnergy()
        {
            double tempE = 0;
            int neuronCount = this.NeuronCount;

            for (int i = 0; i < neuronCount; i++)
                for (int j = 0; j < neuronCount; j++)
                    if (i != j)
                        tempE += this.thermalSynapse.WeightMatrix[i, j]
                        * this.currentState[i]
                        * this.currentState[j];
            return -1 * tempE / 2;

        }

        /// <summary>
        /// Clear any connection weights.
        /// </summary>
        public void Clear()
        {
            this.thermalSynapse.WeightMatrix.Clear();
        }

        /// <summary>
        /// The main thermal layer.
        /// </summary>
        public ILayer ThermalLayer
        {
            get
            {
                return thermalLayer;
            }
        }

        /// <summary>
        /// The thermal synapse.
        /// </summary>
        public ISynapse ThermalSynapse
        {
            get
            {
                return thermalSynapse;
            }
        }

        /// <summary>
        /// The current state of the network.
        /// </summary>
        public BiPolarNeuralData CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    this.currentState[i] = value[i];
                }
            }
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public override void Init(BasicNetwork network)
        {
            base.Init(network);
            // hold references to parts of the network we will need later
            this.thermalLayer = this.Network.GetLayer(BasicNetwork.TAG_INPUT);
            this.thermalSynapse = this.Network.Structure.FindSynapse(this.thermalLayer, this.thermalLayer, true);
            this.currentState = new BiPolarNeuralData(this.thermalLayer.NeuronCount);
        }
    }
}
