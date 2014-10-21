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
using Encog.MathUtil.Randomize;

namespace Encog.Util.Obj
{
    /// <summary>
    /// This class is used to choose between several objects with a specified probability.
    /// </summary>
    /// <typeparam name="T">The type of object to choose from.</typeparam>
    [Serializable]
    public class ChooseObject<T>
    {
        /// <summary>
        /// The objects that we are choosing from.
        /// </summary>
        private IList<ObjectHolder<T>> list = new List<ObjectHolder<T>>();

        /// <summary>
        /// The random choose.
        /// </summary>
        private RandomChoice chooser;

        /// <summary>
        /// Finalize the structure and set the probabilities.
        /// </summary>
        public void FinalizeStructure()
        {
            double[] d = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                d[i] = list[i].probability;
            }

            this.chooser = new RandomChoice(d);
        }

        /// <summary>
        /// Add an object. 
        /// </summary>
        /// <param name="probability">The probability to choose this object.</param>
        /// <param name="opp">The object to add.</param>
        public void Add(double probability, T opp)
        {
            ObjectHolder<T> holder = new ObjectHolder<T>();
            holder.obj = opp;
            holder.probability = probability;
            list.Add(holder);
        }

        /// <summary>
        /// The number of objects added.
        /// </summary>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        /// <summary>
        /// Choose a random object. 
        /// </summary>
        /// <param name="theGenerator">Random number generator.</param>
        /// <returns>The random choice.</returns>
        public T Pick(Random theGenerator)
        {
            int index = this.chooser.Generate(theGenerator);
            return this.list[index].obj;
        }


        /// <summary>
        /// The object to choose from.
        /// </summary>
        public IList<ObjectHolder<T>> Contents
        {
            get
            {
                return this.list;
            }
        }

        /// <summary>
        /// Clear all objects from the collection.
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Pick the first object in the list.
        /// </summary>
        /// <returns> The first object in the list.</returns>
        public T PickFirst()
        {
            return this.list[0].obj;
        }
    }
}
