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
using Encog.Neural.Networks.Training.Propagation.Resilient;

namespace Encog.Neural.Networks.Training.Propagation.Manhattan
{
    /// <summary>
    /// One problem that the backpropagation technique has is that the magnitude of
    /// the partial derivative may be calculated too large or too small. The
    /// Manhattan update algorithm attempts to solve this by using the partial
    /// derivative to only indicate the sign of the update to the weight matrix. The
    /// actual amount added or subtracted from the weight matrix is obtained from a
    /// simple constant. This constant must be adjusted based on the type of neural
    /// network being trained. In general, start with a higher constant and decrease
    /// it as needed.
    /// The Manhattan update algorithm can be thought of as a simplified version of
    /// the resilient algorithm. The resilient algorithm uses more complex techniques
    /// to determine the update value.
    /// </summary>
    ///
    public class ManhattanPropagation : Propagation, ILearningRate
    {
        /// <summary>
        /// The default tolerance to determine of a number is close to zero.
        /// </summary>
        ///
        internal const double DefaultZeroTolerance = 0.001d;

        /// <summary>
        /// The zero tolerance to use.
        /// </summary>
        ///
        private readonly double _zeroTolerance;

        /// <summary>
        /// The learning rate.
        /// </summary>
        ///
        private double _learningRate;


        /// <summary>
        /// Construct a Manhattan propagation training object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="learnRate">The learning rate.</param>
        public ManhattanPropagation(IContainsFlat network,
                                    IMLDataSet training, double learnRate) : base(network, training)
        {
            _learningRate = learnRate;
            _zeroTolerance = RPROPConst.DefaultZeroTolerance;            
        }


        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        #region ILearningRate Members

        /// <summary>
        /// Set the learning rate.
        /// </summary>
        public virtual double LearningRate
        {
            get { return _learningRate; }
            set { _learningRate = value; }
        }

        #endregion

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

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override sealed double UpdateWeight(double[] gradients,
                                                   double[] lastGradient, int index)
        {
            if (Math.Abs(gradients[index]) < _zeroTolerance)
            {
                return 0;
            }
            else if (gradients[index] > 0)
            {
                return _learningRate;
            }
            else
            {
                return -_learningRate;
            }
        }

        /// <summary>
        /// Not needed for this training type.
        /// </summary>
        public override void InitOthers()
        {
        }

    }
}
