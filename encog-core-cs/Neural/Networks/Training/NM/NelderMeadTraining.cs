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
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Structure;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;

namespace Encog.Neural.Networks.Training.NM
{
    /// <summary>
    /// The Nelder-Mead method is a commonly used parameter optimization method that
    /// can be used for neural network training. It typically provides a good error
    /// rate and is relatively fast.
    /// 
    /// Nelder-Mead must build a simplex, which is an n*(n+1) matrix of weights. If
    /// you have a large number of weights, this matrix can quickly overflow memory.
    /// 
    /// The biggest enhancement that is needed for this trainer is to make use of
    /// multi-threaded code to evaluate the speed evaluations when training on a
    /// multi-core.
    /// 
    /// This implementation is based on the source code provided by John Burkardt
    /// (http://people.sc.fsu.edu/~jburkardt/)
    /// 
    /// http://people.sc.fsu.edu/~jburkardt/c_src/asa047/asa047.c
    /// </summary>
    public class NelderMeadTraining : BasicTraining
    {
        private readonly int _konvge;

        /// <summary>
        /// The network to be trained.
        /// </summary>
        private readonly BasicNetwork _network;

        private readonly int _nn;
        private readonly double[] _p;
        private readonly double[] _p2Star;
        private readonly double[] _pbar;
        private readonly double[] _pstar;
        private readonly double _rq;
        private readonly double[] _start;
        private readonly double[] _step;
        private readonly double[] _trainedWeights;
        private readonly double[] _y;

        /// <summary>
        /// Used to calculate the centroid.
        /// </summary>
        private const double ccoeff = 0.5;

        /// <summary>
        /// True if the network has converged, and no further training is needed.
        /// </summary>
        private bool _converged;

        private double _del;
        private const double ecoeff = 2.0;
        private const double eps = 0.001;
        private int _ihi;
        private int _ilo;
        private int _jcount;
        private int _l;
        private const double rcoeff = 1.0;
        private double _y2Star;
        private double _ylo;

        /// <summary>
        /// The best error rate.
        /// </summary>
        private double _ynewlo;

        private double _ystar;
        private double _z;

        /// <summary>
        /// Construct a Nelder Mead trainer with a step size of 100.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training set to use.</param>
        public NelderMeadTraining(BasicNetwork network,
                                  IMLDataSet training)
            : this(network, training, 100)
        {
        }

        /// <summary>
        /// Construct a Nelder Mead trainer with a definable step. 
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data to use.</param>
        /// <param name="stepValue">The step value. This value defines, to some degree the range
        /// of different weights that will be tried.</param>
        public NelderMeadTraining(BasicNetwork network,
                                  IMLDataSet training, double stepValue) :
                                      base(TrainingImplementationType.OnePass)
        {
            this._network = network;
            Training = training;

            _start = NetworkCODEC.NetworkToArray(network);
            _trainedWeights = NetworkCODEC.NetworkToArray(network);

            int n = _start.Length;

            _p = new double[n*(n + 1)];
            _pstar = new double[n];
            _p2Star = new double[n];
            _pbar = new double[n];
            _y = new double[n + 1];

            _nn = n + 1;
            _del = 1.0;
            _rq = EncogFramework.DefaultDoubleEqual*n;

            _step = new double[NetworkCODEC.NetworkSize(network)];
            _jcount = _konvge = 500;
            EngineArray.Fill(_step, stepValue);
        }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <inheritdoc/>
        public new bool TrainingDone
        {
            get
            {
                return _converged || base.TrainingDone;
            }
        }

        /// <summary>
        /// Calculate the error for the neural network with a given set of weights. 
        /// </summary>
        /// <param name="weights">The weights to use.</param>
        /// <returns>The current error.</returns>
        public double Fn(double[] weights)
        {
            NetworkCODEC.ArrayToNetwork(weights, _network);
            return _network.CalculateError(Training);
        }

        /// <inheritdoc/>
        public override void Iteration()
        {
            if (_converged)
            {
                return;
            }

            int n = _start.Length;

            for (int i = 0; i < n; i++)
            {
                _p[i + n*n] = _start[i];
            }
            _y[n] = Fn(_start);
            for (int j = 0; j < n; j++)
            {
                double x = _start[j];
                _start[j] = _start[j] + _step[j]*_del;
                for (int i = 0; i < n; i++)
                {
                    _p[i + j*n] = _start[i];
                }
                _y[j] = Fn(_start);
                _start[j] = x;
            }
            /*
             * The simplex construction is complete.
             * 
             * Find highest and lowest Y values. YNEWLO = Y(IHI) indicates the
             * vertex of the simplex to be replaced.
             */
            _ylo = _y[0];
            _ilo = 0;

            for (int i = 1; i < _nn; i++)
            {
                if (_y[i] < _ylo)
                {
                    _ylo = _y[i];
                    _ilo = i;
                }
            }
            /*
             * Inner loop.
             */
            for (;;)
            {
                /*
                 * if (kcount <= icount) { break; }
                 */
                _ynewlo = _y[0];
                _ihi = 0;

                for (int i = 1; i < _nn; i++)
                {
                    if (_ynewlo < _y[i])
                    {
                        _ynewlo = _y[i];
                        _ihi = i;
                    }
                }
                /*
                 * Calculate PBAR, the centroid of the simplex vertices excepting
                 * the vertex with Y value YNEWLO.
                 */
                for (int i = 0; i < n; i++)
                {
                    _z = 0.0;
                    for (int j = 0; j < _nn; j++)
                    {
                        _z = _z + _p[i + j*n];
                    }
                    _z = _z - _p[i + _ihi*n];
                    _pbar[i] = _z/n;
                }
                /*
                 * Reflection through the centroid.
                 */
                for (int i = 0; i < n; i++)
                {
                    _pstar[i] = _pbar[i] + rcoeff
                               *(_pbar[i] - _p[i + _ihi*n]);
                }
                _ystar = Fn(_pstar);
                /*
                 * Successful reflection, so extension.
                 */
                if (_ystar < _ylo)
                {
                    for (int i = 0; i < n; i++)
                    {
                        _p2Star[i] = _pbar[i] + ecoeff
                                    *(_pstar[i] - _pbar[i]);
                    }
                    _y2Star = Fn(_p2Star);
                    /*
                     * Check extension.
                     */
                    if (_ystar < _y2Star)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            _p[i + _ihi*n] = _pstar[i];
                        }
                        _y[_ihi] = _ystar;
                    }
                        /*
                     * Retain extension or contraction.
                     */
                    else
                    {
                        for (int i = 0; i < n; i++)
                        {
                            _p[i + _ihi*n] = _p2Star[i];
                        }
                        _y[_ihi] = _y2Star;
                    }
                }
                    /*
                 * No extension.
                 */
                else
                {
                    _l = 0;
                    for (int i = 0; i < _nn; i++)
                    {
                        if (_ystar < _y[i])
                        {
                            _l = _l + 1;
                        }
                    }

                    if (1 < _l)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            _p[i + _ihi*n] = _pstar[i];
                        }
                        _y[_ihi] = _ystar;
                    }
                        /*
                     * Contraction on the Y(IHI) side of the centroid.
                     */
                    else if (_l == 0)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            _p2Star[i] = _pbar[i] + ccoeff
                                        *(_p[i + _ihi*n] - _pbar[i]);
                        }
                        _y2Star = Fn(_p2Star);
                        /*
                         * Contract the whole simplex.
                         */
                        if (_y[_ihi] < _y2Star)
                        {
                            for (int j = 0; j < _nn; j++)
                            {
                                for (int i = 0; i < n; i++)
                                {
                                    _p[i + j*n] = (_p[i + j*n] + _p[i
                                                                 + _ilo*n])*0.5;
                                    _trainedWeights[i] = _p[i + j*n];
                                }
                                _y[j] = Fn(_trainedWeights);
                            }
                            _ylo = _y[0];
                            _ilo = 0;

                            for (int i = 1; i < _nn; i++)
                            {
                                if (_y[i] < _ylo)
                                {
                                    _ylo = _y[i];
                                    _ilo = i;
                                }
                            }
                            continue;
                        }
                            /*
                         * Retain contraction.
                         */
                        for (int i = 0; i < n; i++)
                        {
                            _p[i + _ihi*n] = _p2Star[i];
                        }
                        _y[_ihi] = _y2Star;
                    }
                        /*
                     * Contraction on the reflection side of the centroid.
                     */
                    else if (_l == 1)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            _p2Star[i] = _pbar[i] + ccoeff
                                        *(_pstar[i] - _pbar[i]);
                        }
                        _y2Star = Fn(_p2Star);
                        /*
                         * Retain reflection?
                         */
                        if (_y2Star <= _ystar)
                        {
                            for (int i = 0; i < n; i++)
                            {
                                _p[i + _ihi*n] = _p2Star[i];
                            }
                            _y[_ihi] = _y2Star;
                        }
                        else
                        {
                            for (int i = 0; i < n; i++)
                            {
                                _p[i + _ihi*n] = _pstar[i];
                            }
                            _y[_ihi] = _ystar;
                        }
                    }
                }
                /*
                 * Check if YLO improved.
                 */
                if (_y[_ihi] < _ylo)
                {
                    _ylo = _y[_ihi];
                    _ilo = _ihi;
                }
                _jcount = _jcount - 1;

                if (0 < _jcount)
                {
                    continue;
                }
                /*
                 * Check to see if minimum reached.
                 */
                // if (icount <= kcount)
                {
                    _jcount = _konvge;

                    _z = 0.0;
                    for (int i = 0; i < _nn; i++)
                    {
                        _z = _z + _y[i];
                    }
                    double x = _z/_nn;

                    _z = 0.0;
                    for (int i = 0; i < _nn; i++)
                    {
						var inner = _y[i] - x;
                        _z = _z + inner * inner;
                    }

                    if (_z <= _rq)
                    {
                        break;
                    }
                }
            }
            /*
             * Factorial tests to check that YNEWLO is a local minimum.
             */
            for (int i = 0; i < n; i++)
            {
                _trainedWeights[i] = _p[i + _ilo*n];
            }
            _ynewlo = _y[_ilo];

            bool fault = false;

            for (int i = 0; i < n; i++)
            {
                _del = _step[i]*eps;
                _trainedWeights[i] += _del;
                _z = Fn(_trainedWeights);
                if (_z < _ynewlo)
                {
                    fault = true;
                    break;
                }
                _trainedWeights[i] = _trainedWeights[i] - _del
                                    - _del;
                _z = Fn(_trainedWeights);
                if (_z < _ynewlo)
                {
                    fault = true;
                    break;
                }
                _trainedWeights[i] += _del;
            }

            if (!fault)
            {
                _converged = true;
            }
            else
            {
                /*
                 * Restart the procedure.
                 */
                for (int i = 0; i < n; i++)
                {
                    _start[i] = _trainedWeights[i];
                }
                _del = eps;
            }

            Error = _ynewlo;
            NetworkCODEC.ArrayToNetwork(_trainedWeights, _network);
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
