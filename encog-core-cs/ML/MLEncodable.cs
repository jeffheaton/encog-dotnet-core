namespace Encog.ML
{
    /// <summary>
    /// Defines a Machine Learning Method that can be encoded to a double array.  
    /// This is very useful for certain training, such as genetic algorithms 
    /// and simulated annealing. 
    /// </summary>
    ///
    public interface MLEncodable : MLMethod
    {
        /// <returns>The length of an encoded array.</returns>
        int EncodedArrayLength();

        /// <summary>
        /// Encode the object to the specified array.
        /// </summary>
        ///
        /// <param name="encoded">The array.</param>
        void EncodeToArray(double[] encoded);

        /// <summary>
        /// Decode an array to this object.
        /// </summary>
        ///
        /// <param name="encoded">The encoded array.</param>
        void DecodeFromArray(double[] encoded);
    }
}