//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.MathUtil.RBF;
using Encog.ML;
using Encog.Neural.RBF;

namespace Encog.Neural.Pattern
{
    /// <summary>
    /// A radial basis function (RBF) network uses several radial basis functions to
    /// provide a more dynamic hidden layer activation function than many other types
    /// of neural network. It consists of a input, output and hidden layer.
    /// </summary>
    ///
    public class RadialBasisPattern : NeuralNetworkPattern
    {
        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int hiddenNeurons;

        /// <summary>
        /// The number of input neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int inputNeurons;

        /// <summary>
        /// The number of hidden neurons to use. Must be set, default to invalid -1
        /// value.
        /// </summary>
        ///
        private int outputNeurons;

        /// <summary>
        /// The RBF type.
        /// </summary>
        private RBFEnum rbfType;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public RadialBasisPattern()
        {
            rbfType = RBFEnum.Gaussian;
            inputNeurons = -1;
            outputNeurons = -1;
            hiddenNeurons = -1;
        }

        /// <summary>
        /// The RBF type.
        /// </summary>
        public RBFEnum RBF
        {
            set { rbfType = value; }
        }

        #region NeuralNetworkPattern Members

        /// <summary>
        /// Add the hidden layer, this should be called once, as a RBF has a single
        /// hidden layer.
        /// </summary>
        ///
        /// <param name="count">The number of neurons in the hidden layer.</param>
        public void AddHiddenLayer(int count)
        {
            if (hiddenNeurons != -1)
            {
                throw new PatternError("A RBF network usually has a single "
                                       + "hidden layer.");
            }
            else
            {
                hiddenNeurons = count;
            }
        }

        /// <summary>
        /// Clear out any hidden neurons.
        /// </summary>
        ///
        public void Clear()
        {
            hiddenNeurons = -1;
        }

        /// <summary>
        /// Generate the RBF network.
        /// </summary>
        ///
        /// <returns>The neural network.</returns>
        public IMLMethod Generate()
        {
            var result = new RBFNetwork(inputNeurons, hiddenNeurons,
                                        outputNeurons, rbfType);
            return result;
        }

        /// <summary>
        /// Set the activation function, this is an error. The activation function
        /// may not be set on a RBF layer.
        /// </summary>
        public IActivationFunction ActivationFunction
        {
            set
            {
                throw new PatternError("Can't set the activation function for "
                                       + "a radial basis function network.");
            }
        }


        /// <summary>
        /// Set the number of input neurons.
        /// </summary>
        public int InputNeurons
        {
            set { inputNeurons = value; }
        }


        /// <summary>
        /// Set the number of output neurons.
        /// </summary>
        public int OutputNeurons
        {
            set { outputNeurons = value; }
        }

        #endregion
    }
}
