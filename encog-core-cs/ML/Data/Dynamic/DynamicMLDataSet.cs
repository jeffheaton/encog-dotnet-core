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
	/// For support of sliding windows or other custom input and ideal collection providers.
	/// </summary>
	public class DynamicMLDataSet : IMLDataSet
	{
        /// <summary>
        ///  Initializes a new instance of the DynamicMLDataSet class.
        /// </summary>
        /// <param name="input"> The input. </param>
        /// <param name="ideal"> The ideal. </param>
		public DynamicMLDataSet(IDynamicMLDataProvider input, IDynamicMLDataProvider ideal)
		{
			InputArgs = input;
			IdealArgs = ideal;
			Count = Math.Min(input.Count, ideal.Count);
		}

        /// <summary>
        ///  The ideal arguments.
        /// </summary>
		public readonly IDynamicMLDataProvider InputArgs, IdealArgs;

        /// <summary>
        ///  The size of the ideal data (>0 supervised), 0 if no ideal data (unsupervised)
        /// </summary>
        /// <value>
        ///  The size of the ideal.
        /// </value>
		public int IdealSize { get { return IdealArgs.Size; } }

        /// <summary>
        ///  The size of the input data.
        /// </summary>
        /// <value>
        ///  The size of the input.
        /// </value>
		public int InputSize { get { return InputArgs.Size; } }

        /// <summary>
        ///  The number of records in the data set.
        /// </summary>
        /// <value>
        ///  The count.
        /// </value>
		public int Count { get; private set; }

        /// <summary>
        ///  Return true if supervised.
        /// </summary>
        /// <value>
        ///  true if supervised, false if not.
        /// </value>
		public bool Supervised
		{
			get { return true; }
		}

        /// <summary>
        ///  Close this datasource and release any resources obtained by it, including any iterators
        ///  created.
        /// </summary>
		public void Close()
		{
			// nothing to close
		}

        /// <summary>
        ///  Get an enumerator to access the data.
        /// </summary>
        /// <returns>
        ///  The enumerator.
        /// </returns>
		public IEnumerator<IMLDataPair> GetEnumerator()
		{
			return new DynamicMLDataSetEnumerator(this);
		}

        /// <summary>
        ///  Dynamic ML data set enumerator.
        /// </summary>
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
        ///  eg. Clone.
        /// </summary>
        /// <returns>
        ///  .
        /// </returns>
		public IMLDataSet OpenAdditional()
		{
			return new DynamicMLDataSet(InputArgs, IdealArgs);
		}

        /// <summary>
        ///  Get the specified record.
        /// </summary>
        /// <value>
        ///  The indexed item.
        /// </value>
        ///
        /// ### <param name="x"> The index to access. </param>
        /// ### <returns>
        ///  .
        /// </returns>
		public IMLDataPair this[int x]
		{
			get 
            {
				return new DynamicMLDataPair(this, x);
			}
		}

        /// <summary>
        ///  Dynamic ML data pair.
        /// </summary>
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

            /// <summary>
            ///  Makes a deep copy of this object.
            /// </summary>
            /// <returns>
            ///  A copy of this object.
            /// </returns>
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
					get 
                    {
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
