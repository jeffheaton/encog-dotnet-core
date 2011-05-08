using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Normalize
{
    /// <summary>
    /// This object holds the normalization stats for a column.  This includes
    /// the actual and desired high-low range for this column.
    /// </summary>
    public class NormalizedFieldStats
    {
        /// <summary>
        /// The actual high from the sample data.
        /// </summary>
        public double ActualHigh { get; set; }

        /// <summary>
        /// The actual low from the sample data.
        /// </summary>
        public double ActualLow { get; set; }

        /// <summary>
        /// The desired normalized high.
        /// </summary>
        public double NormalizedHigh { get; set; }

        /// <summary>
        /// The desired normalized low from the sample data.
        /// </summary>
        public double NormalizedLow { get; set; }

        /// <summary>
        /// The action that should be taken on this column.
        /// </summary>
        public NormalizationDesired Action { get; set; }

        /// <summary>
        /// The name of this column.
        /// </summary>
        public String Name { get; set; }


        /// <summary>
        /// Construct an object.
        /// </summary>
        /// <param name="action">The desired action.</param>
        /// <param name="name">The name of this column.</param>
        public NormalizedFieldStats(NormalizationDesired action, String name)
            : this(action, name, 0, 0, 0, 0)
        {
        }

        /// <summary>
        /// Construct the field, with no defaults.
        /// </summary>
        /// <param name="action">The normalization action to take.</param>
        /// <param name="name">The name of this field.</param>
        /// <param name="ahigh">The actual high.</param>
        /// <param name="alow">The actual low.</param>
        /// <param name="nhigh">The normalized high.</param>
        /// <param name="nlow">The normalized low.</param>
        public NormalizedFieldStats(NormalizationDesired action, String name, double ahigh, double alow, double nhigh, double nlow)
        {
            this.Action = action;
            this.ActualHigh = ahigh;
            this.ActualLow = alow;
            this.NormalizedHigh = nhigh;
            this.NormalizedLow = nlow;
            this.Name = name;
        }

        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="normalizedHigh">The normalized high.</param>
        /// <param name="normalizedLow">The normalized low.</param>
        public NormalizedFieldStats(double normalizedHigh, double normalizedLow)
        {
            this.NormalizedHigh = normalizedHigh;
            this.NormalizedLow = normalizedLow;
            this.ActualHigh = Double.MinValue;
            this.ActualLow = Double.MaxValue;
            this.Action = NormalizationDesired.Normalize;
        }

        /// <summary>
        /// Construct the object with a range of 1 and -1.
        /// </summary>
        public NormalizedFieldStats()
            : this(1, -1)
        {
        }

        /// <summary>
        /// Make this a pass-through field.
        /// </summary>
        public void MakePassThrough()
        {
            this.NormalizedHigh = 0;
            this.NormalizedLow = 0;
            this.ActualHigh = 0;
            this.ActualLow = 0;
            this.Action = NormalizationDesired.PassThrough;
        }

        /// <summary>
        /// Analyze the specified value.  Adjust min/max as needed.  Usually used only internally.
        /// </summary>
        /// <param name="d">The value to analyze.</param>
        public void Analyze(double d)
        {
            this.ActualHigh = Math.Max(this.ActualHigh, d);
            this.ActualLow = Math.Min(this.ActualLow, d);
        }

        /// <summary>
        /// Normalize the specified value.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double Normalize(double value)
        {
            return ((value - ActualLow)
                    / (ActualHigh - ActualLow))
                    * (NormalizedHigh - NormalizedLow) + NormalizedLow;
        }

        /// <summary>
        /// Denormalize the specified value.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        public double DeNormalize(double value)
        {
            double result = ((ActualLow - ActualHigh) * value - NormalizedHigh
                    * ActualLow + ActualHigh * NormalizedLow)
                    / (NormalizedLow - NormalizedHigh);
            return result;
        }

        /// <summary>
        /// Fix normalized fields that have a single value for the min/max.  Separate them by 2 units.
        /// </summary>
        public void FixSingleValue()
        {
            if (Action == NormalizationDesired.Normalize)
            {
                if (Math.Abs(ActualHigh - ActualLow) < EncogFramework.DEFAULT_DOUBLE_EQUAL)
                {
                    ActualHigh += 1;
                    ActualLow -= 1;
                }
            }
        }
    }
}
