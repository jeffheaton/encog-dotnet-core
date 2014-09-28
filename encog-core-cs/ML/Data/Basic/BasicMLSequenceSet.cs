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
using System.Linq;
using Encog.Util;

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// A basic implementation of the MLSequenceSet.
    /// </summary>
    [Serializable]
    public class BasicMLSequenceSet : IMLSequenceSet
    {
        /// <summary>
        /// The data held by this object.
        /// </summary>
        private readonly IList<IMLDataSet> _sequences = new List<IMLDataSet>();

        private IMLDataSetAddable _currentSequence;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicMLSequenceSet()
        {
            _currentSequence = new BasicMLDataSet();
            _sequences.Add(_currentSequence);
        }

        public BasicMLSequenceSet(BasicMLSequenceSet other)
        {
            _sequences = other._sequences;
            _currentSequence = other._currentSequence;
        }

        /// <summary>
        /// Construct a data set from an input and ideal array.
        /// </summary>
        /// <param name="input">The input into the machine learning method for training.</param>
        /// <param name="ideal">The ideal output for training.</param>
        public BasicMLSequenceSet(double[][] input, double[][] ideal)
        {
            _currentSequence = new BasicMLDataSet(input, ideal);
            _sequences.Add(_currentSequence);
        }

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="theData">The data to use.</param>
        public BasicMLSequenceSet(IList<IMLDataPair> theData)
        {
            _currentSequence = new BasicMLDataSet(theData);
            _sequences.Add(_currentSequence);
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset. 
        /// </summary>
        /// <param name="set">The dataset to copy.</param>
        public BasicMLSequenceSet(IMLDataSet set)
        {
            _currentSequence = new BasicMLDataSet();
            _sequences.Add(_currentSequence);

            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;

            foreach (IMLDataPair pair in set)
            {
                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
					pair.Input.CopyTo(input.Data, 0, pair.Input.Count);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
					pair.Ideal.CopyTo(ideal.Data, 0, pair.Ideal.Count);
                }

                _currentSequence.Add(new BasicMLDataPair(input, ideal));
            }
        }

        #region IMLSequenceSet Members

        /// <inheritdoc/>
        public void Add(IMLData theData)
        {
            _currentSequence.Add(theData);
        }

        /// <inheritdoc/>
        public void Add(IMLData inputData, IMLData idealData)
        {
            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            _currentSequence.Add(pair);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair inputData)
        {
            _currentSequence.Add(inputData);
        }

        /// <inheritdoc/>
        public void Close()
        {
            // nothing to close
        }


        /// <inheritdoc/>
        public int IdealSize
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return 0;
                }
                return _sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public int InputSize
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return 0;
                }
                return _sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _sequences.Sum(ds => ds.Count);
            }
        }

        /// <inheritdoc/>
        public bool Supervised
        {
            get
            {
                if (_sequences[0].Count == 0)
                {
                    return false;
                }
                return _sequences[0].Supervised;
            }
        }

        /// <inheritdoc/>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLSequenceSet(this);
        }

        public void StartNewSequence()
        {
            if (_currentSequence.Count > 0)
            {
                _currentSequence = new BasicMLDataSet();
                _sequences.Add(_currentSequence);
            }
        }

        /// <inheritdoc/>
        public int SequenceCount
        {
            get { return _sequences.Count; }
        }

        /// <inheritdoc/>
        public IMLDataSet GetSequence(int i)
        {
            return this._sequences[i];
        }

        /// <inheritdoc/>
        public ICollection<IMLDataSet> Sequences
        {
            get { return this._sequences; }
        }

        /// <inheritdoc/>
        public void Add(IMLDataSet sequence)
        {
            foreach (IMLDataPair pair in sequence)
            {
                this.Add(pair);
            }
        }

        /// <summary>
        /// Get an enumerator to access the data with.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BasicMLSequenceSetEnumerator(this);
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
                int sequenceIndex = 0;
                int recordIndex = x;
                while(_sequences[sequenceIndex].Count <= recordIndex)
                {
                    recordIndex -= _sequences[sequenceIndex].Count;
                    sequenceIndex++;
                    if(sequenceIndex > _sequences.Count)
                    {
                        throw new MLDataError("Record out of range: " + x);
                    }
                }
                return _sequences[sequenceIndex][recordIndex];
            }
        }

        #endregion

        /// <inheritdoc/>
        public Object Clone()
        {
            return ObjectCloner.DeepCopy(this);
        }

        #region Nested type: BasicMLSequenceSetEnumerator

        /// <summary>
        /// Enumerate.
        /// </summary>
        public class BasicMLSequenceSetEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLSequenceSet _owner;

            /// <summary>
            /// The index that the iterator is currently at.
            /// </summary>
            private int _currentIndex;

            /// <summary>
            /// The sequence index.
            /// </summary>
            private int _currentSequenceIndex;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicMLSequenceSetEnumerator(BasicMLSequenceSet owner)
            {
                Reset();
                _owner = owner;
            }

            #region IEnumerator<IMLDataPair> Members

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get
                {
                    if (_currentSequenceIndex >= _owner.SequenceCount)
                    {
                        throw new InvalidOperationException("Trying to read past the end of the dataset.");
                    }

                    if (_currentIndex < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner.GetSequence(_currentSequenceIndex)[_currentIndex];
                }
            }

            /// <summary>
            /// Dispose of this object.
            /// </summary>
            public void Dispose()
            {
                // nothing needed
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (_currentSequenceIndex >= _owner.SequenceCount)
                    {
                        throw new InvalidOperationException("Trying to read past the end of the dataset.");
                    }

                    if (_currentIndex < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner.GetSequence(_currentSequenceIndex)[_currentIndex];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                if (_currentSequenceIndex >= _owner.SequenceCount)
                {
                    return false;
                }

                IMLDataSet current = _owner.GetSequence(_currentSequenceIndex);
                _currentIndex++;

                if (_currentIndex >= current.Count)
                {
                    _currentIndex = 0;
                    _currentSequenceIndex++;
                }

                if (_currentSequenceIndex >= _owner.SequenceCount)
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _currentIndex = -1;
                _currentSequenceIndex = 0;
            }

            #endregion
        }

        #endregion
    }
}
