using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Data.Basic
{
    /// <summary>
    /// The enumerator for the basic neural data set.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BasicNeuralEnumerator : IEnumerator<INeuralDataPair>
    {
        /// <summary>
        /// The current index.
        /// </summary>
        private int current;

        /// <summary>
        /// The owner.
        /// </summary>
        private BasicNeuralDataSet owner;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner of the enumerator.</param>
        public BasicNeuralEnumerator(BasicNeuralDataSet owner)
        {
            this.current = -1;
            this.owner = owner;
        }

        /// <summary>
        /// The current data item.
        /// </summary>
        public INeuralDataPair Current
        {
            get
            {
                return owner.Data[this.current];
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
        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (this.current < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return this.owner.Data[this.current];
            }
        }

        /// <summary>
        /// Move to the next item.
        /// </summary>
        /// <returns>True if there is a next item.</returns>
        public bool MoveNext()
        {
            this.current++;
            if (current >= this.owner.Data.Count)
                return false;
            return true;
        }

        /// <summary>
        /// Reset to the beginning.
        /// </summary>
        public void Reset()
        {
            this.current = -1;
        }
    }

}
