// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Temporal
{
    /// <summary>
    /// A point in tme for a temporal data set.
    /// </summary>
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

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
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

        /// <summary>
        /// The sequence number, used to sort.
        /// </summary>
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

        /// <summary>
        /// Allowes indexed access to the data.
        /// </summary>
        /// <param name="x">The index.</param>
        /// <returns>The data at the specified index.</returns>
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
        public override String ToString()
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
