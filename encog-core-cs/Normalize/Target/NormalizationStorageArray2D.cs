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

namespace Encog.Normalize.Target
{
    /// <summary>
    /// Output the normalized data to a 2D array.
    /// </summary>
    public class NormalizationStorageArray2D : INormalizationStorage
    {
        /// <summary>
        /// The array to output to.
        /// </summary>
        private double[][] array;

        /// <summary>
        /// The current data.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Construct an object to store to a 2D array.
        /// </summary>
        /// <param name="array">The array to store to.</param>
        public NormalizationStorageArray2D(double[][] array)
        {
            this.array = array;
            this.currentIndex = 0;
        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Close()
        {

        }

        /// <summary>
        /// Not needed for this storage type.
        /// </summary>
        public void Open()
        {

        }

        /// <summary>
        /// Write an array.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="inputCount">How much of the data is input.</param>
        public void Write(double[] data, int inputCount)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.array[this.currentIndex][i] = data[i];
            }
            this.currentIndex++;
        }

    }
}
