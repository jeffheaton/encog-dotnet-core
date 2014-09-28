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
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.Neural.PNN;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// Pattern to create a PNN.
    /// </summary>
    ///
    public class PNNPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of input neurons.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The kernel type.
        /// </summary>
        ///
        private PNNKernelType _kernel;

        /// <summary>
        /// The output model.
        /// </summary>
        ///
        private PNNOutputMode _outmodel;

        /// <summary>
        /// The number of output neurons.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public PNNPattern()
        {
            _kernel = PNNKernelType.Gaussian;
            _outmodel = PNNOutputMode.Regression;
        }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        public PNNKernelType Kernel
        {
            get { return _kernel; }
            set { _kernel = value; }
        }


        /// <summary>
        /// Set the output model.
        /// </summary>
        public PNNOutputMode Outmodel
        {
            get { return _outmodel; }
            set { _outmodel = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add a hidden layer. PNN networks do not have hidden layers, so this will
        /// throw an error.
        /// </summary>
        ///
        /// <param name="count">The number of hidden neurons.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A PNN network does not have hidden layers.");
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public virtual void Clear()
        {
        }

        /// <summary>
        /// Generate the RSOM network.
        /// </summary>
        ///
        /// <returns>The neural network.</returns>
        public IMLMethod Generate()
        {
            var pnn = new BasicPNN(_kernel, _outmodel,
                                   _inputNeurons, _outputNeurons);
            return pnn;
        }

        /// <summary>
        /// Set the input neuron count.
        /// </summary>
        public int InputNeurons
        {
            get { return _inputNeurons; }
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the output neuron count.
        /// </summary>
        ///
        /// <value>The number of neurons.</value>
        public int OutputNeurons
        {
            get { return _outputNeurons; }
            set { _outputNeurons = value; }
        }


        /// <summary>
        /// Set the activation function. A PNN uses a linear activation function, so
        /// this method throws an error.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A SOM network can't define an activation function.");
            }
        }

        #endregion
    }
}
