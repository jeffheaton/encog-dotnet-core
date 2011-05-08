using System;
using System.Collections.Generic;
using System.Linq;
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

using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Data.Folded
{
    /// <summary>
    /// The enumerator for a folded dataset.
    /// </summary>
    public class FoldedEnumerator : IEnumerator<MLDataPair>
    {
        /// <summary>
        /// The current index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current data item.
        /// </summary>
        private MLDataPair currentPair;

        /// <summary>
        /// The owner.
        /// </summary>
        private FoldedDataSet owner;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FoldedEnumerator(FoldedDataSet owner)
        {
            this.owner = owner;
            this.currentIndex = -1;
        }

        /// <summary>
        /// The current object.
        /// </summary>
        public MLDataPair Current
        {
            get
            {
                if (this.currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return this.currentPair;
            }

        }

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
                MLDataPair pair = BasicMLDataPair.CreatePair(
                        this.owner.InputSize, this.owner.IdealSize);
                this.owner.GetRecord(this.currentIndex++, pair);
                this.currentPair = pair;
                return true;
            }
            else
            {
                this.currentPair = null;
                return false;
            }
        }

        /// <summary>
        /// Determine if there is a next record.
        /// </summary>
        /// <returns>True, if there is a next record.</returns>
        public bool HasNext()
        {
            return this.currentIndex < this.owner.CurrentFoldSize;
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
        object System.Collections.IEnumerator.Current
        {
            get
            {
                if (this.currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return this.currentPair;
            }

        }
    }
}
