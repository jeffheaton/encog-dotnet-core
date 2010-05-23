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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Activation;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// Used to create feedforward neural networks.  A feedforward 
    /// network has an input and output layers separated by zero
    /// or more hidden layers.  The feedforward neural network is
    /// one of the most common neural network patterns.
    /// </summary>
    public class FeedForwardPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons.
        /// </summary>
        private int inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        private int outputNeurons;

        /// <summary>
        /// The activation function.
        /// </summary>
        private IActivationFunction activation;

        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        private IList<int> hidden = new List<int>();

        /// <summary>
        /// Add a hidden layer, with the specified number of neurons.
        /// </summary>
        /// <param name="count">The number of neurons to add.</param>
        public void AddHiddenLayer(int count)
        {
            this.hidden.Add(count);
        }

        /// <summary>
        /// Generate the Elman neural network.
        /// </summary>
        /// <returns>The Elman neural network.</returns>
        public BasicNetwork Generate()
        {
            int y = PatternConst.START_Y;
            ILayer input = new BasicLayer(this.activation, false,
                   this.inputNeurons);

            BasicNetwork result = new BasicNetwork();
            result.AddLayer(input);

            input.X = PatternConst.START_X;
            input.Y = y;
            y += PatternConst.INC_Y;

            foreach (int count in this.hidden)
            {

                ILayer hidden = new BasicLayer(
                       this.activation, true, count);

                result.AddLayer(hidden);
                hidden.X = PatternConst.START_X;
                hidden.Y = y;
                y += PatternConst.INC_Y;
            }

            ILayer output = new BasicLayer(this.activation, true,
                   this.outputNeurons);
            result.AddLayer(output);
            output.X = PatternConst.START_X;
            output.Y = y;
            y += PatternConst.INC_Y;

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        public void Clear()
        {
            this.hidden.Clear();
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                this.activation = value;
            }
            get
            {
                return this.activation;
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
    }
}
