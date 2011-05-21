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
using System;
using Encog.ML.Factory.Method;

namespace Encog.ML.Factory
{
    /// <summary>
    /// This factory is used to create machine learning methods.
    /// </summary>
    ///
    public class MLMethodFactory
    {
        /// <summary>
        /// String constant for feedforward neural networks.
        /// </summary>
        ///
        public const String TypeFeedforward = "feedforward";

        /// <summary>
        /// String constant for RBF neural networks.
        /// </summary>
        ///
        public const String TypeRbfnetwork = "rbfnetwork";

        /// <summary>
        /// String constant for support vector machines.
        /// </summary>
        ///
        public const String TypeSVM = "svm";

        /// <summary>
        /// String constant for SOMs.
        /// </summary>
        ///
        public const String TypeSOM = "som";

        /// <summary>
        /// A probabilistic neural network. Supports both PNN and GRNN.
        /// </summary>
        ///
        public const String TypePNN = "pnn";

        /// <summary>
        /// A factory used to create feedforward neural networks.
        /// </summary>
        ///
        private readonly FeedforwardFactory _feedforwardFactory;

        /// <summary>
        /// The factory for PNN's.
        /// </summary>
        ///
        private readonly PNNFactory _pnnFactory;

        /// <summary>
        /// A factory used to create RBF networks.
        /// </summary>
        ///
        private readonly RBFNetworkFactory _rbfFactory;

        /// <summary>
        /// A factory used to create SOM's.
        /// </summary>
        ///
        private readonly SOMFactory _somFactory;

        /// <summary>
        /// A factory used to create support vector machines.
        /// </summary>
        ///
        private readonly SVMFactory _svmFactory;

        /// <summary>
        /// Construct the object.
        /// </summary>
        public MLMethodFactory()
        {
            _feedforwardFactory = new FeedforwardFactory();
            _svmFactory = new SVMFactory();
            _rbfFactory = new RBFNetworkFactory();
            _pnnFactory = new PNNFactory();
            _somFactory = new SOMFactory();
        }

        /// <summary>
        /// Create a new machine learning method.
        /// </summary>
        ///
        /// <param name="methodType">The method to create.</param>
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created machine learning method.</returns>
        public IMLMethod Create(String methodType,
                               String architecture, int input, int output)
        {
            if (TypeFeedforward.Equals(methodType))
            {
                return _feedforwardFactory.Create(architecture, input, output);
            }
            if (TypeRbfnetwork.Equals(methodType))
            {
                return _rbfFactory.Create(architecture, input, output);
            }
            if (TypeSVM.Equals(methodType))
            {
                return _svmFactory.Create(architecture, input, output);
            }
            if (TypeSOM.Equals(methodType))
            {
                return _somFactory.Create(architecture, input, output);
            }
            if (TypePNN.Equals(methodType))
            {
                return _pnnFactory.Create(architecture, input, output);
            }
            throw new EncogError("Unknown method type: " + methodType);
        }
    }
}
