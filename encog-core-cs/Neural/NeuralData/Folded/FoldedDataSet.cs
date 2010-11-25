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
using Encog.Engine.Data;
using Encog.Neural.Networks.Training;

namespace Encog.Neural.Data.Folded
{
    /// <summary>
    /// A folded data set allows you to "fold" the data into several equal(or nearly
    /// equal) datasets. You then have the ability to select which fold the dataset
    /// will process. This is very useful for crossvalidation.
    /// 
    /// This dataset works off of an underlying dataset. By default there are no
    /// folds (fold size 1). Call the fold method to create more folds. 
    /// </summary>
    public class FoldedDataSet : IIndexable
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String ADD_NOT_SUPPORTED = "Direct adds to the folded dataset are not supported.";

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        private IIndexable underlying;

        /// <summary>
        /// The fold that we are currently on.
        /// </summary>
        private int currentFold;

        /// <summary>
        /// The total number of folds. Or 0 if the data has not been folded yet.
        /// </summary>
        private int numFolds;

        /// <summary>
        /// The size of all folds, except the last fold, the last fold may have a
        /// different number.
        /// </summary>
        private int foldSize;

        /// <summary>
        /// The size of the last fold.
        /// </summary>
        private int lastFoldSize;

        /// <summary>
        /// The offset to the current fold.
        /// </summary>
        private int currentFoldOffset;

        /// <summary>
        /// The size of the current fold.
        /// </summary>
        private int currentFoldSize;

 		/// <summary>
        /// The owner object(from openAdditional)
 		/// </summary>
        public FoldedDataSet Owner { get; set; }

        /// <summary>
        /// Create a folded dataset. 
        /// </summary>
        /// <param name="underlying">The underlying folded dataset.</param>
        public FoldedDataSet(IIndexable underlying)
        {
            this.underlying = underlying;
            Fold(1);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(INeuralData data1)
        {
            throw new TrainingError(FoldedDataSet.ADD_NOT_SUPPORTED);

        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new TrainingError(FoldedDataSet.ADD_NOT_SUPPORTED);

        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(INeuralDataPair inputData)
        {
            throw new TrainingError(FoldedDataSet.ADD_NOT_SUPPORTED);

        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            this.underlying.Close();
        }

        /// <summary>
        /// Fold the dataset. Must be done before the dataset is used. 
        /// </summary>
        /// <param name="numFolds">The number of folds.</param>
        public void Fold(int numFolds)
        {
            this.numFolds = (int)Math.Min(numFolds, this.underlying
                    .Count);
            this.foldSize = (int)(this.underlying.Count / this.numFolds);
            this.lastFoldSize = (int)(this.underlying.Count - (this.foldSize * this.numFolds));
            CurrentFold = 0;
        }

        /// <summary>
        /// The current fold.
        /// </summary>
        public int CurrentFold
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFold;
                }
                else
                {
                    return this.currentFold;
                }
            }
            set
            {
                if( Owner!=null ) 
                {
 				    throw new TrainingError("Can't set the fold on a non-top-level set."); 
 			    }

                if (value >= this.numFolds)
                {
                    throw new TrainingError(
                            "Can't set the current fold to be greater than the number of folds.");
                }
                this.currentFold = value;
                this.currentFoldOffset = this.foldSize * this.currentFold;

                if (this.currentFold == (this.numFolds - 1))
                {
                    this.currentFoldSize = this.lastFoldSize;
                }
                else
                {
                    this.currentFoldSize = this.foldSize;
                }

            }
        }

        /// <summary>
        /// The current fold offset.
        /// </summary>
        public int CurrentFoldOffset
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldOffset;
                }
                else
                {
                    return this.currentFoldOffset;
                }
            }
        }

        /// <summary>
        /// The current fold size.
        /// </summary>
        public int CurrentFoldSize
        {
            get
            {
                if (Owner != null)
                {
                    return Owner.CurrentFoldSize;
                }
                else
                {
                    return this.currentFoldSize;
                }
            }
        }


        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get
            {
                return this.underlying.IdealSize;
            }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get
            {
                return this.underlying.InputSize;
            }
        }

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int NumFolds
        {
            get
            {
                return this.numFolds;
            }
        }

        /// <summary>
        /// Get a record.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pair">The record.</param>
        public void GetRecord(long index, IEngineData pair)
        {
            this.underlying.GetRecord(this.CurrentFoldOffset + index, pair);
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public long Count
        {
            get
            {
                return CurrentFoldSize;
            }
        }

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        public IIndexable Underlying
        {
            get
            {
                return this.underlying;
            }
        }

        /// <summary>
        /// True if this is a supervised set.
        /// </summary>
        public bool Supervised
        {
            get
            {
                return this.underlying.Supervised;
            }
        }


        /// <summary>
        /// Open an additional dataset.
        /// </summary>
        /// <returns>The dataset.</returns>
        public IEngineIndexableSet OpenAdditional()
        {
            FoldedDataSet folded = new FoldedDataSet(
                    (IIndexable)this.underlying.OpenAdditional());
            folded.Owner = this;
            return folded;
        }


        /// <summary>
        /// Get an enumberator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new FoldedEnumerator(this);
        }
    }
}
