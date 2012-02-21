using System;
using System.Text;
using Encog.Util.CSV;

namespace Encog.ML.Bayesian
{
    /// <summary>
    /// A choice in a Bayesian network. Choices can be either discrete or continuous.
    /// For continuous choices the number must be made discrete by mapping it to
    /// discrete ranges.
    /// </summary>
    [Serializable]
    public class BayesianChoice
    {
        /// <summary>
        /// The label for this choice.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The max values, if continuous, or the index if discrete.
        /// </summary>
        private readonly double _max;

        /// <summary>
        /// The min values, if continuous, or the index if discrete.
        /// </summary>
        private readonly double _min;

        /// <summary>
        /// Construct a continuous choice that covers the specified range. 
        /// </summary>
        /// <param name="label">The label for this choice.</param>
        /// <param name="min">The minimum value for this range.</param>
        /// <param name="max">The maximum value for this range.</param>
        public BayesianChoice(String label, double min, double max)
        {
            _label = label;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Construct a discrete choice for the specified index. 
        /// </summary>
        /// <param name="label">The label for this choice.</param>
        /// <param name="index">The index for this choice.</param>
        public BayesianChoice(String label, int index)
        {
            _label = label;
            _min = index;
            _max = index;
        }

        /// <summary>
        /// Get the label.
        /// </summary>
        public String Label
        {
            get { return _label; }
        }

        /// <summary>
        /// Get the min.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// Get the max.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// True, if this choice has an index, as opposed to min/max. If the
        /// value has an idex, then it is discrete.
        /// </summary>
        public bool IsIndex
        {
            get { return Math.Abs(_min - _max) < EncogFramework.DefaultDoubleEqual; }
        }

        /// <inheritdoc/>
        public override String ToString()
        {
            return _label;
        }

        /// <summary>
        /// A string representation of this choice.
        /// </summary>
        /// <returns>A string representation of this choice.</returns>
        public String ToFullString()
        {
            var result = new StringBuilder();
            result.Append(Label);
            if (!IsIndex)
            {
                result.Append(":");
                result.Append(CSVFormat.EgFormat.Format(Min, 4));
                result.Append(" to ");
                result.Append(CSVFormat.EgFormat.Format(Max, 4));
            }
            return result.ToString();
        }
    }
}