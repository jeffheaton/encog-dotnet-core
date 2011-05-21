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
using Encog.ML.Data;

namespace Encog.Neural.Flat.Train
{
    /// <summary>
    /// Common interface for training a flat neural network.
    /// </summary>
    ///
    public interface TrainFlatNetwork
    {
        /// <summary>
        /// The error.
        /// </summary>
        double Error { get; }


        /// <value>The trained neural network.</value>
        FlatNetwork Network { get; }

        /// <value>The data we are training with.</value>
        IMLDataSet Training { get; }

        /// <summary>
        /// Set the number of threads to use.
        /// </summary>
        ///
        /// <value>The number of threads to use.</value>
        int NumThreads { get; set; }

        /// <summary>
        /// Set the iteration.
        /// </summary>
        int IterationNumber { get; set; }


        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        void Iteration();

        /// <summary>
        /// Perform one training iteration.
        /// </summary>
        ///
        /// <param name="count">The number of iterations.</param>
        void Iteration(int count);


        /// <summary>
        /// Training is to stop, free any resources.
        /// </summary>
        ///
        void FinishTraining();
    }
}
