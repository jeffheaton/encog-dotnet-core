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
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Data;
using Encog.MathUtil.Matrices;
using Encog.Neural.NeuralData;
using Encog.MathUtil.Matrices.Decomposition;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Networks.Training.LMA
{
    /// <summary>
    /// Trains a neural network using a Levenberg Marquardt algorithm (LMA). This
    /// training technique is based on the mathematical technique of the same name. ì
    /// 
    /// http://en.wikipedia.org/wiki/Levenberg%E2%80%93Marquardt_algorithm
    /// 
    /// The LMA training technique has some important limitations that you should be
    /// aware of, before using it.
    /// 
    /// Only neural networks that have a single output neuron can be used with this
    /// training technique.
    /// 
    /// The entire training set must be loaded into memory. Because of this an
    /// Indexable training set must be used.
    /// 
    /// However, despite these limitations, the LMA training technique can be a very
    /// effective training method.    
    /// 
    /// References: 
    /// - http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf
    /// - http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html
    /// -------------------------------------------------------------------------------
    /// This implementation of the Levenberg Marquardt algorithm is based heavily on code
    /// published in an article by César Roberto de Souza.  The original article can be
    /// found here:
    /// 
    /// http://crsouza.blogspot.com/2009/11/neural-network-learning-by-levenberg_18.html
    /// 
    /// Portions of this class are under the following copyright/license.
    /// Copyright 2009 by César Roberto de Souza, Released under the LGPL.
    /// 
    /// </summary>
    public class LevenbergMarquardtTraining : BasicTraining
    {
        /// <summary>
        /// The amount to scale the lambda by.
        /// </summary>
        public const double SCALE_LAMBDA = 10.0;

        /// <summary>
        /// The max amount for the LAMBDA.
        /// </summary>
        public const double LAMBDA_MAX = 1e25;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The training set that we are using to train.
        /// </summary>
        private IIndexable indexableTraining;

        /// <summary>
        /// The training set length.
        /// </summary>
        private int trainingLength;

        /// <summary>
        /// The number of "parameters" in the LMA algorithm. The parameters are what
        /// the LMA adjusts to achieve the desired outcome. For neural network
        /// optimization, the parameters are the weights and thresholds.
        /// </summary>
        private int parametersLength;

        /// <summary>
        /// The neural network weights and threshold values.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The "hessian" matrix, used by the LMA.
        /// </summary>
        private Matrix hessianMatrix;

        /// <summary>
        /// The "hessian" matrix as a 2d array.
        /// </summary>
        private double[][] hessian;

        /// <summary>
        /// The alpha is multiplied by sum squared of weights. This scales the effect
        /// that the sum squared of the weights has.
        /// </summary>
        private double alpha;

        /// <summary>
        /// The beta is multiplied by the sum squared of the errors.
        /// </summary>
        private double beta;

        /// <summary>
        /// The lambda, or damping factor. This is increased until a desirable
        /// adjustment is found.
        /// </summary>
        private double lambda;

        /// <summary>
        /// The calculated gradients.
        /// </summary>
        private double[] gradient;

        /// <summary>
        /// The diagonal of the hessian.
        /// </summary>
        private double[] diagonal;

        /// <summary>
        /// The amount to change the weights by.
        /// </summary>
        private double[] deltas;

        private double gamma;

        /// <summary>
        /// The training elements.
        /// </summary>
        private INeuralDataPair pair;

        /// <summary>
        /// Should we use Bayesian regularization.
        /// </summary>
        private bool useBayesianRegularization;

        /// <summary>
        /// Construct the LMA object.
        /// </summary>
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public LevenbergMarquardtTraining(BasicNetwork network,
                INeuralDataSet training)
        {
            if (!(training is IIndexable))
            {
                throw new TrainingError(
                        "Levenberg Marquardt requires an indexable training set.");
            }

            ILayer outputLayer = network.GetLayer(BasicNetwork.TAG_OUTPUT);

            if (outputLayer == null)
            {
                throw new TrainingError(
                        "Levenberg Marquardt requires an output layer.");
            }

            if (outputLayer.NeuronCount != 1)
            {
                throw new TrainingError(
                        "Levenberg Marquardt requires an output layer with a single neuron.");
            }

            this.Training = training;
            this.indexableTraining = (IIndexable)Training;
            this.network = network;
            this.trainingLength = (int)this.indexableTraining.Count;
            this.parametersLength = this.network.Structure.CalculateSize();
            this.hessianMatrix = new Matrix(this.parametersLength,
                    this.parametersLength);
            this.hessian = this.hessianMatrix.Data;
            this.alpha = 0.0;
            this.beta = 1.0;
            this.lambda = 0.1;
            this.deltas = new double[this.parametersLength];
            this.gradient = new double[this.parametersLength];
            this.diagonal = new double[this.parametersLength];

            BasicNeuralData input = new BasicNeuralData(
                    this.indexableTraining.InputSize);
            BasicNeuralData ideal = new BasicNeuralData(
                    this.indexableTraining.IdealSize);
            this.pair = new BasicNeuralDataPair(input, ideal);
        }

        /// <summary>
        /// Calculate the Hessian matrix.
        /// </summary>
        /// <param name="jacobian">The Jacobian matrix.</param>
        /// <param name="errors">The errors.</param>
        public void CalculateHessian(double[][] jacobian,
                double[] errors)
        {
            for (int i = 0; i < this.parametersLength; i++)
            {
                // Compute Jacobian Matrix Errors
                double s = 0.0;
                for (int j = 0; j < this.trainingLength; j++)
                {
                    s += jacobian[j][i] * errors[j];
                }
                this.gradient[i] = s;

                // Compute Quasi-Hessian Matrix using Jacobian (H = J'J)
                for (int j = 0; j < this.parametersLength; j++)
                {
                    double c = 0.0;
                    for (int k = 0; k < this.trainingLength; k++)
                    {
                        c += jacobian[k][i] * jacobian[k][j];
                    }
                    this.hessian[i][j] = this.beta * c;
                }
            }

            for (int i = 0; i < this.parametersLength; i++)
            {
                this.diagonal[i] = this.hessian[i][i];
            }
        }

        /// <summary>
        /// Calculate the sum squared of the weights. 
        /// </summary>
        /// <returns>The sum squared of the weights.</returns>
        private double CalculateSumOfSquaredWeights()
        {
            double result = 0;

            foreach (double weight in this.weights)
            {
                result += weight * weight;
            }

            return result / 2.0;
        }

        /// <summary>
        /// The trained network.
        /// </summary>
        public override BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Return the sum of the diagonal. 
        /// </summary>
        /// <param name="m">The matrix to sum.</param>
        /// <returns>The trace of the matrix.</returns>
        public static double Trace(double[][] m)
        {
            double result = 0.0;
            for (int i = 0; i < m.Length; i++)
            {
                result += m[i][i];
            }
            return result;
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        public override void Iteration()
        {

            LUDecomposition decomposition = null;
            double trace = 0;

            PreIteration();

            this.weights = NetworkCODEC.NetworkToArray(this.network);

            IComputeJacobian j = new JacobianChainRule(this.network,
                    this.indexableTraining);

            double sumOfSquaredErrors = j.Calculate(this.weights);
            double sumOfSquaredWeights = CalculateSumOfSquaredWeights();

            // this.setError(j.getError());
            CalculateHessian(j.Jacobian, j.RowErrors);

            // Define the objective function
            // bayesian regularization objective function
            double objective = this.beta * sumOfSquaredErrors + this.alpha
                    * sumOfSquaredWeights;
            double current = objective + 1.0;

            // Start the main Levenberg-Macquardt method
            this.lambda /= LevenbergMarquardtTraining.SCALE_LAMBDA;

            // We'll try to find a direction with less error
            // (or where the objective function is smaller)
            while ((current >= objective)
                    && (this.lambda < LevenbergMarquardtTraining.LAMBDA_MAX))
            {
                this.lambda *= LevenbergMarquardtTraining.SCALE_LAMBDA;

                // Update diagonal (Levenberg-Marquardt formula)
                for (int i = 0; i < this.parametersLength; i++)
                {
                    this.hessian[i][i] = this.diagonal[i]
                            + (this.lambda + this.alpha);
                }

                // Decompose to solve the linear system
                decomposition = new LUDecomposition(
                        this.hessianMatrix);

                // Check if the Jacobian has become non-invertible
                if (!decomposition.IsNonsingular)
                {
                    continue;
                }

                // Solve using LU (or SVD) decomposition
                this.deltas = decomposition.Solve(this.gradient);

                // Update weights using the calculated deltas
                sumOfSquaredWeights = UpdateWeights();

                // Calculate the new error
                sumOfSquaredErrors = 0.0;
                for (int i = 0; i < this.trainingLength; i++)
                {
                    this.indexableTraining.GetRecord(i, this.pair);
                    INeuralData actual = this.network.Compute(this.pair
                            .Input);
                    double e = this.pair.Ideal[0]
                            - actual[0];
                    sumOfSquaredErrors += e * e;
                }
                sumOfSquaredErrors /= 2.0;

                // Update the objective function
                current = this.beta * sumOfSquaredErrors + this.alpha
                        * sumOfSquaredWeights;

                // If the object function is bigger than before, the method
                // is tried again using a greater dumping factor.
            }

            // If this iteration caused a error drop, then next iteration
            // will use a smaller damping factor.
            this.lambda /= LevenbergMarquardtTraining.SCALE_LAMBDA;

            if (useBayesianRegularization && decomposition != null)
            {
                // Compute the trace for the inverse Hessian
                trace = Trace(decomposition.Inverse());

                // Poland update's formula:
                gamma = this.parametersLength - (alpha * trace);
                alpha = this.parametersLength / (2.0 * sumOfSquaredWeights + trace);
                beta = Math.Abs((this.trainingLength - gamma) / (2.0 * sumOfSquaredErrors));
            }

            this.Error = sumOfSquaredErrors;

            PostIteration();
        }

        /// <summary>
        /// Update the weights. 
        /// </summary>
        /// <returns>The sum squared of the weights.</returns>
        public double UpdateWeights()
        {
            double result = 0;
            double[] w = (double[])this.weights.Clone();

            for (int i = 0; i < w.Length; i++)
            {
                w[i] += this.deltas[i];
                result += w[i] * w[i];
            }

            NetworkCODEC.ArrayToNetwork(w, this.network);

            return result / 2.0;
        }

        /// <summary>
        /// Should we use Bayesian regulation?
        /// </summary>
        public bool UseBayesianRegularization
        {
            get
            {
                return useBayesianRegularization;
            }
            set
            {
                this.useBayesianRegularization = value;
            }
        }
    }
}
