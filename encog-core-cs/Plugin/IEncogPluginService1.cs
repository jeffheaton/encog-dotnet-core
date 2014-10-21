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
using System.Linq;
using System.Text;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Train;
using Encog.ML.Data;

namespace Encog.Plugin
{
    public interface IEncogPluginService1 : EncogPluginBase
    {        
        /// <summary>
        /// Create an activation function.
        /// </summary>
        /// <param name="name">The name of the activation function.</param>
        /// <returns>The newly created activation function.</returns>
        IActivationFunction CreateActivationFunction(String name);

        /// <summary>
        /// Create a new machine learning method. 
        /// </summary>
        /// <param name="methodType">The method to create.</param>
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created machine learning method.</returns>
        IMLMethod CreateMethod(String methodType,
                String architecture,
                int input, int output);

        /// <summary>
        /// Create a trainer. 
        /// </summary>
        /// <param name="method">The method to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="type">Type type of trainer.</param>
        /// <param name="args">The training args.</param>
        /// <returns>The new training method.</returns>
        
        IMLTrain CreateTraining(IMLMethod method,
                 IMLDataSet training,
                 String type, String args);

    }
}
