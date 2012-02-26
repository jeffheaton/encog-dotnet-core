using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Fuzzy
{
    using System;

    /// <summary>
    /// Interface which specifies set of methods required to be implemented by all membership
    /// functions.
    /// This interface is to be used on double (not float like AForge's version).
    /// </summary>
    /// <remarks>
    /// All membership functions must implement this interface, which is used by
    /// <see cref="FuzzySet"/> class to calculate value's membership to a particular fuzzy set.
    /// </remarks>
    public interface IMembershipFunctionDoubles
    {
        /// <summary>
        /// Calculate membership of a given value to the fuzzy set.
        /// </summary>
        /// 
        /// <param name="x">Value which membership will to be calculated.</param>
        /// 
        /// <returns>Degree of membership [0..1] of the value to the fuzzy set.</returns>
        /// 
        double GetMembership(double x);

        /// <summary>
        /// The leftmost x value of the membership function.
        /// </summary>
        double LeftLimit { get; }

        /// <summary>
        /// The rightmost x value of the membership function.
        /// </summary>
        double RightLimit { get; }

    }
}
