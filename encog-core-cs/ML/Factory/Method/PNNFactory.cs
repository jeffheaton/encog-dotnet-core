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
using Encog.ML.Factory.Parse;
using Encog.Neural;
using Encog.Neural.PNN;
using Encog.Util;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// A factory to create PNN networks.
    /// </summary>
    ///
    public class PNNFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MaxLayers = 3;

        /// <summary>
        /// Create a PNN network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MaxLayers)
            {
                throw new EncogError(
                    "PNN Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer pnnLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            PNNKernelType kernel;
            PNNOutputMode outmodel;

            if (pnnLayer.Name.Equals("c", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Classification;
            }
            else if (pnnLayer.Name.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Regression;
            }
            else if (pnnLayer.Name.Equals("u", StringComparison.InvariantCultureIgnoreCase))
            {
                outmodel = PNNOutputMode.Unsupervised;
            }
            else
            {
                throw new NeuralNetworkError("Unknown model: " + pnnLayer.Name);
            }

            var holder = new ParamsHolder(pnnLayer.Params);

            String kernelStr = holder.GetString("KERNEL", false, "gaussian");

            if (kernelStr.Equals("gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Gaussian;
            }
            else if (kernelStr.Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase))
            {
                kernel = PNNKernelType.Reciprocal;
            }
            else
            {
                throw new NeuralNetworkError("Unknown kernel: " + kernelStr);
            }

            var result = new BasicPNN(kernel, outmodel, inputCount,
                                      outputCount);

            return result;
        }
    }
}
