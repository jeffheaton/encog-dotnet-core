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
using Encog.MathUtil.Matrices;
using Encog.MathUtil.Matrices.Decomposition;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util.Validate;

namespace Encog.Neural.Networks.Training.Lma
{
    /// <summary>
    /// Trains a neural network using a Levenberg Marquardt algorithm (LMA). This
    /// training technique is based on the mathematical technique of the same name.
    /// http://en.wikipedia.org/wiki/Levenberg%E2%80%93Marquardt_algorithm
    /// The LMA training technique has some important limitations that you should be
    /// aware of, before using it.
    /// Only neural networks that have a single output neuron can be used with this
    /// training technique.
    /// The entire training set must be loaded into memory. Because of this an
    /// Indexable training set must be used.
    /// However, despite these limitations, the LMA training technique can be a very
    /// effective training method.
    /// References: - http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf -
    /// http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html
    /// ----------------------------------------------------------------
    /// This implementation of the Levenberg Marquardt algorithm is based heavily on code
    /// published in an article by Cesar Roberto de Souza.  The original article can be
    /// found here:
    /// http://crsouza.blogspot.com/2009/11/neural-network-learning-by-levenberg_18.html
    /// Portions of this class are under the following copyright/license.
    /// Copyright 2009 by Cesar Roberto de Souza, Released under the LGPL.
    /// </summary>
    ///
    public class LevenbergMarquardtTraining : BasicTraining
    {
        /// <summary>
        /// The amount to scale the lambda by.
        /// </summary>
        ///
        public const double ScaleLambda = 10.0d;

        /// <summary>
        /// The max amount for the LAMBDA.
        /// </summary>
        ///
        public const double LambdaMax = 1e25d;

        /// <summary>
        /// The diagonal of the hessian.
        /// </summary>
        ///
        private readonly double[] _diagonal;

        /// <summary>
        /// The calculated gradients.
        /// </summary>
        ///
        private readonly double[] _gradient;

        /// <summary>
        /// The "hessian" matrix as a 2d array.
        /// </summary>
        ///
        private readonly double[][] _hessian;

        /// <summary>
        /// The "hessian" matrix, used by the LMA.
        /// </summary>
        ///
        private readonly Matrix _hessianMatrix;

        /// <summary>
        /// The training set that we are using to train.
        /// </summary>
        ///
        private readonly IMLDataSet _indexableTraining;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly BasicNetwork _network;

        /// <summary>
        /// The training elements.
        /// </summary>
        ///
        private readonly IMLDataPair _pair;

        /// <summary>
        /// The number of "parameters" in the LMA algorithm. The parameters are what
        /// the LMA adjusts to achieve the desired outcome. For neural network
        /// optimization, the parameters are the weights and bias values.
        /// </summary>
        ///
        private readonly int _parametersLength;

        /// <summary>
        /// The training set length.
        /// </summary>
        ///
        private readonly int _trainingLength;

        /// <summary>
        /// The alpha is multiplied by sum squared of weights. This scales the effect
        /// that the sum squared of the weights has.
        /// </summary>
        ///
        private double _alpha;

        /// <summary>
        /// The beta is multiplied by the sum squared of the errors.
        /// </summary>
        ///
        private double _beta;

        /// <summary>
        /// The amount to change the weights by.
        /// </summary>
        ///
        private double[] _deltas;

        /// <summary>
        /// Gamma, used for Bayesian regularization.
        /// </summary>
        ///
        private double _gamma;

        /// <summary>
        /// The lambda, or damping factor. This is increased until a desirable
        /// adjustment is found.
        /// </summary>
        ///
        private double _lambda;

        /// <summary>
        /// Should we use Bayesian regularization.
        /// </summary>
        ///
        private bool _useBayesianRegularization;

        /// <summary>
        /// The neural network weights and bias values.
        /// </summary>
        ///
        private double[] _weights;

        /// <summary>
        /// Construct the LMA object.
        /// </summary>
        ///
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public LevenbergMarquardtTraining(BasicNetwork network,
                                          IMLDataSet training) : base(TrainingImplementationType.Iterative)
        {
            ValidateNetwork.ValidateMethodToData(network, training);
            if (network.OutputCount != 1)
            {
                throw new TrainingError(
                    "Levenberg Marquardt requires an output layer with a single neuron.");
            }

            Training = training;
            _indexableTraining = Training;
            _network = network;
            _trainingLength = (int) _indexableTraining.Count;
            _parametersLength = _network.Structure.CalculateSize();
            _hessianMatrix = new Matrix(_parametersLength,
                                       _parametersLength);
            _hessian = _hessianMatrix.Data;
            _alpha = 0.0d;
            _beta = 1.0d;
            _lambda = 0.1d;
            _deltas = new double[_parametersLength];
            _gradient = new double[_parametersLength];
            _diagonal = new double[_parametersLength];

            var input = new BasicMLData(
                _indexableTraining.InputSize);
            var ideal = new BasicMLData(
                _indexableTraining.IdealSize);
            _pair = new BasicMLDataPair(input, ideal);
        }


        /// <value>The trained network.</value>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <summary>
        /// Set if Bayesian regularization should be used.
        /// </summary>
        public bool UseBayesianRegularization
        {
            get { return _useBayesianRegularization; }
            set { _useBayesianRegularization = value; }
        }

        /// <inheritdoc />
        public override sealed bool CanContinue
        {
            get { return false; }
        }

        /// <summary>
        /// Return the sum of the diagonal.
        /// </summary>
        ///
        /// <param name="m">The matrix to sum.</param>
        /// <returns>The trace of the matrix.</returns>
        public static double Trace(double[][] m)
        {
            double result = 0.0d;
            for (int i = 0; i < m.Length; i++)
            {
                result += m[i][i];
            }
            return result;
        }

        /// <summary>
        /// Calculate the Hessian matrix.
        /// </summary>
        ///
        /// <param name="jacobian">The Jacobian matrix.</param>
        /// <param name="errors">The errors.</param>
        public void CalculateHessian(double[][] jacobian,
                                     double[] errors)
        {
            for (int i = 0; i < _parametersLength; i++)
            {
                // Compute Jacobian Matrix Errors
                double s = 0.0d;
                for (int j = 0; j < _trainingLength; j++)
                {
                    s += jacobian[j][i]*errors[j];
                }
                _gradient[i] = s;

                // Compute Quasi-Hessian Matrix using Jacobian (H = J'J)
                for (int j = 0; j < _parametersLength; j++)
                {
                    double c = 0.0d;
                    for (int k = 0; k < _trainingLength; k++)
                    {
                        c += jacobian[k][i]*jacobian[k][j];
                    }
                    _hessian[i][j] = _beta*c;
                }
            }

            for (int i = 0; i < _parametersLength; i++)
            {
                _diagonal[i] = _hessian[i][i];
            }
        }

        /// <summary>
        /// Calculate the sum squared of the weights.
        /// </summary>
        ///
        /// <returns>The sum squared of the weights.</returns>
        private double CalculateSumOfSquaredWeights()
        {
            double result = 0;


            foreach (double weight  in  _weights)
            {
                result += weight*weight;
            }

            return result/2.0d;
        }


        /// <summary>
        /// Perform one iteration.
        /// </summary>
        ///
        public override void Iteration()
        {
            LUDecomposition decomposition = null;

            PreIteration();

            _weights = NetworkCODEC.NetworkToArray(_network);

            IComputeJacobian j = new JacobianChainRule(_network,
                                                      _indexableTraining);

            double sumOfSquaredErrors = j.Calculate(_weights);
            double sumOfSquaredWeights = CalculateSumOfSquaredWeights();

            // this.setError(j.getError());
            CalculateHessian(j.Jacobian, j.RowErrors);

            // Define the objective function
            // bayesian regularization objective function
            double objective = _beta*sumOfSquaredErrors + _alpha
                               *sumOfSquaredWeights;
            double current = objective + 1.0d;

            // Start the main Levenberg-Macquardt method
            _lambda /= ScaleLambda;

            // We'll try to find a direction with less error
            // (or where the objective function is smaller)
            while ((current >= objective)
                   && (_lambda < LambdaMax))
            {
                _lambda *= ScaleLambda;

                // Update diagonal (Levenberg-Marquardt formula)
                for (int i = 0; i < _parametersLength; i++)
                {
                    _hessian[i][i] = _diagonal[i]
                                    + (_lambda + _alpha);
                }

                // Decompose to solve the linear system
                decomposition = new LUDecomposition(_hessianMatrix);

                // Check if the Jacobian has become non-invertible
                if (!decomposition.IsNonsingular)
                {
                    continue;
                }

                // Solve using LU (or SVD) decomposition
                _deltas = decomposition.Solve(_gradient);

                // Update weights using the calculated deltas
                sumOfSquaredWeights = UpdateWeights();

                // Calculate the new error
                sumOfSquaredErrors = 0.0d;
                for (int i = 0; i < _trainingLength; i++)
                {
                    _indexableTraining.GetRecord(i, _pair);
                    IMLData actual = _network
                        .Compute(_pair.Input);
                    double e = _pair.Ideal[0]
                               - actual[0];
                    sumOfSquaredErrors += e*e;
                }
                sumOfSquaredErrors /= 2.0d;

                // Update the objective function
                current = _beta*sumOfSquaredErrors + _alpha
                          *sumOfSquaredWeights;

                // If the object function is bigger than before, the method
                // is tried again using a greater dumping factor.
            }

            // If this iteration caused a error drop, then next iteration
            // will use a smaller damping factor.
            _lambda /= ScaleLambda;

            if (_useBayesianRegularization && (decomposition != null))
            {
                // Compute the trace for the inverse Hessian
                double trace = Trace(decomposition.Inverse());

                // Poland update's formula:
                _gamma = _parametersLength - (_alpha*trace);
                _alpha = _parametersLength
                        /(2.0d*sumOfSquaredWeights + trace);
                _beta = Math.Abs((_trainingLength - _gamma)
                                /(2.0d*sumOfSquaredErrors));
            }

            Error = sumOfSquaredErrors;

            PostIteration();
        }

        /// <summary>
        /// Update the weights.
        /// </summary>
        ///
        /// <returns>The sum squared of the weights.</returns>
        public double UpdateWeights()
        {
            double result = 0;
            var w = (double[]) _weights.Clone();

            for (int i = 0; i < w.Length; i++)
            {
                w[i] += _deltas[i];
                result += w[i]*w[i];
            }

            NetworkCODEC.ArrayToNetwork(w, _network);

            return result/2.0d;
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
