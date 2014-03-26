//
// Encog(tm) Core v3.2 - .Net Version
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
using System.IO;
using Encog.Util.File;

namespace Encog.Examples.Forest
{
    /// <summary>
    /// Config info for the forest example.
    /// </summary>
    public class ForestConfig
    {
        /// <summary>
        /// We will limit the number of samples per "tree type" to 3000, this causes the data
        /// to be more balanced and will not allow one tree type to over-fit the network.
        /// The training file is narrowed and placed into this file in "raw form".
        /// </summary>
        public FileInfo BalanceFile;

        /// <summary>
        /// CSV files are slow to parse with because the text must be converted into numbers.
        /// The balanced file will be converted to a binary file to be used for training.
        /// </summary>
        public FileInfo BinaryFile;

        /// <summary>
        /// The source data file from which all others are built.  This file can
        /// be downloaded from:
        /// 
        /// http://kdd.ics.uci.edu/databases/covertype/covertype.html
        /// </summary>
        public FileInfo CoverTypeFile;

        /// <summary>
        /// 25% of the data will be moved into this file to be used as evaluation data.  The 
        /// data is still in "raw form" in this file.
        /// </summary>
        public FileInfo EvaluateFile;

        /// <summary>
        /// The normalizer will be saved into a ser file.
        /// </summary>
        public FileInfo NormalizeFile;

        /// <summary>
        /// The training file is normalized and placed into this file.
        /// </summary>
        public FileInfo NormalizedDataFile;

        /// <summary>
        /// The trained network saved into an Encog EG file.
        /// </summary>
        public FileInfo TrainedNetworkFile;

        /// <summary>
        /// 75% of the data will be moved into this file to be used as training data.  The 
        /// data is still in "raw form" in this file.
        /// </summary>
        public FileInfo TrainingFile;

        public ForestConfig(FileInfo theBasePath)
        {
            BasePath = theBasePath;

            CoverTypeFile = FileUtil.CombinePath(BasePath, "covtype.data");
            TrainingFile = FileUtil.CombinePath(BasePath, "training.csv");
            EvaluateFile = FileUtil.CombinePath(BasePath, "evaluate.csv");
            BalanceFile = FileUtil.CombinePath(BasePath, "balance.csv");
            NormalizedDataFile = FileUtil.CombinePath(BasePath, "normalized.csv");
            BinaryFile = FileUtil.CombinePath(BasePath, "normalized.egb");
            TrainedNetworkFile = FileUtil.CombinePath(BasePath, "forest.eg");
            NormalizeFile = FileUtil.CombinePath(BasePath, "norm.ser");

            TrainingMinutes = 10;
            HiddenCount = 100;
        }

        /// <summary>
        /// The base directory that all of the data for this example is stored in.
        /// </summary>
        public FileInfo BasePath { get; set; }

        /// <summary>
        /// How many minutes to train for (console mode only)
        /// </summary>
        public int TrainingMinutes { get; set; }

        /// <summary>
        /// How many hidden neurons to use.
        /// </summary>
        public int HiddenCount { get; set; }
    }
}
