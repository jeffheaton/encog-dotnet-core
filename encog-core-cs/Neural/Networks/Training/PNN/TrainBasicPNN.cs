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
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.PNN;
using Encog.Util;

namespace Encog.Neural.Networks.Training.PNN
{
    /// <summary>
    /// Train a PNN.
    /// </summary>
    ///
    public class TrainBasicPNN : BasicTraining, ICalculationCriteria
    {
        /// <summary>
        /// The default max error.
        /// </summary>
        ///
        public const double DefaultMaxError = 0.0d;

        /// <summary>
        /// The default minimum improvement before stop.
        /// </summary>
        ///
        public const double DefaultMinImprovement = 0.0001d;

        /// <summary>
        /// THe default sigma low value.
        /// </summary>
        ///
        public const double DefaultSigmaLow = 0.0001d;

        /// <summary>
        /// The default sigma high value.
        /// </summary>
        ///
        public const double DefaultSigmaHigh = 10.0d;

        /// <summary>
        /// The default number of sigmas to evaluate between the low and high.
        /// </summary>
        ///
        public const int DefaultNumSigmas = 10;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly BasicPNN _network;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly IMLDataSet _training;

        /// <summary>
        /// Temp storage for derivative computation.
        /// </summary>
        ///
        private double[] _dsqr;

        /// <summary>
        /// The maximum error to allow.
        /// </summary>
        ///
        private double _maxError;

        /// <summary>
        /// The minimum improvement allowed.
        /// </summary>
        ///
        private double _minImprovement;

        /// <summary>
        /// The number of sigmas to evaluate between the low and high.
        /// </summary>
        ///
        private int _numSigmas;

        /// <summary>
        /// Have the samples been loaded.
        /// </summary>
        ///
        private bool _samplesLoaded;

        /// <summary>
        /// The high value for the sigma search.
        /// </summary>
        ///
        private double _sigmaHigh;

        /// <summary>
        /// The low value for the sigma search.
        /// </summary>
        ///
        private double _sigmaLow;

        /// <summary>
        /// Temp storage for derivative computation.
        /// </summary>
        ///
        private double[] _v;

        /// <summary>
        /// Temp storage for derivative computation.
        /// </summary>
        ///
        private double[] _w;

        /// <summary>
        /// Train a BasicPNN.
        /// </summary>
        ///
        /// <param name="network">The network to train.</param>
        /// <param name="training">The training data.</param>
        public TrainBasicPNN(BasicPNN network, IMLDataSet training) : base(TrainingImplementationType.OnePass)
        {
            _network = network;
            _training = training;

            _maxError = DefaultMaxError;
            _minImprovement = DefaultMinImprovement;
            _sigmaLow = DefaultSigmaLow;
            _sigmaHigh = DefaultSigmaHigh;
            _numSigmas = DefaultNumSigmas;
            _samplesLoaded = false;
        }

        /// <inheritdoc/>
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <value>the maxError to set</value>
        public double MaxError
        {
            get { return _maxError; }
            set { _maxError = value; }
        }


        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }


        /// <value>the minImprovement to set</value>
        public double MinImprovement
        {
            get { return _minImprovement; }
            set { _minImprovement = value; }
        }


        /// <value>the numSigmas to set</value>
        public int NumSigmas
        {
            get { return _numSigmas; }
            set { _numSigmas = value; }
        }


        /// <value>the sigmaHigh to set</value>
        public double SigmaHigh
        {
            get { return _sigmaHigh; }
            set { _sigmaHigh = value; }
        }


        /// <value>the sigmaLow to set</value>
        public double SigmaLow
        {
            get { return _sigmaLow; }
            set { _sigmaLow = value; }
        }

        #region CalculationCriteria Members

        /// <summary>
        /// Calculate the error with multiple sigmas.
        /// </summary>
        ///
        /// <param name="x">The data.</param>
        /// <param name="der1">The first derivative.</param>
        /// <param name="der2">The 2nd derivatives.</param>
        /// <param name="der">Calculate the derivative.</param>
        /// <returns>The error.</returns>
        public double CalcErrorWithMultipleSigma(double[] x,
                                                 double[] der1, double[] der2, bool der)
        {
            int ivar;

            for (ivar = 0; ivar < _network.InputCount; ivar++)
            {
                _network.Sigma[ivar] = x[ivar];
            }

            if (!der)
            {
                return CalculateError(_network.Samples, false);
            }

            double err = CalculateError(_network.Samples, true);

            for (ivar = 0; ivar < _network.InputCount; ivar++)
            {
                der1[ivar] = _network.Deriv[ivar];
                der2[ivar] = _network.Deriv2[ivar];
            }

            return err;
        }

        /// <summary>
        /// Calculate the error using a common sigma.
        /// </summary>
        ///
        /// <param name="sig">The sigma to use.</param>
        /// <returns>The training error.</returns>
        public double CalcErrorWithSingleSigma(double sig)
        {
            int ivar;

            for (ivar = 0; ivar < _network.InputCount; ivar++)
            {
                _network.Sigma[ivar] = sig;
            }

            return CalculateError(_network.Samples, false);
        }

        #endregion

        /// <summary>
        /// Calculate the error for the entire training set.
        /// </summary>
        ///
        /// <param name="training">Training set to use.</param>
        /// <param name="deriv">Should we find the derivative.</param>
        /// <returns>The error.</returns>
        public double CalculateError(IMLDataSet training,
                                     bool deriv)
        {
            double totErr;
            double diff;
            totErr = 0.0d;

            if (deriv)
            {
                int num = (_network.SeparateClass)
                              ? _network.InputCount*_network.OutputCount
                              : _network.InputCount;
                for (int i = 0; i < num; i++)
                {
                    _network.Deriv[i] = 0.0d;
                    _network.Deriv2[i] = 0.0d;
                }
            }

            _network.Exclude = (int) training.Count;

			IMLDataPair pair;
            var xout = new double[_network.OutputCount];

            for (int r = 0; r < training.Count; r++)
            {
                pair = training[r];
                _network.Exclude = _network.Exclude - 1;

                double err = 0.0d;

                IMLData input = pair.Input;
                IMLData target = pair.Ideal;

                if (_network.OutputMode == PNNOutputMode.Unsupervised)
                {
                    if (deriv)
                    {
                        IMLData output = ComputeDeriv(input, target);
                        for (int z = 0; z < _network.OutputCount; z++)
                        {
                            xout[z] = output[z];
                        }
                    }
                    else
                    {
                        IMLData output = _network.Compute(input);
                        for (int z = 0; z < _network.OutputCount; z++)
                        {
                            xout[z] = output[z];
                        }
                    }
                    for (int i = 0; i < _network.OutputCount; i++)
                    {
                        diff = input[i] - xout[i];
                        err += diff*diff;
                    }
                }
                else if (_network.OutputMode == PNNOutputMode.Classification)
                {
                    var tclass = (int) target[0];
                    IMLData output;

                    if (deriv)
                    {
                        output = ComputeDeriv(input, pair.Ideal);
                    }
                    else
                    {
                        output = _network.Compute(input);
                    }

					output.CopyTo(xout, 0, output.Count);

                    for (int i = 0; i < xout.Length; i++)
                    {
                        if (i == tclass)
                        {
                            diff = 1.0d - xout[i];
                            err += diff*diff;
                        }
                        else
                        {
                            err += xout[i]*xout[i];
                        }
                    }
                }

                else if (_network.OutputMode == PNNOutputMode.Regression)
                {
                    if (deriv)
                    {
                        IMLData output = _network.Compute(input);
                        for (int z = 0; z < _network.OutputCount; z++)
                        {
                            xout[z] = output[z];
                        }
                    }
                    else
                    {
                        IMLData output = _network.Compute(input);
                        for (int z = 0; z < _network.OutputCount; z++)
                        {
                            xout[z] = output[z];
                        }
                    }
                    for (int i = 0; i < _network.OutputCount; i++)
                    {
                        diff = target[i] - xout[i];
                        err += diff*diff;
                    }
                }

                totErr += err;
            }

            _network.Exclude = -1;

            _network.Error = totErr/training.Count;
            if (deriv)
            {
                for (int i = 0; i < _network.Deriv.Length; i++)
                {
                    _network.Deriv[i] /= training.Count;
                    _network.Deriv2[i] /= training.Count;
                }
            }

            if ((_network.OutputMode == PNNOutputMode.Unsupervised)
                || (_network.OutputMode == PNNOutputMode.Regression))
            {
                _network.Error = _network.Error
                                /_network.OutputCount;
                if (deriv)
                {
                    for (int i = 0; i < _network.InputCount; i++)
                    {
                        _network.Deriv[i] /= _network.OutputCount;
                        _network.Deriv2[i] /= _network.OutputCount;
                    }
                }
            }

            return _network.Error;
        }

        /// <summary>
        /// Compute the derivative for target data.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <param name="target">The target data.</param>
        /// <returns>The output.</returns>
        public IMLData ComputeDeriv(IMLData input, IMLData target)
        {
            int pop, ivar;
            int ibest = 0;
            int outvar;
            double dist, truedist;
            double vtot, wtot;
            double temp, der1, der2, psum;
            int vptr, wptr, vsptr = 0, wsptr = 0;

            var xout = new double[_network.OutputCount];

            for (pop = 0; pop < _network.OutputCount; pop++)
            {
                xout[pop] = 0.0d;
                for (ivar = 0; ivar < _network.InputCount; ivar++)
                {
                    _v[pop*_network.InputCount + ivar] = 0.0d;
                    _w[pop*_network.InputCount + ivar] = 0.0d;
                }
            }

            psum = 0.0d;

            if (_network.OutputMode != PNNOutputMode.Classification)
            {
                vsptr = _network.OutputCount
                        *_network.InputCount;
                wsptr = _network.OutputCount
                        *_network.InputCount;
                for (ivar = 0; ivar < _network.InputCount; ivar++)
                {
                    _v[vsptr + ivar] = 0.0d;
                    _w[wsptr + ivar] = 0.0d;
                }
            }

            IMLDataPair pair;

            for (int r = 0; r < _network.Samples.Count; r++)
            {
                pair = _network.Samples[r];

                if (r == _network.Exclude)
                {
                    continue;
                }

                dist = 0.0d;
                for (ivar = 0; ivar < _network.InputCount; ivar++)
                {
                    double diff = input[ivar] - pair.Input[ivar];
                    diff /= _network.Sigma[ivar];
                    _dsqr[ivar] = diff*diff;
                    dist += _dsqr[ivar];
                }

                if (_network.Kernel == PNNKernelType.Gaussian)
                {
                    dist = Math.Exp(-dist);
                }
                else if (_network.Kernel == PNNKernelType.Reciprocal)
                {
                    dist = 1.0d/(1.0d + dist);
                }

                truedist = dist;
                if (dist < 1.0e-40d)
                {
                    dist = 1.0e-40d;
                }

                if (_network.OutputMode == PNNOutputMode.Classification)
                {
                    pop = (int) pair.Ideal[0];
                    xout[pop] += dist;
                    vptr = pop*_network.InputCount;
                    wptr = pop*_network.InputCount;
                    for (ivar = 0; ivar < _network.InputCount; ivar++)
                    {
                        temp = truedist*_dsqr[ivar];
                        _v[vptr + ivar] += temp;
                        _w[wptr + ivar] += temp*(2.0d*_dsqr[ivar] - 3.0d);
                    }
                }

                else if (_network.OutputMode == PNNOutputMode.Unsupervised)
                {
                    for (ivar = 0; ivar < _network.InputCount; ivar++)
                    {
                        xout[ivar] += dist*pair.Input[ivar];
                        temp = truedist*_dsqr[ivar];
                        _v[vsptr + ivar] += temp;
                        _w[wsptr + ivar] += temp
                                           *(2.0d*_dsqr[ivar] - 3.0d);
                    }
                    vptr = 0;
                    wptr = 0;
                    for (outvar = 0; outvar < _network.OutputCount; outvar++)
                    {
                        for (ivar = 0; ivar < _network.InputCount; ivar++)
                        {
                            temp = truedist*_dsqr[ivar]
                                   *pair.Input[ivar];
                            _v[vptr++] += temp;
                            _w[wptr++] += temp*(2.0d*_dsqr[ivar] - 3.0d);
                        }
                    }
                    psum += dist;
                }
                else if (_network.OutputMode == PNNOutputMode.Regression)
                {
                    for (ivar = 0; ivar < _network.OutputCount; ivar++)
                    {
                        xout[ivar] += dist*pair.Ideal[ivar];
                    }
                    vptr = 0;
                    wptr = 0;
                    for (outvar = 0; outvar < _network.OutputCount; outvar++)
                    {
                        for (ivar = 0; ivar < _network.InputCount; ivar++)
                        {
                            temp = truedist*_dsqr[ivar]
                                   *pair.Ideal[outvar];
                            _v[vptr++] += temp;
                            _w[wptr++] += temp*(2.0d*_dsqr[ivar] - 3.0d);
                        }
                    }
                    for (ivar = 0; ivar < _network.InputCount; ivar++)
                    {
                        temp = truedist*_dsqr[ivar];
                        _v[vsptr + ivar] += temp;
                        _w[wsptr + ivar] += temp
                                           *(2.0d*_dsqr[ivar] - 3.0d);
                    }
                    psum += dist;
                }
            }

            if (_network.OutputMode == PNNOutputMode.Classification)
            {
                psum = 0.0d;
                for (pop = 0; pop < _network.OutputCount; pop++)
                {
                    if (_network.Priors[pop] >= 0.0d)
                    {
                        xout[pop] *= _network.Priors[pop]
                                     /_network.CountPer[pop];
                    }
                    psum += xout[pop];
                }

                if (psum < 1.0e-40d)
                {
                    psum = 1.0e-40d;
                }
            }

            for (pop = 0; pop < _network.OutputCount; pop++)
            {
                xout[pop] /= psum;
            }

            for (ivar = 0; ivar < _network.InputCount; ivar++)
            {
                if (_network.OutputMode == PNNOutputMode.Classification)
                {
                    vtot = wtot = 0.0d;
                }
                else
                {
                    vtot = _v[vsptr + ivar]*2.0d
                           /(psum*_network.Sigma[ivar]);
                    wtot = _w[wsptr + ivar]
                           *2.0d
                           /(psum*_network.Sigma[ivar]*_network.Sigma[ivar]);
                }

                for (outvar = 0; outvar < _network.OutputCount; outvar++)
                {
                    if ((_network.OutputMode == PNNOutputMode.Classification)
                        && (_network.Priors[outvar] >= 0.0d))
                    {
                        _v[outvar*_network.InputCount + ivar] *= _network.Priors[outvar]
                                                               /_network.CountPer[outvar];
                        _w[outvar*_network.InputCount + ivar] *= _network.Priors[outvar]
                                                               /_network.CountPer[outvar];
                    }
                    _v[outvar*_network.InputCount + ivar] *= 2.0d/(psum*_network.Sigma[ivar]);

                    _w[outvar*_network.InputCount + ivar] *= 2.0d/(psum
                                                                 *_network.Sigma[ivar]*_network.Sigma[ivar]);
                    if (_network.OutputMode == PNNOutputMode.Classification)
                    {
                        vtot += _v[outvar*_network.InputCount + ivar];
                        wtot += _w[outvar*_network.InputCount + ivar];
                    }
                }

                for (outvar = 0; outvar < _network.OutputCount; outvar++)
                {
                    der1 = _v[outvar*_network.InputCount + ivar]
                           - xout[outvar]*vtot;
                    der2 = _w[outvar*_network.InputCount + ivar]
                           + 2.0d*xout[outvar]*vtot*vtot - 2.0d
                                                           *_v[outvar*_network.InputCount + ivar]
                                                           *vtot - xout[outvar]*wtot;
                    if (_network.OutputMode == PNNOutputMode.Classification)
                    {
                        if (outvar == (int) target[0])
                        {
                            temp = 2.0d*(xout[outvar] - 1.0d);
                        }
                        else
                        {
                            temp = 2.0d*xout[outvar];
                        }
                    }
                    else
                    {
                        temp = 2.0d*(xout[outvar] - target[outvar]);
                    }
                    _network.Deriv[ivar] += temp*der1;
                    _network.Deriv2[ivar] += temp*der2 + 2.0d*der1
                                            *der1;
                }
            }

            return new BasicMLData(xout);
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            if (!_samplesLoaded)
            {
                _network.Samples = new BasicMLDataSet(_training);
                _samplesLoaded = true;
            }

            var globalMinimum = new GlobalMinimumSearch();
            var dermin = new DeriveMinimum();

            int k;

            if (_network.OutputMode == PNNOutputMode.Classification)
            {
                k = _network.OutputCount;
            }
            else
            {
                k = _network.OutputCount + 1;
            }

            _dsqr = new double[_network.InputCount];
            _v = new double[_network.InputCount*k];
            _w = new double[_network.InputCount*k];

            var x = new double[_network.InputCount];
            var bs = new double[_network.InputCount];
            var direc = new double[_network.InputCount];
            var g = new double[_network.InputCount];
            var h = new double[_network.InputCount];
            var dwk2 = new double[_network.InputCount];

            if (_network.Trained)
            {
                for (int i = 0; i < _network.InputCount; i++)
                {
                    x[i] = _network.Sigma[i];
                }
                globalMinimum.Y2 = 1.0e30d;
            }
            else
            {
                globalMinimum.FindBestRange(_sigmaLow, _sigmaHigh,
                                            _numSigmas, true, _maxError, this);

                for (int i = 0; i < _network.InputCount; i++)
                {
                    x[i] = globalMinimum.X2;
                }
            }

            double d = dermin.Calculate(32767, _maxError, 1.0e-8d,
                                        _minImprovement, this, _network.InputCount, x,
                                        globalMinimum.Y2, bs, direc, g, h, dwk2);
            globalMinimum.Y2 = d;

            for (int i = 0; i < _network.InputCount; i++)
            {
                _network.Sigma[i] = x[i];
            }

            _network.Error = Math.Abs(globalMinimum.Y2);
            _network.Trained = true; // Tell other routines net is trained

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed TrainingContinuation Pause()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override void Resume(TrainingContinuation state)
        {
        }
    }
}
