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
using Encog.ML;
using Encog.ML.Data.Folded;
using Encog.ML.Train;

namespace Encog.Neural.Networks.Training.Cross
{
    /// <summary>
    /// Base class for cross training trainers. Must use a folded dataset.  
    /// </summary>
    ///
    public abstract class CrossTraining : BasicTraining
    {
        /// <summary>
        /// The folded dataset.
        /// </summary>
        ///
        private readonly FoldedDataSet folded;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly MLMethod network;

        /// <summary>
        /// Construct a cross trainer.
        /// </summary>
        ///
        /// <param name="network_0">The network.</param>
        /// <param name="training">The training data.</param>
        public CrossTraining(MLMethod network_0, FoldedDataSet training) : base(TrainingImplementationType.Iterative)
        {
            network = network_0;
            Training = training;
            folded = training;
        }


        /// <value>The folded training data.</value>
        public FoldedDataSet Folded
        {
            get { return folded; }
        }


        /// <inheritdoc/>
        public override MLMethod Method
        {            
            get { return network; }
        }
    }
}
