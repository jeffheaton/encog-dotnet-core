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
    /// This segregator takes a starting and ending index. Everything that is between
    /// these two indexes will be used.
    /// </summary>
    public class IndexRangeSegregator : IndexSegregator
    {
        /// <summary>
        /// The starting index.
        /// </summary>
        [EGAttribute]
        private int startingIndex;

        /// <summary>
        /// The ending index.
        /// </summary>
        [EGAttribute]
        private int endingIndex;

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
        /// Determines if the current row should be included.
        /// </summary>
        /// <returns>True if the current row should be included.</returns>
        public override bool ShouldInclude()
        {
            bool result = ((CurrentIndex >= this.startingIndex) && (CurrentIndex <= this.endingIndex));
            RollIndex();
            return result;
        }

    }
}
