using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output.Nominal
{
    /// <summary>
    /// A nominal item.
    /// </summary>
    public class NominalItem
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
        /// The input field used to verify against the range.
        /// </summary>
        [EGReference]
        private IInputField inputField;

        /// <summary>
        /// Construct a empty range item.  Used mainly for reflection.
        /// </summary>
        public NominalItem()
        {
        }

        /// <summary>
        /// Create a nominal item.
        /// </summary>
        /// <param name="inputField">The field that this item is based on.</param>
        /// <param name="high">The high value.</param>
        /// <param name="low">The low value.</param>
        public NominalItem(IInputField inputField, double high,
                 double low)
        {
            this.high = high;
            this.low = low;
            this.inputField = inputField;
        }

        /// <summary>
        /// Begin a row.
        /// </summary>
        public void BeginRow()
        {
        }

        /// <summary>
        /// The high value.
        /// </summary>
        public double High
        {
            get
            {
                return this.high;
            }
        }

        /// <summary>
        /// The input field value.
        /// </summary>
        public IInputField InputField
        {
            get
            {
                return this.inputField;
            }
        }

        /// <summary>
        /// The low value.
        /// </summary>
        public double Low
        {
            get
            {
                return this.low;
            }
        }

        /// <summary>
        /// Determine if the specified value is in range.
        /// </summary>
        /// <returns>True if this item is within range.</returns>
        public bool IsInRange()
        {
            double currentValue = this.inputField.CurrentValue;
            return ((currentValue >= this.low) && (currentValue <= this.high));
        }
    }
}
