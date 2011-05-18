using System;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Util;

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
    public class BasicPNN : AbstractPNN, MLRegression
    {      
        /// <summary>
        /// The sigma's specify the widths of each kernel used.
        /// </summary>
        ///
        private readonly double[] sigma;

        /// <summary>
        /// Used for classification, the number of cases in each class.
        /// </summary>
        ///
        private int[] countPer;

        /// <summary>
        /// The prior probability weights.
        /// </summary>
        ///
        private double[] priors;

        /// <summary>
        /// The training samples that form the memory of this network.
        /// </summary>
        ///
        private BasicMLDataSet samples;

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

            sigma = new double[inputCount];
        }


        /// <value>the countPer</value>
        public int[] CountPer
        {
            /// <returns>the countPer</returns>
            get { return countPer; }
        }


        /// <value>the priors</value>
        public double[] Priors
        {
            /// <returns>the priors</returns>
            get { return priors; }
        }


        /// <value>the samples to set</value>
        public BasicMLDataSet Samples
        {
            /// <returns>the samples</returns>
            get { return samples; }
            /// <param name="samples_0">the samples to set</param>
            set
            {
                samples = value;

                // update counts per
                if (OutputMode == PNNOutputMode.Classification)
                {
                    countPer = new int[OutputCount];
                    priors = new double[OutputCount];


                    foreach (MLDataPair pair  in  value)
                    {
                        var i = (int) pair.Ideal[0];
                        if (i >= countPer.Length)
                        {
                            throw new NeuralNetworkError(
                                "Training data contains more classes than neural network has output neurons to hold.");
                        }
                        countPer[i]++;
                    }

                    for (int i_1 = 0; i_1 < priors.Length; i_1++)
                    {
                        priors[i_1] = -1;
                    }
                }
            }
        }


        /// <value>the sigma</value>
        public double[] Sigma
        {
            /// <returns>the sigma</returns>
            get { return sigma; }
        }

        #region MLRegression Members

        /// <summary>
        /// Compute the output from this network.
        /// </summary>
        ///
        /// <param name="input">The input to the network.</param>
        /// <returns>The output from the network.</returns>
        public override sealed MLData Compute(MLData input)
        {
            var xout = new double[OutputCount];

            double psum = 0.0d;

            int r = -1;

            foreach (MLDataPair pair  in  samples)
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
                    diff /= sigma[i];
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
                    for (int i_0 = 0; i_0 < InputCount; i_0++)
                    {
                        xout[i_0] += dist*pair.Input[i_0];
                    }
                    psum += dist;
                }
                else if (OutputMode == PNNOutputMode.Regression)
                {
                    for (int i_1 = 0; i_1 < OutputCount; i_1++)
                    {
                        xout[i_1] += dist*pair.Ideal[i_1];
                    }

                    psum += dist;
                }
            }

            if (OutputMode == PNNOutputMode.Classification)
            {
                psum = 0.0d;
                for (int i_2 = 0; i_2 < OutputCount; i_2++)
                {
                    if (priors[i_2] >= 0.0d)
                    {
                        xout[i_2] *= priors[i_2]/countPer[i_2];
                    }
                    psum += xout[i_2];
                }

                if (psum < 1.0e-40d)
                {
                    psum = 1.0e-40d;
                }

                for (int i_3 = 0; i_3 < OutputCount; i_3++)
                {
                    xout[i_3] /= psum;
                }

                MLData result = new BasicMLData(1);
                result[0] = EngineArray.MaxIndex(xout);
                return result;
            }
            else if (OutputMode == PNNOutputMode.Unsupervised)
            {
                for (int i_4 = 0; i_4 < InputCount; i_4++)
                {
                    xout[i_4] /= psum;
                }
            }
            else if (OutputMode == PNNOutputMode.Regression)
            {
                for (int i_5 = 0; i_5 < OutputCount; i_5++)
                {
                    xout[i_5] /= psum;
                }
            }

            return new BasicMLData(xout);
        }

        #endregion

        /// <inheritdoc/>
        public override void UpdateProperties()
        {
            // unneeded
        }
    }
}