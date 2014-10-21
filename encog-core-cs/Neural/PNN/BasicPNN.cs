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
using Encog.Util;
using Encog.Util.Simple;

namespace Encog.Neural.PNN
{
    /// <summary>
    /// This class implements either a:
    /// Probabilistic Neural Network (PNN)
    /// General Regression Neural Network (GRNN)
    /// To use a PNN specify an output mode of classification, to make use of a GRNN
    /// specify either an output mode of regression or un-supervised autoassociation.
    /// The PNN/GRNN networks are potentially very useful. They share some
    /// similarities with RBF-neural networks and also the Support Vector Machine
    /// (SVM). These network types directly support the use of classification.
    /// The following book was very helpful in implementing PNN/GRNN's in Encog.
    /// Advanced Algorithms for Neural Networks: A C++ Sourcebook
    /// by Timothy Masters, PhD (http://www.timothymasters.info/) John Wiley Sons
    /// Inc (Computers); April 3, 1995, ISBN: 0471105880
    /// </summary>
    [Serializable]
    public class BasicPNN : AbstractPNN, IMLRegression, IMLClassification, IMLError
    {      
        /// <summary>
        /// The sigma's specify the widths of each kernel used.
        /// </summary>
        ///
        private readonly double[] _sigma;

        /// <summary>
        /// Used for classification, the number of cases in each class.
        /// </summary>
        ///
        private int[] _countPer;

        /// <summary>
        /// The prior probability weights.
        /// </summary>
        ///
        private double[] _priors;

        /// <summary>
        /// The training samples that form the memory of this network.
        /// </summary>
        ///
        private BasicMLDataSet _samples;

        /// <summary>
        /// Construct a BasicPNN network.
        /// </summary>
        ///
        /// <param name="kernel">The kernel to use.</param>
        /// <param name="outmodel">The output model for this network.</param>
        /// <param name="inputCount">The number of inputs in this network.</param>
        /// <param name="outputCount">The number of outputs in this network.</param>
        public BasicPNN(PNNKernelType kernel, PNNOutputMode outmodel,
                        int inputCount, int outputCount) : base(kernel, outmodel, inputCount, outputCount)
        {
            SeparateClass = false;

            _sigma = new double[inputCount];
        }


        /// <value>the countPer</value>
        public int[] CountPer
        {
            get { return _countPer; }
        }


        /// <value>the priors</value>
        public double[] Priors
        {
            get { return _priors; }
        }


        /// <value>the samples to set</value>
        public BasicMLDataSet Samples
        {        
            get { return _samples; }
            set
            {
                _samples = value;

                // update counts per
                if (OutputMode == PNNOutputMode.Classification)
                {
                    _countPer = new int[OutputCount];
                    _priors = new double[OutputCount];


                    foreach (IMLDataPair pair  in  value)
                    {
                        var i = (int) pair.Ideal[0];
                        if (i >= _countPer.Length)
                        {
                            throw new NeuralNetworkError(
                                "Training data contains more classes than neural network has output neurons to hold.");
                        }
                        _countPer[i]++;
                    }

                    for (int i = 0; i < _priors.Length; i++)
                    {
                        _priors[i] = -1;
                    }
                }
            }
        }


        /// <value>the sigma</value>
        public double[] Sigma
        {
            get { return _sigma; }
        }

        /// <summary>
        /// Compute the output from this network.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public override sealed IMLData Compute(IMLData input)
        {
            var xout = new double[OutputCount];

            double psum = 0.0d;

            int r = -1;

            foreach (IMLDataPair pair  in  _samples)
            {
                r++;

                if (r == Exclude)
                {
                    continue;
                }

                double dist = 0.0d;
                for (int i = 0; i < InputCount; i++)
                {
                    double diff = input[i] - pair.Input[i];
                    diff /= _sigma[i];
                    dist += diff*diff;
                }

                if (Kernel == PNNKernelType.Gaussian)
                {
                    dist = Math.Exp(-dist);
                }
                else if (Kernel == PNNKernelType.Reciprocal)
                {
                    dist = 1.0d/(1.0d + dist);
                }

                if (dist < 1.0e-40d)
                {
                    dist = 1.0e-40d;
                }

                if (OutputMode == PNNOutputMode.Classification)
                {
                    var pop = (int) pair.Ideal[0];
                    xout[pop] += dist;
                }
                else if (OutputMode == PNNOutputMode.Unsupervised)
                {
                    for (int i = 0; i < InputCount; i++)
                    {
                        xout[i] += dist*pair.Input[i];
                    }
                    psum += dist;
                }
                else if (OutputMode == PNNOutputMode.Regression)
                {
                    for (int i = 0; i < OutputCount; i++)
                    {
                        xout[i] += dist*pair.Ideal[i];
                    }

                    psum += dist;
                }
            }

            if (OutputMode == PNNOutputMode.Classification)
            {
                psum = 0.0d;
                for (int i = 0; i < OutputCount; i++)
                {
                    if (_priors[i] >= 0.0d)
                    {
                        xout[i] *= _priors[i]/_countPer[i];
                    }
                    psum += xout[i];
                }

                if (psum < 1.0e-40d)
                {
                    psum = 1.0e-40d;
                }

                for (int i = 0; i < OutputCount; i++)
                {
                    xout[i] /= psum;
                }
            }
            else if (OutputMode == PNNOutputMode.Unsupervised)
            {
                for (int i = 0; i < InputCount; i++)
                {
                    xout[i] /= psum;
                }
            }
            else if (OutputMode == PNNOutputMode.Regression)
            {
                for (int i = 0; i < OutputCount; i++)
                {
                    xout[i] /= psum;
                }
            }

            return new BasicMLData(xout, false);
        }

        /// <inheritdoc/>
        public override void UpdateProperties()
        {
            // unneeded
        }


        /// <inheritdoc/>
        public double CalculateError(IMLDataSet data)
        {
            if (OutputMode == PNNOutputMode.Classification)
            {
                return EncogUtility.CalculateClassificationError(this, data);
            }
            else
            {
                return EncogUtility.CalculateRegressionError(this, data);
            }
        }

        /// <inheritdoc/>
        public int Classify(IMLData input)
        {
            IMLData output = Compute(input);
            return EngineArray.MaxIndex(output);
        }
    }
}
