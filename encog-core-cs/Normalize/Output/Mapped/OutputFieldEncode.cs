using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;

namespace Encog.Normalize.Output.Mapped
{
    /// <summary>
    /// An encoded output field.  This allows ranges of output values to be
    /// mapped to specific values.
    /// </summary>
    public class OutputFieldEncode : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        private IInputField sourceField;

        /// <summary>
        /// The catch all value, if nothing matches, then use this value.
        /// </summary>
        private double catchAll;

        /// <summary>
        /// The ranges.
        /// </summary>
        private IList<MappedRange> ranges = new List<MappedRange>();


        /// <summary>
        /// Construct an encoded field.
        /// </summary>
        /// <param name="sourceField">The field that this is based on.</param>
        public OutputFieldEncode(IInputField sourceField)
        {
            this.sourceField = sourceField;
        }

        /// <summary>
        /// Add a ranged mapped to a value.
        /// </summary>
        /// <param name="low">The low value for the range.</param>
        /// <param name="high">The high value for the range.</param>
        /// <param name="value">The value that the field should produce for this range.</param>
        public void addRange(double low, double high, double value)
        {
            MappedRange range = new MappedRange(low, high, value);
            this.ranges.Add(range);
        }

        /// <summary>
        /// Calculate the value for this field.
        /// </summary>
        /// <param name="subfield">Not used.</param>
        /// <returns>Return the value for the range the input falls within, or return
        /// the catchall if nothing matches.</returns>
        public override double Calculate(int subfield)
        {
            foreach (MappedRange range in this.ranges)
            {
                if (range.InRange(this.sourceField.CurrentValue))
                {
                    return range.Value;
                }
            }

            return this.catchAll;
        }

        /// <summary>
        /// The source field.
        /// </summary>
        public IInputField SourceField
        {
            get
            {
                return this.sourceField;
            }
        }

        /// <summary>
        /// Return 1, no subfield supported.
        /// </summary>
        public override int SubfieldCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Not needed for this sort of output field.
        /// </summary>
        public override void RowInit()
        {
        }

        /// <summary>
        /// The catch all value that is to be returned if none
        /// of the ranges match.
        /// </summary>
        public double CatchAll
        {
            get
            {
                return this.catchAll;
            }
            set
            {
                this.catchAll = value;
            }
        }
    }
}
