using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Temporal
{
    public class TemporalPoint : IComparable<TemporalPoint>
    {

        /// <summary>
        /// The sequence number for this point.
        /// </summary>
        private int sequence;

        /// <summary>
        /// The data for this point.
        /// </summary>
        private double[] data;

        /// <summary>
        /// Construct a temporal point of the specified size.
        /// </summary>
        /// <param name="size">The size to create the temporal point for.</param>
        public TemporalPoint(int size)
        {
            this.data = new double[size];
        }

        public double[] Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
            set
            {
                this.sequence = value;
            }
        }

        /// <summary>
        /// Compare two temporal points.
        /// </summary>
        /// <param name="that">The other temporal point to compare.</param>
        /// <returns>Returns 0 if they are equal, less than 0 if this point is less,
        /// greater than zero if this point is greater.</returns>
        public int CompareTo(TemporalPoint that)
        {
            if (this.Sequence == that.Sequence )
            {
                return 0;
            }
            else if (this.Sequence < that.Sequence )
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public double this[int x]
        {
            get
            {
                return this.data[x];
            }
            set
            {
                this.data[x] = value;
            }
        }




        /**
         * Convert this point to string form.
         * @return This point as a string.
         */
        public new String ToString()
        {
            StringBuilder builder = new StringBuilder("[TemporalPoint:");
            builder.Append("Seq:");
            builder.Append(this.sequence);
            builder.Append(",Data:");
            for (int i = 0; i < this.data.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }
                builder.Append(this.data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
