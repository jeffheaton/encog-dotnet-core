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
using Encog.ML.Factory.Method;
using Encog.Plugin;

namespace Encog.ML.Factory
{
    /// <summary>
    /// This factory is used to create machine learning methods.
    /// </summary>
    ///
    public class MLMethodFactory
    {
        /// <summary>
        /// String constant for a bayesian neural network.
        /// </summary>
        public const String TypeBayesian = "bayesian";

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
        /// A NEAT neural network.
        /// </summary>
        public const String TypeNEAT = "neat";

        /// <summary>
        /// A Encog program.
        /// </summary>
        public const String TypeEPL = "epl";


        public const String PropertyAF = "AF";

        /// <summary>
        /// Population size.
        /// </summary>
        public const String PropertyPopulationSize = "population";

        public const String PropertyCycles = "cycles";

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
            foreach (EncogPluginBase plugin in EncogFramework.Instance.Plugins)
            {
                if (plugin is IEncogPluginService1)
                {
                    IMLMethod result = ((IEncogPluginService1)plugin).CreateMethod(
                            methodType, architecture, input, output);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            throw new EncogError("Unknown method type: " + methodType);
        }
    }
}
