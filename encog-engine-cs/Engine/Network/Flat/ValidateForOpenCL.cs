/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */

namespace Encog.Engine.Network.Flat
{

    using Encog.Engine;
    using Encog.Engine.Network.Activation;
    using Encog.Engine.Validate;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Validate the network to be sure it can run on OpenCL.
    /// </summary>
    ///
    public class ValidateForOpenCL : BasicMachineLearningValidate
    {

        /// <summary>
        /// Determine if the network is valid for OpenCL.
        /// </summary>
        ///
        /// <param name="network"/>The network to check.</param>
        /// <returns>The string indicating the error that prevents OpenCL from usingthe network, or null if the network is fine for OpenCL.</returns>
        public override String IsValid(IEngineMachineLearning network)
        {

            if (!(network is FlatNetwork))
            {
                return "Only flat networks are valid to be used for OpenCL";
            }

            FlatNetwork flat = (FlatNetwork)network;

            /* foreach */
            foreach (IActivationFunction activation in flat.ActivationFunctions)
            {
                if (activation.GetOpenCLExpression(true, true) == null)
                {
                    return "Can't use OpenCL if activation function does not have an OpenCL expression.";
                }
            }

            if (flat.HasSameActivationFunction() == null)
            {
                return "Can't use OpenCL training on a neural network that uses multiple activation functions.";
            }

            bool hasContext = false;
            for (int i = 0; i < flat.LayerCounts.Length; i++)
            {
                if (flat.ContextTargetOffset[i] != 0)
                {
                    hasContext = true;
                }

                if (flat.ContextTargetSize[i] != 0)
                {
                    hasContext = true;
                }
            }

            if (hasContext)
            {
                return "Can't use OpenCL if context neurons are present.";
            }

            return null;
        }

    }
}
