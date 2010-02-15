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
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Layers;
using Encog.Util;
using Encog.Util.MathUtil;

namespace Encog.Neural.Networks.Training.Propagation.SCG
{
    /// <summary>
    ///  This is a training class that makes use of scaled conjugate 
    /// gradient methods.  It is a very fast and efficient training
    /// algorithm.
    /// </summary>
    public class ScaledConjugateGradient : Propagation
    {
        /// <summary>
        /// The starting value for sigma.
        /// </summary>
        protected const double FIRST_SIGMA = 1E-4;

        /// <summary>
        /// The starting value for lambda.
        /// </summary>
        protected const double FIRST_LAMBDA = 1E-6;

        /// <summary>
        /// Should we restart?
        /// </summary>
        private bool restart;

        /// <summary>
        /// The second lambda value.
        /// </summary>
        private double lambda2;

        /// <summary>
        /// The first lambda value.
        /// </summary>
        private double lambda;

        /// <summary>
        /// The number of iterations.  The network will reset when this value
        /// increases over the number of weights in the network.
        /// </summary>
        private int k;

        /// <summary>
        /// Success?
        /// </summary>
        private bool success = true;

        /// <summary>
        /// The magnitude of p.
        /// </summary>
        private double magP;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        private double[] p;

        /// <summary>
        /// Step direction vector.
        /// </summary>
        private double[] r;

        /// <summary>
        /// The neural network weights.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The gradients after the training cycles.
        /// </summary>
        private double[] gradient;

        /// <summary>
        /// The current delta.
        /// </summary>
        private double delta;

        /// <summary>
        /// The old error value, used to make sure an improvement happened.
        /// </summary>
        private double oldError;

        /// <summary>
        /// The old weight values, used to restore the neural network.
        /// </summary>
        private double[] oldWeights;

        /// <summary>
        /// The old gradients, used to compare.
        /// </summary>
        private double[] oldGradient;

        /// <summary>
        /// Construct a training class.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public ScaledConjugateGradient(BasicNetwork network,
                INeuralDataSet training)
            : base(network, training)
        {

            this.success = true;
            this.delta = 0;
            this.lambda2 = 0;
            this.lambda = ScaledConjugateGradient.FIRST_LAMBDA;
            this.oldError = 0;
            this.magP = 0;
            this.restart = false;

            this.weights = NetworkCODEC.NetworkToArray(Network);
            int numWeights = this.weights.Length;

            this.gradient = new double[numWeights];
            this.oldWeights = new double[numWeights];
            this.oldGradient = new double[numWeights];

            this.p = new double[numWeights];
            this.r = new double[numWeights];

            // Calculate the starting set of gradients.
            this.gradient = CalcGradients(this.weights);

            this.k = 1;

            for (int i = 0; i < numWeights; ++i)
            {
                this.p[i] = this.r[i] = -this.gradient[i];
            }

        }

        /// <summary>
        /// Calculate the gradients.  They are normalized as well.
        /// </summary>
        /// <param name="weights">The weights.</param>
        /// <returns>The gradients.</returns>
        private double[] CalcGradients(double[] weights)
        {

            ILayer output = Network.GetLayer(BasicNetwork.TAG_OUTPUT);
            int outCount = output.NeuronCount;

            CalculateGradient prop = new CalculateGradient(Network,
                   Training, NumThreads);
            prop.Calculate(weights);

            // normalize
            double[] d = prop.Gradients;

            double factor = -2D / prop.Count / outCount;

            for (int i = 0; i < d.Length; i++)
            {
                d[i] *= factor;
            }

            Error = prop.Error;
            return prop.Gradients;
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        public override void Iteration()
        {

            int numWeights = this.weights.Length;
            // Storage space for previous iteration values.

            if (this.restart)
            {
                // First time through, set initial values for SCG parameters.
                this.lambda = ScaledConjugateGradient.FIRST_LAMBDA;
                this.lambda2 = 0;
                this.k = 1;
                this.success = true;
                this.restart = false;
            }

            // If an error reduction is possible, calculate 2nd order info.
            if (this.success)
            {

                // If the search direction is small, stop.
                this.magP = EncogArray.VectorProduct(this.p, this.p);

                double sigma = ScaledConjugateGradient.FIRST_SIGMA
                        / Math.Sqrt(this.magP);

                // In order to compute the new step, we need a new gradient.
                // First, save off the old data.
                EncogArray.ArrayCopy(this.gradient, this.oldGradient);
                EncogArray.ArrayCopy(this.weights, this.oldWeights);
                this.oldError = Error;

                // Now we move to the new point in weight space.
                for (int i = 0; i < numWeights; ++i)
                {
                    this.weights[i] += sigma * this.p[i];
                }

                NetworkCODEC.ArrayToNetwork(this.weights, Network);

                // And compute the new gradient.
                this.gradient = CalcGradients(this.weights);

                // Now we have the new gradient, and we continue the step
                // computation.
                this.delta = 0;
                for (int i = 0; i < numWeights; ++i)
                {
                    double step = (this.gradient[i] - this.oldGradient[i])
                            / sigma;
                    this.delta += this.p[i] * step;
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
            double mu = EncogArray.VectorProduct(this.p, this.r);
            double alpha = mu / this.delta;

            // Calculate the comparison parameter.
            // We must compute a new gradient, but this time we do not
            // want to keep the old values. They were useful only for
            // approximating the Hessian.
            for (int i = 0; i < numWeights; ++i)
            {
                this.weights[i] = this.oldWeights[i] + alpha * this.p[i];
            }

            NetworkCODEC.ArrayToNetwork(this.weights, Network);

            this.gradient = CalcGradients(this.weights);

            double gdelta = 2 * this.delta * (this.oldError - Error)
                    / (mu * mu);

            // If gdelta >= 0, a successful reduction in error is possible.
            if (gdelta >= 0)
            {
                // Product of r(k+1) by r(k)
                double rsum = 0;

                // Now r = r(k+1).
                for (int i = 0; i < numWeights; ++i)
                {
                    double tmp = -this.gradient[i];
                    rsum += tmp * this.r[i];
                    this.r[i] = tmp;
                }
                this.lambda2 = 0;
                this.success = true;

                // Do we need to restart?
                if (this.k >= numWeights)
                {
                    this.restart = true;
                    EncogArray.ArrayCopy(this.r, this.p);

                }
                else
                {
                    // Compute new conjugate direction.
                    double beta = (EncogArray.VectorProduct(this.r, this.r)
                            - rsum) / mu;

                    // Update direction vector.
                    for (int i = 0; i < numWeights; ++i)
                    {
                        this.p[i] = this.r[i] + beta * this.p[i];
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
                EncogArray.ArrayCopy(this.oldWeights, this.weights);
                Error = this.oldError;
                this.lambda2 = this.lambda;
                this.success = false;
            }

            if (gdelta < 0.25D)
            {
                this.lambda += this.delta * (1 - gdelta) / this.magP;
            }

            this.lambda = BoundNumbers.Bound(this.lambda);

            ++this.k;

            NetworkCODEC.ArrayToNetwork(this.weights, Network);
        }

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="prop">Not used.</param>
        /// <param name="weights">Not used.</param>
        public override void PerformIteration(CalculateGradient prop,
                double[] weights)
        {
        }
    }
}
