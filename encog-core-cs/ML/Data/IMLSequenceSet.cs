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

namespace Encog.ML.Data
{
    /// <summary>
    /// A sequence set is a collection of data sets. Where each individual data set
    /// is one "unbroken sequence" within the sequence set. This allows individual
    /// observations to occur individually, indicating a break between them.
    /// 
    /// The sequence set, itself, is a data set, so it can be used with any Encog
    /// trainer. However, not all trainers are aware of sequence sets. Further, some
    /// machine learning methods are unaffected by them. Sequence sets are typically
    /// used with Hidden Markov Models (HMM)'s.
    /// </summary>
    public interface IMLSequenceSet : IMLDataSetAddable
    {
        /// <summary>
        /// Cause a "break" in the data by creating a the beginning of a new sequence.
        /// </summary>
        void StartNewSequence();

        /// <summary>
        /// Get a count of the number of sequences.
        /// </summary>
        int SequenceCount { get; }

        /// <summary>
        /// Get an individual sequence. 
        /// </summary>
        /// <param name="i">The index to get.</param>
        /// <returns>The sequence.</returns>
        IMLDataSet GetSequence(int i);

        /// <summary>
        /// A list of all of the sequences.
        /// </summary>
        /// <returns>The index of the sequence.</returns>
        ICollection<IMLDataSet> Sequences { get; }
        
        /// <summary>
        /// Add a new sequence. 
        /// </summary>
        /// <param name="sequence">The sequence to add.</param>
        void Add(IMLDataSet sequence);
    }
}
