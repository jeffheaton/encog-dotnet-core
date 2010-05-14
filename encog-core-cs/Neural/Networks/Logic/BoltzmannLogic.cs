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
using Encog.MathUtil;
using Encog.MathUtil.Randomize;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Data;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Logic
{
    /// <summary>
    /// Provides the neural logic for an Boltzmann type network.  See BoltzmannPattern
    /// for more information on this type of network.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BoltzmannLogic : ThermalLogic
    {
        /// <summary>
        /// Neural network property, the number of cycles to run.
        /// </summary>
        public const String PROPERTY_RUN_CYCLES = "RCYCLE";

        /// <summary>
        /// Neural network property, the number of annealing cycles to run.
        /// </summary>
        public const String PROPERTY_ANNEAL_CYCLES = "ACYCLE";

        /// <summary>
        /// /Neural network property, the temperature.
        /// </summary>
        public const String PROPERTY_TEMPERATURE = "TEMPERATURE";

        /// <summary>
        /// The current temperature of the neural network.  The higher the 
        /// temperature, the more random the network will behave.
        /// </summary>
        private double temperature;

        /// <summary>
        /// Count used to internally determine if a neuron is "on".
        /// </summary>
        private int[] on;

        /// <summary>
        /// Count used to internally determine if a neuron is "off".
        /// </summary>
        private int[] off;

        /// <summary>
        /// The number of cycles to anneal for.
        /// </summary>
        private int annealCycles;

        /// <summary>
        /// The number of cycles to run the network through before annealing.
        /// </summary>
        private int runCycles;
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(BoltzmannLogic));
#endif
        /// <summary>
        /// Run the network for the specified neuron.
        /// </summary>
        /// <param name="i">The neuron to run for.</param>
        void Run(int i)
        {
            int j;
            double sum, probability;

            int count = this.ThermalSynapse.FromNeuronCount;

            sum = 0;
            for (j = 0; j < count; j++)
            {
                sum += this.ThermalSynapse.WeightMatrix[i, j]
                        * (this.CurrentState.GetBoolean(j) ? 1 : 0);
            }
            sum -= this.ThermalLayer.BiasWeights[i];
            probability = 1 / (1 + BoundMath.Exp(-sum / temperature));
            if (RangeRandomizer.Randomize(0, 1) <= probability)
                this.CurrentState.SetBoolean(i, true);
            else
                this.CurrentState.SetBoolean(i, false);
        }

        /// <summary>
        /// Run the network for all neurons present.
        /// </summary>
        public void Run()
        {
            int count = this.ThermalSynapse.FromNeuronCount;
            for (int i = 0; i < count; i++)
            {
                Run(i);
            }
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// NOT USED, call the run method.
        /// </summary>
        /// <param name="input">Not used</param>
        /// <param name="useHolder">Not used</param>
        /// <returns>Not used</returns>
        public override INeuralData Compute(INeuralData input, NeuralOutputHolder useHolder)
        {
            String str = "Compute on BasicNetwork cannot be used, rather call" +
                    " the run method on the logic class.";
#if logging
            if (logger.IsErrorEnabled)
            {
                logger.Error(str);
            }
#endif
            throw new NeuralNetworkError(str);
        }

        /// <summary>
        /// Run the network until thermal equalibrium is established.
        /// </summary>
        public void EstablishEquilibrium()
        {
            int n, i;

            int count = this.ThermalSynapse.FromNeuronCount;

            for (i = 0; i < count; i++)
            {
                on[i] = 0;
                off[i] = 0;
            }

            for (n = 0; n < runCycles * count; n++)
            {
                Run(i = (int)RangeRandomizer.Randomize(0, count - 1));
            }
            for (n = 0; n < this.annealCycles * count; n++)
            {
                Run(i = (int)RangeRandomizer.Randomize(0, count - 1));
                if (this.CurrentState.GetBoolean(i))
                    on[i]++;
                else
                    off[i]++;
            }

            for (i = 0; i < count; i++)
            {
                this.CurrentState.SetBoolean(i, on[i] > off[i]);
            }
        }

        /// <summary>
        /// /The temperature the network is currently operating at.
        /// </summary>
        public double Temperature
        {
            get
            {
                return this.temperature;
            }
            set
            {
                this.temperature = value;
            }
        }

        /// <summary>
        /// Decrease the temperature by the specified amount.
        /// </summary>
        /// <param name="d">The amount to decrease b</param>
        public void DecreaseTemperature(double d)
        {
            this.temperature *= d;
        }

        /// <summary>
        /// Setup the network logic, read parameters from the network.
        /// </summary>
        /// <param name="network">The network that this logic class belongs to.</param>
        public override void Init(BasicNetwork network)
        {
            base.Init(network);

            ILayer layer = this.Network.GetLayer(BasicNetwork.TAG_INPUT);

            this.on = new int[layer.NeuronCount];
            this.off = new int[layer.NeuronCount];

            this.temperature = this.Network.GetPropertyDouble(BoltzmannLogic.PROPERTY_TEMPERATURE);
            this.runCycles = (int)this.Network.GetPropertyLong(BoltzmannLogic.PROPERTY_RUN_CYCLES);
            this.annealCycles = (int)this.Network.GetPropertyLong(BoltzmannLogic.PROPERTY_ANNEAL_CYCLES);
        }
    }
}
