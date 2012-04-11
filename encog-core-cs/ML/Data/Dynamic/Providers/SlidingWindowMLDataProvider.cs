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
		public readonly int WindowSize, WindowOffset, StepSize;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="list">List of data to be used as input or ideal.</param>
		/// <param name="windowSize">Size of Input/Ideal.</param>
		/// <param name="windowOffset">Shift +/- for a given index.</param>
		/// <param name="stepSize">How much we move by for each discrete index.</param>
		public SlidingWindowMLDataProvider(IList<double> list, int windowSize, int windowOffset, int stepSize = 1)
		{
			if(list == null) throw new ArgumentNullException("list");
			if(list.Count < 2) throw new ArgumentException("List is too small.", "list");
			if(stepSize < 1) throw new ArgumentException("Value is too small.", "stepSize");
			if(windowSize < 1) throw new ArgumentException("Value is too small.", "windowSize");

			List = list;
			WindowSize = windowSize;
			WindowOffset = windowOffset;
			StepSize = stepSize;
		}

		public int Count
		{
			get { return List.Count / StepSize; }
		}

		public int Size
		{
			get { return WindowSize; }
		}

		/// <summary>
		/// If true, the list boundaries are extended when the window takes us beyond the boundary.
		/// </summary>
		public bool PadWithNearest { get; set; }

		/// <summary>
		/// Defaults to 0.0
		/// </summary>
		public double DefaultPadValue { get; set; }

		public double this[int chunk, int index]
		{
			get 
			{
				var offset = chunk * StepSize + index + WindowOffset;
				if(offset < 0) return PadWithNearest ? List.First() : DefaultPadValue;
				if(offset >= List.Count) return PadWithNearest ? List.Last() : DefaultPadValue;
				return List[offset];
			}
		}
	}
}
