using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

namespace Encog.Normalize.Input
{
    /// <summary>
    /// Simple holder class used internally for Encog.
    /// Used as a holder for a:
    /// 
    ///  NeuralDataPair
    ///  Enumeration
    ///  InputFieldNeuralDataSet
    /// </summary>
    public class NeuralDataFieldHolder
    {
        /// <summary>
        /// A neural data pair.
        /// </summary>
        private INeuralDataPair pair;

        /// <summary>
        /// An iterator.
        /// </summary>
        private IEnumerator<INeuralDataPair> iterator;

        /// <summary>
        /// A field.
        /// </summary>
        private InputFieldNeuralDataSet field;

        /// <summary>
        /// Construct the class.
        /// </summary>
        /// <param name="iterator">An iterator.</param>
        /// <param name="field">A field.</param>
        public NeuralDataFieldHolder(IEnumerator<INeuralDataPair> iterator,
                 InputFieldNeuralDataSet field)
        {
            this.iterator = iterator;
            this.field = field;
        }

        /// <summary>
        /// The field.
        /// </summary>
        public InputFieldNeuralDataSet Field
        {
            get
            {
                return this.field;
            }
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return this.iterator;
        }

        /// <summary>
        /// The pair.
        /// </summary>
        public INeuralDataPair Pair
        {
            get
            {
                return this.pair;
            }
            set
            {
                this.pair = value;
            }
        }

        /// <summary>
        /// Obtain the next pair.
        /// </summary>
        public void ObtainPair()
        {
            this.iterator.MoveNext();
            this.pair = this.iterator.Current;
        }
    }
}
