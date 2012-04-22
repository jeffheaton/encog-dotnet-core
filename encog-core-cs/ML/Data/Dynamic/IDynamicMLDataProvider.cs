using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Dynamic
{
	/// <summary>
	/// Used for the input parameters to the dynamic dataset.
	/// </summary>
	public interface IDynamicMLDataProvider
	{
		/// <summary>
		/// Total number of data chunks available.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// The size of an individual chunk.
		/// </summary>
		int Size { get; }

		double this[int chunk, int index] { get; }
	}
}
