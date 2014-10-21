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
using Encog.ML.Data;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Population;
using Encog.ML.EA.Species;
using Encog.Parse.Expression.Common;

namespace Encog.ML.Prg.Train
{
    /// <summary>
    ///     A population that contains EncogProgram's. The primary difference between
    ///     this class and BasicPopulation is that a "compute" method is provided that
    ///     automatically uses the "best" genome to provide a MLRegression compute
    ///     method. This population type also holds the common context that all of the
    ///     EncogProgram genomes make use of.
    /// </summary>
    [Serializable]
    public class PrgPopulation : BasicPopulation, IMLRegression
    {
        /// <summary>
        ///     The context.
        /// </summary>
        private readonly EncogProgramContext _context;

        /// <summary>
        ///     Construct the population.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="thePopulationSize">The population size.</param>
        public PrgPopulation(EncogProgramContext theContext,
                             int thePopulationSize)
            : base(thePopulationSize, new PrgGenomeFactory(theContext))
        {
            _context = theContext;
        }

        /// <summary>
        ///     The context for the programs.
        /// </summary>
        public EncogProgramContext Context
        {
            get { return _context; }
        }

        /// <summary>
        ///     Compute the output from the best Genome. Note: it is not safe to call
        ///     this method while training is progressing.
        /// </summary>
        /// <param name="input">The input to the program.</param>
        /// <returns>The output.</returns>
        public IMLData Compute(IMLData input)
        {
            var best = (EncogProgram) BestGenome;
            return best.Compute(input);
        }

        /// <inheritdoc />
        public int InputCount
        {
            get { return Context.DefinedVariables.Count; }
        }

        /// <inheritdoc />
        public int OutputCount
        {
            get { return 1; }
        }

        /// <summary>
        ///     Dump the specified number of genomes.
        /// </summary>
        /// <param name="i">The specified number of genomes.</param>
        public void DumpMembers(int i)
        {
            var render = new RenderCommonExpression();

            int index = 0;
            foreach (ISpecies species in Species)
            {
                Console.Out.WriteLine("** Species: " + species);
                foreach (IGenome obj in species.Members)
                {
                    var prg = (EncogProgram) obj;
                    Console.WriteLine(index + ": Score " + prg.Score + " : "
                                      + render.Render(prg));
                    index++;
                    if (index > i)
                    {
                        break;
                    }
                }
            }
        }
    }
}
