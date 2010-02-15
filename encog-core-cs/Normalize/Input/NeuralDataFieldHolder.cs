// Encog(tm) Artificial Intelligence Framework v2.3
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
