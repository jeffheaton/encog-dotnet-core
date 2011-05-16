using System;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.PNN;

namespace Encog.Neural.Networks.Training.PNN
{
    /// <summary>
    /// Train a PNN.
    /// </summary>
    ///
    public class TrainBasicPNN : BasicTraining, CalculationCriteria
    {
        /// <summary>
        /// The default max error.
        /// </summary>
        ///
        public const double DEFAULT_MAX_ERROR = 0.0d;

        /// <summary>
        /// The default minimum improvement before stop.
        /// </summary>
        ///
        public const double DEFAULT_MIN_IMPROVEMENT = 0.0001d;

        /// <summary>
        /// THe default sigma low value.
        /// </summary>
        ///
        public const double DEFAULT_SIGMA_LOW = 0.0001d;

        /// <summary>
        /// The default sigma high value.
        /// </summary>
        ///
        public const double DEFAULT_SIGMA_HIGH = 10.0d;

        /// <summary>
        /// The default number of sigmas to evaluate between the low and high.
        /// </summary>
        ///
        public const int DEFAULT_NUM_SIGMAS = 10;

        /// <summary>
        /// The network to train.
        /// </summary>
        ///
        private readonly BasicPNN network;

        /// <summary>
        /// The training data.
        /// </summary>
        ///
        private readonly MLDataSet training;

        /// <summary>
        /// Temp storage for derivative computation.
        /// </summary>
        ///
        private double[] dsqr;

        /// <summary>
        /// The maximum error to allow.
        /// </summary>
        ///
        private double maxError;

        /// <summary>
        /// The minimum improvement allowed.
        /// </summary>
        ///
        private double minImprovement;

        /// <summary>
        /// The number of sigmas to evaluate between the low and high.
        /// </summary>
        ///
        private int numSigmas;

        /// <summary>
        /// Have the samples been loaded.
        /// </summary>
        ///
        private bool samplesLoaded;

        /// <summary>
        /// The high value for the sigma search.
        /// </summary>
        ///
        private double sigmaHigh;

        /// <summary>
        /// The low value for the sigma search.
        /// </summary>
        ///
        private double sigmaLow;

        /// <summary>
        /// Temp storage for derivative computation.
        /// </summary>
        ///
        private double[] v;

        /// <summary>
        /// Temp storage for derivative computation.
        /// </summary>
        ///
        private double[] w;

        /// <summary>
        /// Train a BasicPNN.
        /// </summary>
        ///
        /// <param name="network_0">The network to train.</param>
        /// <param name="training_1">The training data.</param>
        public TrainBasicPNN(BasicPNN network_0, MLDataSet training_1) : base(TrainingImplementationType.OnePass)
        {
            network = network_0;
            training = training_1;

            maxError = DEFAULT_MAX_ERROR;
            minImprovement = DEFAULT_MIN_IMPROVEMENT;
            sigmaLow = DEFAULT_SIGMA_LOW;
            sigmaHigh = DEFAULT_SIGMA_HIGH;
            numSigmas = DEFAULT_NUM_SIGMAS;
            samplesLoaded = false;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed bool CanContinue
        {
            get { return false; }
        }


        /// <value>the maxError to set</value>
        public double MaxError
        {
            /// <returns>the maxError</returns>
            get { return maxError; }
            /// <param name="maxError_0">the maxError to set</param>
            set { maxError = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override MLMethod Method
        {
            /// <summary>
            /// 
            /// </summary>
            ///
            get { return network; }
        }


        /// <value>the minImprovement to set</value>
        public double MinImprovement
        {
            /// <returns>the minImprovement</returns>
            get { return minImprovement; }
            /// <param name="minImprovement_0">the minImprovement to set</param>
            set { minImprovement = value; }
        }


        /// <value>the numSigmas to set</value>
        public int NumSigmas
        {
            /// <returns>the numSigmas</returns>
            get { return numSigmas; }
            /// <param name="numSigmas_0">the numSigmas to set</param>
            set { numSigmas = value; }
        }


        /// <value>the sigmaHigh to set</value>
        public double SigmaHigh
        {
            /// <returns>the sigmaHigh</returns>
            get { return sigmaHigh; }
            /// <param name="sigmaHigh_0">the sigmaHigh to set</param>
            set { sigmaHigh = value; }
        }


        /// <value>the sigmaLow to set</value>
        public double SigmaLow
        {
            /// <returns>the sigmaLow</returns>
            get { return sigmaLow; }
            /// <param name="sigmaLow_0">the sigmaLow to set</param>
            set { sigmaLow = value; }
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
            double err;

            for (ivar = 0; ivar < network.InputCount; ivar++)
            {
                network.Sigma[ivar] = x[ivar];
            }

            if (!der)
            {
                return CalculateError(network.Samples, false);
            }

            err = CalculateError(network.Samples, true);

            for (ivar = 0; ivar < network.InputCount; ivar++)
            {
                der1[ivar] = network.Deriv[ivar];
                der2[ivar] = network.Deriv2[ivar];
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

            for (ivar = 0; ivar < network.InputCount; ivar++)
            {
                network.Sigma[ivar] = sig;
            }

            return CalculateError(network.Samples, false);
        }

        #endregion

        /// <summary>
        /// Calculate the error for the entire training set.
        /// </summary>
        ///
        /// <param name="training_0">Training set to use.</param>
        /// <param name="deriv">Should we find the derivative.</param>
        /// <returns>The error.</returns>
        public double CalculateError(MLDataSet training_0,
                                     bool deriv)
        {
            double err, totErr;
            double diff;
            totErr = 0.0d;

            if (deriv)
            {
                int num = (network.SeparateClass)
                              ? network.InputCount*network.OutputCount
                              : network.InputCount;
                for (int i = 0; i < num; i++)
                {
                    network.Deriv[i] = 0.0d;
                    network.Deriv2[i] = 0.0d;
                }
            }

            network.Exclude = (int) training_0.Count;

            MLDataPair pair = BasicMLDataPair.CreatePair(
                training_0.InputSize, training_0.IdealSize);

            var xout = new double[network.OutputCount];

            for (int r = 0; r < training_0.Count; r++)
            {
                training_0.GetRecord(r, pair);
                network.Exclude = network.Exclude - 1;

                err = 0.0d;

                MLData input = pair.Input;
                MLData target = pair.Ideal;

                if (network.OutputMode == PNNOutputMode.Unsupervised)
                {
                    if (deriv)
                    {
                        MLData output = ComputeDeriv(input, target);
                        for (int z = 0; z < network.OutputCount; z++)
                        {
                            xout[z] = output[z];
                        }
                    }
                    else
                    {
                        MLData output_1 = network.Compute(input);
                        for (int z_2 = 0; z_2 < network.OutputCount; z_2++)
                        {
                            xout[z_2] = output_1[z_2];
                        }
                    }
                    for (int i_3 = 0; i_3 < network.OutputCount; i_3++)
                    {
                        diff = input[i_3] - xout[i_3];
                        err += diff*diff;
                    }
                }
                else if (network.OutputMode == PNNOutputMode.Classification)
                {
                    var tclass = (int) target[0];
                    MLData output_4;

                    if (deriv)
                    {
                        output_4 = ComputeDeriv(input, pair.Ideal);
                        //output_4.GetData(0); //**FIX**?
                    }
                    else
                    {
                        output_4 = network.Compute(input);
                        //output_4.GetData(0); **FIX**?
                    }

                    xout[0] = output_4[0];

                    for (int i_5 = 0; i_5 < xout.Length; i_5++)
                    {
                        if (i_5 == tclass)
                        {
                            diff = 1.0d - xout[i_5];
                            err += diff*diff;
                        }
                        else
                        {
                            err += xout[i_5]*xout[i_5];
                        }
                    }
                }

                else if (network.OutputMode == PNNOutputMode.Regression)
                {
                    if (deriv)
                    {
                        MLData output_6 = network.Compute(input);
                        for (int z_7 = 0; z_7 < network.OutputCount; z_7++)
                        {
                            xout[z_7] = output_6[z_7];
                        }
                    }
                    else
                    {
                        MLData output_8 = network.Compute(input);
                        for (int z_9 = 0; z_9 < network.OutputCount; z_9++)
                        {
                            xout[z_9] = output_8[z_9];
                        }
                    }
                    for (int i_10 = 0; i_10 < network.OutputCount; i_10++)
                    {
                        diff = target[i_10] - xout[i_10];
                        err += diff*diff;
                    }
                }

                totErr += err;
            }

            network.Exclude = -1;

            network.Error = totErr/training_0.Count;
            if (deriv)
            {
                for (int i_11 = 0; i_11 < network.Deriv.Length; i_11++)
                {
                    network.Deriv[i_11] /= training_0.Count;
                    network.Deriv2[i_11] /= training_0.Count;
                }
            }

            if ((network.OutputMode == PNNOutputMode.Unsupervised)
                || (network.OutputMode == PNNOutputMode.Regression))
            {
                network.Error = network.Error
                                /network.OutputCount;
                if (deriv)
                {
                    for (int i_12 = 0; i_12 < network.InputCount; i_12++)
                    {
                        network.Deriv[i_12] /= network.OutputCount;
                        network.Deriv2[i_12] /= network.OutputCount;
                    }
                }
            }

            return network.Error;
        }

        /// <summary>
        /// Compute the derivative for target data.
        /// </summary>
        ///
        /// <param name="input">The input.</param>
        /// <param name="target">The target data.</param>
        /// <returns>The output.</returns>
        public MLData ComputeDeriv(MLData input, MLData target)
        {
            int pop, ivar;
            int ibest = 0;
            int outvar;
            double diff, dist, truedist;
            double vtot, wtot;
            double temp, der1, der2, psum;
            int vptr, wptr, vsptr = 0, wsptr = 0;

            var xout = new double[network.OutputCount];

            for (pop = 0; pop < network.OutputCount; pop++)
            {
                xout[pop] = 0.0d;
                for (ivar = 0; ivar < network.InputCount; ivar++)
                {
                    v[pop*network.InputCount + ivar] = 0.0d;
                    w[pop*network.InputCount + ivar] = 0.0d;
                }
            }

            psum = 0.0d;

            if (network.OutputMode != PNNOutputMode.Classification)
            {
                vsptr = network.OutputCount
                        *network.InputCount;
                wsptr = network.OutputCount
                        *network.InputCount;
                for (ivar = 0; ivar < network.InputCount; ivar++)
                {
                    v[vsptr + ivar] = 0.0d;
                    w[wsptr + ivar] = 0.0d;
                }
            }

            MLDataPair pair = BasicMLDataPair.CreatePair(network.Samples.InputSize, network.Samples.IdealSize);

            for (int r = 0; r < network.Samples.Count; r++)
            {
                network.Samples.GetRecord(r, pair);

                if (r == network.Exclude)
                {
                    continue;
                }

                dist = 0.0d;
                for (ivar = 0; ivar < network.InputCount; ivar++)
                {
                    diff = input[ivar] - pair.Input[ivar];
                    diff /= network.Sigma[ivar];
                    dsqr[ivar] = diff*diff;
                    dist += dsqr[ivar];
                }

                if (network.Kernel == PNNKernelType.Gaussian)
                {
                    dist = Math.Exp(-dist);
                }
                else if (network.Kernel == PNNKernelType.Reciprocal)
                {
                    dist = 1.0d/(1.0d + dist);
                }

                truedist = dist;
                if (dist < 1.0e-40d)
                {
                    dist = 1.0e-40d;
                }

                if (network.OutputMode == PNNOutputMode.Classification)
                {
                    pop = (int) pair.Ideal[0];
                    xout[pop] += dist;
                    vptr = pop*network.InputCount;
                    wptr = pop*network.InputCount;
                    for (ivar = 0; ivar < network.InputCount; ivar++)
                    {
                        temp = truedist*dsqr[ivar];
                        v[vptr + ivar] += temp;
                        w[wptr + ivar] += temp*(2.0d*dsqr[ivar] - 3.0d);
                    }
                }

                else if (network.OutputMode == PNNOutputMode.Unsupervised)
                {
                    for (ivar = 0; ivar < network.InputCount; ivar++)
                    {
                        xout[ivar] += dist*pair.Input[ivar];
                        temp = truedist*dsqr[ivar];
                        v[vsptr + ivar] += temp;
                        w[wsptr + ivar] += temp
                                           *(2.0d*dsqr[ivar] - 3.0d);
                    }
                    vptr = 0;
                    wptr = 0;
                    for (outvar = 0; outvar < network.OutputCount; outvar++)
                    {
                        for (ivar = 0; ivar < network.InputCount; ivar++)
                        {
                            temp = truedist*dsqr[ivar]
                                   *pair.Input[ivar];
                            v[vptr++] += temp;
                            w[wptr++] += temp*(2.0d*dsqr[ivar] - 3.0d);
                        }
                    }
                    psum += dist;
                }
                else if (network.OutputMode == PNNOutputMode.Regression)
                {
                    for (ivar = 0; ivar < network.OutputCount; ivar++)
                    {
                        xout[ivar] += dist*pair.Ideal[ivar];
                    }
                    vptr = 0;
                    wptr = 0;
                    for (outvar = 0; outvar < network.OutputCount; outvar++)
                    {
                        for (ivar = 0; ivar < network.InputCount; ivar++)
                        {
                            temp = truedist*dsqr[ivar]
                                   *pair.Ideal[outvar];
                            v[vptr++] += temp;
                            w[wptr++] += temp*(2.0d*dsqr[ivar] - 3.0d);
                        }
                    }
                    for (ivar = 0; ivar < network.InputCount; ivar++)
                    {
                        temp = truedist*dsqr[ivar];
                        v[vsptr + ivar] += temp;
                        w[wsptr + ivar] += temp
                                           *(2.0d*dsqr[ivar] - 3.0d);
                    }
                    psum += dist;
                }
            }

            if (network.OutputMode == PNNOutputMode.Classification)
            {
                psum = 0.0d;
                for (pop = 0; pop < network.OutputCount; pop++)
                {
                    if (network.Priors[pop] >= 0.0d)
                    {
                        xout[pop] *= network.Priors[pop]
                                     /network.CountPer[pop];
                    }
                    psum += xout[pop];
                }

                if (psum < 1.0e-40d)
                {
                    psum = 1.0e-40d;
                }
            }

            for (pop = 0; pop < network.OutputCount; pop++)
            {
                xout[pop] /= psum;
            }

            for (ivar = 0; ivar < network.InputCount; ivar++)
            {
                if (network.OutputMode == PNNOutputMode.Classification)
                {
                    vtot = wtot = 0.0d;
                }
                else
                {
                    vtot = v[vsptr + ivar]*2.0d
                           /(psum*network.Sigma[ivar]);
                    wtot = w[wsptr + ivar]
                           *2.0d
                           /(psum*network.Sigma[ivar]*network.Sigma[ivar]);
                }

                for (outvar = 0; outvar < network.OutputCount; outvar++)
                {
                    if ((network.OutputMode == PNNOutputMode.Classification)
                        && (network.Priors[outvar] >= 0.0d))
                    {
                        v[outvar*network.InputCount + ivar] *= network.Priors[outvar]
                                                               /network.CountPer[outvar];
                        w[outvar*network.InputCount + ivar] *= network.Priors[outvar]
                                                               /network.CountPer[outvar];
                    }
                    v[outvar*network.InputCount + ivar] *= 2.0d/(psum*network.Sigma[ivar]);

                    w[outvar*network.InputCount + ivar] *= 2.0d/(psum
                                                                 *network.Sigma[ivar]*network.Sigma[ivar]);
                    if (network.OutputMode == PNNOutputMode.Classification)
                    {
                        vtot += v[outvar*network.InputCount + ivar];
                        wtot += w[outvar*network.InputCount + ivar];
                    }
                }

                for (outvar = 0; outvar < network.OutputCount; outvar++)
                {
                    der1 = v[outvar*network.InputCount + ivar]
                           - xout[outvar]*vtot;
                    der2 = w[outvar*network.InputCount + ivar]
                           + 2.0d*xout[outvar]*vtot*vtot - 2.0d
                                                           *v[outvar*network.InputCount + ivar]
                                                           *vtot - xout[outvar]*wtot;
                    if (network.OutputMode == PNNOutputMode.Classification)
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
                    network.Deriv[ivar] += temp*der1;
                    network.Deriv2[ivar] += temp*der2 + 2.0d*der1
                                            *der1;
                }
            }

            if (network.OutputMode == PNNOutputMode.Classification)
            {
                MLData result = new BasicMLData(1);
                result[0] = ibest;
                return result;
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed void Iteration()
        {
            if (!samplesLoaded)
            {
                network.Samples = new BasicMLDataSet(training);
                samplesLoaded = true;
            }

            var globalMinimum = new GlobalMinimumSearch();
            var dermin = new DeriveMinimum();

            int k;

            if (network.OutputMode == PNNOutputMode.Classification)
            {
                k = network.OutputCount;
            }
            else
            {
                k = network.OutputCount + 1;
            }

            dsqr = new double[network.InputCount];
            v = new double[network.InputCount*k];
            w = new double[network.InputCount*k];

            var x = new double[network.InputCount];
            var bs = new double[network.InputCount];
            var direc = new double[network.InputCount];
            var g = new double[network.InputCount];
            var h = new double[network.InputCount];
            var dwk2 = new double[network.InputCount];

            if (network.Trained)
            {
                k = 0;
                for (int i = 0; i < network.InputCount; i++)
                {
                    x[i] = network.Sigma[i];
                }
                globalMinimum.Y2 = 1.0e30d;
            }
            else
            {
                globalMinimum.FindBestRange(sigmaLow, sigmaHigh,
                                            numSigmas, true, maxError, this);

                for (int i_0 = 0; i_0 < network.InputCount; i_0++)
                {
                    x[i_0] = globalMinimum.X2;
                }
            }

            double d = dermin.Calculate(32767, maxError, 1.0e-8d,
                                        minImprovement, this, network.InputCount, x,
                                        globalMinimum.Y2, bs, direc, g, h, dwk2);
            globalMinimum.Y2 = d;

            for (int i_1 = 0; i_1 < network.InputCount; i_1++)
            {
                network.Sigma[i_1] = x[i_1];
            }

            network.Error = Math.Abs(globalMinimum.Y2);
            network.Trained = true; // Tell other routines net is trained

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