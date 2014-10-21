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
    /// This segregator takes a starting and ending index, as well as a smple size.
    /// Everything that is between these two indexes will be used.  The sample 
    /// repeats over and over.  For example, if you choose a sample size of 10, 
    /// and a beginning index of 0 and an ending index of 5, you would get
    /// half of the first 10 element, then half of the next ten, and so on.
    /// 
    /// </summary>
    [Serializable]
    public class IndexSampleSegregator : IndexSegregator
    {
        /// <summary>
        /// The ending index (within a sample).
        /// </summary>
        private readonly int _endingIndex;

        /// <summary>
        /// The sample size.
        /// </summary>
        private readonly int _sampleSize;

        /// <summary>
        /// The starting index (within a sample).
        /// </summary>
        private readonly int _startingIndex;

        /// <summary>
        /// The default constructor, for reflection.
        /// </summary>
        public IndexSampleSegregator()
        {
        }

        /// <summary>
        /// Construct an index sample segregator.
        /// </summary>
        /// <param name="startingIndex">The starting index.</param>
        /// <param name="endingIndex">The ending index.</param>
        /// <param name="sampleSize">The sample size.</param>
        public IndexSampleSegregator(int startingIndex,
                                     int endingIndex, int sampleSize)
        {
            _sampleSize = sampleSize;
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
        /// The sample size.
        /// </summary>
        public int SampleSize
        {
            get { return _sampleSize; }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get { return _startingIndex; }
        }

        /// <summary>
        /// Should this row be included.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public override bool ShouldInclude()
        {
            int sampleIndex = CurrentIndex%_sampleSize;
            RollIndex();
            return ((sampleIndex >= _startingIndex) && (sampleIndex <= _endingIndex));
        }
    }
}
