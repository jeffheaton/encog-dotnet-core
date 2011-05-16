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
        public const double SCALE_LAMBDA = 10.0d;

        /// <summary>
        /// The max amount for the LAMBDA.
        /// </summary>
        ///
        public const double LAMBDA_MAX = 1e25d;

        /// <summary>
        /// The diagonal of the hessian.
        /// </summary>
        ///
        private readonly double[] diagonal;

        /// <summary>
        /// The calculated gradients.
        /// </summary>
        ///
        private readonly double[] gradient;

        /// <summary>
        /// The "hessian" matrix as a 2d array.
        /// </summary>
        ///
        private readonly double[][] hessian;

        /// <summary>
        /// The "hessian" matrix, used by the LMA.
        /// </summary>
        ///
        private readonly Matrix hessianMatrix;

        /// <summary>
        /// The training set that we are using to train.
        /// </summary>
        ///
        private readonly MLDataSet indexableTraining;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        ///
        private readonly BasicNetwork network;

        /// <summary>
        /// The training elements.
        /// </summary>
        ///
        private readonly MLDataPair pair;

        /// <summary>
        /// The number of "parameters" in the LMA algorithm. The parameters are what
        /// the LMA adjusts to achieve the desired outcome. For neural network
        /// optimization, the parameters are the weights and bias values.
        /// </summary>
        ///
        private readonly int parametersLength;

        /// <summary>
        /// The training set length.
        /// </summary>
        ///
        private readonly int trainingLength;

        /// <summary>
        /// The alpha is multiplied by sum squared of weights. This scales the effect
        /// that the sum squared of the weights has.
        /// </summary>
        ///
        private double alpha;

        /// <summary>
        /// The beta is multiplied by the sum squared of the errors.
        /// </summary>
        ///
        private double beta;

        /// <summary>
        /// The amount to change the weights by.
        /// </summary>
        ///
        private double[] deltas;

        /// <summary>
        /// Gamma, used for Bayesian regularization.
        /// </summary>
        ///
        private double gamma;

        /// <summary>
        /// The lambda, or damping factor. This is increased until a desirable
        /// adjustment is found.
        /// </summary>
        ///
        private double lambda;

        /// <summary>
        /// Should we use Bayesian regularization.
        /// </summary>
        ///
        private bool useBayesianRegularization;

        /// <summary>
        /// The neural network weights and bias values.
        /// </summary>
        ///
        private double[] weights;

        /// <summary>
        /// Construct the LMA object.
        /// </summary>
        ///
        /// <param name="network_0">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public LevenbergMarquardtTraining(BasicNetwork network_0,
                                          MLDataSet training) : base(TrainingImplementationType.Iterative)
        {
            ValidateNetwork.ValidateMethodToData(network_0, training);
            if (network_0.OutputCount != 1)
            {
                throw new TrainingError(
                    "Levenberg Marquardt requires an output layer with a single neuron.");
            }

            Training = training;
            indexableTraining = Training;
            network = network_0;
            trainingLength = (int) indexableTraining.Count;
            parametersLength = network.Structure.CalculateSize();
            hessianMatrix = new Matrix(parametersLength,
                                       parametersLength);
            hessian = hessianMatrix.Data;
            alpha = 0.0d;
            beta = 1.0d;
            lambda = 0.1d;
            deltas = new double[parametersLength];
            gradient = new double[parametersLength];
            diagonal = new double[parametersLength];

            var input = new BasicMLData(
                indexableTraining.InputSize);
            var ideal = new BasicMLData(
                indexableTraining.IdealSize);
            pair = new BasicMLDataPair(input, ideal);
        }


        /// <value>The trained network.</value>
        public override MLMethod Method
        {
            /// <returns>The trained network.</returns>
            get { return network; }
        }


        /// <summary>
        /// Set if Bayesian regularization should be used.
        /// </summary>
        ///
        /// <value>True to use Bayesian regularization.</value>
        public bool UseBayesianRegularization
        {
            /// <returns>True, if Bayesian regularization is used.</returns>
            get { return useBayesianRegularization; }
            /// <summary>
            /// Set if Bayesian regularization should be used.
            /// </summary>
            ///
            /// <param name="useBayesianRegularization_0">True to use Bayesian regularization.</param>
            set { useBayesianRegularization = value; }
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
            for (int i = 0; i < parametersLength; i++)
            {
                // Compute Jacobian Matrix Errors
                double s = 0.0d;
                for (int j = 0; j < trainingLength; j++)
                {
                    s += jacobian[j][i]*errors[j];
                }
                gradient[i] = s;

                // Compute Quasi-Hessian Matrix using Jacobian (H = J'J)
                for (int j_0 = 0; j_0 < parametersLength; j_0++)
                {
                    double c = 0.0d;
                    for (int k = 0; k < trainingLength; k++)
                    {
                        c += jacobian[k][i]*jacobian[k][j_0];
                    }
                    hessian[i][j_0] = beta*c;
                }
            }

            for (int i_1 = 0; i_1 < parametersLength; i_1++)
            {
                diagonal[i_1] = hessian[i_1][i_1];
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


            foreach (double weight  in  weights)
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
            double trace = 0;

            PreIteration();

            weights = NetworkCODEC.NetworkToArray(network);

            ComputeJacobian j = new JacobianChainRule(network,
                                                      indexableTraining);

            double sumOfSquaredErrors = j.Calculate(weights);
            double sumOfSquaredWeights = CalculateSumOfSquaredWeights();

            // this.setError(j.getError());
            CalculateHessian(j.Jacobian, j.RowErrors);

            // Define the objective function
            // bayesian regularization objective function
            double objective = beta*sumOfSquaredErrors + alpha
                               *sumOfSquaredWeights;
            double current = objective + 1.0d;

            // Start the main Levenberg-Macquardt method
            lambda /= SCALE_LAMBDA;

            // We'll try to find a direction with less error
            // (or where the objective function is smaller)
            while ((current >= objective)
                   && (lambda < LAMBDA_MAX))
            {
                lambda *= SCALE_LAMBDA;

                // Update diagonal (Levenberg-Marquardt formula)
                for (int i = 0; i < parametersLength; i++)
                {
                    hessian[i][i] = diagonal[i]
                                    + (lambda + alpha);
                }

                // Decompose to solve the linear system
                decomposition = new LUDecomposition(hessianMatrix);

                // Check if the Jacobian has become non-invertible
                if (!decomposition.IsNonsingular)
                {
                    continue;
                }

                // Solve using LU (or SVD) decomposition
                deltas = decomposition.Solve(gradient);

                // Update weights using the calculated deltas
                sumOfSquaredWeights = UpdateWeights();

                // Calculate the new error
                sumOfSquaredErrors = 0.0d;
                for (int i_0 = 0; i_0 < trainingLength; i_0++)
                {
                    indexableTraining.GetRecord(i_0, pair);
                    MLData actual = network
                        .Compute(pair.Input);
                    double e = pair.Ideal[0]
                               - actual[0];
                    sumOfSquaredErrors += e*e;
                }
                sumOfSquaredErrors /= 2.0d;

                // Update the objective function
                current = beta*sumOfSquaredErrors + alpha
                          *sumOfSquaredWeights;

                // If the object function is bigger than before, the method
                // is tried again using a greater dumping factor.
            }

            // If this iteration caused a error drop, then next iteration
            // will use a smaller damping factor.
            lambda /= SCALE_LAMBDA;

            if (useBayesianRegularization && (decomposition != null))
            {
                // Compute the trace for the inverse Hessian
                trace = Trace(decomposition.Inverse());

                // Poland update's formula:
                gamma = parametersLength - (alpha*trace);
                alpha = parametersLength
                        /(2.0d*sumOfSquaredWeights + trace);
                beta = Math.Abs((trainingLength - gamma)
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
            var w = (double[]) weights.Clone();

            for (int i = 0; i < w.Length; i++)
            {
                w[i] += deltas[i];
                result += w[i]*w[i];
            }

            NetworkCODEC.ArrayToNetwork(w, network);

            return result/2.0d;
        }

        public override TrainingContinuation Pause()
        {
            return null;
        }

        public override void Resume(TrainingContinuation state)
        {
        }
    }
}