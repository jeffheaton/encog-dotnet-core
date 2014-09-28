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
using Encog.Util;
using Encog.ML.Prg;
using Encog.ML.Prg.Train;
using Encog.ML.Prg.Generator;
using Encog.MathUtil.Randomize;
using Encog.ML.Prg.Ext;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// Create an EPL method.
    /// </summary>
    public class EPLFactory
    {
        /// <summary>
        /// Create a feed forward network.
        /// </summary>
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The feedforward network.</returns>
        public IMLMethod Create(String architecture, int input,
                int output)
        {

            if (input <= 0)
            {
                throw new EncogError("Must have at least one input for EPL.");
            }

            if (output <= 0)
            {
                throw new EncogError("Must have at least one output for EPL.");
            }


            IDictionary<String, String> args = ArchitectureParse.ParseParams(architecture);
            var holder = new ParamsHolder(args);

            int populationSize = holder.GetInt(
                    MLMethodFactory.PropertyPopulationSize, false, 1000);
            String variables = holder.GetString("vars", false, "x");
            String funct = holder.GetString("funct", false, null);

            var context = new EncogProgramContext();
            string[] tok = variables.Split(',');
            foreach (string v in tok)
            {
                context.DefineVariable(v);
            }

            if (String.Compare("numeric", funct, StringComparison.OrdinalIgnoreCase) == 0)
            {
                StandardExtensions.CreateNumericOperators(context);
            }

            var pop = new PrgPopulation(context, populationSize);

            if (context.Functions.Count > 0)
            {
                (new RampedHalfAndHalf(context, 2, 6)).Generate(new EncogRandom(), pop);
            }
            return pop;
        }
    }
}
