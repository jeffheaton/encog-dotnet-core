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
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Neural.Freeform.Training
{
    /// <summary>
    ///     Train freeform network with RPROP.
    /// </summary>
    [Serializable]
    public class FreeformResilientPropagation: FreeformPropagationTraining
    {
        /// <summary>
        /// Temp value #0: the gradient.
        /// </summary>
        public const int TempGradient = 0;

        /// <summary>
        /// Temp value #1: the last gradient.
        /// </summary>
        public const int TempLastGradient = 1;

        /// <summary>
        /// Temp value #2: the update.
        /// </summary>
        public const int TempUpdate = 2;

        /// <summary>
        /// Temp value #3: the the last weight delta.
        /// </summary>
        public const int TempLastWeightDelta = 3;

        /// <summary>
        /// The max step.
        /// </summary>
        private readonly double _maxStep;

  
        /// <summary>
        /// Construct the RPROP trainer, Use default intiial update and max step.
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theTraining">The training set.</param>
        public FreeformResilientPropagation(FreeformNetwork theNetwork,
            IMLDataSet theTraining)
            : this(theNetwork, theTraining, RPROPConst.DefaultInitialUpdate,
                RPROPConst.DefaultMaxStep)
        {
        }

        /// <summary>
        ///     Construct the RPROP trainer.
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theTraining">The training set.</param>
        /// <param name="initialUpdate">The initial update.</param>
        /// <param name="theMaxStep">The max step.</param>
        public FreeformResilientPropagation(FreeformNetwork theNetwork,
            IMLDataSet theTraining, double initialUpdate,
            double theMaxStep)
            : base(theNetwork, theTraining)
        {
            _maxStep = theMaxStep;
            theNetwork.TempTrainingAllocate(1, 4);
            theNetwork.PerformConnectionTask(c => c.SetTempTraining(TempUpdate,
                initialUpdate));
        }

        /// <inheritdoc />
        protected override void LearnConnection(IFreeformConnection connection)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = EncogMath
                .Sign(connection
                    .GetTempTraining(TempGradient)
                      *connection
                          .GetTempTraining(TempLastGradient));
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = connection
                    .GetTempTraining(TempUpdate)
                               *RPROPConst.PositiveEta;
                delta = Math.Min(delta, _maxStep);
                weightChange = EncogMath
                    .Sign(connection
                        .GetTempTraining(TempGradient))
                               *delta;
                connection.SetTempTraining(
                    TempUpdate, delta);
                connection
                    .SetTempTraining(
                        TempLastGradient,
                        connection
                            .GetTempTraining(TempGradient));
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = connection
                    .GetTempTraining(TempUpdate)
                               *RPROPConst.NegativeEta;
                delta = Math.Max(delta, RPROPConst.DeltaMin);
                connection.SetTempTraining(
                    TempUpdate, delta);
                weightChange = -connection
                    .GetTempTraining(TempLastWeightDelta);
                // set the previous gradient to zero so that there will be no
                // adjustment the next iteration
                connection.SetTempTraining(
                    TempLastGradient, 0);
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = connection
                    .GetTempTraining(TempUpdate);
                weightChange = EncogMath
                    .Sign(connection
                        .GetTempTraining(TempGradient))
                               *delta;
                connection
                    .SetTempTraining(
                        TempLastGradient,
                        connection
                            .GetTempTraining(TempGradient));
            }

            // apply the weight change, if any
            connection.Weight += weightChange;
            connection.SetTempTraining(
                TempLastWeightDelta,
                weightChange);
        }

        /// <inheritdoc />
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc />
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
