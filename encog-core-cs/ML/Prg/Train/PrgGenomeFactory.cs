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
using Encog.ML.EA.Genome;

namespace Encog.ML.Prg.Train
{
    /// <summary>
    ///     A GenomeFactory that creates EncogProgram genomes.
    /// </summary>
    [Serializable]
    public class PrgGenomeFactory : IGenomeFactory
    {
        /// <summary>
        ///     The context.
        /// </summary>
        private readonly EncogProgramContext _context;

        /// <summary>
        ///     Construct a factory.
        /// </summary>
        /// <param name="theContext">The context to use.</param>
        public PrgGenomeFactory(EncogProgramContext theContext)
        {
            _context = theContext;
        }

        /// <inheritdoc />
        public IGenome Factor()
        {
            var result = new EncogProgram(_context,
                                          new EncogProgramVariables());
            return result;
        }

        /// <inheritdoc />
        public IGenome Factor(IGenome other)
        {
            var result = new EncogProgram(_context,
                                          new EncogProgramVariables());
            result.Copy(other);
            return result;
        }
    }
}
