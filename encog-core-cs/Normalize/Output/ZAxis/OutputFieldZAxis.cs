using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;

namespace Encog.Normalize.Output.ZAxis
{
    /// <summary>
    /// Both the multiplicative and z-axis normalization types allow a group of 
    /// outputs to be adjusted so that the "vector length" is 1.  Both go about it
    /// in different ways.  Certain types of neural networks require a vector length 
    /// of 1.
    /// 
    /// Z-Axis normalization is usually a better choice than multiplicative.    
    /// However, multiplicative can perform better than Z-Axis when all of the 
    /// values are near zero most of the time.  This can cause the "synthetic value"
    /// that z-axis uses to dominate and skew the answer.
    /// 
    ///  Z-Axis gets its name from 3D computer graphics, where there is a Z-Axis
    ///  extending from the plane created by the X and Y axes.  It has nothing to 
    ///  do with z-scores or the z-transform of signal theory.
    ///  
    ///  To implement Z-Axis normalization a scaling factor must be created to multiply
    ///  each of the inputs against.  Additionally, a synthetic field must be added.
    ///  It is very important that this synthetic field be added to any z-axis
    ///  group that you might use.  The synthetic field is represented by the
    ///  OutputFieldZAxisSynthetic class.
    /// </summary>
    public class OutputFieldZAxis : OutputFieldGrouped
    {
        /// <summary>
        /// Construct a ZAxis output field.
        /// </summary>
        /// <param name="group">The group this field belongs to.</param>
        /// <param name="field">The input field this is based on.</param>
        public OutputFieldZAxis(IOutputFieldGroup group,
                 IInputField field)
            : base(group, field)
        {
            if (!(group is ZAxisGroup))
            {
                throw new NormalizationError(
                        "Must use ZAxisGroup with OutputFieldZAxis.");
            }
        }

        /// <summary>
        /// Calculate the current value for this field. 
        /// </summary>
        /// <param name="subfield">Ignored, this field type does not have subfields.</param>
        /// <returns>The current value for this field.</returns>
        public override double Calculate(int subfield)
        {
            return (SourceField.CurrentValue * ((ZAxisGroup)Group)
                    .Multiplier);
        }

        /// <summary>
        /// The subfield count, which is one, as this field type does not
        /// have subfields.
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
