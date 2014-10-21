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
    /// For most training situations, we suggest that the resilient propagation
    /// algorithm (this class) be used for training.
    /// There are a total of three parameters that must be provided to the resilient
    /// training algorithm. Defaults are provided for each, and in nearly all cases,
    /// these defaults are acceptable. This makes the resilient propagation algorithm
    /// one of the easiest and most efficient training algorithms available.
    /// The optional parameters are:
    /// zeroTolerance - How close to zero can a number be to be considered zero. The
    /// default is 0.00000000000000001.
    /// initialUpdate - What are the initial update values for each matrix value. The
    /// default is 0.1.
    /// maxStep - What is the largest amount that the update values can step. The
    /// default is 50.
    /// Usually you will not need to use these, and you should use the constructor
    /// that does not require them.
    /// </summary>
    ///
    public class ResilientPropagation : Propagation
    {
        /// <summary>
        /// Continuation tag for the last gradients.
        /// </summary>
        ///
        public const String LastGradientsConst = "LAST_GRADIENTS";

        /// <summary>
        /// Continuation tag for the last values.
        /// </summary>
        ///
        public const String UpdateValuesConst = "UPDATE_VALUES";

        /// <summary>
        /// The last deltas.
        /// </summary>
        private readonly double[] _lastDelta;

        /// <summary>
        /// The last weight changed.
        /// </summary>
        private readonly double[] _lastWeightChanged;

        /// <summary>
        /// The maximum step value for rprop.
        /// </summary>
        ///
        private readonly double _maxStep;

        /// <summary>
        /// The update values, for the weights and thresholds.
        /// </summary>
        ///
        private readonly double[] _updateValues;

        /// <summary>
        /// The zero tolerance.
        /// </summary>
        ///
        private readonly double _zeroTolerance;

        /// <summary>
        /// The last error.
        /// </summary>
        private double _lastError;


        /// <summary>
        /// Construct an RPROP trainer, allows an OpenCL device to be specified. Use
        /// the defaults for all training parameters. Usually this is the constructor
        /// to use as the resilient training algorithm is designed for the default
        /// parameters to be acceptable for nearly all problems.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        public ResilientPropagation(IContainsFlat network,
                                    IMLDataSet training)
            : this(network, training, RPROPConst.DefaultInitialUpdate, RPROPConst.DefaultMaxStep)
        {
        }

        /// <summary>
        /// Construct a resilient training object, allow the training parameters to
        /// be specified. Usually the default parameters are acceptable for the
        /// resilient training algorithm. Therefore you should usually use the other
        /// constructor, that makes use of the default values.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        /// <param name="initialUpdate"></param>
        /// <param name="maxStep">The maximum that a delta can reach.</param>
        public ResilientPropagation(IContainsFlat network,
                                    IMLDataSet training, double initialUpdate,
                                    double maxStep) : base(network, training)
        {
            _updateValues = new double[network.Flat.Weights.Length];
            _zeroTolerance = RPROPConst.DefaultZeroTolerance;
            _maxStep = maxStep;
            _lastWeightChanged = new double[Network.Flat.Weights.Length];
            _lastDelta = new double[Network.Flat.Weights.Length];

            for (int i = 0; i < _updateValues.Length; i++)
            {
                _updateValues[i] = initialUpdate;
            }
        }


        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return true; }
        }

        /// <summary>
        /// Determine if the specified continuation object is valid to resume with.
        /// </summary>
        ///
        /// <param name="state">The continuation object to check.</param>
        /// <returns>True if the specified continuation object is valid for this
        /// training method and network.</returns>
        public bool IsValidResume(TrainingContinuation state)
        {
            if (!state.Contents.ContainsKey(
                LastGradientsConst)
                || !state.Contents.ContainsKey(
                    UpdateValuesConst))
            {
                return false;
            }

            if (!state.TrainingType.Equals(GetType().Name))
            {
                return false;
            }

            var d = (double[]) state.Get(LastGradientsConst);
            return d.Length == Network.Flat.Weights.Length;
        }

        /// <summary>
        /// Pause the training.
        /// </summary>
        ///
        /// <returns>A training continuation object to continue with.</returns>
        public override sealed TrainingContinuation Pause()
        {
            var result = new TrainingContinuation();

            result.TrainingType = GetType().Name;

            result.Set(LastGradientsConst,LastGradient);
            result.Set(UpdateValuesConst,_updateValues);

            return result;
        }

        /// <summary>
        /// Resume training.
        /// </summary>
        ///
        /// <param name="state">The training state to return to.</param>
        public override sealed void Resume(TrainingContinuation state)
        {
            if (!IsValidResume(state))
            {
                throw new TrainingError("Invalid training resume data length");
            }
            var lastGradient = (double[]) state.Get(LastGradientsConst);
            var updateValues = (double[]) state.Get(UpdateValuesConst);

            EngineArray.ArrayCopy(lastGradient,LastGradient);
            EngineArray.ArrayCopy(updateValues,_updateValues);
        }

        /// <summary>
        /// The type of RPROP to use.
        /// </summary>
        public RPROPType RType { get; set; }

        /// <value>The RPROP update values.</value>
        public double[] UpdateValues
        {
            get { return _updateValues; }
        }

        /// <summary>
        /// Determine the sign of the value.
        /// </summary>
        ///
        /// <param name="v">The value to check.</param>
        /// <returns>-1 if less than zero, 1 if greater, or 0 if zero.</returns>
        private int Sign(double v)
        {
            if (Math.Abs(v) < _zeroTolerance)
            {
                return 0;
            }
            if (v > 0)
            {
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// Calculate the amount to change the weight by.
        /// </summary>
        ///
        /// <param name="gradients">The gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index to update.</param>
        /// <returns>The amount to change the weight by.</returns>
        public override double UpdateWeight(double[] gradients,
                                            double[] lastGradient, int index)
        {
            double weightChange = 0;

            switch (RType)
            {
                case RPROPType.RPROPp:
                    weightChange = UpdateWeightPlus(gradients, lastGradient, index);
                    break;
                case RPROPType.RPROPm:
                    weightChange = UpdateWeightMinus(gradients, lastGradient, index);
                    break;
                case RPROPType.iRPROPp:
                    weightChange = UpdateiWeightPlus(gradients, lastGradient, index);
                    break;
                case RPROPType.iRPROPm:
                    weightChange = UpdateiWeightMinus(gradients, lastGradient, index);
                    break;
                default:
                    throw new TrainingError("Unknown RPROP type: " + RType);
            }

            _lastWeightChanged[index] = weightChange;
            return weightChange;
        }


        public double UpdateWeightPlus(double[] gradients,
                                       double[] lastGradient, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index] * lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = UpdateValues[index]
                               * RPROPConst.PositiveEta;
                delta = Math.Min(delta, _maxStep);
                weightChange = Sign(gradients[index]) * delta;
                UpdateValues[index] = delta;
                lastGradient[index] = gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = UpdateValues[index]
                               * RPROPConst.NegativeEta;
                delta = Math.Max(delta, RPROPConst.DeltaMin);
                UpdateValues[index] = delta;
                weightChange = -_lastWeightChanged[index];
                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = _updateValues[index];
                weightChange = Sign(gradients[index]) * delta;
                lastGradient[index] = gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }

        public double UpdateWeightMinus(double[] gradients,
                                        double[] lastGradient, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index] * lastGradient[index]);
            double weightChange = 0;
            double delta;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                delta = _lastDelta[index]
                        * RPROPConst.PositiveEta;
                delta = Math.Min(delta, _maxStep);
            }
            else
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                delta = _lastDelta[index]
                        * RPROPConst.NegativeEta;
                delta = Math.Max(delta, RPROPConst.DeltaMin);
            }

            lastGradient[index] = gradients[index];
            weightChange = Sign(gradients[index]) * delta;
            _lastDelta[index] = delta;

            // apply the weight change, if any
            return weightChange;
        }

        public double UpdateiWeightPlus(double[] gradients,
                                        double[] lastGradient, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index] * lastGradient[index]);
            double weightChange = 0;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                double delta = _updateValues[index]
                               * RPROPConst.PositiveEta;
                delta = Math.Min(delta, _maxStep);
                weightChange = Sign(gradients[index]) * delta;
                _updateValues[index] = delta;
                lastGradient[index] = gradients[index];
            }
            else if (change < 0)
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                double delta = UpdateValues[index]
                               * RPROPConst.NegativeEta;
                delta = Math.Max(delta, RPROPConst.DeltaMin);
                UpdateValues[index] = delta;

                if (Error > _lastError)
                {
                    weightChange = -_lastWeightChanged[index];
                }

                // set the previous gradent to zero so that there will be no
                // adjustment the next iteration
                lastGradient[index] = 0;
            }
            else if (change == 0)
            {
                // if change==0 then there is no change to the delta
                double delta = _updateValues[index];
                weightChange = Sign(gradients[index]) * delta;
                lastGradient[index] = gradients[index];
            }

            // apply the weight change, if any
            return weightChange;
        }

        public double UpdateiWeightMinus(double[] gradients,
                                         double[] lastGradient, int index)
        {
            // multiply the current and previous gradient, and take the
            // sign. We want to see if the gradient has changed its sign.
            int change = Sign(gradients[index] * lastGradient[index]);
            double weightChange = 0;
            double delta;

            // if the gradient has retained its sign, then we increase the
            // delta so that it will converge faster
            if (change > 0)
            {
                delta = _lastDelta[index]
                        * RPROPConst.PositiveEta;
                delta = Math.Min(delta, _maxStep);
            }
            else
            {
                // if change<0, then the sign has changed, and the last
                // delta was too big
                delta = _lastDelta[index]
                        * RPROPConst.NegativeEta;
                delta = Math.Max(delta, RPROPConst.DeltaMin);
                lastGradient[index] = 0;
            }

            lastGradient[index] = gradients[index];
            weightChange = Sign(gradients[index]) * delta;
            _lastDelta[index] = delta;

            // apply the weight change, if any
            return weightChange;
        }


        /// <summary>
        /// Not needed for this training type.
        /// </summary>
        public override void InitOthers()
        {
        }

        public override void PostIteration()
        {
            _lastError = Error;
			
			// call the base method to apply strategies
			base.PostIteration();
        }
    }
}
