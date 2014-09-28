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

namespace Encog.Util.Normalize.Segregate.Index
{
    /// <summary>
    /// An index segregator is used to segregate the data according to its index.
    /// Nothing about the data is actually compared. This makes the index range
    /// segregator very useful for breaking the data into training and validation
    /// sets. For example, you could very easily determine that 70% of the data is
    /// for training, and 30% for validation.
    /// 
    /// This segregator takes a starting and ending index. Everything that is between
    /// these two indexes will be used.
    /// </summary>
    [Serializable]
    public class IndexRangeSegregator : IndexSegregator
    {
        /// <summary>
        /// The ending index.
        /// </summary>        
        private readonly int _endingIndex;

        /// <summary>
        /// The starting index.
        /// </summary>
        private readonly int _startingIndex;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public IndexRangeSegregator()
        {
        }

        /// <summary>
        /// Construct an index range segregator.
        /// </summary>
        /// <param name="startingIndex">The starting index to allow.</param>
        /// <param name="endingIndex">The ending index to allow.</param>
        public IndexRangeSegregator(int startingIndex, int endingIndex)
        {
            _startingIndex = startingIndex;
            _endingIndex = endingIndex;
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int EndingIndex
        {
            get { return _endingIndex; }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get { return _startingIndex; }
        }

        /// <summary>
        /// Determines if the current row should be included.
        /// </summary>
        /// <returns>True if the current row should be included.</returns>
        public override bool ShouldInclude()
        {
            bool result = ((CurrentIndex >= _startingIndex) && (CurrentIndex <= _endingIndex));
            RollIndex();
            return result;
        }
    }
}
