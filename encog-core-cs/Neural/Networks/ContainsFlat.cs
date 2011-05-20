using Encog.ML;
using Encog.Neural.Flat;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Interface that specifies that a machine learning method contains a 
    /// flat network.
    /// </summary>
    ///
    public interface ContainsFlat : MLMethod
    {
        /// <value>The flat network associated with this neural network.</value>
        FlatNetwork Flat { get; }
    }
}