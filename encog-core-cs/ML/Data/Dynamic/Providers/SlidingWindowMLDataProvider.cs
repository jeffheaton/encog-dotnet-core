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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Dynamic
{
	/// <summary>
	/// Used for the input parameters to the sliding window dataset.
	/// </summary>
	public class SlidingWindowMLDataProvider: IDynamicMLDataProvider
	{
		public readonly IList<double> List;
		public readonly int WindowSize, WindowOffset, StepSize, Gap;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="list">List of data to be used as input or ideal.</param>
		/// <param name="windowSize">Size of Input/Ideal.</param>
		/// <param name="windowOffset">Shift +/- for a given index.</param>
		/// <param name="stepSize">How much we move by for each discrete/outer index.</param>
		/// <param name="gap">How much we move by for each inner index.</param>
		public SlidingWindowMLDataProvider(IList<double> list, int windowSize, int windowOffset, int stepSize = 1, int gap = 1)
		{
			if(list == null) throw new ArgumentNullException("list");
			if(list.Count < 2) throw new ArgumentException("List is too small.", "list");
			if(stepSize < 1) throw new ArgumentException("Value is too small.", "stepSize");
			if(windowSize < 1) throw new ArgumentException("Value is too small.", "windowSize");
			if(gap < 1) throw new ArgumentException("Value is too small.", "gap");

			List = list;
			WindowSize = windowSize;
			WindowOffset = windowOffset;
			StepSize = stepSize;
			Gap = gap;
		}

		/// <summary>
		/// Number of chunks/windows/samplesets in this data.
		/// </summary>
		public virtual int Count
		{
			get { return List.Count / StepSize; }
		}

		/// <summary>
		/// Size of any one chunk/window/sampleset.
		/// </summary>
		public virtual int Size
		{
			get { return WindowSize / Gap; }
		}

		/// <summary>
		/// If true, the list boundaries are extended when the window takes us beyond the boundary.
		/// </summary>
		public bool PadWithNearest { get; set; }

		/// <summary>
		/// Defaults to 0.0
		/// </summary>
		public double DefaultPadValue { get; set; }

		/// <summary>
		/// Return data from the sliding window.
		/// </summary>
		/// <param name="chunk">The window we are on</param>
		/// <param name="index">An index into the window</param>
		public virtual double this[int chunk, int index]
		{
			get 
			{
				var offset = chunk * StepSize + index * Gap + WindowOffset;
				if(offset < 0) return PadWithNearest ? List.First() : DefaultPadValue;
				if(offset >= List.Count) return PadWithNearest ? List.Last() : DefaultPadValue;
				return List[offset];
			}
		}
	}
}
