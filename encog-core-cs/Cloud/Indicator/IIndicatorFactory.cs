using System;

namespace Encog.Cloud.Indicator
{
    /// <summary>
    /// A factory used to create indicators. This is passed to the indicator
    /// server. 
    /// </summary>
    public interface IIndicatorFactory
    {
        /// <summary>
        /// The name of the indicator.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Create the indicator.
        /// </summary>
        /// <returns>The new indicator.</returns>
        IIndicatorListener Create();
    }
}