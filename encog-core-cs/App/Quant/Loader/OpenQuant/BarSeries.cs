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
using System.Collections;
using System.Runtime.CompilerServices;

namespace Encog.App.Quant.Loader.OpenQuant
{
    // Fields
    [Serializable]
    public class DataArray : IEnumerable
    {
        protected double fDivisor;
        protected ArrayList fList;
        protected int fStopRecurant;

        // Methods
        [MethodImpl(MethodImplOptions.NoInlining)]
        public DataArray()
        {
            fList = new ArrayList();
        }

        /// <summary>
        ///     Gets the number of object in the array.
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get { return fList.Count; }
        }

        /// <summary>
        ///     Gets the <see cref="Encog.App.Quant.Loader.OpenQuant.Data.Data.IDataObject" /> at the specified index.
        /// </summary>
        public Data.Data.IDataObject this[int index]
        {
            [MethodImpl(MethodImplOptions.NoInlining)] get { return (fList[index] as Data.Data.IDataObject); }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IEnumerator GetEnumerator()
        {
            return fList.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Add(Data.Data.IDataObject obj)
        {
            fList.Add(obj);
        }

        /// <summary>
        ///     Clears this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Clear()
        {
            fList.Clear();
        }

        /// <summary>
        ///     Determines whether [contains] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///     <c>true</c> if [contains] [the specified obj]; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool Contains(Data.Data.IDataObject obj)
        {
            return fList.Contains(obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool Contains(Data.Data.Bar bar)
        {
            return fList.Contains(bar);
        }

        /// <summary>
        ///     Gets the index for a certain datetime.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int GetIndex(DateTime datetime)
        {
            if (Count != 0)
            {
                DateTime dateTime = this[0].DateTime;
                DateTime time2 = this[Count - 1].DateTime;
                if ((dateTime <= datetime) && (time2 >= datetime))
                {
                    return GetIndex(datetime, 0, Count - 1);
                }
            }
            return -1;
        }

        /// <summary>
        ///     Gets the index for a certain date time.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="index1">The index1.</param>
        /// <param name="index2">The index2.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int GetIndex(DateTime datetime, int index1, int index2)
        {
            int num4;
            long ticks = this[index1].DateTime.Ticks;
            long num2 = this[index2].DateTime.Ticks;
            long num3 = datetime.Ticks;
            if (num2 != ticks)
            {
                num4 = index1 + ((int) ((index2 - index1)*((num3 - ticks)/(num2 - ticks))));
            }
            else
            {
                num4 = (index1 + index2)/2;
            }
            Data.Data.IDataObject obj2 = this[num4];
            if (obj2.DateTime == datetime)
            {
                return num4;
            }
            if (((index2 - num4) < fStopRecurant) || ((num4 - index1) < fStopRecurant))
            {
                for (int i = index2; i >= index1; i--)
                {
                    obj2 = this[i];
                    if (obj2.DateTime < datetime)
                    {
                        return i;
                    }
                }
                return -1;
            }
            var num6 = (int) (((index2 - index1))/fDivisor);
            int num7 = Math.Max(index1, num4 - num6);
            obj2 = this[num7];
            if (obj2.DateTime > datetime)
            {
                return GetIndex(datetime, index1, num7);
            }
            int num8 = Math.Min(index2, num4 + num6);
            obj2 = this[num8];
            if (obj2.DateTime < datetime)
            {
                return GetIndex(datetime, num8, index2);
            }
            return GetIndex(datetime, num7, num8);
        }

        /// <summary>
        ///     Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="obj">The obj.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Insert(int index, Data.Data.IDataObject obj)
        {
            fList.Insert(index, obj);
        }

        /// <summary>
        ///     Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="bar">The bar.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Insert(int index, Data.Data.Bar bar)
        {
            fList.Insert(index, bar);
        }

        /// <summary>
        ///     Removes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Remove(Data.Data.IDataObject obj)
        {
            fList.Remove(obj);
        }


        /// <summary>
        ///     Removes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Remove(Data.Data.Bar bar)
        {
            fList.Remove(bar);
        }

        /// <summary>
        ///     Removes an object at the specified index in the array of objects.
        /// </summary>
        /// <param name="index">The index.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void RemoveAt(int index)
        {
            fList.RemoveAt(index);
        }

        // Properties

        /// <summary>
        ///     Adds the specified bar.
        /// </summary>
        /// <param name="bar">The bar.</param>
        public void Add(Data.Data.Bar bar)
        {
            Add(bar);
        }
    }
}
