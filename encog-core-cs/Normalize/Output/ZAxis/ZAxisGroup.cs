using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Output.ZAxis
{
    /// <summary>
    /// Used to group Z-Axis fields together. Both OutputFieldZAxis and
    /// OutputFieldZAxisSynthetic fields may belong to this group. For
    /// more information see the OutputFieldZAxis class.
    /// </summary>
    public class ZAxisGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The calculated length.
        /// </summary>
        private double length;

        /// <summary>
        /// The multiplier, which is the value that all other values will be
        /// multiplied to become normalized.
        /// </summary>
        private double multiplier;

        /// <summary>
        /// The vector length.
        /// </summary>
        public double Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// The value to multiply the other values by to normalize them.
        /// </summary>
        public double Multiplier
        {
            get
            {
                return this.multiplier;
            }
        }

        /// <summary>
        /// Initialize this group for a new row.
        /// </summary>
        public override void RowInit()
        {
            double value = 0;

            foreach (OutputFieldGrouped field in this.GroupedFields)
            {
                if (!(field is OutputFieldZAxisSynthetic))
                {
                    if (field.SourceField != null)
                    {
                        value += (field.SourceField.CurrentValue * field
                                .SourceField.CurrentValue);
                    }
                }
            }
            this.length = Math.Sqrt(value);
            this.multiplier = 1.0 / Math.Sqrt(GroupedFields.Count);
        }

    }
}
