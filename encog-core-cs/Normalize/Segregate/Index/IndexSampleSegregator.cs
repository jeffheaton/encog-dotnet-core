// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Segregate.Index
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
    public class IndexSampleSegregator : IndexSegregator
    {
        /// <summary>
        /// The starting index (within a sample).
        /// </summary>
        [EGAttribute]
        private int startingIndex;

        /// <summary>
        /// The ending index (within a sample).
        /// </summary>
        [EGAttribute]
        private int endingIndex;

        /// <summary>
        /// The sample size.
        /// </summary>
        [EGAttribute]
        private int sampleSize;

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
            this.sampleSize = sampleSize;
            this.startingIndex = startingIndex;
            this.endingIndex = endingIndex;
        }

        /// <summary>
        /// The ending index.
        /// </summary>
        public int EndingIndex
        {
            get
            {
                return this.endingIndex;
            }
        }

        /// <summary>
        /// The sample size.
        /// </summary>
        public int SampleSize
        {
            get
            {
                return this.sampleSize;
            }
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public int StartingIndex
        {
            get
            {
                return this.startingIndex;
            }
        }

        /// <summary>
        /// Should this row be included.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public override bool ShouldInclude()
        {
            int sampleIndex = CurrentIndex % this.sampleSize;
            RollIndex();
            return ((sampleIndex >= this.startingIndex) && (sampleIndex <= this.endingIndex));
        }

    }
}
