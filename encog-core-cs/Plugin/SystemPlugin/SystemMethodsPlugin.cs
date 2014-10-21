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
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Factory;
using Encog.ML.Factory.Method;
using Encog.ML.Train;

namespace Encog.Plugin.SystemPlugin
{
    /// <summary>
    /// System plugin for core ML Methods.
    /// </summary>
    public class SystemMethodsPlugin : IEncogPluginService1
    {
        /// <summary>
        /// A factory used to create Bayesian networks
        /// </summary>
        private readonly BayesianFactory _bayesianFactory = new BayesianFactory();

        /// <summary>
        /// A factory used to create feedforward neural networks.
        /// </summary>
        private readonly FeedforwardFactory _feedforwardFactory = new FeedforwardFactory();

        /// <summary>
        /// The factory for PNN's.
        /// </summary>
        private readonly PNNFactory _pnnFactory = new PNNFactory();

        /// <summary>
        /// A factory used to create RBF networks.
        /// </summary>
        private readonly RBFNetworkFactory _rbfFactory = new RBFNetworkFactory();

        /// <summary>
        /// A factory used to create SOM's.
        /// </summary>
        private readonly SOMFactory _somFactory = new SOMFactory();

        /// <summary>
        /// A factory used to create support vector machines.
        /// </summary>
        private readonly SVMFactory _svmFactory = new SVMFactory();

        /// <summary>
        /// A factory used to create NEAT populations.
        /// </summary>
        private readonly NEATFactory _neatFactory = new NEATFactory();

        /// <summary>
        /// A factory used to create EPL populations.
        /// </summary>
        private readonly EPLFactory _eplFactory = new EPLFactory();

        #region IEncogPluginService1 Members

        /// <inheritdoc/>
        public String PluginDescription
        {
            get
            {
                return "This plugin provides the built in machine " +
                       "learning methods for Encog.";
            }
        }

        /// <inheritdoc/>
        public String PluginName
        {
            get { return "HRI-System-Methods"; }
        }

        /// <summary>
        /// This is a type-1 plugin.
        /// </summary>
        public int PluginType
        {
            get { return 1; }
        }


        /// <inheritdoc/>
        public IActivationFunction CreateActivationFunction(String name)
        {
            return null;
        }

        /// <inheritdoc/>
        public IMLMethod CreateMethod(String methodType, String architecture,
                                      int input, int output)
        {
            if (MLMethodFactory.TypeFeedforward.Equals(methodType))
            {
                return _feedforwardFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeRbfnetwork.Equals(methodType))
            {
                return _rbfFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeSVM.Equals(methodType))
            {
                return _svmFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeSOM.Equals(methodType))
            {
                return _somFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypePNN.Equals(methodType))
            {
                return _pnnFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeBayesian.Equals(methodType))
            {
                return _bayesianFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeNEAT.Equals(methodType))
            {
                return _neatFactory.Create(architecture, input, output);
            }
            if (MLMethodFactory.TypeEPL.Equals(methodType))
            {
                return _eplFactory.Create(architecture, input, output);
            }

            throw new EncogError("Unknown method type: " + methodType);
        }

        /// <inheritdoc/>
        public IMLTrain CreateTraining(IMLMethod method, IMLDataSet training,
                                       String type, String args)
        {
            return null;
        }

        /// <inheritdoc/>
        public int PluginServiceType
        {
            get { return EncogPluginBaseConst.SERVICE_TYPE_GENERAL; }
        }

        #endregion
    }
}
