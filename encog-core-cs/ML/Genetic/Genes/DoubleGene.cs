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
    /// A gene that contains a floating point value.
    /// </summary>
    ///
    public class DoubleGene : BasicGene
    {
        /// <summary>
        /// The value of this gene.
        /// </summary>
        ///
        private double _value;

        /// <summary>
        /// Set the value of the gene.
        /// </summary>
        ///
        /// <value>The gene's value.</value>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene to copy.</param>
        public override sealed void Copy(IGene gene)
        {
            _value = ((DoubleGene)gene).Value;
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            return "" + _value;
        }
    }
}
