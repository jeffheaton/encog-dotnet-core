using Encog.ML.Data;

namespace Encog.ML
{
    /// <summary>
    /// Defines a Machine Learning Method that supports regression.  Regression 
    /// takes an input and produces numeric output.  Function approximation 
    /// uses regression.  Contrast this to classification, which uses the input 
    /// to assign a class.
    /// </summary>
    ///
    public interface MLRegression : MLInputOutput
    {
        /// <summary>
        /// Compute regression.
        /// </summary>
        ///
        /// <param name="input">The input data.</param>
        /// <returns>The output data.</returns>
        MLData Compute(MLData input);
    }
}