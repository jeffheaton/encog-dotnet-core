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

using Encog.ML.Train;
using Encog.Util.Concurrency;
using Encog.MathUtil.Matrices.Hessian;
using Encog.ML.Data;
using Encog.Util.Validate;
using Encog.ML;
using Encog.ML.Data.Basic;
using Encog.MathUtil.Error;
using Encog.MathUtil.Matrices.Decomposition;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Training.Propagation;
namespace Encog.Neural.Networks.Training.Lma
{
    /// <summary>
    /// Trains a neural network using a Levenberg Marquardt algorithm (LMA). This
    /// training technique is based on the mathematical technique of the same name.
    /// 
    /// The LMA interpolates between the Gauss-Newton algorithm (GNA) and the 
    /// method of gradient descent (similar to what is used by backpropagation. 
    /// The lambda parameter determines the degree to which GNA and Gradient 
    /// Descent are used.  A lower lambda results in heavier use of GNA, 
    /// whereas a higher lambda results in a heavier use of gradient descent.
    /// 
    /// Each iteration starts with a low lambda that  builds if the improvement 
    /// to the neural network is not desirable.  At some point the lambda is
    /// high enough that the training method reverts totally to gradient descent.
    /// 
    /// This allows the neural network to be trained effectively in cases where GNA
    /// provides the optimal training time, but has the ability to fall back to the
    /// more primitive gradient descent method
    ///
    /// LMA finds only a local minimum, not a global minimum.
    ///  
    /// References:
    /// http://www.heatonresearch.com/wiki/LMA
    /// http://en.wikipedia.org/wiki/Levenberg%E2%80%93Marquardt_algorithm
    /// http://en.wikipedia.org/wiki/Finite_difference_method
    /// http://crsouza.blogspot.com/2009/11/neural-network-learning-by-levenberg_18.html
    /// http://mathworld.wolfram.com/FiniteDifference.html 
    /// http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf -
    /// http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html
    /// </summary>
    public class LevenbergMarquardtTraining : BasicTraining, IMultiThreadable
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
        /// Utility class to compute the Hessian.
        /// </summary>
        private IComputeHessian hessian;

        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The training set that we are using to train.
        /// </summary>
        private IMLDataSet indexableTraining;

        /// <summary>
        /// The training set length.
        /// </summary>
        private int trainingLength;

        /// <summary>
        /// How many weights are we dealing with?
        /// </summary>
        private int weightCount;

        /// <summary>
        /// The neural network weights and bias values.
        /// </summary>
        private double[] weights;

        /// <summary>
        /// The lambda, or damping factor. This is increased until a desirable
        /// adjustment is found.
        /// </summary>
        private double lambda;

        /// <summary>
        /// The diagonal of the hessian.
        /// </summary>
        private double[] diagonal;

        /// <summary>
        /// The amount to change the weights by.
        /// </summary>
        private double[] deltas;

        /// <summary>
        /// The training elements.
        /// </summary>
        private IMLDataPair pair;

        /// <summary>
        /// Construct the LMA object. Use the chain rule for Hessian calc.
        /// </summary>
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public LevenbergMarquardtTraining(BasicNetwork network,
                IMLDataSet training)
            : this(network, training, new HessianCR())
        {

        }
        
        /// <summary>
        /// Construct the LMA object. 
        /// </summary>
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        /// <param name="h">The Hessian calculator to use.</param>
        public LevenbergMarquardtTraining(BasicNetwork network,
                IMLDataSet training, IComputeHessian h)
            : base(TrainingImplementationType.Iterative)
        {

            ValidateNetwork.ValidateMethodToData(network, training);

            Training = training;
            this.indexableTraining = Training;
            this.network = network;
            this.trainingLength = (int)this.indexableTraining.Count;
            this.weightCount = this.network.Structure.CalculateSize();
            this.lambda = 0.1;
            this.deltas = new double[this.weightCount];
            this.diagonal = new double[this.weightCount];

            BasicMLData input = new BasicMLData(
                    this.indexableTraining.InputSize);
            BasicMLData ideal = new BasicMLData(
                    this.indexableTraining.IdealSize);
            this.pair = new BasicMLDataPair(input, ideal);

            this.hessian = h;
            this.hessian.Init(network, training);


        }

        /// <summary>
        /// Save the diagonal of the Hessian.  Will be used to apply the lambda.
        /// </summary>
        private void SaveDiagonal()
        {
            double[][] h = this.hessian.Hessian;
            for (int i = 0; i < this.weightCount; i++)
            {
                this.diagonal[i] = h[i][i];
            }
        }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The trained neural network.
        /// </summary>
        public override IMLMethod Method
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// Calculate the SSE error.
        /// </summary>
        /// <returns>The SSE error with the current weights.</returns>
        private double CalculateError()
        {
            ErrorCalculation result = new ErrorCalculation();

            for (int i = 0; i < this.trainingLength; i++)
            {
                this.indexableTraining.GetRecord(i, this.pair);
                IMLData actual = this.network.Compute(this.pair.Input);
                result.UpdateError(actual.Data, this.pair.Ideal.Data, pair.Significance);
            }

            return result.CalculateSSE();
        }

        /// <summary>
        /// Apply the lambda, this will dampen the GNA.
        /// </summary>
        private void ApplyLambda()
        {
            double[][] h = this.hessian.Hessian;
            for (int i = 0; i < this.weightCount; i++)
            {
                h[i][i] = this.diagonal[i] + this.lambda;
            }
        }

        /// <summary>
        /// Perform one iteration.
        /// </summary>
        public override void Iteration()
        {

            LUDecomposition decomposition = null;
            PreIteration();

            this.hessian.Clear();
            this.weights = NetworkCODEC.NetworkToArray(this.network);

            this.hessian.Compute();
            double currentError = this.hessian.SSE;
            SaveDiagonal();

            double startingError = currentError;
            bool done = false;

            while (!done)
            {
                ApplyLambda();
                decomposition = new LUDecomposition(this.hessian.HessianMatrix);

                if (decomposition.IsNonsingular)
                {
                    this.deltas = decomposition.Solve(this.hessian.Gradients);

                    UpdateWeights();
                    currentError = CalculateError();
                }

                if (currentError >= startingError)
                {
                    this.lambda *= LevenbergMarquardtTraining.SCALE_LAMBDA;
                    if (this.lambda > LevenbergMarquardtTraining.LAMBDA_MAX)
                    {
                        this.lambda = LevenbergMarquardtTraining.LAMBDA_MAX;
                        done = true;
                    }
                }
                else
                {
                    this.lambda /= LevenbergMarquardtTraining.SCALE_LAMBDA;
                    done = true;
                }
            }

            Error = currentError;

            PostIteration();
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

        /// <summary>
        /// Update the weights in the neural network.
        /// </summary>
        public void UpdateWeights()
        {
            double[] w = (double[])this.weights.Clone();

            for (int i = 0; i < w.Length; i++)
            {
                w[i] += this.deltas[i];
            }

            NetworkCODEC.ArrayToNetwork(w, this.network);
        }

        /// <summary>
        /// The Hessian calculation method used.
        /// </summary>
        public IComputeHessian Hessian
        {
            get
            {
                return hessian;
            }
        }

        /// <summary>
        /// The thread count, specify 0 for Encog to automatically select (default).  
        /// If the underlying Hessian calculator does not support multithreading, an error 
        /// will be thrown.  The default chain rule calc does support multithreading.
        /// </summary>
        public int ThreadCount
        {
            get
            {
                if (this.hessian is IMultiThreadable)
                {
                    return ((IMultiThreadable)this.hessian).ThreadCount;
                }
                else
                {
                    throw new TrainingError("The Hessian object in use(" + this.hessian.GetType().Name + ") does not support multi-threaded mode.");
                }
            }
            set
            {
                if (this.hessian is IMultiThreadable)
                {
                    ((IMultiThreadable)this.hessian).ThreadCount = value;
                }
                else
                {
                    throw new TrainingError("The Hessian object in use(" + this.hessian.GetType().Name + ") does not support multi-threaded mode.");
                }
            }
        }
    }
}
