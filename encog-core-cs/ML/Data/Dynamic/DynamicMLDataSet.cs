using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Dynamic
{
	/// <summary>
	/// For support of sliding windows or other custom input and ideal collection providers.
	/// </summary>
	public class DynamicMLDataSet : IMLDataSet
	{
		public DynamicMLDataSet(IDynamicMLDataProvider input, IDynamicMLDataProvider ideal)
		{
			InputArgs = input;
			IdealArgs = ideal;
			Count = Math.Min(input.Count, ideal.Count);
		}

		public readonly IDynamicMLDataProvider InputArgs, IdealArgs;

		/// <summary>
        /// The size of the ideal data, 0 if no ideal data.
        /// </summary>
		public int IdealSize { get { return IdealArgs.Size; } }

        /// <summary>
        /// The size of the input data.
        /// </summary>
		public int InputSize { get { return InputArgs.Size; } }

        /// <summary>
        /// The number of records in the data set.
        /// </summary>
		public int Count { get; private set; }

		public bool Supervised
		{
			get { return true; }
		}

		public void Close()
		{
			// nothing to close
		}

		public IEnumerator<IMLDataPair> GetEnumerator()
		{
			return new DynamicMLDataSetEnumerator(this);
		}

		private class DynamicMLDataSetEnumerator: IEnumerator<IMLDataPair>
		{
			private int _position = -1;
			private readonly DynamicMLDataSet _ds;
			public DynamicMLDataSetEnumerator(DynamicMLDataSet ds) { _ds = ds; }

			public IMLDataPair Current
			{
				get { return _position < 0 || _position >= _ds.Count ? null : _ds[_position]; }
			}

			public void Dispose()
			{
			}

			object System.Collections.IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				return ++_position < _ds.Count; 
			}

			public void Reset()
			{
				_position = -1;
			}
		}

		/// <summary>
		/// eg. Clone
		/// </summary>
		public IMLDataSet OpenAdditional()
		{
			return new DynamicMLDataSet(InputArgs, IdealArgs);
		}

		public IMLDataPair this[int x]
		{
			get {
				return new DynamicMLDataPair(this, x);
			}
		}

		private class DynamicMLDataPair: IMLDataPair
		{
			private readonly DynamicMLDataSet _ds;
			private readonly int _index;
			public DynamicMLDataPair(DynamicMLDataSet ds, int index)
			{
				_ds = ds;
				_index = index;
				Significance = 1.0;
				Input = new DynamicWindowMLData(_ds.InputArgs, index);
				Ideal = new DynamicWindowMLData(_ds.IdealArgs, index);
			}

			public IMLData Input { get; private set; }

			public IMLData Ideal { get; private set; }

			public bool Supervised
			{
				get { return true; }
			}

			public double Significance
			{
				get; set;
			}

			public object Clone()
			{
				return new DynamicMLDataPair(_ds, _index);
			}

			public Util.KMeans.ICentroid<IMLDataPair> CreateCentroid()
			{
				throw new NotImplementedException();
			}

			private class DynamicWindowMLData: IMLData
			{
				private readonly int _index;
				private readonly IDynamicMLDataProvider _provider;
				public DynamicWindowMLData(IDynamicMLDataProvider provider, int index)
				{
					_provider = provider;
					_index = index;
				}

				public double this[int x]
				{
					get {
						return _provider[_index, x];
					}
				}

				public int Count
				{
					get { return _provider.Size; }
				}

				public void CopyTo(double[] target, int targetIndex, int count)
				{
					for(int i = 0; i < count; i++)
						target[i + targetIndex] = this[i];
				}

				public object Clone()
				{
					return new DynamicWindowMLData(_provider, _index);
				}

				public Util.KMeans.ICentroid<IMLData> CreateCentroid()
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}
