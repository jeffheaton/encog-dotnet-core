using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Input
{
    /// <summary>
    /// Is this input field of a fixed length, such as an array?  Or is it
    /// read "iterator style" where we call "next" until there is no more 
    /// data.  If the length can be "known" ahead of time, then the input 
    /// field should support this interface.
    /// </summary>
    public interface IHasFixedLength
    {
        /// <summary>
        /// The number of records in this input field.
        /// </summary>
        int Length { get; }
    }
}
