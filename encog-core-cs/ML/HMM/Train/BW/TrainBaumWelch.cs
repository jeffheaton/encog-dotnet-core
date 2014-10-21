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
using Encog.ML.Data;
using Encog.ML.HMM.Alog;
using Encog.Util;

namespace Encog.ML.HMM.Train.BW
{
    /// <summary>
    /// Baum Welch Learning allows a HMM to be constructed from a series of sequence
    /// observations. This implementation of Baum Welch does not scale and is
    /// susceptible to underflows in long sequences of data.
    /// 
    /// Baum Welch requires a starting point. You should create a HMM that has a
    /// reasonable guess as to the observation and transition probabilities. If you
    /// can make no such guess, you should consider using KMeans training.
    /// 
    /// L. E. Baum, T. Petrie, G. Soules, and N. Weiss,
    /// "A maximization technique occurring in the statistical analysis of probabilistic functions of Markov chains"
    /// , Ann. Math. Statist., vol. 41, no. 1, pp. 164-171, 1970.
    /// 
    /// Hidden Markov Models and the Baum-Welch Algorithm, IEEE Information Theory
    /// Society Newsletter, Dec. 2003.
    /// </summary>
    public class TrainBaumWelch : BaseBaumWelch
    {
        public TrainBaumWelch(HiddenMarkovModel hmm,
                              IMLSequenceSet training)
            : base(hmm, training)
        {
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

        public override double[][][] EstimateXi(IMLDataSet sequence,
                                                ForwardBackwardCalculator fbc, HiddenMarkovModel hmm)
        {
            if (sequence.Count <= 1)
            {
                throw new EncogError(
                    "Must have more than one observation");
            }

            double[][][] xi = EngineArray.AllocDouble3D((int) sequence.Count - 1, hmm.StateCount, hmm.StateCount);
            double probability = fbc.Probability();

            for (int t = 0; t < (sequence.Count - 1); t++)
            {
                IMLDataPair o = sequence[t + 1];

                for (int i = 0; i < hmm.StateCount; i++)
                {
                    for (int j = 0; j < hmm.StateCount; j++)
                    {
                        xi[t][i][j] = (fbc.AlphaElement(t, i)
                                       *hmm.TransitionProbability[i][j]
                                       *hmm.StateDistributions[j].Probability(o)*fbc
                                                                                     .BetaElement(t + 1, j))/probability;
                    }
                }
            }

            return xi;
        }

        public override ForwardBackwardCalculator GenerateForwardBackwardCalculator(
            IMLDataSet sequence, HiddenMarkovModel hmm)
        {
            return new ForwardBackwardCalculator(sequence, hmm,
                                                 true, true);
        }
    }
}
