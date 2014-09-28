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
using Encog.Neural.Networks.Training;
using Encog.ML.Data.Basic;

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
    public class FoldedDataSet : IMLDataSet
    {
        /// <summary>
        /// The underlying dataset.
        /// </summary>
        private readonly IMLDataSet _underlying;

        /// <summary>
        /// The fold that we are currently on.
        /// </summary>
        private int _currentFold;

        /// <summary>
        /// The offset to the current fold.
        /// </summary>
        private int _currentFoldOffset;

        /// <summary>
        /// The size of the current fold.
        /// </summary>
        private int _currentFoldSize;

        /// <summary>
        /// The size of all folds, except the last fold, the last fold may have a
        /// different number.
        /// </summary>
        private int _foldSize;

        /// <summary>
        /// The size of the last fold.
        /// </summary>
        private int _lastFoldSize;

        /// <summary>
        /// The total number of folds. Or 0 if the data has not been folded yet.
        /// </summary>
        private int _numFolds;

        /// <summary>
        /// Create a folded dataset. 
        /// </summary>
        /// <param name="underlying">The underlying folded dataset.</param>
        public FoldedDataSet(IMLDataSet underlying)
        {
            _underlying = underlying;
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
                return _currentFold;
            }
            set
            {
                if (Owner != null)
                {
                    throw new TrainingError("Can't set the fold on a non-top-level set.");
                }

                if (value >= _numFolds)
                {
                    throw new TrainingError(
                        "Can't set the current fold to be greater than the number of folds.");
                }
                _currentFold = value;
                _currentFoldOffset = _foldSize*_currentFold;

                _currentFoldSize = _currentFold == (_numFolds - 1) ? _lastFoldSize : _foldSize;
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
                return _currentFoldOffset;
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
                return _currentFoldSize;
            }
        }

        /// <summary>
        /// The number of folds.
        /// </summary>
        public int NumFolds
        {
            get { return _numFolds; }
        }

        /// <summary>
        /// The underlying dataset.
        /// </summary>
        public IMLDataSet Underlying
        {
            get { return _underlying; }
        }

        #region MLDataSet Members

        /// <summary>
        /// Close the dataset.
        /// </summary>
        public void Close()
        {
            _underlying.Close();
        }

        /// <summary>
        /// The ideal size.
        /// </summary>
        public int IdealSize
        {
            get { return _underlying.IdealSize; }
        }

        /// <summary>
        /// The input size.
        /// </summary>
        public int InputSize
        {
            get { return _underlying.InputSize; }
        }

        /// <summary>
        /// The record count.
        /// </summary>
        public int Count
        {
            get { return CurrentFoldSize; }
        }

        /// <summary>
        /// True if this is a supervised set.
        /// </summary>
        public bool Supervised
        {
            get { return _underlying.Supervised; }
        }


        /// <summary>
        /// Open an additional dataset.
        /// </summary>
        /// <returns>The dataset.</returns>
        public IMLDataSet OpenAdditional()
        {
            var folded = new FoldedDataSet(_underlying.OpenAdditional()) {Owner = this};
            return folded;
        }


        /// <summary>
        /// Get an enumberator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IMLDataPair> GetEnumerator()
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
            _numFolds = Math.Min(numFolds, _underlying
                                               .Count);
            _foldSize = _underlying.Count/_numFolds;
            _lastFoldSize = _underlying.Count - (_foldSize*_numFolds);
            CurrentFold = 0;
        }

        /// <inheritdoc/>
        public IMLDataPair this[int x]
        {
            get
            {
				return _underlying[CurrentFoldOffset + x];
            }
        }
    }
}
