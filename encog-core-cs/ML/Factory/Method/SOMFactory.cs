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
using Encog.Neural.Pattern;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// A factory that is used to produce self-organizing maps.
    /// </summary>
    ///
    public class SOMFactory
    {
        /// <summary>
        /// Create a SOM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SOM.</returns>
        public IMLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != 2)
            {
                throw new EncogError(
                    "SOM's must have exactly two elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[1], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            var pattern = new SOMPattern {InputNeurons = inputCount, OutputNeurons = outputCount};
            return pattern.Generate();
        }
    }
}
