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
using Encog.ML.SVM;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// A factory that is used to create support vector machines (SVM).
    /// </summary>
    ///
    public class SVMFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MAX_LAYERS = 3;

        /// <summary>
        /// Create the SVM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SVM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MAX_LAYERS)
            {
                throw new EncogError(
                    "SVM's must have exactly three elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer paramsLayer = ArchitectureParse.ParseLayer(
                layers[1], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            String name = paramsLayer.Name;
            String kernelStr = paramsLayer.Params.ContainsKey("KERNEL") ? paramsLayer.Params["KERNEL"] : null;
            String svmTypeStr = paramsLayer.Params.ContainsKey("TYPE") ? paramsLayer.Params["TYPE"] : null;

            SVMType svmType = SVMType.NewSupportVectorClassification;
            KernelType kernelType = KernelType.RadialBasisFunction;

            bool useNew = true;

            if (svmTypeStr == null)
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("NEW", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = true;
            }
            else if (svmTypeStr.Equals("OLD", StringComparison.InvariantCultureIgnoreCase))
            {
                useNew = false;
            }
            else
            {
                throw new EncogError("Unsupported type: " + svmTypeStr
                                     + ", must be NEW or OLD.");
            }

            if (name.Equals("C", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorClassification;
                }
                else
                {
                    svmType = SVMType.SupportVectorClassification;
                }
            }
            else if (name.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                if (useNew)
                {
                    svmType = SVMType.NewSupportVectorRegression;
                }
                else
                {
                    svmType = SVMType.EpsilonSupportVectorRegression;
                }
            }
            else
            {
                throw new EncogError("Unsupported mode: " + name
                                     + ", must be C for classify or R for regression.");
            }

            if (kernelStr == null)
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("linear".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Linear;
            }
            else if ("poly".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Poly;
            }
            else if ("precomputed".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Precomputed;
            }
            else if ("rbf".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.RadialBasisFunction;
            }
            else if ("sigmoid".Equals(kernelStr, StringComparison.InvariantCultureIgnoreCase))
            {
                kernelType = KernelType.Sigmoid;
            }
            else
            {
                throw new EncogError("Unsupported kernel: " + kernelStr
                                     + ", must be linear,poly,precomputed,rbf or sigmoid.");
            }

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            if (outputCount != 1)
            {
                throw new EncogError("SVM can only have an output size of 1.");
            }

            var result = new SupportVectorMachine(inputCount, svmType, kernelType);

            return result;
        }
    }
}
