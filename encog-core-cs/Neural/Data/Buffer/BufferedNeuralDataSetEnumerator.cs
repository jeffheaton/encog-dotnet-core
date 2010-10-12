using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using System.IO;

namespace Encog.Neural.Data.Buffer
{
    /// <summary>
    /// An enumerator to move through the buffered data set.
    /// </summary>
    public class BufferedNeuralDataSetEnumerator : IEnumerator<INeuralDataPair>
    {

        /// <summary>
        /// The dataset being iterated over.
        /// </summary>
        private BufferedNeuralDataSet data;

        /// <summary>
        /// The current record.
        /// </summary>
        private int current;

        /// <summary>
        /// The current record.
        /// </summary>
        INeuralDataPair currentRecord;

        /// <summary>
        /// Construct the buffered enumerator. This is where the file is actually
        /// opened.
        /// </summary>
        /// <param name="owner">The object that created this enumeration.</param>
        public BufferedNeuralDataSetEnumerator(BufferedNeuralDataSet owner)
        {
            this.data = data;
            this.current = 0;
        }

        /// <summary>
        /// Close the enumerator, and the underlying file.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Get the current record
        /// </summary>
        public INeuralDataPair Current
        {
            get
            {
                return this.currentRecord;
            }
        }

        /// <summary>
        /// Dispose of the enumerator.
        /// </summary>
        public void Dispose()
        {
        }


        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (this.currentRecord == null)
                {
                    throw new NeuralDataError("Can't read current record until MoveNext is called once.");
                }
                return this.currentRecord;
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
                if (this.current >= data.Count )
                    return false;

                this.currentRecord = BasicNeuralDataPair.CreatePair(this.data
                        .InputSize, this.data.IdealSize);
                this.data.GetRecord(this.current++, this.currentRecord);
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
