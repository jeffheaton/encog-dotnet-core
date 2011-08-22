using System;
using Encog.ML.Data;
using Encog.Neural.Flat.Train.Prop;
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

        /**
* 
* 
* @param network
*            
* @param training
*            
* @param theLearningRate
*            
*/


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
            var backFlat = new TrainFlatNetworkQPROP(
                network.Flat, Training, learnRate);
            FlatTraining = backFlat;
        }

        /// <inheritdoc />
        public override bool CanContinue
        {
            get { return true; }
        }

        /// <summary>
        /// The last delta values.
        /// </summary>
        public double[] LastDelta
        {
            get { return ((TrainFlatNetworkQPROP) FlatTraining).LastDelta; }
        }

        /// <summary>
        /// The output epsilon.
        /// </summary>
        public double OutputEpsilon
        {
            get
            {
                return ((TrainFlatNetworkQPROP) FlatTraining)
                    .OutputEpsilon;
            }
            set
            {
                ((TrainFlatNetworkQPROP) FlatTraining)
                    .OutputEpsilon = value;
            }
        }

        /// <summary>
        /// Shrink.
        /// </summary>
        public double Shrink
        {
            get
            {
                return ((TrainFlatNetworkQPROP) FlatTraining)
                    .Shrink;
            }
            set
            {
                ((TrainFlatNetworkQPROP) FlatTraining)
                    .Shrink = value;
            }
        }
        
        #region ILearningRate Members

        /// <summary>
        /// The learning rate, this is value is essentially a percent. It is
	    ///         the degree to which the gradients are applied to the weight
	    ///         matrix to allow learning.
        /// </summary>
        public double LearningRate
        {
            get
            {
                return ((TrainFlatNetworkQPROP) FlatTraining)
                    .LearningRate;
            }
            set
            {
                ((TrainFlatNetworkQPROP) FlatTraining)
                    .LearningRate = value;
            }
        }

        #endregion

       
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
            var qprop = (TrainFlatNetworkQPROP) FlatTraining;
            double[] d = qprop.LastGradient;
            result.Contents[LastGradients] = d;
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

            EngineArray.ArrayCopy(lastGradient,
                                  ((TrainFlatNetworkQPROP) FlatTraining).LastGradient);
        }
    }
}