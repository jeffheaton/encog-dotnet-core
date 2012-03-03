using Encog.ML.Data;

namespace Encog.ML.HMM.Distributions
{
    /// <summary>
    /// This class represents a "state distribution". This is the means by which the
    /// probabilities between the states and observations are mapped. Currently two
    /// are supported. Use ContinousDistribution to use a Gaussian-based continuous
    /// distribution. Use DiscreteDistribution for a item-based distribution.
    /// </summary>
    public interface IStateDistribution
    {
        /// <summary>
        /// Clone this distribution.
        /// </summary>
        /// <returns>A clone of this distribution.</returns>
        IStateDistribution Clone();

        /// <summary>
        /// Fit this distribution to the specified data set.
        /// </summary>
        /// <param name="set">The data set to fit to.</param>
        void Fit(IMLDataSet set);


        /// <summary>
        /// Fit this distribution to the specified data set, given the specified
        /// weights, per element. 
        /// </summary>
        /// <param name="set">The data set to fit to.</param>
        /// <param name="weights">The weights.</param>
        void Fit(IMLDataSet set, double[] weights);

        /// <summary>
        /// Generate a random data pair, based on the probabilities.
        /// </summary>
        /// <returns>A random data pair.</returns>
        IMLDataPair Generate();

        /// <summary>
        /// Determine the probability of the specified data pair.
        /// </summary>
        /// <param name="o">The pair to consider.</param>
        /// <returns>The probability.</returns>
        double Probability(IMLDataPair o);
    }
}
