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
using Encog.Util;
using Encog.Util.Validate;

namespace Encog.Neural.Networks.Training.Propagation.Quick
{
    /// <summary>
    /// QPROP is an efficient training method that is based on Newton's Method.  
    /// QPROP was introduced in a paper:
    /// 
    /// An Empirical Study of Learning Speed in Back-Propagation Networks" (Scott E. Fahlman, 1988)
    /// 
    ///  
    /// http://www.heatonresearch.com/wiki/Quickprop
    /// </summary>
    public sealed class QuickPropagation : Propagation, ILearningRate
    {
        /// <summary>
        /// This factor times the current weight is added to the slope 
        /// at the start of each output epoch. Keeps weights from growing 
        /// too big.
        /// </summary>
        public double Decay { get; set; }

        /// <summary>
        /// Used to scale for the size of the training set.
        /// </summary>
        public double EPS { get; set; }

        /// <summary>
        /// The last deltas.
        /// </summary>
        public double[] LastDelta { get; set; }

        /// <summary>
        /// The learning rate.
        /// </summary>
        public double LearningRate { get; set; }

        /// <summary>
        /// Controls the amount of linear gradient descent 
        /// to use in updating output weights.
        /// </summary>
        public double OutputEpsilon { get; set; }

        /// <summary>
        /// Used in computing whether the proposed step is 
        /// too large.  Related to learningRate.
        /// </summary>
        public double Shrink { get; set; }


        /// <summary>
        /// Continuation tag for the last gradients.
        /// </summary>
        public const String LastGradients = "LAST_GRADIENTS";
        
        /// <summary>
        /// Construct a QPROP trainer for flat networks.  Uses a learning rate of 2.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public QuickPropagation(IContainsFlat network, IMLDataSet training) : this(network, training, 2.0)
        {
        }


        /// <summary>
        /// Construct a QPROP trainer for flat networks.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        /// <param name="learnRate">The learning rate.  2 is a good suggestion as 
        ///            a learning rate to start with.  If it fails to converge, 
        ///            then drop it.  Just like backprop, except QPROP can 
        ///            take higher learning rates.</param>
        public QuickPropagation(IContainsFlat network,
                                IMLDataSet training, double learnRate) : base(network, training)
        {
            ValidateNetwork.ValidateMethodToData(network, training);
            LearningRate = learnRate;
            LastDelta = new double[Network.Flat.Weights.Length];
            OutputEpsilon = 1.0;
        }

        /// <inheritdoc />
        public override bool CanContinue
        {
            get { return true; }
        }

       
        /// <summary>
        /// Determine if the specified continuation object is valid to resume with.
        /// </summary>
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
	    /// training method and network.</returns>
        public bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(LastGradients))
            {
                return false;
            }

            if (!state.TrainingType.Equals(GetType().Name))
            {
                return false;
            }

            var d = (double[]) state.Contents[LastGradients];
            return d.Length == ((IContainsFlat) Method).Flat.Weights.Length;
        }

        /// <summary>
        /// Pause the training.
        /// </summary>
        /// <returns>A training continuation object to continue with.</returns>
        public override TrainingContinuation Pause()
        {
            var result = new TrainingContinuation {TrainingType = (GetType().Name)};
            result.Contents[LastGradients] = LastGradient;
            return result;
        }
        
        /// <summary>
        /// Resume training.
        /// </summary>
        /// <param name="state">The training state to return to.</param>
        public override void Resume(TrainingContinuation state)
        {
            if (!IsValidResume(state))
            {
                throw new TrainingError("Invalid training resume data length");
            }

            var lastGradient = (double[]) state.Contents[
                LastGradients];

            EngineArray.ArrayCopy(lastGradient,LastGradient);
        }

        /// <summary>
        /// Called to init the QPROP.
        /// </summary>
        public override void InitOthers()
        {
            EPS = OutputEpsilon / Training.Count;
            Shrink = LearningRate / (1.0 + LearningRate);
        }

        /// <summary>
        /// Update a weight.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The weight delta.</returns>
        public override double UpdateWeight(double[] gradients,
                                            double[] lastGradient, int index)
        {
            double w = Network.Flat.Weights[index];
            double d = LastDelta[index];
            double s = -Gradients[index] + Decay * w;
            double p = -lastGradient[index];
            double nextStep = 0.0;

            // The step must always be in direction opposite to the slope.
            if (d < 0.0)
            {
                // If last step was negative...
                if (s > 0.0)
                {
                    // Add in linear term if current slope is still positive.
                    nextStep -= EPS * s;
                }
                // If current slope is close to or larger than prev slope...
                if (s >= (Shrink * p))
                {
                    // Take maximum size negative step.
                    nextStep += LearningRate * d;
                }
                else
                {
                    // Else, use quadratic estimate.
                    nextStep += d * s / (p - s);
                }
            }
            else if (d > 0.0)
            {
                // If last step was positive...
                if (s < 0.0)
                {
                    // Add in linear term if current slope is still negative.
                    nextStep -= EPS * s;
                }
                // If current slope is close to or more neg than prev slope...
                if (s <= (Shrink * p))
                {
                    // Take maximum size negative step.
                    nextStep += LearningRate * d;
                }
                else
                {
                    // Else, use quadratic estimate.
                    nextStep += d * s / (p - s);
                }
            }
            else
            {
                // Last step was zero, so use only linear term. 
                nextStep -= EPS * s;
            }

            // update global data arrays
            LastDelta[index] = nextStep;
            LastGradient[index] = gradients[index];

            return nextStep;
        }

    }
}
