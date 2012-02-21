using System;

namespace Encog.ML.Bayesian.Parse
{
    /// <summary>
    /// A parsed choice.
    /// </summary>
    public class ParsedChoice
    {
        /// <summary>
        /// The label for this choice.
        /// </summary>
        private readonly String _label;

        /// <summary>
        /// The max value for this choice.
        /// </summary>
        private readonly double _max;

        /// <summary>
        /// The min value for this choice.
        /// </summary>
        private readonly double _min;

        /// <summary>
        /// Construct a continuous choice, with a min and max. 
        /// </summary>
        /// <param name="label">The label, for this chocie.</param>
        /// <param name="min">The min value, for this choice.</param>
        /// <param name="max">The max value, for this choice.</param>
        public ParsedChoice(String label, double min, double max)
        {
            _label = label;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Construct a discrete value for this choice.
        /// </summary>
        /// <param name="label">The choice label.</param>
        /// <param name="index">The index.</param>
        public ParsedChoice(String label, int index)
        {
            _label = label;
            _min = index;
            _max = index;
        }

        /// <summary>
        /// The label.
        /// </summary>
        public String Label
        {
            get { return _label; }
        }

        /// <summary>
        /// The min value.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// The max value.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// True, if this choice is indexed, or discrete.
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
    }
}