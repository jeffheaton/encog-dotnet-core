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

namespace Encog.ML.Data.Versatile.Division
{
    /// <summary>
    ///     Perform a data division.
    /// </summary>
    public class PerformDataDivision
    {
        /// <summary>
        ///     A random number generator.
        /// </summary>
        private readonly IGenerateRandom _rnd;

        /// <summary>
        ///     True, if we should shuffle during division.
        /// </summary>
        private readonly bool _shuffle;

        /// <summary>
        ///     Construct the data division processor.
        /// </summary>
        /// <param name="theShuffle">Should we shuffle?</param>
        /// <param name="theRandom">Random number generator, often seeded to be consistent. </param>
        public PerformDataDivision(bool theShuffle, IGenerateRandom theRandom)
        {
            _shuffle = theShuffle;
            _rnd = theRandom;
        }

        /// <summary>
        ///     Should we shuffle.
        /// </summary>
        public bool Shuffle
        {
            get { return _shuffle; }
        }

        /// <summary>
        ///     Random number generator.
        /// </summary>
        public IGenerateRandom Random
        {
            get { return _rnd; }
        }

        /// <summary>
        ///     Perform the split.
        /// </summary>
        /// <param name="dataDivisionList">The list of data divisions.</param>
        /// <param name="dataset">The dataset to split.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The ideal count.</param>
        public void Perform(IList<DataDivision> dataDivisionList, MatrixMLDataSet dataset,
            int inputCount, int idealCount)
        {
            GenerateCounts(dataDivisionList, dataset.Data.Length);
            GenerateMasks(dataDivisionList);
            if (_shuffle)
            {
                PerformShuffle(dataDivisionList, dataset.Data.Length);
            }
            CreateDividedDatasets(dataDivisionList, dataset, inputCount, idealCount);
        }

        /// <summary>
        ///     Create the datasets that we will divide into.
        /// </summary>
        /// <param name="dataDivisionList">The list of divisions.</param>
        /// <param name="parentDataset">The data set to divide.</param>
        /// <param name="inputCount">The input count.</param>
        /// <param name="idealCount">The ideal count.</param>
        private void CreateDividedDatasets(IEnumerable<DataDivision> dataDivisionList,
            MatrixMLDataSet parentDataset, int inputCount, int idealCount)
        {
            foreach (DataDivision division in dataDivisionList)
            {
                var dataset = new MatrixMLDataSet(parentDataset.Data, inputCount,
                    idealCount, division.Mask)
                {
                    LagWindowSize = parentDataset.LagWindowSize,
                    LeadWindowSize = parentDataset.LeadWindowSize
                };
                division.Dataset = dataset;
            }
        }

        /// <summary>
        ///     Perform a Fisher-Yates shuffle.
        ///     http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        /// <param name="dataDivisionList">The division list.</param>
        /// <param name="totalCount">Total count across divisions.</param>
        private void PerformShuffle(IList<DataDivision> dataDivisionList,
            int totalCount)
        {
            for (int i = totalCount - 1; i > 0; i--)
            {
                int n = _rnd.NextInt(i + 1);
                VirtualSwap(dataDivisionList, i, n);
            }
        }

        /// <summary>
        ///     Swap two items, across all divisions.
        /// </summary>
        /// <param name="dataDivisionList">The division list.</param>
        /// <param name="a">The index of the first item to swap.</param>
        /// <param name="b">The index of the second item to swap.</param>
        private void VirtualSwap(IEnumerable<DataDivision> dataDivisionList, int a, int b)
        {
            DataDivision divA = null;
            DataDivision divB = null;
            int offsetA = 0;
            int offsetB = 0;

            // Find points a and b in the collections.
            int baseIndex = 0;
            foreach (DataDivision division in dataDivisionList)
            {
                baseIndex += division.Count;

                if (divA == null && a < baseIndex)
                {
                    divA = division;
                    offsetA = a - (baseIndex - division.Count);
                }
                if (divB == null && b < baseIndex)
                {
                    divB = division;
                    offsetB = b - (baseIndex - division.Count);
                }
            }

            // Swap a and b.
            int temp = divA.Mask[offsetA];
            divA.Mask[offsetA] = divB.Mask[offsetB];
            divB.Mask[offsetB] = temp;
        }

        /// <summary>
        ///     Generate the masks, for all divisions.
        /// </summary>
        /// <param name="dataDivisionList">The divisions.</param>
        private void GenerateMasks(IEnumerable<DataDivision> dataDivisionList)
        {
            int idx = 0;
            foreach (DataDivision division in dataDivisionList)
            {
                division.AllocateMask(division.Count);
                for (int i = 0; i < division.Count; i++)
                {
                    division.Mask[i] = idx++;
                }
            }
        }

        /// <summary>
        ///     Generate the counts for all divisions, give remaining items to final division.
        /// </summary>
        /// <param name="dataDivisionList">The division list.</param>
        /// <param name="totalCount">The total count.</param>
        private void GenerateCounts(IList<DataDivision> dataDivisionList,
            int totalCount)
        {
            // First pass at division.
            int countSofar = 0;
            foreach (DataDivision division in dataDivisionList)
            {
                var count = (int) (division.Percent*totalCount);
                division.Count = count;
                countSofar += count;
            }
            // Adjust any remaining count
            int remaining = totalCount - countSofar;
            while (remaining-- > 0)
            {
                int idx = _rnd.NextInt(dataDivisionList.Count);
                DataDivision div = dataDivisionList[idx];
                div.Count = div.Count + 1;
            }
        }
    }
}
