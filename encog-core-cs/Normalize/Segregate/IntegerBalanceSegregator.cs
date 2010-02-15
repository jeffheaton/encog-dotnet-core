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
using Encog.Normalize.Input;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Segregate
{
    /// <summary>
    /// Balance based on an input value. This allows you to make sure that one input
    /// class does not saturate the training data. To do this, you specify the input
    /// value to check and the number of occurrences of each integer value of this
    /// field to allow.
    /// </summary>
    public class IntegerBalanceSegregator : ISegregator
    {
        /// <summary>
        /// The normalization object to use.
        /// </summary>
        [EGReference]
        private DataNormalization normalization;

        /// <summary>
        /// The input field.
        /// </summary>
        [EGReference]
        private IInputField target;

        /// <summary>
        /// The count per each of the int values for the input field.
        /// </summary>
        private int count;

        /// <summary>
        /// The running totals.
        /// </summary>
        [EGIgnore]
        private IDictionary<int, int> runningCounts = new Dictionary<int, int>();

        /// <summary>
        /// Construct a balanced segregator.
        /// </summary>
        /// <param name="target">The input field to base this on, should 
        /// be an integer value.</param>
        /// <param name="count">The number of rows to accept from each 
        /// unique value for the input.</param>
        public IntegerBalanceSegregator(IInputField target, int count)
        {
            this.target = target;
            this.count = count;
        }

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public IntegerBalanceSegregator()
        {
        }

        /// <summary>
        /// The owner of this segregator.
        /// </summary>
        public DataNormalization Owner
        {
            get
            {
                return this.normalization;
            }
        }

        /// <summary>
        /// Get information on how many rows fall into each group.
        /// </summary>
        /// <returns>A string that contains the counts for each group.</returns>
        public String DumpCounts()
        {
            StringBuilder result = new StringBuilder();

            foreach (int key in this.runningCounts.Keys)
            {
                int value = this.runningCounts[key];
                result.Append(key);
                result.Append(" -> ");
                result.Append(value);
                result.Append(" count\n");

            }

            return result.ToString();
        }

        /// <summary>
        /// The number of groups found.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// A map of the running count for each group.
        /// </summary>
        public IDictionary<int, int> RunningCounts
        {
            get
            {
                return this.runningCounts;
            }
        }

        /// <summary>
        /// The target input field.
        /// </summary>
        public IInputField Target
        {
            get
            {
                return this.target;
            }
        }

        /// <summary>
        /// Init the segregator with the owning normalization object.
        /// </summary>
        /// <param name="normalization">The data normalization object to use.</param>
        public void Init(DataNormalization normalization)
        {
            this.normalization = normalization;

        }

        /// <summary>
        /// Init for a new pass.
        /// </summary>
        public void PassInit()
        {
            this.runningCounts.Clear();
        }

        /// <summary>
        /// Determine of the current row should be included.
        /// </summary>
        /// <returns>True if the current row should be included.</returns>
        public bool ShouldInclude()
        {
            int key = (int)this.target.CurrentValue;
            int value = 0;
            if (this.runningCounts.ContainsKey(key))
            {
                value = this.runningCounts[key];
            }

            if (value < this.count)
            {
                value++;
                this.runningCounts[key] = value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
