// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Normalize.Input;
using Encog.Persist.Attributes;

namespace Encog.Normalize.Segregate
{
    /// <summary>
    /// Range segregators are used to segregate data and include or exclude if it is
    /// within a certain range.
    /// </summary>
    public class RangeSegregator : ISegregator
    {
        /// <summary>
        /// The source field that this is based on.
        /// </summary>
        [EGReference]
        private IInputField sourceField;

        /// <summary>
        /// If none of the ranges match, should this data be included.
        /// </summary>
        private bool include;

        /// <summary>
        /// The ranges.
        /// </summary>
        private ICollection<SegregationRange> ranges = new List<SegregationRange>();

        /// <summary>
        /// The normalization object.
        /// </summary>
        [EGReference]
        private DataNormalization normalization;

        /// <summary>
        /// Default constructor for reflection.
        /// </summary>
        public RangeSegregator()
        {
        }

        /// <summary>
        /// Construct a range segregator.
        /// </summary>
        /// <param name="sourceField">The source field.</param>
        /// <param name="include">Default action, if the data is not in any of the ranges,
        /// should it be included.</param>
        public RangeSegregator(IInputField sourceField, bool include)
        {
            this.sourceField = sourceField;
            this.include = include;
        }


        /// <summary>
        /// Add a range.
        /// </summary>
        /// <param name="low">The low end of the range.</param>
        /// <param name="high">The high end of the range.</param>
        /// <param name="include">Should this range be included.</param>
        public void AddRange(double low, double high,
                 bool include)
        {
            SegregationRange range = new SegregationRange(low, high, include);
            AddRange(range);
        }
        
        /// <summary>
        /// Add a range.
        /// </summary>
        /// <param name="range">The range to add.</param>
        public void AddRange(SegregationRange range)
        {
            this.ranges.Add(range);
        }

        /// <summary>
        /// The normalization object used by this object.
        /// </summary>
        public DataNormalization Owner
        {
            get
            {
                return this.normalization;
            }
        }

        /// <summary>
        /// The source field that the ranges are compared against.
        /// </summary>
        public IInputField SourceField
        {
            get
            {
                return this.sourceField;
            }
        }

        /// <summary>
        /// Init the object.
        /// </summary>
        /// <param name="normalization">The normalization object that owns this range.</param>
        public void Init(DataNormalization normalization)
        {
            this.normalization = normalization;
        }

        /// <summary>
        /// True if the current row should be included according to this
        /// segregator.
        /// </summary>
        /// <returns></returns>
        public bool ShouldInclude()
        {
            double value = this.sourceField.CurrentValue;
            foreach (SegregationRange range in this.ranges)
            {
                if (range.InRange(value))
                {
                    return range.IsIncluded;
                }
            }
            return this.include;
        }

        /// <summary>
        /// Init for pass... nothing to do fo this class.
        /// </summary>
        public void PassInit()
        {
        }

    }
}
