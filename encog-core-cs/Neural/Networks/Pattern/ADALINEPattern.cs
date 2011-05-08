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
using Encog.Neural.Networks.Layers;
using Encog.MathUtil.Randomize;
using Encog.Engine.Network.Activation;
#if logging
using log4net;
#endif
namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Construct an adaline neural network.
    /// </summary>
    public class ADALINEPattern : INeuralNetworkPattern
    {

        /// <summary>
        /// The number of neurons in the input layer.
        /// </summary>
        public int InputNeurons { get; set; }

        /// <summary>
        /// The number of neurons in the output layer.
        /// </summary>
        public int OutputNeurons { get; set; }

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ADALINEPattern));
#endif

        /// <summary>
        /// Not used, the ADALINE has no hidden layers, this will throw an error.
        /// </summary>
        /// <param name="count">The neuron count.</param>
        public void AddHiddenLayer(int count)
        {
            String str = "An ADALINE network has no hidden layers.";
#if logging

            if (this.logger.IsErrorEnabled)
            {
                this.logger.Error(str);
            }
#endif
            throw new PatternError(str);

        }

        /// <summary>
        /// Clear out any parameters.
        /// </summary>
        public void Clear()
        {
            this.InputNeurons = this.OutputNeurons = 0;
        }

        /// <summary>
        /// Generate the network.
        /// </summary>
        /// <returns>The generated network.</returns>
        public BasicNetwork Generate()
        {
            BasicNetwork network = new BasicNetwork();

            int y = PatternConst.START_Y;

            ILayer inputLayer = new BasicLayer(new ActivationLinear(), false, InputNeurons);
            ILayer outputLayer = new BasicLayer(new ActivationLinear(), true, OutputNeurons);

            network.AddLayer(inputLayer);
            network.AddLayer(outputLayer);
            network.Structure.FinalizeStructure();

            (new RangeRandomizer(-0.5, 0.5)).Randomize(network);

            inputLayer.X = PatternConst.START_X;
            inputLayer.Y = y;
            y += PatternConst.INC_Y;

            outputLayer.X = PatternConst.START_X;
            outputLayer.Y = y;

            return network;
        }


        /// <summary>
        /// The activation function, not used for this sort of network.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                String str = "A BAM network can't specify a custom activation function.";
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
                String str = "A BAM network can't specify a custom activation function.";
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new PatternError(str);

            }
        }

    }
}
