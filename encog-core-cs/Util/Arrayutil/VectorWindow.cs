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
using System.Collections.Generic;

namespace Encog.Util.Arrayutil
{
    /// <summary>
    ///     Create a sliding window of double arrays. New vectors can be added to the
    ///     window. Once the required number of vectors have been added then the entire
    ///     window can be copied to an output vector large enough to hold it.
    /// </summary>
    public class VectorWindow
    {
        /// <summary>
        ///     The number of slices in a window.
        /// </summary>
        private readonly int _sliceCount;

        /// <summary>
        ///     The window.
        /// </summary>
        private readonly IList<double[]> _window = new List<double[]>();

        /// <summary>
        ///     Construct a sliding window.
        /// </summary>
        /// <param name="theSliceCount">The number of slices in a window.</param>
        public VectorWindow(int theSliceCount)
        {
            _sliceCount = theSliceCount;
        }

        /// <summary>
        ///     Add a single vector to the window.
        /// </summary>
        /// <param name="vec">The vector to add to the window.</param>
        public void Add(double[] vec)
        {
            _window.Add((double[]) vec.Clone());
            while (_window.Count > _sliceCount)
            {
                _window.RemoveAt(0);
            }
        }

        /// <summary>
        ///     True, if we've added enough slices for a complete window.
        /// </summary>
        /// <returns>True, if we've added enough slices for a complete window.</returns>
        public bool IsReady()
        {
            return _window.Count >= _sliceCount;
        }

        /// <summary>
        ///     Copy the entire window to a complete vector.
        /// </summary>
        /// <param name="output">The vector to copy to.</param>
        /// <param name="startPos">The starting position to write to.</param>
        public void CopyWindow(double[] output, int startPos)
        {
            if (!IsReady())
            {
                throw new EncogError("Can't produce a timeslice of size "
                                     + _sliceCount + ", there are only "
                                     + _window.Count + " vectors loaded.");
            }

            int currentIndex = startPos;
            for (int i = 0; i < _window.Count; i++)
            {
                double[] source = _window[i];
                EngineArray.ArrayCopy(source, 0, output, currentIndex,
                    source.Length);
                currentIndex += source.Length;
            }
        }
    }
}
