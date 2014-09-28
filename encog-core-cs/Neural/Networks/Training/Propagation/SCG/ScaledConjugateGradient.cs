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
using Encog.Util;

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
        /// The starting value for sigma.
        /// </summary>
        ///
        protected internal const double FirstSigma = 1.0E-4D;

        /// <summary>
        /// The starting value for lambda.
        /// </summary>
        ///
        protected internal const double FirstLambda = 1.0E-6D;

        /// <summary>
        /// The old gradients, used to compare.
        /// </summary>
        ///
        private readonly double[] _oldGradient;

        /// <summary>
        /// The old weight values, used to restore the neural network.
        /// </summary>
        ///
        private readonly double[] _oldWeights;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        ///
        private readonly double[] _p;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        ///
        private readonly double[] _r;

        /// <summary>
        /// The neural network weights.
        /// </summary>
        ///
        private readonly double[] _weights;

        /// <summary>
        /// The current delta.
        /// </summary>
        ///
        private double _delta;

        /// <summary>
        /// The number of iterations. The network will reset when this value
        /// increases over the number of weights in the network.
        /// </summary>
        ///
        private int _k;

        /// <summary>
        /// The first lambda value.
        /// </summary>
        ///
        private double _lambda;

        /// <summary>
        /// The second lambda value.
        /// </summary>
        ///
        private double _lambda2;

        /// <summary>
        /// The magnitude of p.
        /// </summary>
        ///
        private double _magP;

        /// <summary>
        /// Should the initial gradients be calculated.
        /// </summary>
        ///
        private bool _mustInit;

        /// <summary>
        /// The old error value, used to make sure an improvement happened.
        /// </summary>
        ///
        private double _oldError;

        /// <summary>
        /// Should we restart?
        /// </summary>
        ///
        private bool _restart;

        /// <summary>
        /// Tracks if the latest training cycle was successful.
        /// </summary>
        ///
        private bool _success;


        /// <summary>
        /// Construct a training class.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public ScaledConjugateGradient(IContainsFlat network,
                                       IMLDataSet training) : base(network, training)
        {
            _success = true;

            _success = true;
            _delta = 0;
            _lambda2 = 0;
            _lambda = FirstLambda;
            _oldError = 0;
            _magP = 0;
            _restart = false;

            _weights = EngineArray.ArrayCopy(network.Flat.Weights);
            int numWeights = _weights.Length;

            _oldWeights = new double[numWeights];
            _oldGradient = new double[numWeights];

            _p = new double[numWeights];
            _r = new double[numWeights];

            _mustInit = true;

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

        /// <summary>
        /// Calculate the gradients. They are normalized as well.
        /// </summary>
        ///
        public override void CalculateGradients()
        {
            int outCount = Network.Flat.OutputCount;

            base.CalculateGradients();

            // normalize

            double factor = -2D / Gradients.Length / outCount;

            for (int i = 0; i < Gradients.Length; i++)
            {
                Gradients[i] *= factor;
            }
        }

        /// <summary>
        /// Calculate the starting set of gradients.
        /// </summary>
        ///
        private void Init()
        {
            int numWeights = _weights.Length;

            CalculateGradients();

            _k = 1;

            for (int i = 0; i < numWeights; ++i)
            {
                _p[i] = _r[i] = -Gradients[i];
            }

            _mustInit = false;
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            if (_mustInit)
            {
                Init();
            }

            base.PreIteration();

            RollIteration();
            int numWeights = _weights.Length;
            // Storage space for previous iteration values.

            if (_restart)
            {
                // First time through, set initial values for SCG parameters.
                _lambda = FirstLambda;
                _lambda2 = 0;
                _k = 1;
                _success = true;
                _restart = false;
            }

            // If an error reduction is possible, calculate 2nd order info.
            if (_success)
            {
                // If the search direction is small, stop.
                _magP = EngineArray.VectorProduct(_p, _p);

                double sigma = FirstSigma
                               / Math.Sqrt(_magP);

                // In order to compute the new step, we need a new gradient.
                // First, save off the old data.
                EngineArray.ArrayCopy(Gradients, _oldGradient);
                EngineArray.ArrayCopy(_weights, _oldWeights);
                _oldError = Error;

                // Now we move to the new point in weight space.
                for (int i = 0; i < numWeights; ++i)
                {
                    _weights[i] += sigma * _p[i];
                }

                EngineArray.ArrayCopy(_weights, Network.Flat.Weights);

                // And compute the new gradient.
                CalculateGradients();

                // Now we have the new gradient, and we continue the step
                // computation.
                _delta = 0;
                for (int i = 0; i < numWeights; ++i)
                {
                    double step = (Gradients[i] - _oldGradient[i])
                                  / sigma;
                    _delta += _p[i] * step;
                }
            }

            // Scale delta.
            _delta += (_lambda - _lambda2) * _magP;

            // If delta <= 0, make Hessian positive definite.
            if (_delta <= 0)
            {
                _lambda2 = 2 * (_lambda - _delta / _magP);
                _delta = _lambda * _magP - _delta;
                _lambda = _lambda2;
            }

            // Calculate step size.
            double mu = EngineArray.VectorProduct(_p, _r);
            double alpha = mu / _delta;

            // Calculate the comparison parameter.
            // We must compute a new gradient, but this time we do not
            // want to keep the old values. They were useful only for
            // approximating the Hessian.
            for (int i = 0; i < numWeights; ++i)
            {
                _weights[i] = _oldWeights[i] + alpha * _p[i];
            }

            EngineArray.ArrayCopy(_weights, Network.Flat.Weights);

            CalculateGradients();

            double gdelta = 2 * _delta * (_oldError - Error)
                            / (mu * mu);

            // If gdelta >= 0, a successful reduction in error is possible.
            if (gdelta >= 0)
            {
                // Product of r(k+1) by r(k)
                double rsum = 0;

                // Now r = r(k+1).
                for (int i = 0; i < numWeights; ++i)
                {
                    double tmp = -Gradients[i];
                    rsum += tmp * _r[i];
                    _r[i] = tmp;
                }
                _lambda2 = 0;
                _success = true;

                // Do we need to restart?
                if (_k >= numWeights)
                {
                    _restart = true;
                    EngineArray.ArrayCopy(_r, _p);
                }
                else
                {
                    // Compute new conjugate direction.
                    double beta = (EngineArray.VectorProduct(_r, _r) - rsum)
                                  / mu;

                    // Update direction vector.
                    for (int i = 0; i < numWeights; ++i)
                    {
                        _p[i] = _r[i] + beta * _p[i];
                    }

                    _restart = false;
                }

                if (gdelta >= 0.75D)
                {
                    _lambda *= 0.25D;
                }
            }
            else
            {
                // A reduction in error was not possible.
                // under_tolerance = false;

                // Go back to w(k) since w(k) + alpha*p(k) is not better.
                EngineArray.ArrayCopy(_oldWeights, _weights);
                Error = _oldError;
                _lambda2 = _lambda;
                _success = false;
            }

            if (gdelta < 0.25D)
            {
                _lambda += _delta * (1 - gdelta) / _magP;
            }

            _lambda = BoundNumbers.Bound(_lambda);

            ++_k;

            EngineArray.ArrayCopy(_weights, Network.Flat.Weights);

            base.PostIteration();
        }

        /// <summary>
        /// Update the weights.
        /// </summary>
        ///
        /// <param name="gradients">The current gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The weight index being updated.</param>
        /// <returns>The new weight value.</returns>
        public override double UpdateWeight(double[] gradients,
                                            double[] lastGradient, int index)
        {
            return 0;
        }

        /// <summary>
        /// Not needed for this training type.
        /// </summary>
        public override void InitOthers()
        {
        }

    }
}
