// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
using Encog.Persist.Attributes;

namespace Encog.Normalize.Segregate.Index
{
    /// <summary>
    ///  The index segregator. An abstract class to build index based segregators off
    /// of. An index segregator is used to segregate the data according to its index.
    /// Nothing about the data is actually compared. This makes the index range
    /// segregator very useful for breaking the data into training and validation
    /// sets. For example, you could very easily determine that 70% of the data is
    /// for training, and 30% for validation.
    /// </summary>
    public abstract class IndexSegregator : ISegregator
    {
        /// <summary>
        /// The current index.  Updated rows are processed.
        /// </summary>
        [EGIgnore]
        private int currentIndex = 0;

        /// <summary>
        /// THe normalization object this belongs to.
        /// </summary>
        [EGReference]
        private DataNormalization normalization;

        /// <summary>
        /// The current index.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return this.currentIndex;
            }
        }

        /// <summary>
        /// The normalization object this object will use.
        /// </summary>
        public DataNormalization Owner
        {
            get
            {
                return this.normalization;
            }
        }

        /// <summary>
        /// Setup this class with the specified normalization object.
        /// </summary>
        /// <param name="normalization">Normalization object.</param>
        public void Init(DataNormalization normalization)
        {
            this.normalization = normalization;
        }

       /// <summary>
        /// Used to increase the current index as data is processed.
       /// </summary>
        public void RollIndex()
        {
            this.currentIndex++;
        }

        /// <summary>
        /// Should this row be included, according to this segregator.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public abstract bool ShouldInclude();

    }
}
