using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output.Mapped
{
    /// <summary>
    /// Simple class that is used internally to hold a range mapping.
    /// </summary>
    public class MappedRange
    {
        /// <summary>
        /// The low value for the range.
        /// </summary>
        [EGAttribute]
        private double low;

        /// <summary>
        /// The high value for the range.
        /// </summary>
        [EGAttribute]
        private double high;

        /// <summary>
        /// The value that should be returned for this range.
        /// </summary>
        [EGAttribute]
        private double value;

        /// <summary>
        /// Construct the range mapping.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that this range represents.</param>
        public MappedRange(double low, double high, double value)
        {
            this.low = low;
            this.high = high;
            this.value = value;
        }

        /// <summary>
        /// The high value for this range.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The low value for this range.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// The value that this range represents.
        /// </summary>
        public double Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Determine if the specified value is in the range.
        /// </summary>
        /// <param name="d">The value to check.</param>
        /// <returns>True if this value is within the range.</returns>
        public bool InRange(double d)
        {
            if ((d >= this.low) && (d <= this.high))
            {
                return true;
            }
            return false;
        }
    }
}
