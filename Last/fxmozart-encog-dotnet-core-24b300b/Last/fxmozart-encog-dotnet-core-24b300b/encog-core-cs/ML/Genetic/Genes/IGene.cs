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
    /// Describes a gene. A gene is the smallest piece of genetic information in
    /// Encog.
    /// </summary>
    ///
    public interface IGene : IComparable<IGene>
    {
        /// <summary>
        /// Get the ID of this gene, -1 for undefined.
        /// </summary>
        ///
        /// <value>The ID of this gene.</value>
        long Id { 
            get; }


        /// <value>The innovation ID of this gene.</value>
        long InnovationId { 
            get; }


        /// <summary>
        /// Determine if this gene is enabled.
        /// </summary>
        ///
        /// <value>True if this gene is enabled.</value>
        bool Enabled { 
            get;
            set; }

        /// <summary>
        /// Copy another gene to this one.
        /// </summary>
        ///
        /// <param name="gene">The other gene to copy.</param>
        void Copy(IGene gene);
    }
}
