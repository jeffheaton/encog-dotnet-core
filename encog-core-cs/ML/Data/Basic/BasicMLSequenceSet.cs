using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;
using System.Collections;

namespace Encog.ML.Data.Basic
{
    /// <summary>
    /// A basic implementation of the MLSequenceSet.
    /// </summary>
    [Serializable]
    public class BasicMLSequenceSet : IMLSequenceSet
    {
        public class BasicMLSequenceSetEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The index that the iterator is currently at.
            /// </summary>
            private int _currentIndex = 0;

            /// <summary>
            /// The sequence index.
            /// </summary>
            private int _currentSequenceIndex = 0;

            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BasicMLSequenceSet _owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BasicMLSequenceSetEnumerator(BasicMLSequenceSet owner)
            {
                Reset();
                _owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get
                {
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
                IMLDataSet current = _owner.GetSequence(_currentSequenceIndex);

                if (_currentSequenceIndex >= _owner.SequenceCount && _currentIndex >= _owner.Count)
                {
                    return false;
                }

                _currentIndex++;
                if (_currentIndex >= current.Count)
                {
                    _currentIndex = 0;
                    _currentSequenceIndex++;
                }

                if (_currentSequenceIndex >= _owner.SequenceCount && _currentIndex >= _owner.Count)
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
        }

        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<IMLDataSet> sequences = new List<IMLDataSet>();

        private IMLDataSet currentSequence;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicMLSequenceSet()
        {
            this.currentSequence = new BasicMLDataSet();
            sequences.Add(this.currentSequence);
        }

        public BasicMLSequenceSet(BasicMLSequenceSet other)
        {
            this.sequences = other.sequences;
            this.currentSequence = other.currentSequence;
        }

        /// <summary>
        /// Construct a data set from an input and ideal array.
        /// </summary>
        /// <param name="input">The input into the machine learning method for training.</param>
        /// <param name="ideal">The ideal output for training.</param>
        public BasicMLSequenceSet(double[][] input, double[][] ideal)
        {
            this.currentSequence = new BasicMLDataSet(input, ideal);
            this.sequences.Add(this.currentSequence);
        }

        /// <summary>
        /// Construct a data set from an already created list. Mostly used to
        /// duplicate this class.
        /// </summary>
        /// <param name="theData">The data to use.</param>
        public BasicMLSequenceSet(IList<IMLDataPair> theData)
        {
            this.currentSequence = new BasicMLDataSet(theData);
            this.sequences.Add(this.currentSequence);
        }

        /// <summary>
        /// Copy whatever dataset type is specified into a memory dataset. 
        /// </summary>
        /// <param name="set">The dataset to copy.</param>
        public BasicMLSequenceSet(IMLDataSet set)
        {
            this.currentSequence = new BasicMLDataSet();
            this.sequences.Add(this.currentSequence);

            int inputCount = set.InputSize;
            int idealCount = set.IdealSize;

            foreach (IMLDataPair pair in set)
            {

                BasicMLData input = null;
                BasicMLData ideal = null;

                if (inputCount > 0)
                {
                    input = new BasicMLData(inputCount);
                    EngineArray.ArrayCopy(pair.InputArray, input.Data);
                }

                if (idealCount > 0)
                {
                    ideal = new BasicMLData(idealCount);
                    EngineArray.ArrayCopy(pair.IdealArray, ideal.Data);
                }

                this.currentSequence.Add(new BasicMLDataPair(input, ideal));
            }
        }

        /// <inheritdoc/>
        public void Add(IMLData theData)
        {
            this.currentSequence.Add(theData);
        }

        /// <inheritdoc/>
        public void Add(IMLData inputData, IMLData idealData)
        {

            IMLDataPair pair = new BasicMLDataPair(inputData, idealData);
            this.currentSequence.Add(pair);
        }

        /// <inheritdoc/>
        public void Add(IMLDataPair inputData)
        {
            this.currentSequence.Add(inputData);
        }

        /// <inheritdoc/>
        public Object Clone()
        {
            return ObjectCloner.DeepCopy(this);
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
                if (this.sequences[0].Count == 0)
                {
                    return 0;
                }
                return this.sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public int InputSize
        {
            get
            {
                if (this.sequences[0].Count == 0)
                {
                    return 0;
                }
                return this.sequences[0].IdealSize;
            }
        }

        /// <inheritdoc/>
        public void GetRecord(long index, IMLDataPair pair)
        {
            long recordIndex = index;
            int sequenceIndex = 0;

            while (this.sequences[sequenceIndex].Count < recordIndex)
            {
                recordIndex -= this.sequences[sequenceIndex].Count;
                sequenceIndex++;
                if (sequenceIndex > this.sequences.Count)
                {
                    throw new MLDataError("Record out of range: " + index);
                }
            }

            this.sequences[sequenceIndex].GetRecord(recordIndex, pair);
        }

        /// <inheritdoc/>
        public long Count
        {
            get
            {
                long result = 0;
                foreach (IMLDataSet ds in this.sequences)
                {
                    result += ds.Count;
                }
                return result;
            }
        }

        /// <inheritdoc/>
        public bool Supervised
        {
            get
            {
                if (this.sequences[0].Count == 0)
                {
                    return false;
                }
                return this.sequences[0].Supervised;
            }
        }

        /// <inheritdoc/>
        public IMLDataSet OpenAdditional()
        {
            return new BasicMLSequenceSet(this);
        }

        public void StartNewSequence()
        {
            if (this.currentSequence.Count > 0)
            {
                this.currentSequence = new BasicMLDataSet();
                this.sequences.Add(this.currentSequence);
            }
        }

        /// <inheritdoc/>
        public int SequenceCount
        {
            get
            {
                return this.sequences.Count;
            }
        }

        /// <inheritdoc/>
        public IMLDataSet GetSequence(int i)
        {
            return this.sequences[i];
        }

        /// <inheritdoc/>
        public ICollection<IMLDataSet> Sequences
        {
            get
            {
                return this.sequences;
            }
        }

        /// <inheritdoc/>
        public void Add(IMLDataSet sequence)
        {
            foreach (IMLDataPair pair in sequence)
            {
                Add(pair);
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
                IMLDataPair result = BasicMLDataPair.CreatePair(InputSize, IdealSize);
                this.GetRecord(x, result);
                return result;
            }
        }
    }
}
