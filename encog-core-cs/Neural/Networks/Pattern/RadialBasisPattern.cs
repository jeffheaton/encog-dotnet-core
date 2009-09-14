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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Activation;
#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// A radial basis function (RBF) network uses several radial basis
    /// functions to provide a more dynamic hidden layer activation function
    /// than many other types of neural network.  It consists of a 
    /// input, output and hidden layer.
    /// </summary>
    public class RadialBasisPattern : INeuralNetworkPattern
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(RadialBasisPattern));
#endif
        /// <summary>
        /// The number of input neurons to use. Must be set, default to invalid
        /// -1 value.
        /// </summary>
        private int inputNeurons = -1;

        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid
        /// -1 value.
        /// </summary>
        private int outputNeurons = -1;

        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid
        /// -1 value.
        /// </summary>
        private int hiddenNeurons = -1;

        /// <summary>
        /// Add the hidden layer, this should be called once, as a RBF
        /// has a single hidden layer.
        /// </summary>
        /// <param name="count">The number of neurons in the hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (this.hiddenNeurons != -1)
            {
                String str = "A RBF network usually has a single "
                   + "hidden layer.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);
            }
            else
            {
                this.hiddenNeurons = count;
            }
        }

        /// <summary>
        /// Generate the RBF network.
        /// </summary>
        /// <returns>The neural network.</returns>
        public BasicNetwork Generate()
        {
            ILayer input = new BasicLayer(new ActivationLinear(), false,
                    this.inputNeurons);
            ILayer output = new BasicLayer(this.outputNeurons);
            BasicNetwork network = new BasicNetwork();
            RadialBasisFunctionLayer rbfLayer = new RadialBasisFunctionLayer(
                   this.hiddenNeurons);
            network.AddLayer(input);
            network.AddLayer(rbfLayer, SynapseType.Direct);
            network.AddLayer(output);
            network.Structure.FinalizeStructure();
            network.Reset();
            rbfLayer.RandomizeGaussianCentersAndWidths(0, 1);
            int y = PatternConst.START_Y;
            input.X = PatternConst.START_X;
            input.Y = y;
            y += PatternConst.INC_Y;
            rbfLayer.X = PatternConst.START_X;
            rbfLayer.Y = y;
            y += PatternConst.INC_Y;
            output.X = PatternConst.START_X;
            output.Y = y;
            return network;
        }

        /// <summary>
        /// Set the activation function, this is an error. The activation function
        /// may not be set on a RBF layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                String str = "Can't set the activation function for "
                       + "a radial basis function network.";
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
        /// Clear out any hidden neurons.
        /// </summary>
        public void Clear()
        {
            this.hiddenNeurons = 0;
        }
    }
}
