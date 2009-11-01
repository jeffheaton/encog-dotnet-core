using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Persist.Attributes;
using Encog.Normalize.Input;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// A direct output field, will simply pass the input value to the output.
    /// </summary>
    public class OutputFieldDirect : BasicOutputField
    {
        /// <summary>
        /// The source field.
        /// </summary>
        [EGReference]
        private IInputField sourceField;

        /// <summary>
        /// Construct a direct output field.
        /// </summary>
        /// <param name="sourceField">The source field to pass directly on.</param>
        public OutputFieldDirect(IInputField sourceField)
        {
            this.sourceField = sourceField;
        }

        /// <summary>
        /// Calculate the value for this field. This will simply be the
        /// value from the input field. 
        /// </summary>
        /// <param name="subfield">Not used, as this output field type does not
        /// support subfields.</param>
        /// <returns>The calculated value.</returns>
        public override double Calculate(int subfield)
        {
            return this.sourceField.CurrentValue;
        }

        /// <summary>
        /// Always returns 1, as subfields are not used.
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
    }
}
