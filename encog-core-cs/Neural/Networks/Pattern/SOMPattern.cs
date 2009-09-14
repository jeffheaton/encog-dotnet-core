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
using Encog.Neural.Activation;
using Encog.Neural.Networks.Layers;
#if logging
using log4net;
#endif
namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// A self organizing map is a neural network pattern with an input
    /// and output layer.  There is no hidden layer.  The winning neuron,
    /// which is that neuron with the higest output is the winner, this
    /// winning neuron is often used to classify the input into a group.
    /// </summary>
    public class SOMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons.
        /// </summary>
        private int inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private int outputNeurons;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(SOMPattern));
#endif
        /// <summary>
        /// Add a hidden layer. SOM networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        /// <param name="count">The number of hidden neurons.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "A SOM network does not have hidden layers.";
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PatternError(str);

        }

        /// <summary>
        /// Generate the RSOM network.
        /// </summary>
        /// <returns>The neural network.</returns>
        public BasicNetwork Generate()
        {
            ILayer input = new BasicLayer(new ActivationLinear(), false,
                    this.inputNeurons);
            ILayer output = new BasicLayer(new ActivationLinear(), false,
                    this.outputNeurons);
            int y = PatternConst.START_Y;
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(input);
            network.AddLayer(output);
            input.X = PatternConst.START_X;
            output.X = PatternConst.START_X;
            input.Y = y;
            y += PatternConst.INC_Y;
            output.Y = y;
            network.Structure.FinalizeStructure();
            network.Reset();
            return network;
        }

        /// <summary>
        /// Set the activation function.  A SOM uses a linear activation
        /// function, so this method throws an error.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                String str = "A SOM network can't define an activation function.";
#if logging     
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);
            }
            get
            {
                return null;
            }

        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get
            {
                return this.outputNeurons;
            }
            set
            {
                this.outputNeurons = value;
            }

        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get
            {
                return this.inputNeurons;
            }
            set
            {
                this.inputNeurons = value;
            }

        }

        /// <summary>
        /// Does nothing, no optinal hidden layers to clear, only the ONE 
        /// predefined hidden layer.
        /// </summary>
        public void Clear()
        {
        }
    }

}
