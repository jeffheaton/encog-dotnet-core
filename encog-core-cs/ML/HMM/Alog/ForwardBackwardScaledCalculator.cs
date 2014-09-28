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
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.Util;

namespace Encog.ML.HMM.Alog
{
    /// <summary>
    /// The forward-backward algorithm is an inference algorithm for hidden Markov
    /// models which computes the posterior marginals of all hidden state variables
    /// given a sequence of observations. This version makes use of scaling, and will
    /// not generate underflows with long sequences.
    /// </summary>
    public class ForwardBackwardScaledCalculator : ForwardBackwardCalculator
    {
        /// <summary>
        /// The factors.
        /// </summary>
        private readonly double[] _ctFactors;

        /// <summary>
        /// Log probability.
        /// </summary>
        private double _lnProbability;

        /// <summary>
        /// Construct the calculator.
        /// </summary>
        /// <param name="seq">The sequences.</param>
        /// <param name="hmm">The Hidden Markov Model.</param>
        public ForwardBackwardScaledCalculator(IMLDataSet seq,
                                               HiddenMarkovModel hmm)
            : this(seq, hmm, true, false)
        {
        }

        /// <summary>
        /// Construct the calculator.
        /// </summary>
        /// <param name="seq">The sequence.</param>
        /// <param name="hmm">The HMM.</param>
        /// <param name="doAlpha">Should alpha be calculated.</param>
        /// <param name="doBeta">Should beta be calculated.</param>
        public ForwardBackwardScaledCalculator(
            IMLDataSet seq, HiddenMarkovModel hmm,
            bool doAlpha, bool doBeta)
        {
            if (seq.Count < 1)
            {
                throw new EncogError("Count cannot be less than one.");
            }

            _ctFactors = new double[seq.Count];
            EngineArray.Fill(_ctFactors, 0.0);

            ComputeAlpha(hmm, seq);

            if (doBeta)
            {
                ComputeBeta(hmm, seq);
            }

            ComputeProbability(seq, hmm, doAlpha, doBeta);
        }

        /// <summary>
        /// Compute alpha.
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        /// <param name="seq">The sequence.</param>
        protected void ComputeAlpha(HiddenMarkovModel hmm,
                                    IMLDataSet seq)
        {
            Alpha = EngineArray.AllocateDouble2D((int) seq.Count, hmm.StateCount);

            for (int i = 0; i < hmm.StateCount; i++)
            {
                ComputeAlphaInit(hmm, seq[0], i);
            }
            Scale(_ctFactors, Alpha, 0);

            IEnumerator<IMLDataPair> seqIterator = seq.GetEnumerator();
            if (seqIterator.MoveNext())
            {
                for (int t = 1; t < seq.Count; t++)
                {
                    seqIterator.MoveNext();
                    IMLDataPair observation = seqIterator.Current;

                    for (int i = 0; i < hmm.StateCount; i++)
                    {
                        ComputeAlphaStep(hmm, observation, t, i);
                    }
                    Scale(_ctFactors, Alpha, t);
                }
            }
        }

        /// <summary>
        /// Compute beta.
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        /// <param name="oseq">The sequence.</param>
        protected new void ComputeBeta(HiddenMarkovModel hmm, IMLDataSet oseq)
        {
            Beta = EngineArray.AllocateDouble2D((int) oseq.Count, hmm.StateCount);

            for (int i = 0; i < hmm.StateCount; i++)
            {
                Beta[oseq.Count - 1][i] = 1.0/_ctFactors[oseq.Count - 1];
            }

            for (var t = (int) (oseq.Count - 2); t >= 0; t--)
            {
                for (int i = 0; i < hmm.StateCount; i++)
                {
                    ComputeBetaStep(hmm, oseq[t + 1], t, i);
                    Beta[t][i] /= _ctFactors[t];
                }
            }
        }

        /// <summary>
        /// Compute the probability.
        /// </summary>
        /// <param name="seq">The sequence.</param>
        /// <param name="hmm">The HMM.</param>
        /// <param name="doAlha">The alpha.</param>
        /// <param name="doBeta">The beta.</param>
        private void ComputeProbability(IMLDataSet seq,
                                        HiddenMarkovModel hmm, bool doAlha, bool doBeta)
        {
            _lnProbability = 0.0;

            for (int t = 0; t < seq.Count; t++)
            {
                _lnProbability += Math.Log(_ctFactors[t]);
            }

            probability = Math.Exp(_lnProbability);
        }

        public double LnProbability()
        {
            return _lnProbability;
        }

        private void Scale(double[] ctFactors, double[][] array,
                           int t)
        {
            double[] table = array[t];
            double sum = 0.0;

            foreach (double element in table)
            {
                sum += element;
            }

            ctFactors[t] = sum;
            for (int i = 0; i < table.Length; i++)
            {
                table[i] /= sum;
            }
        }
    }
}
