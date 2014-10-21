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
using Encog.ML.Factory.Parse;
using Encog.Util;
using Encog.Neural.NEAT;
using Encog.Engine.Network.Activation;

namespace Encog.ML.Factory.Method
{
    public class NEATFactory
    {
        /// <summary>
        /// The activation function factory to use.
        /// </summary>
        private MLActivationFactory factory = new MLActivationFactory();

        /// <summary>
        /// Create a NEAT population.
        /// </summary>
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The population.</returns>
        public IMLMethod Create(String architecture, int input,
                int output)
        {

            if (input <= 0)
            {
                throw new EncogError("Must have at least one input for NEAT.");
            }

            if (output <= 0)
            {
                throw new EncogError("Must have at least one output for NEAT.");
            }


            IDictionary<String, String> args = ArchitectureParse.ParseParams(architecture);
            ParamsHolder holder = new ParamsHolder(args);

            int populationSize = holder.GetInt(
                    MLMethodFactory.PropertyPopulationSize, false, 1000);

            int cycles = holder.GetInt(
                    MLMethodFactory.PropertyCycles, false, NEATPopulation.DefaultCycles);

            IActivationFunction af = this.factory.Create(
                    holder.GetString(MLMethodFactory.PropertyAF, false, MLActivationFactory.AF_SSIGMOID));

            NEATPopulation pop = new NEATPopulation(input, output, populationSize);
            pop.Reset();
            pop.ActivationCycles = cycles;
            pop.NEATActivationFunction = af;

            return pop;
        }
    }
}
