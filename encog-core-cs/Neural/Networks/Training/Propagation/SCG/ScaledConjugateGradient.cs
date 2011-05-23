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
using Encog.Neural.Flat.Train.Prop;

namespace Encog.Neural.Networks.Training.Propagation.SCG
{
    /// <summary>
    /// This is a training class that makes use of scaled conjugate gradient methods.
    /// It is a very fast and efficient training algorithm.
    /// </summary>
    ///
    public class ScaledConjugateGradient : Propagation
    {
        /// <summary>
        /// Construct a training class.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public ScaledConjugateGradient(IContainsFlat network,
                                       IMLDataSet training) : base(network, training)
        {
            var rpropFlat = new TrainFlatNetworkSCG(
                network.Flat, Training);
            FlatTraining = rpropFlat;
        }

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <returns>Always returns false.</returns>
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <returns>Always returns null.</returns>
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// This training type does not support training continue.
        /// </summary>
        ///
        /// <param name="state">Not used.</param>
        public override sealed void Resume(TrainingContinuation state)
        {
        }
    }
}
