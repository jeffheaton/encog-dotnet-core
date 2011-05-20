//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
using Encog.Neural.Networks.Training;

namespace Encog.ML.Data.Folded
{
    /// <summary>
    /// A folded data set allows you to "fold" the data into several equal(or nearly
    /// equal) datasets. You then have the ability to select which fold the dataset
    /// will process. This is very useful for crossvalidation.
    /// 
    /// This dataset works off of an underlying dataset. By default there are no
    /// folds (fold size 1). Call the fold method to create more folds. 
    /// </summary>
    public class FoldedDataSet : MLDataSet
    {
        /// <summary>
        /// Error message: adds are not supported.
        /// </summary>
        public const String ADD_NOT_SUPPORTED = "Direct adds to the folded dataset are not supported.";

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        private readonly MLDataSet underlying;

        /// <summary>
        /// The fold that we are currently on.
        /// </summary>
        private int currentFold;

        /// <summary>
        /// The offset to the current fold.
        /// </summary>
        private int currentFoldOffset;

        /// <summary>
        /// The size of the current fold.
        /// </summary>
        private int currentFoldSize;

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
        /// The total number of folds. Or 0 if the data has not been folded yet.
        /// </summary>
        private int numFolds;

        /// <summary>
        /// Create a folded dataset. 
        /// </summary>
        /// <param name="underlying">The underlying folded dataset.</param>
        public FoldedDataSet(MLDataSet underlying)
        {
            this.underlying = underlying;
            Fold(1);
        }

        /// <summary>
        /// The owner object(from openAdditional)
        /// </summary>
        public FoldedDataSet Owner { get; set; }

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
                    return currentFold;
                }
            }
            set
            {
                if (Owner != null)
                {
                    throw new TrainingError("Can't set the fold on a non-top-level set.");
                }

                if (value >= numFolds)
                {
                    throw new TrainingError(
                        "Can't set the current fold to be greater than the number of folds.");
                }
                currentFold = value;
                currentFoldOffset = foldSize*currentFold;

                if (currentFold == (numFolds - 1))
                {
                    currentFoldSize = lastFoldSize;
                }
                else
                {
                    currentFoldSize = foldSize;
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
                    return currentFoldOffset;
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
                    return currentFoldSize;
                }
            }
        }

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int NumFolds
        {
            get { return numFolds; }
        }

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        public MLDataSet Underlying
        {
            get { return underlying; }
        }

        #region MLDataSet Members

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="data1">Not used.</param>
        public void Add(MLData data1)
        {
            throw new TrainingError(ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        /// <param name="idealData">Not used.</param>
        public void Add(MLData inputData, MLData idealData)
        {
            throw new TrainingError(ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="inputData">Not used.</param>
        public void Add(MLDataPair inputData)
        {
            throw new TrainingError(ADD_NOT_SUPPORTED);
        }

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            underlying.Close();
        }


        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return underlying.IdealSize; }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return underlying.InputSize; }
        }

        /// <summary>
        /// Get a record.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pair">The record.</param>
        public void GetRecord(long index, MLDataPair pair)
        {
            underlying.GetRecord(CurrentFoldOffset + index, pair);
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public long Count
        {
            get { return CurrentFoldSize; }
        }

        /// <summary>
        /// True if this is a supervised set.
        /// </summary>
        public bool Supervised
        {
            get { return underlying.Supervised; }
        }


        /// <summary>
        /// Open an additional dataset.
        /// </summary>
        /// <returns>The dataset.</returns>
        public MLDataSet OpenAdditional()
        {
            var folded = new FoldedDataSet(underlying.OpenAdditional());
            folded.Owner = this;
            return folded;
        }


        /// <summary>
        /// Get an enumberator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<MLDataPair> GetEnumerator()
        {
            return new FoldedEnumerator(this);
        }

        #endregion

        /// <summary>
        /// Fold the dataset. Must be done before the dataset is used. 
        /// </summary>
        /// <param name="numFolds">The number of folds.</param>
        public void Fold(int numFolds)
        {
            this.numFolds = (int) Math.Min(numFolds, underlying
                                                         .Count);
            foldSize = (int) (underlying.Count/this.numFolds);
            lastFoldSize = (int) (underlying.Count - (foldSize*this.numFolds));
            CurrentFold = 0;
        }
    }
}
