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
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.Neural.Freeform.Training
{
    /// <summary>
    /// Perform backpropagation for a freeform neural network.
    /// </summary>
    [Serializable]
    public class FreeformBackPropagation : FreeformPropagationTraining
    {
        /// <summary>
        /// The learning rate.  The coefficient for how much of the gradient is applied to each weight.
        /// </summary>
        private readonly double _learningRate;

        /// <summary>
        /// The momentum.  The coefficient for how much of the previous delta is applied to each weight.  
        /// In theory, prevents local minima stall.
        /// </summary>
        private readonly double _momentum;

        /// <summary>
        /// Construct a back propagation trainer.
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theTraining">The training data to use. The coefficient for how much of the gradient is applied to each weight.</param>
        /// <param name="theLearningRate">The learning rate. The coefficient for how much of the previous delta is applied to each weight.</param>
        /// <param name="theMomentum">The momentum.</param>
        public FreeformBackPropagation(FreeformNetwork theNetwork,
                IMLDataSet theTraining, double theLearningRate,
                double theMomentum)
            : base(theNetwork, theTraining)
        {
            theNetwork.TempTrainingAllocate(1, 2);
            _learningRate = theLearningRate;
            _momentum = theMomentum;
        }

        /// <inheritdoc/>
        protected override void LearnConnection(IFreeformConnection connection)
        {
            double gradient = connection.GetTempTraining(0);
            double delta = (gradient * _learningRate)
                    + (connection.GetTempTraining(1) * _momentum);
            connection.SetTempTraining(1, delta);
            connection.Weight += delta;
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            // TODO Auto-generated method stub
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
            // TODO Auto-generated method stub

        }
    }
}
