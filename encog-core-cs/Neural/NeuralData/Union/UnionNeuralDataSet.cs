// Encog(tm) Artificial Intelligence Framework v2.5
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
using Encog.Neural.NeuralData;
#if logging
using log4net;
#endif

namespace Encog.Neural.Data.Union
{
    /// <summary>
    /// A UnionNeuralDataSet is used to create a compound data set that is made
    /// up of other data sets.  The union set will iterate through all of the date
    /// of the subsets in the order that they were added to the union.  There
    /// are a number of uses for this sort of a dataset.  One is for processing
    /// extremely large SQL datasets.  You can break your query into multiple
    /// SQLNeuralDataSet objects and use the UnionNeuralDataSet to cause them
    /// to appear as one large dataset.  
    /// 
    /// The UnionNeuralDataSet can also be used to combine several different
    /// dataset types into one.
    /// 
    /// You must specify the ideal and input sizes.  All subsets must adhear
    /// to these sizes.
    /// </summary>
    public class UnionNeuralDataSet : INeuralDataSet
    {

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(UnionNeuralDataSet));
#endif

        /// <summary>
        /// The error to report when the user attempts an ADD.
        /// </summary>
        private const String ADD_ERROR = "Add is not supported in UnionNeuralDataSet";

        /// <summary>
        /// The error to report when the user attempts a remove.
        /// </summary>
        private const String REMOVE_ERROR = "Remove is not supported in UnionNeuralDataSet";

        /// <summary>
        /// The size of the input data.
        /// </summary>
        private int inputSize;

        /// <summary>
        /// The size of the ideal data.
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The subsets that this union is made up of.
        /// </summary>
        private IList<INeuralDataSet> subsets = new List<INeuralDataSet>();

        /// <summary>
        /// The enumerators that have been created so far for this data set.
        /// </summary>
        private ICollection<UnionEnumerator> enumerators = new List<UnionEnumerator>();

        /// <summary>
        /// The enumerator used to access the UnionNeuralDataSet.
        /// </summary>
        public class UnionEnumerator : IEnumerator<INeuralDataPair>
        {
            /// <summary>
            /// The next subset.
            /// </summary>
            private int currentSet;

            /// <summary>
            /// An enumerator to the current subset.
            /// </summary>
            private IEnumerator<INeuralDataPair> currentEnumerator;

            /// <summary>
            /// The owner of this enumerator.
            /// </summary>
            private UnionNeuralDataSet owner;

            /// <summary>
            /// Construct the union enumerator.  This sets the current set
            /// and current enumerator.
            /// </summary>
            /// <param name="owner"></param>
            public UnionEnumerator(UnionNeuralDataSet owner)
            {
                this.owner = owner;
                Reset();
            }

            /// <summary>
            /// Determine if there is more data to be read from this enumerator.
            /// </summary>
            /// <returns>True if there is more data to read.</returns>
            public bool MoveNext()
            {

                if (!this.currentEnumerator.MoveNext())
                {
                    if (this.currentSet < owner.Subsets.Count)
                    {
                        this.currentEnumerator = owner.Subsets[currentSet++].GetEnumerator();
                    }
                    return currentEnumerator.MoveNext();
                }
                else
                    return true;


            }


            /// <summary>
            /// Obtain the current piece of data.
            /// </summary>
            public INeuralDataPair Current
            {
                get
                {
                    return this.currentEnumerator.Current;
                }
            }

            /// <summary>
            /// The current item.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return this.currentEnumerator.Current;
                }
            }

           /// <summary>
            /// Not used.
           /// </summary>
            public void Dispose()
            {
            }


            /// <summary>
            /// Reset to the beginning.
            /// </summary>
            public void Reset()
            {
                this.currentSet = 1;
                this.currentEnumerator = owner.Subsets[0].GetEnumerator();
            }
        }
        
        /// <summary>
        /// Not supported.  Will throw an error.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(INeuralData data1)
        {
#if logging
            UnionNeuralDataSet.logger.Error(ADD_ERROR);
#endif
            throw new NeuralDataError(ADD_ERROR);
        }

        /// <summary>
        /// Not supported. Will throw an error.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(INeuralData inputData, INeuralData idealData)
        {
#if logging
            UnionNeuralDataSet.logger.Error(ADD_ERROR);
#endif
            throw new NeuralDataError(ADD_ERROR);
        }

        /// <summary>
        /// Not supported. Will throw an error.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(INeuralDataPair inputData)
        {
#if logging
            UnionNeuralDataSet.logger.Error(ADD_ERROR);
#endif
            throw new NeuralDataError(ADD_ERROR);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            this.enumerators.Clear();
        }

        /// <summary>
        /// The array size of the ideal data.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        /// <summary>
        /// The array size of the input data.
        /// </summary>
        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        /// <summary>
        /// Obtain an enumerator to access the collection of data.
        /// </summary>
        /// <returns>The new enumerator.</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            UnionEnumerator result = new UnionEnumerator(this);
            this.enumerators.Add(result);
            return result;
        }

        /// <summary>
        /// A collection of the subsets that make up this union.
        /// </summary>
        public IList<INeuralDataSet> Subsets
        {
            get
            {
                return this.subsets;
            }
        }

        /// <summary>
        /// Construct the union data set.  All subsets must have input and
        /// ideal sizes to match the parameters specified to this constructor.
        /// For unsupervised training specify 0 for idealSize.
        /// </summary>
        /// <param name="inputSize">The array size of the input data.</param>
        /// <param name="idealSize">The array size of the ideal data.</param>
        public UnionNeuralDataSet(int inputSize, int idealSize)
        {
            this.inputSize = inputSize;
            this.idealSize = idealSize;
        }

        /// <summary>
        /// Add a subset.  This method will validate that the input and
        /// ideal sizes are correct.
        /// </summary>
        /// <param name="set">The subset to add.</param>
        public void AddSubset(INeuralDataSet set)
        {
            if (set.InputSize != this.inputSize)
            {
                String str = "Subset input size of " + set.InputSize +
                " must match union input size of " + inputSize;
#if logging
                UnionNeuralDataSet.logger.Error(str);
#endif
                throw new NeuralDataError(str);
            }
            else if (set.IdealSize != this.idealSize)
            {
                String str = "Subset ideal size of " + set.IdealSize +
                " must match union ideal size of " + idealSize;
#if logging
                UnionNeuralDataSet.logger.Error(str);
#endif
                throw new NeuralDataError(str);
            }
            else
                this.subsets.Add(set);
        }

        /// <summary>
        /// Return true if this dataset is supervised.
        /// </summary>
        public bool Supervised
        {
            get
            {
                return this.idealSize > 0;
            }
        }
    }
}
