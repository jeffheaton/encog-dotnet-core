using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        readonly private String label;

        /// <summary>
        /// The min value for this choice.
        /// </summary>
        readonly private double min;

        /// <summary>
        /// The max value for this choice.
        /// </summary>
        readonly private double max;

        /// <summary>
        /// Construct a continuous choice, with a min and max. 
        /// </summary>
        /// <param name="label">The label, for this chocie.</param>
        /// <param name="min">The min value, for this choice.</param>
        /// <param name="max">The max value, for this choice.</param>
        public ParsedChoice(String label, double min, double max)
        {
            this.label = label;
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Construct a discrete value for this choice.
        /// </summary>
        /// <param name="label">The choice label.</param>
        /// <param name="index">The index.</param>
        public ParsedChoice(String label, int index)
        {
            this.label = label;
            this.min = index;
            this.max = index;
        }

        /// <summary>
        /// The label.
        /// </summary>
        public String Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// The min value.
        /// </summary>
        public double Min
        {
            get
            {
                return min;
            }
        }

        /// <summary>
        /// The max value.
        /// </summary>
        public double Max
        {
            get
            {
                return max;
            }
        }

        /// <summary>
        /// True, if this choice is indexed, or discrete.
        /// </summary>
        public bool IsIndex
        {
            get
            {
                return Math.Abs(this.min - this.max) < EncogFramework.DefaultDoubleEqual;
            }
        }

        /// <inheritdoc/>
        public String ToString()
        {
            return this.label;
        }
    }
}
