using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Output.Multiplicative
{
    /// <summary>
    /// Used to group multiplicative fields together.
    /// </summary>
    public class MultiplicativeGroup : BasicOutputFieldGroup
    {
        /// <summary>
        /// The "length" of this field.
        /// </summary>
        private double length;

        /// <summary>
        /// The length of this field.  This is the sum of the squares of
        /// all of the groupped fields.  The square root of this sum is the 
        /// length. 
        /// </summary>
        public double Length
        {
            get
            {
                return this.length;
            }
        }

        /// <summary>
        /// Called to init this group for a new field.  This recalculates the
        /// "length".
        /// </summary>
        public override void RowInit()
        {
            double value = 0;

            foreach (OutputFieldGrouped field in this.GroupedFields)
            {
                value += (field.SourceField.CurrentValue * field
                        .SourceField.CurrentValue);
            }
            this.length = Math.Sqrt(value);
        }

    }
}
