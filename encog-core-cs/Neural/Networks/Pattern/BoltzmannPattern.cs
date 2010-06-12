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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Logic;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Pattern to create a Boltzmann machine.
    /// </summary>
    public class BoltzmannPattern : INeuralNetworkPattern
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ADALINEPattern));
#endif

        /// <summary>
        /// The number of neurons in the Boltzmann network.
        /// </summary>
        private int neuronCount;

        /// <summary>
        /// The number of annealing cycles per run.
        /// </summary>
        private int annealCycles = 100;

        /// <summary>
        /// The number of cycles per run.
        /// </summary>
        private int runCycles = 1000;

        /// <summary>
        /// The current temperature.
        /// </summary>
        private double temperature = 0.0;

        /// <summary>
        /// Not supported, will throw an exception, Boltzmann networks have
        /// no hidden layers.
        /// </summary>
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "A Boltzmann network has no hidden layers.";
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PatternError(str);
        }

        /// <summary>
        /// Clear any properties set on this network.
        /// </summary>
        public void Clear()
        {
            this.neuronCount = 0;

        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        /// <returns>The generated network.</returns>
        public BasicNetwork Generate()
        {
            ILayer layer = new BasicLayer(new ActivationBiPolar(), true,
                    this.neuronCount);

            BasicNetwork result = new BasicNetwork(new BoltzmannLogic());
            result.SetProperty(BoltzmannLogic.PROPERTY_ANNEAL_CYCLES, this.annealCycles);
            result.SetProperty(BoltzmannLogic.PROPERTY_RUN_CYCLES, this.runCycles);
            result.SetProperty(BoltzmannLogic.PROPERTY_TEMPERATURE, this.temperature);
            result.AddLayer(layer);
            layer.AddNext(layer);
            layer.X = PatternConst.START_X;
            layer.Y = PatternConst.START_Y;
            result.Structure.FinalizeStructure();
            result.Reset();
            return result;
        }

        /// <summary>
        /// Not used, will throw an exception.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            get
            {
                String str =
                    "A Boltzmann network will use the BiPolar activation "
                    + "function, no activation function needs to be specified.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);
            }
            set
            {
                String str =
                    "A Boltzmann network will use the BiPolar activation "
                    + "function, no activation function needs to be specified.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);
            }
        }

        /// <summary>
        /// Number of neurons.
        /// </summary>
        public int OutputNeurons
        {
            set
            {
                this.neuronCount = value;
            }
            get
            {
                return this.neuronCount;
            }
        }

        /// <summary>
        /// Number of neurons.
        /// </summary>
        public int InputNeurons
        {
            set
            {
                this.neuronCount = value;
            }
            get
            {
                return this.neuronCount;
            }
        }

        /// <summary>
        /// The number of anneal cycles.
        /// </summary>
        public int AnnealCycles
        {
            get
            {
                return this.annealCycles;
            }
            set
            {
                this.annealCycles = value;
            }
        }


        /// <summary>
        /// The number of run cycles.
        /// </summary>
        public int RunCycles
        {
            get
            {
                return this.runCycles;
            }
            set
            {
                this.runCycles = value;
            }
        }

        /// <summary>
        /// The temperature.
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

    }

}
