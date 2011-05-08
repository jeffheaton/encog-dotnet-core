// Encog(tm) Artificial Intelligence Framework v2.5
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
using Encog.Neural.Data;
using Encog.Engine.Network.Train.Prop;
using Encog.Engine.Util;

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
        /// Continuation tag for the last gradients.
        /// </summary>
        public readonly static String LAST_GRADIENTS = "LAST_GRADIENTS";

        /// <summary>
        /// Continuation tag for the last values.
        /// </summary>
        public readonly static String UPDATE_VALUES = "UPDATE_VALUES";
        
        /// <summary>
        /// Construct a resilient training object. Use the defaults for all training
        /// parameters. Usually this is the constructor to use as the resilient
        /// training algorithm is designed for the default parameters to be
        /// acceptable for nearly all problems. Use the CPU to train. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        public ResilientPropagation(BasicNetwork network,
                 INeuralDataSet training)
            : this(network, training, null, RPROPConst.DEFAULT_INITIAL_UPDATE,
                RPROPConst.DEFAULT_MAX_STEP)
        {

        }

        /// <summary>
        /// Construct an RPROP trainer, allows an OpenCL device to be specified. Use
        /// the defaults for all training parameters. Usually this is the constructor
        /// to use as the resilient training algorithm is designed for the default
        /// parameters to be acceptable for nearly all problems. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="profile">The profile to use.</param>
        public ResilientPropagation(BasicNetwork network,
                 INeuralDataSet training, OpenCLTrainingProfile profile)
            : this(network, training, profile, RPROPConst.DEFAULT_INITIAL_UPDATE,
                RPROPConst.DEFAULT_MAX_STEP)
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
        /// <param name="profile">Optional EncogCL profile to execute on.</param>
        /// <param name="initialUpdate">The initial update values, this is the amount that the deltas
        /// are all initially set to.</param>
        /// <param name="maxStep">The maximum that a delta can reach.</param>
        public ResilientPropagation(BasicNetwork network,
                 INeuralDataSet training, OpenCLTrainingProfile profile,
                 double initialUpdate, double maxStep)
            : base(network, training)
        {
            if (profile == null)
            {
                TrainFlatNetworkResilient rpropFlat = new TrainFlatNetworkResilient(
                        network.Structure.Flat, this.Training);
                this.FlatTraining = rpropFlat;
            }
#if !SILVERLIGHT
            else
            {
                TrainFlatNetworkOpenCL rpropFlat = new TrainFlatNetworkOpenCL(
                        network.Structure.Flat, this.Training,
                        profile);
                rpropFlat.LearnRPROP(initialUpdate, maxStep);
                this.FlatTraining = rpropFlat;
            }
#endif
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

            if (this.FlatTraining is TrainFlatNetworkResilient)
            {
                result[ResilientPropagation.LAST_GRADIENTS] =
                        ((TrainFlatNetworkResilient)this.FlatTraining)
                                .LastGradient;
                result[ResilientPropagation.UPDATE_VALUES] =
                        ((TrainFlatNetworkResilient)this.FlatTraining)
                                .UpdateValues;
            }
#if !SILVERLIGHT
            else
            {
                result[ResilientPropagation.LAST_GRADIENTS] =
                        ((TrainFlatNetworkOpenCL)this.FlatTraining)
                                .LastGradient;
                result[ResilientPropagation.UPDATE_VALUES] =
                        ((TrainFlatNetworkOpenCL)this.FlatTraining)
                                .UpdateValues;
            }
#endif

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
            double[] lastGradient = (double[])state
                    [ResilientPropagation.LAST_GRADIENTS];
            double[] updateValues = (double[])state
                    [ResilientPropagation.UPDATE_VALUES];

            if (this.FlatTraining is TrainFlatNetworkResilient)
            {
                EngineArray.ArrayCopy(lastGradient,
                        ((TrainFlatNetworkResilient)this.FlatTraining)
                                .LastGradient);
                EngineArray.ArrayCopy(updateValues,
                        ((TrainFlatNetworkResilient)this.FlatTraining)
                                .UpdateValues);
            }
#if !SILVERLIGHT
            else if (this.FlatTraining is TrainFlatNetworkOpenCL)
            {
                EngineArray.ArrayCopy(lastGradient, ((TrainFlatNetworkOpenCL)this
                        .FlatTraining).LastGradient);
                EngineArray.ArrayCopy(updateValues, ((TrainFlatNetworkOpenCL)this
                        .FlatTraining).UpdateValues);
            }
#endif
        }
    }
}
