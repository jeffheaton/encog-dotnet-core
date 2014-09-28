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
using Encog.MathUtil.Error;
using Encog.MathUtil.Matrices.Decomposition;
using Encog.MathUtil.Matrices.Hessian;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util.Concurrency;
using Encog.Util.Validate;

namespace Encog.Neural.Networks.Training.Lma
{
    /// <summary>
    ///     Trains a neural network using a Levenberg Marquardt algorithm (LMA). This
    ///     training technique is based on the mathematical technique of the same name.
    ///     The LMA interpolates between the Gauss-Newton algorithm (GNA) and the
    ///     method of gradient descent (similar to what is used by backpropagation.
    ///     The lambda parameter determines the degree to which GNA and Gradient
    ///     Descent are used.  A lower lambda results in heavier use of GNA,
    ///     whereas a higher lambda results in a heavier use of gradient descent.
    ///     Each iteration starts with a low lambda that  builds if the improvement
    ///     to the neural network is not desirable.  At some point the lambda is
    ///     high enough that the training method reverts totally to gradient descent.
    ///     This allows the neural network to be trained effectively in cases where GNA
    ///     provides the optimal training time, but has the ability to fall back to the
    ///     more primitive gradient descent method
    ///     LMA finds only a local minimum, not a global minimum.
    ///     References:
    ///     C. R. Souza. (2009). Neural Network Learning by the Levenberg-Marquardt Algorithm
    ///     with Bayesian Regularization. Website, available from:
    ///     http://crsouza.blogspot.com/2009/11/neural-network-learning-by-levenberg_18.html
    ///     http://www.heatonresearch.com/wiki/LMA
    ///     http://en.wikipedia.org/wiki/Levenberg%E2%80%93Marquardt_algorithm
    ///     http://en.wikipedia.org/wiki/Finite_difference_method
    ///     http://mathworld.wolfram.com/FiniteDifference.html
    ///     http://www-alg.ist.hokudai.ac.jp/~jan/alpha.pdf -
    ///     http://www.inference.phy.cam.ac.uk/mackay/Bayes_FAQ.html
    /// </summary>
    public class LevenbergMarquardtTraining : BasicTraining, IMultiThreadable
    {
        /// <summary>
        ///     The amount to scale the lambda by.
        /// </summary>
        public const double ScaleLambda = 10.0;

        /// <summary>
        ///     The max amount for the LAMBDA.
        /// </summary>
        public const double LambdaMax = 1e25;

        /// <summary>
        ///     he diagonal of the hessian.
        /// </summary>
        private readonly double[] _diagonal;

        /// <summary>
        ///     Utility class to compute the Hessian.
        /// </summary>
        private readonly IComputeHessian _hessian;

        /// <summary>
        ///     The training set that we are using to train.
        /// </summary>
        private readonly IMLDataSet _indexableTraining;

        /// <summary>
        ///     The network that is to be trained.
        /// </summary>
        private readonly BasicNetwork _network;

        /// <summary>
        ///     The training set length.
        /// </summary>
        private readonly int _trainingLength;

        /// <summary>
        ///     How many weights are we dealing with?
        /// </summary>
        private readonly int _weightCount;

        /// <summary>
        ///     The amount to change the weights by.
        /// </summary>
        private double[] _deltas;

        /// <summary>
        ///     Is the init complete?
        /// </summary>
        private bool _initComplete;

        /// <summary>
        ///     The lambda, or damping factor. This is increased until a desirable
        ///     adjustment is found.
        /// </summary>
        private double _lambda;

        /// <summary>
        ///     The training elements.
        /// </summary>
        private IMLDataPair _pair;

        /// <summary>
        ///     The neural network weights and bias values.
        /// </summary>
        private double[] _weights;

        /// <summary>
        ///     Construct the LMA object.
        /// </summary>
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training">The training data to use. Must be indexable.</param>
        public LevenbergMarquardtTraining(BasicNetwork network,
            IMLDataSet training) : this(network, training, new HessianCR())
        {
        }


        /// <summary>
        ///     Construct the LMA object.
        /// </summary>
        /// <param name="network">The network to train. Must have a single output neuron.</param>
        /// <param name="training"></param>
        /// <param name="h">The training data to use. Must be indexable.</param>
        public LevenbergMarquardtTraining(BasicNetwork network,
            IMLDataSet training, IComputeHessian h) :
                base(TrainingImplementationType.Iterative)
        {
            ValidateNetwork.ValidateMethodToData(network, training);

            Training = training;
            _indexableTraining = Training;
            this._network = network;
            _trainingLength = _indexableTraining.Count;
            _weightCount = this._network.Structure.CalculateSize();
            _lambda = 0.1;
            _deltas = new double[_weightCount];
            _diagonal = new double[_weightCount];

            var input = new BasicMLData(
                _indexableTraining.InputSize);
            var ideal = new BasicMLData(
                _indexableTraining.IdealSize);
            _pair = new BasicMLDataPair(input, ideal);

            _hessian = h;
        }

        /// <inheritdoc />
        public override bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        ///     The Hessian calculation method used.
        /// </summary>
        public IComputeHessian Hessian
        {
            get { return _hessian; }
        }

        /// <inheritdoc />
        public int ThreadCount
        {
            get
            {
                if (_hessian is IMultiThreadable)
                {
                    return ((IMultiThreadable) _hessian).ThreadCount;
                }
                return 1;
            }
            set
            {
                if (_hessian is IMultiThreadable)
                {
                    ((IMultiThreadable) _hessian).ThreadCount = value;
                }
                else if (value != 1 && value != 0)
                {
                    throw new TrainingError("The Hessian object in use(" + _hessian.GetType().Name +
                                            ") does not support multi-threaded mode.");
                }
            }
        }

        /// <summary>
        ///     Save the diagonal of the hessian.
        /// </summary>
        private void SaveDiagonal()
        {
            double[][] h = _hessian.Hessian;
            for (int i = 0; i < _weightCount; i++)
            {
                _diagonal[i] = h[i][i];
            }
        }

        /// <summary>
        ///     The SSE error with the current weights.
        /// </summary>
        /// <returns></returns>
        private double CalculateError()
        {
            var result = new ErrorCalculation();

            for (int i = 0; i < _trainingLength; i++)
            {
                _pair = _indexableTraining[i];
                IMLData actual = _network.Compute(_pair.Input);
                result.UpdateError(actual, _pair.Ideal, _pair.Significance);
            }

            return result.CalculateSSE();
        }

        /// <summary>
        ///     Apply the lambda.
        /// </summary>
        private void ApplyLambda()
        {
            double[][] h = _hessian.Hessian;
            for (int i = 0; i < _weightCount; i++)
            {
                h[i][i] = _diagonal[i] + _lambda;
            }
        }

        /// <inheritdoc />
        public override void Iteration()
        {
            if (!_initComplete)
            {
                _hessian.Init(_network, Training);
                _initComplete = true;
            }

            PreIteration();

            _hessian.Clear();
            _weights = NetworkCODEC.NetworkToArray(_network);

            _hessian.Compute();
            double currentError = _hessian.SSE;
            SaveDiagonal();

            double startingError = currentError;
            bool done = false;
            bool singular;

            while (!done)
            {
                ApplyLambda();
                var decomposition = new LUDecomposition(_hessian.HessianMatrix);

                singular = decomposition.IsNonsingular;

                if (singular)
                {
                    _deltas = decomposition.Solve(_hessian.Gradients);
                    UpdateWeights();
                    currentError = CalculateError();
                }

                if (!singular || currentError >= startingError)
                {
                    _lambda *= ScaleLambda;
                    if (_lambda > LambdaMax)
                    {
                        _lambda = LambdaMax;
                        done = true;
                    }
                }
                else
                {
                    _lambda /= ScaleLambda;
                    done = true;
                }
            }

            Error = currentError;

            PostIteration();
        }


        /// <inheritdoc />
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc />
        public override void Resume(TrainingContinuation state)
        {
        }

        /// <summary>
        ///     Update the weights in the neural network.
        /// </summary>
        public void UpdateWeights()
        {
            var w = (double[]) _weights.Clone();

            for (int i = 0; i < w.Length; i++)
            {
                w[i] += _deltas[i];
            }

            NetworkCODEC.ArrayToNetwork(w, _network);
        }
    }
}
