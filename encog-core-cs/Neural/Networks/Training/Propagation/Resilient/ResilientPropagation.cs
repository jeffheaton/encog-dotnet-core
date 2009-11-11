// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.NeuralData;

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
        /// The POSITIVE ETA value.  This is specified by the resilient 
        /// propagation algorithm.  This is the percentage by which 
        /// the deltas are increased by if the partial derivative is
        /// greater than zero.
        /// </summary>
        public const double POSITIVE_ETA = 1.2;

        /// <summary>
        /// The NEGATIVE ETA value.  This is specified by the resilient 
        /// propagation algorithm.  This is the percentage by which 
        /// the deltas are increased by if the partial derivative is
        /// less than zero.
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
        /// Construct a resilient training object.  Use the defaults for all
        /// training parameters.  Usually this is the constructor to use as
        /// the resilient training algorithm is designed for the default 
        /// parameters to be acceptable for nearly all problems.
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
        /// Construct a resilient training object, allow the training parameters
        /// to be specified.  Usually the default parameters are acceptable for
        /// the resilient training algorithm.  Therefore you should usually
        /// use the other constructor, that makes use of the default values.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="zeroTolerance">The zero tolerance.</param>
        /// <param name="initialUpdate">The initial update values, this is the amount 
        /// that the deltas are all initially set to.</param>
        /// <param name="maxStep">The maximum that a delta can reach.</param>
        public ResilientPropagation(BasicNetwork network,
                 INeuralDataSet training, double zeroTolerance,
                 double initialUpdate, double maxStep)
            : base(network, new ResilientPropagationMethod(zeroTolerance,maxStep,initialUpdate), training)
        {
            this.initialUpdate = initialUpdate;
            this.maxStep = maxStep;
            this.zeroTolerance = zeroTolerance;
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

    }
}
