/*
 * Encog(tm) Core v2.5 - Java Version
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 
 * Copyright 2008-2010 Heaton Research, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *   
 * For more information on Heaton Research copyrights, licenses 
 * and trademarks visit:
 * http://www.heatonresearch.com/copyright
 */
namespace Encog.Engine.Network.Train.Prop
{
    using Encog.Engine.Network.Flat;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Encog.Engine.Util;
    using Encog.Neural.NeuralData;

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
        protected internal const double FIRST_SIGMA = 1.0e-4D;

        /// <summary>
        /// The starting value for lambda.
        /// </summary>
        ///
        protected internal const double FIRST_LAMBDA = 1.0e-6D;

        /// <summary>
        /// Should we restart?
        /// </summary>
        ///
        private bool restart;

        /// <summary>
        /// The second lambda value.
        /// </summary>
        ///
        private double lambda2;

        /// <summary>
        /// The first lambda value.
        /// </summary>
        ///
        private double lambda;

        /// <summary>
        /// The number of iterations. The network will reset when this value
        /// increases over the number of weights in the network.
        /// </summary>
        ///
        private int k;

        /// <summary>
        /// Tracks if the latest training cycle was successful.
        /// </summary>
        ///
        private bool success;

        /// <summary>
        /// The magnitude of p.
        /// </summary>
        ///
        private double magP;

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
        /// The old error value, used to make sure an improvement happened.
        /// </summary>
        ///
        private double oldError;

        /// <summary>
        /// The old weight values, used to restore the neural network.
        /// </summary>
        ///
        private readonly double[] oldWeights;

        /// <summary>
        /// The old gradients, used to compare.
        /// </summary>
        ///
        private readonly double[] oldGradient;

        /// <summary>
        /// Should the initial gradients be calculated.
        /// </summary>
        private bool shouldInit;

        /// <summary>
        /// Construct the training object.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        public TrainFlatNetworkSCG(FlatNetwork network,
                INeuralDataSet training)
            : base(network, training)
        {
            this.success = true;

            this.success = true;
            this.delta = 0;
            this.lambda2 = 0;
            this.lambda = TrainFlatNetworkSCG.FIRST_LAMBDA;
            this.oldError = 0;
            this.magP = 0;
            this.restart = false;

            this.weights = EngineArray.ArrayCopy(network.Weights);
            int numWeights = this.weights.Length;

            // this.gradients = new double[numWeights];
            this.oldWeights = new double[numWeights];
            this.oldGradient = new double[numWeights];

            this.p = new double[numWeights];
            this.r = new double[numWeights];
            this.shouldInit = true;
            
            
        }

        /// <summary>
        /// Calculate the starting set of gradients.
        /// </summary>
        private void Init()
        {
            int numWeights = this.weights.Length;
            CalculateGradients();

            this.k = 1;

            for (int i = 0; i < numWeights; ++i)
            {
                this.p[i] = this.r[i] = -this.gradients[i];
            }

            this.shouldInit = false;
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

            double factor = -2D / this.gradients.Length / outCount;

            for (int i = 0; i < this.gradients.Length; i++)
            {
                this.gradients[i] *= factor;
            }

        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            if (shouldInit)
            {
                Init();
            }

            int numWeights = this.weights.Length;
            // Storage space for previous iteration values.

            if (this.restart)
            {
                // First time through, set initial values for SCG parameters.
                this.lambda = TrainFlatNetworkSCG.FIRST_LAMBDA;
                this.lambda2 = 0;
                this.k = 1;
                this.success = true;
                this.restart = false;
            }

            // If an error reduction is possible, calculate 2nd order info.
            if (this.success)
            {

                // If the search direction is small, stop.
                this.magP = EngineArray.VectorProduct(this.p, this.p);

                double sigma = TrainFlatNetworkSCG.FIRST_SIGMA
                        / Math.Sqrt(this.magP);

                // In order to compute the new step, we need a new gradient.
                // First, save off the old data.
                EngineArray.ArrayCopy(this.gradients, this.oldGradient);
                EngineArray.ArrayCopy(this.weights, this.oldWeights);
                this.oldError = Error;

                // Now we move to the new point in weight space.
                for (int i = 0; i < numWeights; ++i)
                {
                    this.weights[i] += sigma * this.p[i];
                }

                EngineArray.ArrayCopy(this.weights, this.network.Weights);

                // And compute the new gradient.
                CalculateGradients();

                // Now we have the new gradient, and we continue the step
                // computation.
                this.delta = 0;
                for (int i_0 = 0; i_0 < numWeights; ++i_0)
                {
                    double step = (this.gradients[i_0] - this.oldGradient[i_0])
                            / sigma;
                    this.delta += this.p[i_0] * step;
                }
            }

            // Scale delta.
            this.delta += (this.lambda - this.lambda2) * this.magP;

            // If delta <= 0, make Hessian positive definite.
            if (this.delta <= 0)
            {
                this.lambda2 = 2 * (this.lambda - this.delta / this.magP);
                this.delta = this.lambda * this.magP - this.delta;
                this.lambda = this.lambda2;
            }

            // Calculate step size.
            double mu = EngineArray.VectorProduct(this.p, this.r);
            double alpha = mu / this.delta;

            // Calculate the comparison parameter.
            // We must compute a new gradient, but this time we do not
            // want to keep the old values. They were useful only for
            // approximating the Hessian.
            for (int i_1 = 0; i_1 < numWeights; ++i_1)
            {
                this.weights[i_1] = this.oldWeights[i_1] + alpha * this.p[i_1];
            }

            EngineArray.ArrayCopy(this.weights, this.network.Weights);

            CalculateGradients();

            double gdelta = 2 * this.delta * (this.oldError - Error)
                    / (mu * mu);

            // If gdelta >= 0, a successful reduction in error is possible.
            if (gdelta >= 0)
            {
                // Product of r(k+1) by r(k)
                double rsum = 0;

                // Now r = r(k+1).
                for (int i_2 = 0; i_2 < numWeights; ++i_2)
                {
                    double tmp = -this.gradients[i_2];
                    rsum += tmp * this.r[i_2];
                    this.r[i_2] = tmp;
                }
                this.lambda2 = 0;
                this.success = true;

                // Do we need to restart?
                if (this.k >= numWeights)
                {
                    this.restart = true;
                    EngineArray.ArrayCopy(this.r, this.p);

                }
                else
                {
                    // Compute new conjugate direction.
                    double beta = (EngineArray.VectorProduct(this.r, this.r) - rsum)
                            / mu;

                    // Update direction vector.
                    for (int i_3 = 0; i_3 < numWeights; ++i_3)
                    {
                        this.p[i_3] = this.r[i_3] + beta * this.p[i_3];
                    }

                    this.restart = false;
                }

                if (gdelta >= 0.75D)
                {
                    this.lambda *= 0.25D;
                }

            }
            else
            {
                // A reduction in error was not possible.
                // under_tolerance = false;

                // Go back to w(k) since w(k) + alpha*p(k) is not better.
                EngineArray.ArrayCopy(this.oldWeights, this.weights);
                this.currentError = this.oldError;
                this.lambda2 = this.lambda;
                this.success = false;
            }

            if (gdelta < 0.25D)
            {
                this.lambda += this.delta * (1 - gdelta) / this.magP;
            }

            this.lambda = BoundNumbers.Bound(this.lambda);

            ++this.k;

            EngineArray.ArrayCopy(this.weights, this.network.Weights);
        }

        /// <summary>
        /// Update the weights.
        /// </summary>
        /// <param name="gradients">The current gradients.</param>
        /// <param name="lastGradient">The last gradients.</param>
        /// <param name="index">The index.</param>
        /// <returns>The new weight.</returns>
        public override double UpdateWeight(double[] gradients,
                double[] lastGradient, int index)
        {
            return 0;
        }

    }
}
