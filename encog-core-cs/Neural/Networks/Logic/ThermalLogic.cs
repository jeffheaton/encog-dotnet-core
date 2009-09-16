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
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
using Encog.Neural.NeuralData.Bipolar;
using Encog.Neural.Data;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for thermal networks.  Functions as a base 
    /// class for BoltzmannLogic and HopfieldLogic.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
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

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ThermalLogic));
#endif

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
