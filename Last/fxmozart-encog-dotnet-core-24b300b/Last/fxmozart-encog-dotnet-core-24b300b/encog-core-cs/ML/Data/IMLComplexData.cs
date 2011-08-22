using Encog.MathUtil;

namespace Encog.ML.Data
{
    /// <summary>
    /// This class implements a data object that can hold complex numbers.  It 
    /// implements the interface MLData, so it can be used with nearly any Encog 
    /// machine learning method.  However, not all Encog machine learning methods 
    /// are designed to work with complex numbers.  A Encog machine learning method 
    /// that does not support complex numbers will only be dealing with the 
    /// real-number portion of the complex number.
    /// </summary>
    public interface IMLComplexData : IMLData
    {
        /// <summary>
        /// The complex numbers.
        /// </summary>
        ComplexNumber[] ComplexData { get; }
        
        /// <summary>
        /// Get the complex data at the specified index. 
        /// </summary>
        /// <param name="index">The index to get the complex data at.</param>
        /// <returns>The complex data.</returns>
        ComplexNumber GetComplexData(int index);

        /// <summary>
        /// Set the complex number array.
        /// </summary>
        /// <param name="theData">The new array.</param>
        void SetComplexData(ComplexNumber[] theData);

        /// <summary>
        /// Set a data element to a complex number.
        /// </summary>
        /// <param name="index">The index to set.</param>
        /// <param name="d">The complex number.</param>
        void SetComplexData(int index, ComplexNumber d);
    }
}