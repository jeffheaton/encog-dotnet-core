using Encog.MathUtil.Randomize.Generate;
using Encog.ML.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Training.Propagation.SGD
{
    /// <summary>
    /// The BatchDataSet wraps a larger dataset and breaks it up into a series of batches.  This dataset was specifically
    /// created to be used with the StochasticGradientDescent trainer; however, it should work with the others as well.
    /// It is important that the BatchDataSet's advance method be called at the end of each iteration, so that the next
    /// batch can be prepared.All Encog-provided trainers will detect the BatchDataSet and make this call.
    ///
    /// This dataset can be used in two ways, depending on the setting of the randomSamples property.  If this value is
    /// false (the default), then the first batch starts at the beginning of the dataset, and following batches will start
    /// at the end of the previous batch.  This method ensures that every data item is used If randomSamples is true, then
    /// each batch will be sampled from the underlying dataset (without replacement).
    /// </summary>
    public class BatchDataSet : IMLDataSet
    {
        /// <summary>
        /// The enumerator for the batch data set.
        /// </summary>
        [Serializable]
        public class BatchEnumerator : IEnumerator<IMLDataPair>
        {
            /// <summary>
            /// The current index.
            /// </summary>
            private int _current;

            /// <summary>
            /// The owner.
            /// </summary>
            private readonly BatchDataSet _owner;

            /// <summary>
            /// Construct an enumerator.
            /// </summary>
            /// <param name="owner">The owner of the enumerator.</param>
            public BatchEnumerator(BatchDataSet owner)
            {
                _current = -1;
                _owner = owner;
            }

            /// <summary>
            /// The current data item.
            /// </summary>
            public IMLDataPair Current
            {
                get { return _owner[_current]; }
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
                    if (_current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return _owner[_current];
                }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns>True if there is a next item.</returns>
            public bool MoveNext()
            {
                _current++;
                if (_current >= _owner.Count)
                    return false;
                return true;
            }

            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }
        }

        /// <summary>
        /// The source dataset.
        /// </summary>
        private IMLDataSet _dataset;

        /// <summary>
        /// The current location within the source dataset.
        /// </summary>
        private int _currentIndex;
        
        /// <summary>
        /// Random number generator.
        /// </summary>
        private IGenerateRandom _random;

        /// <summary>
        /// Should a random sample be taken for each batch.
        /// </summary>
        public bool RandomBatches { get; set; }

        /// <summary>
        /// Index entries for the current random sample.
        /// </summary>
        private int[] _randomSample;

        private int _batchSize;

        /// <summary>
        /// Construct the batch dataset. 
        /// </summary>
        /// <param name="theDataset">The source dataset.</param>
        /// <param name="theRandom">The random number generator.</param>
        public BatchDataSet(IMLDataSet theDataset, IGenerateRandom theRandom)
        {
            _dataset = theDataset;
            _random = theRandom;
            BatchSize = 500;
        }

        public int BatchSize
        {
            get
            {
                return _batchSize;
            }
            set
            {
                _batchSize = Math.Min(value, _dataset.Count);
                _randomSample = new int[_batchSize];
                if (RandomBatches)
                {
                    GeneraterandomSample();
                }
            }
        }

        /// <summary>
        /// Generate a random sample.
        /// </summary>
        private void GeneraterandomSample()
        {
            for (int i = 0; i < _batchSize; i++)
            {
                bool uniqueFound = true;
                int t;

                // Generate a unique index
                do
                {
                    t = _random.NextInt(0, _dataset.Count);

                    for (int j = 0; j < i; j++)
                    {
                        if (_randomSample[j] == t)
                        {
                            uniqueFound = false;
                            break;
                        }
                    }
                } while (!uniqueFound);

                // Record it
                _randomSample[i] = t;
            }

        }

        public IMLDataPair this[int index]
        {
            get
            {
                int resultIndex = (index + _currentIndex) % _dataset.Count;

                if (RandomBatches)
                {
                    resultIndex = _randomSample[resultIndex];
                }
                return _dataset[resultIndex];
            }
        }

        public int Count
        {
            get
            {
                return BatchSize;
            }
        }

        public int IdealSize
        {
            get
            {
                return _dataset.IdealSize;
            }
        }

        public int InputSize
        {
            get
            {
                return _dataset.InputSize;
            }
        }

        public bool Supervised
        {
            get
            {
                return _dataset.Supervised;
            }
        }

        public void Close()
        {
        }

        public IEnumerator<IMLDataPair> GetEnumerator()
        {
            return new BatchEnumerator(this);
        }

        /// <summary>
        /// This will open an additional batched dataset.  However, please note, the additional datasets will use a
        /// mersenne twister generator that is seeded by a long sampled from this object's random number
        /// generator.
        /// </summary>
        /// <returns>An additional data set.</returns>
        public IMLDataSet OpenAdditional()
        {
            BatchDataSet result = new BatchDataSet(_dataset, new MersenneTwisterGenerateRandom((uint)_random.NextLong()));
            result.BatchSize = BatchSize;
            return result;
        }

        /// <summary>
        /// Advance to the next batch.  Should be called at the end of each training iteration.
        /// </summary>
        public void Advance()
        {
            if (RandomBatches)
            {
                GeneraterandomSample();
            }
            else
            {
                _currentIndex = (_currentIndex + BatchSize) % _dataset.Count;
            }
        }
    }
}
