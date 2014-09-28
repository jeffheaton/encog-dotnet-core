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
using System.Collections;
using System.Collections.Generic;
using Encog.ML.Data.Basic;

// Encog(tm) Artificial Intelligence Framework v2.5
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

namespace Encog.ML.Data.Folded
{
    /// <summary>
    /// The enumerator for a folded dataset.
    /// </summary>
    public class FoldedEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private readonly FoldedDataSet _owner;

        /// <summary>
        /// The current index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// The current data item.
        /// </summary>
        private IMLDataPair _currentPair;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FoldedEnumerator(FoldedDataSet owner)
        {
            _owner = owner;
            _currentIndex = -1;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// The current object.
        /// </summary>
        public IMLDataPair Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move to the next record.
        /// </summary>
        /// <returns>True, if we were able to move to the next record.</returns>
        public bool MoveNext()
        {
            if (HasNext())
            {
				_currentPair = _owner[_currentIndex++];
                return true;
            }
            _currentPair = null;
            return false;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (_currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return _currentPair;
            }
        }

        #endregion

        /// <summary>
        /// Determine if there is a next record.
        /// </summary>
        /// <returns>True, if there is a next record.</returns>
        public bool HasNext()
        {
            return _currentIndex < _owner.CurrentFoldSize;
        }
    }
}
