//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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

namespace Encog.ML.Genetic.Genes
{
    /// <summary>
    /// A gene that holds a single character.
    /// </summary>
    ///
    public class CharGene : BasicGene
    {
        /// <summary>
        /// The character value of the gene.
        /// </summary>
        ///
        private char value_ren;

        /// <summary>
        /// Set the value of this gene.
        /// </summary>
        ///
        /// <value>The new value of this gene.</value>
        public char Value
        {
            get { return value_ren; }           
            set { value_ren = value; }
        }

        /// <summary>
        /// Copy another gene to this gene.
        /// </summary>
        ///
        /// <param name="gene">The source gene.</param>
        public override sealed void Copy(IGene gene)
        {
            value_ren = ((CharGene) gene).Value;
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            return "" + value_ren;
        }
    }
}
