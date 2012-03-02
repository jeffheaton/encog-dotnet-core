using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.ML.Data;
using Encog.ML.HMM.Alog;
using Encog.ML.Train.Strategy;
using Encog.Util;
using Encog.ML.HMM.Distributions;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.HMM.HMM.BW
{
    /// <summary>
    /// This class provides the base implementation for Baum-Welch learning for
    /// HMM's. There are currently two implementations provided.
    /// 
    /// TrainBaumWelch - Regular Baum Welch Learning.
    /// 
    /// TrainBaumWelchScaled - Regular Baum Welch Learning, which can handle
    /// underflows in long sequences.
    /// 
    /// L. E. Baum, T. Petrie, G. Soules, and N. Weiss,
    /// "A maximization technique occurring in the statistical analysis of probabilistic functions of Markov chains"
    /// , Ann. Math. Statist., vol. 41, no. 1, pp. 164-171, 1970.
    /// 
    /// Hidden Markov Models and the Baum-Welch Algorithm, IEEE Information Theory
    /// </summary>
    public abstract class BaseBaumWelch : IMLTrain
    {
        private int Iterations { get; set; }
        private HiddenMarkovModel method;
        private IMLSequenceSet training;

        public BaseBaumWelch(HiddenMarkovModel hmm,
                IMLSequenceSet training)
        {
            this.method = hmm;
            this.training = training;
        }

        public void AddStrategy(IStrategy strategy)
        {

        }

        public bool CanContinue
        {
            get
            {
                return false;
            }
        }

        protected double[][] EstimateGamma(double[][][] xi,
                ForwardBackwardCalculator fbc)
        {
            double[][] gamma = EngineArray.AllocateDouble2D(xi.Length + 1, xi[0].Length);

            for (int t = 0; t < (xi.Length + 1); t++)
            {
                EngineArray.Fill(gamma[t], 0.0);
            }

            for (int t = 0; t < xi.Length; t++)
            {
                for (int i = 0; i < xi[0].Length; i++)
                {
                    for (int j = 0; j < xi[0].Length; j++)
                    {
                        gamma[t][i] += xi[t][i][j];
                    }
                }
            }

            for (int j = 0; j < xi[0].Length; j++)
            {
                for (int i = 0; i < xi[0].Length; i++)
                {
                    gamma[xi.Length][j] += xi[xi.Length - 1][i][j];
                }
            }

            return gamma;
        }

        public abstract double[][][] EstimateXi(IMLDataSet sequence,
                ForwardBackwardCalculator fbc, HiddenMarkovModel hmm);

        public void FinishTraining()
        {

        }

        public abstract ForwardBackwardCalculator GenerateForwardBackwardCalculator(
                IMLDataSet sequence, HiddenMarkovModel hmm);

        public double Error
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public TrainingImplementationType ImplementationType
        {
            get
            {
                return TrainingImplementationType.Iterative;
            }
        }

        public IMLMethod Method
        {
            get
            {
                return this.method;
            }
        }

        public IList<IStrategy> Strategies
        {
            get
            {
                return null;
            }
        }

        public IMLDataSet Training
        {
            get
            {
                return this.training;
            }
        }

        public bool TrainingDone
        {
            get
            {
                return false;
            }
        }

        public void Iteration()
        {
            HiddenMarkovModel nhmm;
            nhmm = this.method.Clone();

            double[][][] allGamma = new double[this.training.SequenceCount][][];
            double[][] aijNum = EngineArray.AllocateDouble2D(this.method.StateCount, this.method.StateCount);
            double[] aijDen = new double[this.method.StateCount];

            EngineArray.Fill(aijDen, 0.0);
            for (int i = 0; i < this.method.StateCount; i++)
            {
                EngineArray.Fill(aijNum[i], 0.0);
            }

            int g = 0;
            foreach (IMLDataSet obsSeq in this.training.Sequences)
            {
                ForwardBackwardCalculator fbc = GenerateForwardBackwardCalculator(
                        obsSeq, this.method);

                double[][][] xi = EstimateXi(obsSeq, fbc, this.method);
                double[][] gamma = allGamma[g++] = EstimateGamma(xi, fbc);

                for (int i = 0; i < this.method.StateCount; i++)
                {
                    for (int t = 0; t < (obsSeq.Count - 1); t++)
                    {
                        aijDen[i] += gamma[t][i];

                        for (int j = 0; j < this.method.StateCount; j++)
                        {
                            aijNum[i][j] += xi[t][i][j];
                        }
                    }
                }
            }

            for (int i = 0; i < this.method.StateCount; i++)
            {
                if (aijDen[i] == 0.0)
                {
                    for (int j = 0; j < this.method.StateCount; j++)
                    {
                        nhmm.TransitionProbability[i][j] =
                                this.method.TransitionProbability[i][j];
                    }
                }
                else
                {
                    for (int j = 0; j < this.method.StateCount; j++)
                    {
                        nhmm.TransitionProbability[i][j] = aijNum[i][j]
                                / aijDen[i];
                    }
                }
            }

            /* compute pi */
            for (int i = 0; i < this.method.StateCount; i++)
            {
                nhmm.Pi[i] = 0.0;
            }

            for (int o = 0; o < this.training.SequenceCount; o++)
            {
                for (int i = 0; i < this.method.StateCount; i++)
                {
                    nhmm.Pi[i] += (allGamma[o][0][i] / this.training
                                            .SequenceCount);
                }
            }

            /* compute pdfs */
            for (int i = 0; i < this.method.StateCount; i++)
            {

                double[] weights = new double[this.training.Count];
                double sum = 0.0;
                int j = 0;

                int o = 0;
                foreach (IMLDataSet obsSeq in this.training.Sequences)
                {
                    for (int t = 0; t < obsSeq.Count; t++, j++)
                    {
                        sum += weights[j] = allGamma[o][t][i];
                    }
                    o++;
                }

                for (j--; j >= 0; j--)
                {
                    weights[j] /= sum;
                }

                IStateDistribution opdf = nhmm.StateDistributions[i];
                opdf.Fit(this.training, weights);
            }

            this.method = nhmm;
        }

        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
        }

        public TrainingContinuation Pause()
        {
            return null;
        }

        public void Resume(TrainingContinuation state)
        {

        }

        public int IterationNumber { get; set; }
    }
}
