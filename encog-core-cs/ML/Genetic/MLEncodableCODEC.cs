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
using Encog.ML.EA.Genome;
using Encog.ML.EA.Codec;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// A CODEC for IMLEncodable classes.
    /// </summary>
    [Serializable]
    public class MLEncodableCODEC: IGeneticCODEC
    {
        /// <inheritdoc/>
        public IMLMethod Decode(IGenome genome)
        {
            MLMethodGenome genome2 = (MLMethodGenome)genome;
            genome2.Decode();
            return genome2.Phenotype;
        }

        /// <inheritdoc/>
        public IGenome Encode(IMLMethod phenotype)
        {
            IMLEncodable phenotype2 = (IMLEncodable)phenotype;
            return new MLMethodGenome(phenotype2);
        }

    }
}
