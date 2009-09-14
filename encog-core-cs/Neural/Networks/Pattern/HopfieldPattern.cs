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
using Encog.Neural.Networks.Logic;

#if logging
using log4net;
#endif

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Create a Hopfield pattern.  A Hopfield neural network has a single
    /// layer that functions both as the input and output layers.  There
    /// are no hidden layers.  Hopfield networks are used for basic
    /// pattern recognition.  When a Hopfield network recognizes a 
    /// pattern, it "echos" that pattern on the output.
    /// </summary>
    public class HopfieldPattern : INeuralNetworkPattern
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicNetwork));
#endif
        /// <summary>
        /// How many neurons in the Hopfield network. Default to -1, which is
        /// invalid. Therefore this value must be set.
        /// </summary>
        private int neuronCount = -1;

        /// <summary>
        /// Add a hidden layer.  This will throw an error, because the
        /// Hopfield neural network has no hidden layers.
        /// </summary>
        /// <param name="count">The number of neurons.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "A Hopfield network has no hidden layers.";
#if logging
            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PatternError(str);

        }

        /// <summary>
        /// Generate the Hopfield neural network.
        /// </summary>
        /// <returns>The generated network.</returns>
        public BasicNetwork Generate()
        {
            ILayer layer = new BasicLayer(new ActivationBiPolar(), false,
                    this.neuronCount);

            BasicNetwork result = new BasicNetwork(new HopfieldLogic());
            result.AddLayer(layer);
            layer.AddNext(layer);
            layer.X = PatternConst.START_X;
            layer.Y = PatternConst.START_Y;
            result.Structure.FinalizeStructure();
            result.Reset();
            return result;
        }

        /// <summary>
        /// Set the activation function to use.  This function will throw
        /// an error, because the Hopfield network must use the BiPolar
        /// activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                String str =
                   "A Hopfield network will use the BiPolar activation "
                   + "function, no activation function needs to be specified.";
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
        /// Set the number of output neurons, should not be used with a hopfield
        /// neural network, because the number of input neurons defines the number of
        /// output neurons.
        /// </summary>
        public int OutputNeurons
        {

            set
            {
                String str = "A Hopfield network has a single layer, so no need "
                       + "to specify the output count.";
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
                return -1;
            }

        }

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get
            {
                return this.neuronCount;
            }
            set
            {
                this.neuronCount = value;
            }

        }

        /// <summary>
        /// Nothing to clear.
        /// </summary>
        public void Clear()
        {
        }
    }
}
