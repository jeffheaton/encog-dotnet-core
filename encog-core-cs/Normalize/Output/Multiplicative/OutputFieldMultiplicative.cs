using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;

namespace Encog.Normalize.Output.Multiplicative
{
    /// <summary>
    ///  * Both the multiplicative and z-axis normalization types allow a group of 
    /// outputs to be adjusted so that the "vector length" is 1.  Both go about it
    /// in different ways.  Certain types of neural networks require a vector length 
    /// of 1.
    /// 
    /// The multiplicative normalization is more simple than Z-Axis normalization.  
    /// Almost always Z=Axis normalization is a better choice.  However, 
    /// multiplicative can perform better than Z-Axis when all of the values
    /// are near zero most of the time.  This can cause the "synthetic value"
    /// that z-axis uses to dominate and skew the answer.
    /// 
    ///  Multiplicative normalization works by calculating the vector length of
    ///  the input fields and dividing each by that value.  This also presents 
    ///  a problem, as the magnitude of the original fields is not used.  For 
    ///  example, multiplicative normalization would not distinguish between
    ///  (-2,1,3) and (-10,5,15).  Both would result in the same output.   
    /// </summary>
    public class OutputFieldMultiplicative : OutputFieldGrouped
    {
        /// <summary>
        /// The default constructor.  Used for reflection.
        /// </summary>
        public OutputFieldMultiplicative()
        {

        }

        /// <summary>
        /// Construct a multiplicative output field.
        /// </summary>
        /// <param name="group">The group this field belongs to.</param>
        /// <param name="field">The input field that this field is based on.</param>
        public OutputFieldMultiplicative(IOutputFieldGroup group,
                 IInputField field)
            : base(group, field)
        {
            if (!(group is MultiplicativeGroup))
            {
                throw new NormalizationError(
                        "Must use MultiplicativeGroup with OutputFieldMultiplicative.");
            }
        }

        /// <summary>
        /// Calculate the value for this output field.
        /// </summary>
        /// <param name="subfield">The subfield is not used.</param>
        /// <returns>The value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return SourceField.CurrentValue
                    / ((MultiplicativeGroup)Group).Length;
        }

        /// <summary>
        /// Always returns 1, subfields are not used for this field.
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
