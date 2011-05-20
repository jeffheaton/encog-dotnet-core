//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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

namespace Encog.Neural.Flat.Train.Prop
{
    /// <summary>
    /// Train a network using scaled conjugate gradient.
    /// </summary>
    ///
    public class TrainFlatNetworkSCG : TrainFlatNetworkProp
    {
        /// <summary>
        /// The starting value for sigma.
        /// </summary>
        ///
        protected internal const double FIRST_SIGMA = 1.0E-4D;

        /// <summary>
        /// The starting value for lambda.
        /// </summary>
        ///
        protected internal const double FIRST_LAMBDA = 1.0E-6D;

        /// <summary>
        /// The old gradients, used to compare.
        /// </summary>
        ///
        private readonly double[] oldGradient;

        /// <summary>
        /// The old weight values, used to restore the neural network.
        /// </summary>
        ///
        private readonly double[] oldWeights;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        ///
        private readonly double[] p;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        ///
        private readonly double[] r;

        /// <summary>
        /// The neural network weights.
        /// </summary>
        ///
        private readonly double[] weights;

        /// <summary>
        /// The current delta.
        /// </summary>
        ///
        private double delta;

        /// <summary>
        /// The number of iterations. The network will reset when this value
        /// increases over the number of weights in the network.
        /// </summary>
        ///
        private int k;

        /// <summary>
        /// The first lambda value.
        /// </summary>
        ///
        private double lambda;

        /// <summary>
        /// The second lambda value.
        /// </summary>
        ///
        private double lambda2;

        /// <summary>
        /// The magnitude of p.
        /// </summary>
        ///
        private double magP;

        /// <summary>
        /// Should the initial gradients be calculated.
        /// </summary>
        ///
        private bool mustInit;

        /// <summary>
        /// The old error value, used to make sure an improvement happened.
        /// </summary>
        ///
        private double oldError;

        /// <summary>
        /// Should we restart?
        /// </summary>
        ///
        private bool restart;

        /// <summary>
        /// Tracks if the latest training cycle was successful.
        /// </summary>
        ///
        private bool success;

        /// <summary>
        /// Construct the training object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        public TrainFlatNetworkSCG(FlatNetwork network,
                                   MLDataSet training) : base(network, training)
        {
            success = true;

            success = true;
            delta = 0;
            lambda2 = 0;
            lambda = FIRST_LAMBDA;
            oldError = 0;
            magP = 0;
            restart = false;

            weights = EngineArray.ArrayCopy(network.Weights);
            int numWeights = weights.Length;

            oldWeights = new double[numWeights];
            oldGradient = new double[numWeights];

            p = new double[numWeights];
            r = new double[numWeights];

            mustInit = true;
        }

        /// <summary>
        /// Calculate the gradients. They are normalized as well.
        /// </summary>
        ///
        public override void CalculateGradients()
        {
            int outCount = Network.OutputCount;

            base.CalculateGradients();

            // normalize

            double factor = -2D/gradients.Length/outCount;

            for (int i = 0; i < gradients.Length; i++)
            {
                gradients[i] *= factor;
            }
        }

        /// <summary>
        /// Calculate the starting set of gradients.
        /// </summary>
        ///
        private void Init()
        {
            int numWeights = weights.Length;

            CalculateGradients();

            k = 1;

            for (int i = 0; i < numWeights; ++i)
            {
                p[i] = r[i] = -gradients[i];
            }

            mustInit = false;
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            if (mustInit)
            {
                Init();
            }
            int numWeights = weights.Length;
            // Storage space for previous iteration values.

            if (restart)
            {
                // First time through, set initial values for SCG parameters.
                lambda = FIRST_LAMBDA;
                lambda2 = 0;
                k = 1;
                success = true;
                restart = false;
            }

            // If an error reduction is possible, calculate 2nd order info.
            if (success)
            {
                // If the search direction is small, stop.
                magP = EngineArray.VectorProduct(p, p);

                double sigma = FIRST_SIGMA
                               /Math.Sqrt(magP);

                // In order to compute the new step, we need a new gradient.
                // First, save off the old data.
                EngineArray.ArrayCopy(gradients, oldGradient);
                EngineArray.ArrayCopy(weights, oldWeights);
                oldError = Error;

                // Now we move to the new point in weight space.
                for (int i = 0; i < numWeights; ++i)
                {
                    weights[i] += sigma*p[i];
                }

                EngineArray.ArrayCopy(weights, network.Weights);

                // And compute the new gradient.
                CalculateGradients();

                // Now we have the new gradient, and we continue the step
                // computation.
                delta = 0;
                for (int i_0 = 0; i_0 < numWeights; ++i_0)
                {
                    double step = (gradients[i_0] - oldGradient[i_0])
                                  /sigma;
                    delta += p[i_0]*step;
                }
            }

            // Scale delta.
            delta += (lambda - lambda2)*magP;

            // If delta <= 0, make Hessian positive definite.
            if (delta <= 0)
            {
                lambda2 = 2*(lambda - delta/magP);
                delta = lambda*magP - delta;
                lambda = lambda2;
            }

            // Calculate step size.
            double mu = EngineArray.VectorProduct(p, r);
            double alpha = mu/delta;

            // Calculate the comparison parameter.
            // We must compute a new gradient, but this time we do not
            // want to keep the old values. They were useful only for
            // approximating the Hessian.
            for (int i_1 = 0; i_1 < numWeights; ++i_1)
            {
                weights[i_1] = oldWeights[i_1] + alpha*p[i_1];
            }

            EngineArray.ArrayCopy(weights, network.Weights);

            CalculateGradients();

            double gdelta = 2*delta*(oldError - Error)
                            /(mu*mu);

            // If gdelta >= 0, a successful reduction in error is possible.
            if (gdelta >= 0)
            {
                // Product of r(k+1) by r(k)
                double rsum = 0;

                // Now r = r(k+1).
                for (int i_2 = 0; i_2 < numWeights; ++i_2)
                {
                    double tmp = -gradients[i_2];
                    rsum += tmp*r[i_2];
                    r[i_2] = tmp;
                }
                lambda2 = 0;
                success = true;

                // Do we need to restart?
                if (k >= numWeights)
                {
                    restart = true;
                    EngineArray.ArrayCopy(r, p);
                }
                else
                {
                    // Compute new conjugate direction.
                    double beta = (EngineArray.VectorProduct(r, r) - rsum)
                                  /mu;

                    // Update direction vector.
                    for (int i_3 = 0; i_3 < numWeights; ++i_3)
                    {
                        p[i_3] = r[i_3] + beta*p[i_3];
                    }

                    restart = false;
                }

                if (gdelta >= 0.75D)
                {
                    lambda *= 0.25D;
                }
            }
            else
            {
                // A reduction in error was not possible.
                // under_tolerance = false;

                // Go back to w(k) since w(k) + alpha*p(k) is not better.
                EngineArray.ArrayCopy(oldWeights, weights);
                currentError = oldError;
                lambda2 = lambda;
                success = false;
            }

            if (gdelta < 0.25D)
            {
                lambda += delta*(1 - gdelta)/magP;
            }

            lambda = BoundNumbers.Bound(lambda);

            ++k;

            EngineArray.ArrayCopy(weights, network.Weights);
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
    }
}
