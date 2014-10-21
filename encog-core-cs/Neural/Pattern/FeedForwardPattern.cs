//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Used to create feedforward neural networks. A feedforward network has an
    /// input and output layers separated by zero or more hidden layers. The
    /// feedforward neural network is one of the most common neural network patterns.
    /// </summary>
    ///
    public class FeedForwardPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of hidden neurons.
        /// </summary>
        ///
        private readonly IList<Int32> _hidden;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction _activationHidden;

        /// <summary>
        /// The activation function.
        /// </summary>
        ///
        private IActivationFunction _activationOutput;

        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public FeedForwardPattern()
        {
            _hidden = new List<Int32>();
        }

        /// <value>the activationOutput to set</value>
        public IActivationFunction ActivationOutput
        {
            get { return _activationOutput; }
            set { _activationOutput = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer, with the specified number of neurons.
        /// </summary>
        public void AddHiddenLayer(int count)
        {
            _hidden.Add(count);
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public void Clear()
        {
            _hidden.Clear();
        }

        /// <summary>
        /// Generate the feedforward neural network.
        /// </summary>
        public IMLMethod Generate()
        {
            if (_activationOutput == null)
                _activationOutput = _activationHidden;

            ILayer input = new BasicLayer(null, true, _inputNeurons);

            var result = new BasicNetwork();
            result.AddLayer(input);


            foreach (Int32 count  in  _hidden)
            {
                ILayer hidden = new BasicLayer(_activationHidden, true,
                                                (count));

                result.AddLayer(hidden);
            }

            ILayer output = new BasicLayer(_activationOutput, false,
                                          _outputNeurons);
            result.AddLayer(output);

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }

        /// <summary>
        /// Set the activation function to use on each of the layers.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set { _activationHidden = value; }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { _outputNeurons = value; }
        }

        #endregion
    }
}
