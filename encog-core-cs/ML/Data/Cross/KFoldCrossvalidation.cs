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
using System.Collections.Generic;
using Encog.MathUtil.Randomize.Generate;
using Encog.ML.Data.Versatile;
using Encog.Util;

namespace Encog.ML.Data.Cross
{
    public class KFoldCrossvalidation
    {
        private readonly MatrixMLDataSet _baseDataset;
        private readonly IList<DataFold> _folds = new List<DataFold>();
        private readonly int _k;

        public KFoldCrossvalidation()
        {
            Rnd = new MersenneTwisterGenerateRandom();
        }

        public KFoldCrossvalidation(MatrixMLDataSet theBaseDataset, int theK) : this()
        {
            _baseDataset = theBaseDataset;
            _k = theK;
        }

        public IGenerateRandom Rnd { get; set; }

        /// <summary>
        ///     The base data set.
        /// </summary>
        public MatrixMLDataSet BaseDataset
        {
            get { return _baseDataset; }
        }

        /**
	 * @return the k
	 */

        public int K
        {
            get { return _k; }
        }

        /**
	 * @return the folds
	 */

        public IList<DataFold> Folds
        {
            get { return _folds; }
        }

        private int[] BuildFirstList(int length)
        {
            var result = new int[length];

            if (_baseDataset == null)
            {
                for (int i = 0; i < length; i++)
                {
                    result[i] = i;
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    result[i] = _baseDataset.Mask[i];
                }
            }

            return result;
        }

        private void ShuffleList(int[] list)
        {
            for (int i = list.Length - 1; i > 0; i--)
            {
                int n = Rnd.NextInt(i + 1);
                int t = list[i];
                list[i] = list[n];
                list[n] = t;
            }
        }

        private IList<int[]> AllocateFolds()
        {
            IList<int[]> folds = new List<int[]>();
            int countPer = _baseDataset.Count/K;
            int countFirst = _baseDataset.Count - (countPer*(K - 1));

            folds.Add(new int[countFirst]);
            for (int i = 1; i < K; i++)
            {
                folds.Add(new int[countPer]);
            }

            return folds;
        }

        private void PopulateFolds(IList<int[]> folds, int[] firstList)
        {
            int idx = 0;
            foreach (var fold in folds)
            {
                for (int i = 0; i < fold.Length; i++)
                {
                    fold[i] = firstList[idx++];
                }
            }
        }

        private void BuildSets(IList<int[]> foldContents)
        {
            _folds.Clear();

            for (int i = 0; i < K; i++)
            {
                // first calculate the size
                int trainingSize = 0;
                int validationSize = 0;
                for (int j = 0; j < foldContents.Count; j++)
                {
                    int foldSize = foldContents[j].Length;
                    if (j == i)
                    {
                        validationSize += foldSize;
                    }
                    else
                    {
                        trainingSize += foldSize;
                    }
                }
                // create the masks
                var trainingMask = new int[trainingSize];
                var validationMask = new int[validationSize];
                int trainingIndex = 0;
                for (int j = 0; j < foldContents.Count; j++)
                {
                    int[] source = foldContents[j];
                    if (j == i)
                    {
                        EngineArray.ArrayCopy(source, 0, validationMask, 0, source.Length);
                    }
                    else
                    {
                        EngineArray.ArrayCopy(source, 0, trainingMask, trainingIndex, source.Length);
                        trainingIndex += source.Length;
                    }
                }
                // Build the set
                var training = new MatrixMLDataSet(_baseDataset, trainingMask);
                var validation = new MatrixMLDataSet(_baseDataset, validationMask);
                _folds.Add(new DataFold(training, validation));
            }
        }

        public void Process(bool shuffle)
        {
            int[] firstList = BuildFirstList(_baseDataset.Count);

            if (shuffle)
            {
                ShuffleList(firstList);
            }

            IList<int[]> foldContents = AllocateFolds();
            PopulateFolds(foldContents, firstList);
            BuildSets(foldContents);
        }
    }
}
