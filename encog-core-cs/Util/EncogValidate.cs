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
using Encog.ML.Data;
using Encog.Neural;
using Encog.Neural.Networks;

namespace Encog.Util
{
    /// <summary>
    /// Used to validate if training is valid.
    /// </summary>
    ///
    public sealed class EncogValidate
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private EncogValidate()
        {
        }

        /// <summary>
        /// Validate a network for training.
        /// </summary>
        ///
        /// <param name="network">The network to validate.</param>
        /// <param name="training">The training set to validate.</param>
        public static void ValidateNetworkForTraining(IContainsFlat network,
                                                      IMLDataSet training)
        {
            int inputCount = network.Flat.InputCount;
            int outputCount = network.Flat.OutputCount;

            if (inputCount != training.InputSize)
            {
                throw new NeuralNetworkError("The input layer size of "
                                             + inputCount + " must match the training input size of "
                                             + training.InputSize + ".");
            }

            if ((training.IdealSize > 0)
                && (outputCount != training.IdealSize))
            {
                throw new NeuralNetworkError("The output layer size of "
                                             + outputCount + " must match the training input size of "
                                             + training.IdealSize + ".");
            }
        }
    }
}
