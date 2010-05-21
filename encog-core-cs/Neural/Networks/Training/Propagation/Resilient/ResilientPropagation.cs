// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;
using Encog.Neural.Networks.Training.Propagation.Gradient;
using Encog.Neural.Networks.Flat;
using Encog.Neural.Data;

namespace Encog.Neural.Networks.Training.Propagation.Resilient
{
    /// <summary>
    /// One problem with the backpropagation algorithm is that the magnitude of the
    /// partial derivative is usually too large or too small. Further, the learning
    /// rate is a single value for the entire neural network. The resilient
    /// propagation learning algorithm uses a special update value(similar to the
    /// learning rate) for every neuron connection. Further these update values are
    /// automatically determined, unlike the learning rate of the backpropagation
    /// algorithm.
    /// 
    /// For most training situations, we suggest that the resilient propagation
    /// algorithm (this class) be used for training.
    /// 
    /// There are a total of three parameters that must be provided to the resilient
    /// training algorithm. Defaults are provided for each, and in nearly all cases,
    /// these defaults are acceptable. This makes the resilient propagation algorithm
    /// one of the easiest and most efficient training algorithms available.
    /// 
    /// The optional parameters are:
    /// 
    /// zeroTolerance - How close to zero can a number be to be considered zero. The
    /// default is 0.00000000000000001.
    /// 
    /// initialUpdate - What are the initial update values for each matrix value. The
    /// default is 0.1.
    /// 
    /// maxStep - What is the largest amount that the update values can step. The
    /// default is 50.
    /// </summary>
    public class ResilientPropagation : Propagation
    {
        /// <summary>
        /// The default zero tolerance.
        /// </summary>
        public const double DEFAULT_ZERO_TOLERANCE = 0.00000000000000001;

        /// <summary>
        /// The POSITIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is greater than zero.
        /// </summary>
        public const double POSITIVE_ETA = 1.2;

        /// <summary>
        /// The NEGATIVE ETA value. This is specified by the resilient propagation
        /// algorithm. This is the percentage by which the deltas are increased by if
        /// the partial derivative is less than zero.
        /// </summary>
        public const double NEGATIVE_ETA = 0.5;

        /// <summary>
        /// The minimum delta value for a weight matrix value.
        /// </summary>
        public const double DELTA_MIN = 1e-6;

        /// <summary>
        /// The starting update for a delta.
        /// </summary>
        public const double DEFAULT_INITIAL_UPDATE = 0.1;

        /// <summary>
        /// The maximum amount a delta can reach.
        /// </summary>
        public const double DEFAULT_MAX_STEP = 50;

        /// <summary>
        /// Continuation tag for the last gradients.
        /// </summary>
        public const String LAST_GRADIENTS = "LAST_GRADIENTS";

        /// <summary>
        /// Continuation tag for the last values.
        /// </summary>
        public const String UPDATE_VALUES = "UPDATE_VALUES";

        /// <summary>
        /// The zero tolerance.
        /// </summary>
        private double zeroTolerance;

        /// <summary>
        /// The initial update value.
        /// </summary>
        private double initialUpdate;

        /// <summary>
        /// The maximum delta amount.
        /// </summary>
        private double maxStep;

        /// <summary>
        /// The update value.
        /// </summary>
        private double[] updateValues;

        /// <summary>
        /// The last gradients.
        /// </summary>
        private double[] lastGradient;

        /// <summary>
        /// The current gradients.
        /// </summary>
        private double[] gradients;

        /// <summary>
        /// Construct a resilient training object. Use the defaults for all training
        /// parameters. Usually this is the constructor to use as the resilient
        /// training algorithm is designed for the default parameters to be
        /// acceptable for nearly all problems.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        public ResilientPropagation(BasicNetwork network,
                 INeuralDataSet training)
            : this(network, training, ResilientPropagation.DEFAULT_ZERO_TOLERANCE,
                ResilientPropagation.DEFAULT_INITIAL_UPDATE,
                ResilientPropagation.DEFAULT_MAX_STEP)
        {

        }
        
        /// <summary>
        /// Construct a resilient training object, allow the training parameters to
        /// be specified. Usually the default parameters are acceptable for the
        /// resilient training algorithm. Therefore you should usually use the other
        /// constructor, that makes use of the default values.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="zeroTolerance">The zero tolerance.</param>
        /// <param name="initialUpdate">The initial update values, this is the amount that the deltas
        /// are all initially set to.</param>
        /// <param name="maxStep">The maximum that a delta can reach.</param>
        public ResilientPropagation(BasicNetwork network,
                 INeuralDataSet training, double zeroTolerance,
                 double initialUpdate, double maxStep)
            : base(network, training)
        {
            this.initialUpdate = initialUpdate;
            this.maxStep = maxStep;
            this.zeroTolerance = zeroTolerance;

            this.updateValues = new double[network.Structure.CalculateSize()];
            this.lastGradient = new double[network.Structure.CalculateSize()];

            for (int i = 0; i < this.updateValues.Length; i++)
            {
                this.updateValues[i] = this.initialUpdate;
            }
        }

        /// <summary>
        /// True, as RPROP can continue.
        /// </summary>
        public override bool CanContinue
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The initial update amount, set by the constructor.
        /// </summary>
        public double InitialUpdate
        {
            get
            {
                return this.initialUpdate;
            }
        }

        /// <summary>
        /// The maximum step, set by the constructor.
        /// </summary>
        public double MaxStep
        {
            get
            {
                return this.maxStep;
            }
        }

        /// <summary>
        /// The zero tolerance, set by the constructor.
        /// </summary>
        public double ZeroTolerance
        {
            get
            {
                return this.zeroTolerance;
            }
        }

        /// <summary>
        /// Determine if the specified continuation object is valid to resume with.
        /// </summary>
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
        /// training method and network.</returns>
        public override bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(
                    ResilientPropagation.LAST_GRADIENTS)
                    || !state.Contents.ContainsKey(
                            ResilientPropagation.UPDATE_VALUES))
            {
                return false;
            }

            double[] d = (double[])state
                   [ResilientPropagation.LAST_GRADIENTS];
            return d.Length == Network.Structure.CalculateSize();
        }

        /// <summary>
        /// Pause the training.
        /// </summary>
        /// <returns>A training continuation object to continue with.</returns>
        public override TrainingContinuation Pause()
        {
            TrainingContinuation result = new TrainingContinuation();
            result[ResilientPropagation.LAST_GRADIENTS] = this.lastGradient;
            result[ResilientPropagation.UPDATE_VALUES] = this.updateValues;
            return result;
        }


        /// <summary>
        /// Perform a training iteration. This is where the actual RPROP specific
        /// training takes place.
        /// </summary>
        /// <param name="prop">The gradients.</param>
        /// <param name="weights">The network weights.</param>
        public override void PerformIteration(CalculateGradient prop,
                double[] weights)
        {
            this.gradients = prop.Gradients;

            for (int i = 0; i < this.gradients.Length; i++)
            {
                weights[i] += UpdateWeight(this.gradients, i);
            }

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
            this.lastGradient = (double[])state
                    [ResilientPropagation.LAST_GRADIENTS];
            this.updateValues = (double[])state
                    [ResilientPropagation.UPDATE_VALUES];
        }

        /// <summary>
        /// Determine the sign of the value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double value)
        {
            if (Math.Abs(value) < this.zeroTolerance)
            {
                return 0;
            }
            else if (value > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Determine the amount to change a weight by.
        /// </summary>
        /// <param name="gradients">The gradients.</param>
        /// <param name="index">The weight to adjust.</param>
        /// <returns>The amount to change this weight by.</returns>
        private double UpdateWeight(double[] gradients, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(this.gradients[index]
                   * this.lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = this.updateValues[index]
                        * ResilientPropagation.POSITIVE_ETA;
                delta = Math.Min(delta, this.maxStep);
                weightChange = Sign(this.gradients[index]) * delta;
                this.updateValues[index] = delta;
                this.lastGradient[index] = this.gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = this.updateValues[index]
                        * ResilientPropagation.NEGATIVE_ETA;
                delta = Math.Max(delta, ResilientPropagation.DELTA_MIN);
                this.updateValues[index] = delta;
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                this.lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = this.lastGradient[index];
                weightChange = Sign(this.gradients[index]) * delta;
                this.lastGradient[index] = this.gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }
    }
}
