using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Segregate
{
    /// <summary>
    /// Specifies a range that might be included or excluded.
    /// </summary>
    public class SegregationRange
    {
        /// <summary>
        /// The low end of this range.
        /// </summary>
        [EGAttribute]
        private double low;

        /// <summary>
        /// The high end of this range.
        /// </summary>
        [EGAttribute]
        private double high;

        /// <summary>
        /// Should this range be included.
        /// </summary>
        [EGAttribute]
        private bool include;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public SegregationRange()
        {

        }

        /// <summary>
        /// Construct a segregation range.
        /// </summary>
        /// <param name="low">The low end of the range.</param>
        /// <param name="high">The high end of the range.</param>
        /// <param name="include">Specifies if the range should be included.</param>
        public SegregationRange(double low, double high,
                 bool include)
        {

            this.low = low;
            this.high = high;
            this.include = include;
        }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// Is this value within the range. 
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is within the range.</returns>
        public bool InRange(double value)
        {
            return ((value >= this.low) && (value <= this.high));
        }

        /// <summary>
        /// True if this range should be included.
        /// </summary>
        public bool IsIncluded
        {
            get
            {
                return this.include;
            }
        }
    }
}
