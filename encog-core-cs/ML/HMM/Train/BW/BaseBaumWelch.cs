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
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.HMM.Alog;
using Encog.ML.HMM.Distributions;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Util;

namespace Encog.ML.HMM.Train.BW
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
        private readonly IMLSequenceSet _training;
        private HiddenMarkovModel _method;

        protected BaseBaumWelch(HiddenMarkovModel hmm,
                             IMLSequenceSet training)
        {
            _method = hmm;
            _training = training;
        }

        private int Iterations { get; set; }

        #region IMLTrain Members

        public void AddStrategy(IStrategy strategy)
        {
        }

        public bool CanContinue
        {
            get { return false; }
        }

        public void FinishTraining()
        {
        }

        public double Error
        {
            get { return 0; }
            set { }
        }

        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }

        public IMLMethod Method
        {
            get { return _method; }
        }

        public IList<IStrategy> Strategies
        {
            get { return null; }
        }

        public IMLDataSet Training
        {
            get { return _training; }
        }

        public bool TrainingDone
        {
            get { return false; }
        }

        public void Iteration()
        {
            HiddenMarkovModel nhmm;
            nhmm = _method.Clone();

            var allGamma = new double[_training.SequenceCount][][];
            double[][] aijNum = EngineArray.AllocateDouble2D(_method.StateCount, _method.StateCount);
            var aijDen = new double[_method.StateCount];

            EngineArray.Fill(aijDen, 0.0);
            for (int i = 0; i < _method.StateCount; i++)
            {
                EngineArray.Fill(aijNum[i], 0.0);
            }

            int g = 0;
            foreach (IMLDataSet obsSeq in _training.Sequences)
            {
                ForwardBackwardCalculator fbc = GenerateForwardBackwardCalculator(
                    obsSeq, _method);

                double[][][] xi = EstimateXi(obsSeq, fbc, _method);
                double[][] gamma = allGamma[g++] = EstimateGamma(xi, fbc);

                for (int i = 0; i < _method.StateCount; i++)
                {
                    for (int t = 0; t < (obsSeq.Count - 1); t++)
                    {
                        aijDen[i] += gamma[t][i];

                        for (int j = 0; j < _method.StateCount; j++)
                        {
                            aijNum[i][j] += xi[t][i][j];
                        }
                    }
                }
            }

            for (int i = 0; i < _method.StateCount; i++)
            {
                if (aijDen[i] == 0.0)
                {
                    for (int j = 0; j < _method.StateCount; j++)
                    {
                        nhmm.TransitionProbability[i][j] =
                            _method.TransitionProbability[i][j];
                    }
                }
                else
                {
                    for (int j = 0; j < _method.StateCount; j++)
                    {
                        nhmm.TransitionProbability[i][j] = aijNum[i][j]
                                                           /aijDen[i];
                    }
                }
            }

            /* compute pi */
            for (int i = 0; i < _method.StateCount; i++)
            {
                nhmm.Pi[i] = 0.0;
            }

            for (int o = 0; o < _training.SequenceCount; o++)
            {
                for (int i = 0; i < _method.StateCount; i++)
                {
                    nhmm.Pi[i] += (allGamma[o][0][i]/_training
                                                         .SequenceCount);
                }
            }

            /* compute pdfs */
            for (int i = 0; i < _method.StateCount; i++)
            {
                var weights = new double[_training.Count];
                double sum = 0.0;
                int j = 0;

                int o = 0;
                foreach (IMLDataSet obsSeq in _training.Sequences)
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
                opdf.Fit(_training, weights);
            }

            _method = nhmm;
            Iterations++;
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

        #endregion

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

        public abstract ForwardBackwardCalculator GenerateForwardBackwardCalculator(
            IMLDataSet sequence, HiddenMarkovModel hmm);
    }
}
