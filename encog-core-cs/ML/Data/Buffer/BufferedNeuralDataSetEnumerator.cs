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
using System.IO;
using Encog.ML.Data.Basic;

namespace Encog.ML.Data.Buffer
{
    /// <summary>
    /// An enumerator to move through the buffered data set.
    /// </summary>
    public class BufferedNeuralDataSetEnumerator : IEnumerator<IMLDataPair>
    {
        /// <summary>
        /// The dataset being iterated over.
        /// </summary>
        private readonly BufferedMLDataSet _data;

        /// <summary>
        /// The current record.
        /// </summary>
        private int _current;

        /// <summary>
        /// The current record.
        /// </summary>
		private IMLDataPair _currentRecord;

        /// <summary>
        /// Construct the buffered enumerator. This is where the file is actually
        /// opened.
        /// </summary>
        /// <param name="owner">The object that created this enumeration.</param>
        public BufferedNeuralDataSetEnumerator(BufferedMLDataSet owner)
        {
            _data = owner;
            _current = 0;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// Get the current record
        /// </summary>
        public IMLDataPair Current
        {
            get { return _currentRecord; }
        }

        /// <summary>
        /// Dispose of the enumerator.
        /// </summary>
        public void Dispose()
        {
        }


        object IEnumerator.Current
        {
            get
            {
                if (_currentRecord == null)
                {
                    throw new IMLDataError("Can't read current record until MoveNext is called once.");
                }
                return _currentRecord;
            }
        }

        /// <summary>
        /// Move to the next element.
        /// </summary>
        /// <returns>True if there are more elements to read.</returns>
        public bool MoveNext()
        {
            try
            {
                if (_current >= _data.Count)
                    return false;

				_currentRecord = _data[_current++];
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        /// <summary>
        /// Resets the enumeration.
        /// </summary>
        public void Reset()
        {
			_current = 0;
        }

        #endregion

        /// <summary>
        /// Close the enumerator, and the underlying file.
        /// </summary>
        public void Close()
        {
        }
    }
}
