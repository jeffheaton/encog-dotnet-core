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
using Encog.Util;

namespace Encog.ML.HMM.Alog
{
    /// <summary>
    /// The forward-backward algorithm is an inference algorithm for hidden Markov
    /// models which computes the posterior marginals of all hidden state variables
    /// given a sequence of observations.
    /// </summary>
    public class ForwardBackwardCalculator
    {
        /// <summary>
        /// Alpha matrix.
        /// </summary>
        protected double[][] Alpha;

        /// <summary>
        /// Beta matrix.
        /// </summary>
        protected double[][] Beta;

        /// <summary>
        /// Probability.
        /// </summary>
        protected double probability;

        /// <summary>
        /// Construct an empty object.
        /// </summary>
        protected ForwardBackwardCalculator()
        {
        }

        /// <summary>
        /// Construct the forward/backward calculator. 
        /// </summary>
        /// <param name="oseq">The sequence to use.</param>
        /// <param name="hmm">THe hidden markov model to use.</param>
        public ForwardBackwardCalculator(IMLDataSet oseq,
                                         HiddenMarkovModel hmm)
            : this(oseq, hmm, true, false)
        {
        }


        /// <summary>
        /// Construct the object. 
        /// </summary>
        /// <param name="oseq">The sequence.</param>
        /// <param name="hmm">The hidden markov model to use.</param>
        /// <param name="doAlpha">Do alpha?</param>
        /// <param name="doBeta">Do beta?</param>
        public ForwardBackwardCalculator(IMLDataSet oseq,
                                         HiddenMarkovModel hmm, bool doAlpha, bool doBeta)
        {
            if (oseq.Count < 1)
            {
                throw new EncogError("Empty sequence");
            }

            if (doAlpha)
            {
                ComputeAlpha(hmm, oseq);
            }

            if (doBeta)
            {
                ComputeBeta(hmm, oseq);
            }

            ComputeProbability(oseq, hmm, doAlpha, doBeta);
        }

        /// <summary>
        /// Alpha element.
        /// </summary>
        /// <param name="t">The row.</param>
        /// <param name="i">The column.</param>
        /// <returns>The element.</returns>
        public double AlphaElement(int t, int i)
        {
            if (Alpha == null)
            {
                throw new EncogError("Alpha array has not "
                                     + "been computed");
            }

            return Alpha[t][i];
        }


        /// <summary>
        /// Beta element, best element. 
        /// </summary>
        /// <param name="t">From.</param>
        /// <param name="i">To.</param>
        /// <returns>The element.</returns>
        public double BetaElement(int t, int i)
        {
            if (Beta == null)
            {
                throw new EncogError("Beta array has not "
                                     + "been computed");
            }

            return Beta[t][i];
        }

        /// <summary>
        /// Compute alpha. 
        /// </summary>
        /// <param name="hmm">The hidden markov model.</param>
        /// <param name="oseq">The sequence.</param>
        protected void ComputeAlpha(HiddenMarkovModel hmm,
                                    IMLDataSet oseq)
        {
            Alpha = EngineArray.AllocateDouble2D((int) oseq.Count, hmm.StateCount);

            for (int i = 0; i < hmm.StateCount; i++)
            {
                ComputeAlphaInit(hmm, oseq[0], i);
            }

            IEnumerator<IMLDataPair> seqIterator = oseq.GetEnumerator();
            if (seqIterator.MoveNext())
            {
                for (int t = 1; t < oseq.Count; t++)
                {
                    seqIterator.MoveNext(); /////
                    IMLDataPair observation = seqIterator.Current;

                    for (int i = 0; i < hmm.StateCount; i++)
                    {
                        ComputeAlphaStep(hmm, observation, t, i);
                    }
                }
            }
        }
       
        /// <summary>
        /// Compute the alpha init. 
        /// </summary>
        /// <param name="hmm">THe hidden markov model.</param>
        /// <param name="o">The element.</param>
        /// <param name="i">The state.</param>
        protected void ComputeAlphaInit(HiddenMarkovModel hmm,
                                        IMLDataPair o, int i)
        {
            Alpha[0][i] = hmm.GetPi(i)
                          *hmm.StateDistributions[i].Probability(o);
        }

        /// <summary>
        /// Compute the alpha step. 
        /// </summary>
        /// <param name="hmm">The hidden markov model.</param>
        /// <param name="o">The sequence element.</param>
        /// <param name="t">The alpha step.</param>
        /// <param name="j">The column.</param>
        protected void ComputeAlphaStep(HiddenMarkovModel hmm,
                                        IMLDataPair o, int t, int j)
        {
            double sum = 0.0;

            for (int i = 0; i < hmm.StateCount; i++)
            {
                sum += Alpha[t - 1][i]*hmm.TransitionProbability[i][j];
            }

            Alpha[t][j] = sum*hmm.StateDistributions[j].Probability(o);
        }


        
        /// <summary>
        /// Compute the beta step. 
        /// </summary>
        /// <param name="hmm">The hidden markov model.</param>
        /// <param name="oseq">The sequence.</param>
        protected void ComputeBeta(HiddenMarkovModel hmm, IMLDataSet oseq)
        {
            Beta = EngineArray.AllocateDouble2D((int) oseq.Count, hmm.StateCount);

            for (int i = 0; i < hmm.StateCount; i++)
            {
                Beta[oseq.Count - 1][i] = 1.0;
            }

            for (var t = (int) (oseq.Count - 2); t >= 0; t--)
            {
                for (int i = 0; i < hmm.StateCount; i++)
                {
                    ComputeBetaStep(hmm, oseq[t + 1], t, i);
                }
            }
        }

        /// <summary>
        /// Compute the beta step. 
        /// </summary>
        /// <param name="hmm">The hidden markov model.</param>
        /// <param name="o">THe data par to compute.</param>
        /// <param name="t">THe matrix row.</param>
        /// <param name="i">THe matrix column.</param>
        protected void ComputeBetaStep(HiddenMarkovModel hmm,
                                       IMLDataPair o, int t, int i)
        {
            double sum = 0.0;

            for (int j = 0; j < hmm.StateCount; j++)
            {
                sum += Beta[t + 1][j]*hmm.TransitionProbability[i][j]
                       *hmm.StateDistributions[j].Probability(o);
            }

            Beta[t][i] = sum;
        }

        /// <summary>
        /// Compute the probability. 
        /// </summary>
        /// <param name="oseq">The sequence.</param>
        /// <param name="hmm">THe hidden markov model.</param>
        /// <param name="doAlpha">Perform alpha step?</param>
        /// <param name="doBeta">Perform beta step?</param>
        private void ComputeProbability(IMLDataSet oseq,
                                        HiddenMarkovModel hmm, bool doAlpha, bool doBeta)
        {
            probability = 0.0;

            if (doAlpha)
            {
                for (int i = 0; i < hmm.StateCount; i++)
                {
                    probability += Alpha[oseq.Count - 1][i];
                }
            }
            else
            {
                for (int i = 0; i < hmm.StateCount; i++)
                {
                    probability += hmm.GetPi(i)
                                   *hmm.StateDistributions[i].Probability(oseq[0])
                                   *Beta[0][i];
                }
            }
        }

        /// <summary>
        /// The probability.
        /// </summary>
        /// <returns>The probability.</returns>
        public double Probability()
        {
            return probability;
        }
    }
}
