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
using Encog.ML.SVM;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// A pattern to create support vector machines.
    /// </summary>
    ///
    public class SVMPattern : INeuralNetworkPattern
    {
        /// <summary>
        /// The number of neurons in the first layer.
        /// </summary>
        ///
        private int _inputNeurons;

        /// <summary>
        /// The kernel type.
        /// </summary>
        ///
        private KernelType _kernelType;

        /// <summary>
        /// The number of neurons in the second layer.
        /// </summary>
        ///
        private int _outputNeurons;

        /// <summary>
        /// The SVM type.
        /// </summary>
        ///
        private SVMType _svmType;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public SVMPattern()
        {
            Regression = true;
            _kernelType = KernelType.RadialBasisFunction;
            _svmType = SVMType.EpsilonSupportVectorRegression;
        }

        /// <summary>
        /// Set if regression is used.
        /// </summary>
        public bool Regression { get; set; }

        /// <summary>
        /// Set the kernel type.
        /// </summary>
        public KernelType KernelType
        {
            set { _kernelType = value; }
        }


        /// <summary>
        /// Set the SVM type.
        /// </summary>
        public SVMType SVMType
        {
            set { _svmType = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Unused, a BAM has no hidden layers.
        /// </summary>
        ///
        /// <param name="count">Not used.</param>
        public void AddHiddenLayer(int count)
        {
            throw new PatternError("A SVM network has no hidden layers.");
        }

        /// <summary>
        /// Clear any settings on the pattern.
        /// </summary>
        ///
        public void Clear()
        {
            _inputNeurons = 0;
            _outputNeurons = 0;
        }


        /// <returns>The generated network.</returns>
        public IMLMethod Generate()
        {
            if (_outputNeurons != 1)
            {
                throw new PatternError("A SVM may only have one output.");
            }
            var network = new SupportVectorMachine(_inputNeurons, _svmType,
                                                   _kernelType);
            return network;
        }

        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            get { return _inputNeurons; }
            set { _inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            get { return _outputNeurons; }
            set { _outputNeurons = value; }
        }


        /// <summary>
        /// Not used, the BAM uses a bipoloar activation function.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError(
                    "A SVM network can't specify a custom activation function.");
            }
        }

        #endregion
    }
}
