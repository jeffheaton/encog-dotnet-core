using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Dynamic
{
	/// <summary>
	/// Used for the input parameters to the sliding window dataset.
	/// </summary>
	public class FuncMLDataProvider: IDynamicMLDataProvider
	{
		Func<int, int, double> _handler;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="handler">A method that takes as input the input/ideal sample and offset into that sample.</param>
		/// <param name="count">Total number of elements in this dataset.</param>
		/// <param name="size">The number of items per sample. eg. size of the input/output layer.</param>
		public FuncMLDataProvider(Func<int, int, double> handler, int count, int size)
		{
			if(handler == null) throw new ArgumentNullException("handler");
			if(count < 1) throw new ArgumentException("Value is too small.", "count");
			if(size < 1) throw new ArgumentException("Value is too small.", "size");
			
			_handler = handler;
			Count = count;
			Size = size;
		}

		public int Count
		{
			get;
			protected set;
		}

		public int Size
		{
			get;
			protected set;
		}

		public double this[int chunk, int index]
		{
			get { return _handler.Invoke(chunk, index); }
		}
	}
}
