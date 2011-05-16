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
    public class FoldedEnumerator : IEnumerator<MLDataPair>
    {
        /// <summary>
        /// The owner.
        /// </summary>
        private readonly FoldedDataSet owner;

        /// <summary>
        /// The current index.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current data item.
        /// </summary>
        private MLDataPair currentPair;

        /// <summary>
        /// Construct an enumerator.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public FoldedEnumerator(FoldedDataSet owner)
        {
            this.owner = owner;
            currentIndex = -1;
        }

        #region IEnumerator<MLDataPair> Members

        /// <summary>
        /// The current object.
        /// </summary>
        public MLDataPair Current
        {
            get
            {
                if (currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return currentPair;
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
                    owner.InputSize, owner.IdealSize);
                owner.GetRecord(currentIndex++, pair);
                currentPair = pair;
                return true;
            }
            else
            {
                currentPair = null;
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

        /// <summary>
        /// The current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (currentIndex < 0)
                {
                    throw new InvalidOperationException("Must call MoveNext before reading Current.");
                }
                return currentPair;
            }
        }

        #endregion

        /// <summary>
        /// Determine if there is a next record.
        /// </summary>
        /// <returns>True, if there is a next record.</returns>
        public bool HasNext()
        {
            return currentIndex < owner.CurrentFoldSize;
        }
    }
}