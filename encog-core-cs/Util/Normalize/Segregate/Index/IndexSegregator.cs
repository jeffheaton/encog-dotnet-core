//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;

namespace Encog.Util.Normalize.Segregate.Index
{
    /// <summary>
    ///  The index segregator. An abstract class to build index based segregators off
    /// of. An index segregator is used to segregate the data according to its index.
    /// Nothing about the data is actually compared. This makes the index range
    /// segregator very useful for breaking the data into training and validation
    /// sets. For example, you could very easily determine that 70% of the data is
    /// for training, and 30% for validation.
    /// </summary>
    [Serializable]
    public abstract class IndexSegregator : ISegregator
    {
        /// <summary>
        /// The current index.  Updated rows are processed.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// THe normalization object this belongs to.
        /// </summary>
        private DataNormalization _normalization;

        /// <summary>
        /// The current index.
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
        }

        #region ISegregator Members

        /// <summary>
        /// The normalization object this object will use.
        /// </summary>
        public DataNormalization Owner
        {
            get { return _normalization; }
        }

        /// <summary>
        /// Setup this class with the specified normalization object.
        /// </summary>
        /// <param name="normalization">Normalization object.</param>
        public void Init(DataNormalization normalization)
        {
            _normalization = normalization;
        }

        /// <summary>
        /// Should this row be included, according to this segregator.
        /// </summary>
        /// <returns>True if this row should be included.</returns>
        public abstract bool ShouldInclude();

        /// <summary>
        /// Init for pass... nothing to do fo this class.
        /// </summary>
        public void PassInit()
        {
            _currentIndex = 0;
        }

        #endregion

        /// <summary>
        /// Used to increase the current index as data is processed.
        /// </summary>
        public void RollIndex()
        {
            _currentIndex++;
        }
    }
}
