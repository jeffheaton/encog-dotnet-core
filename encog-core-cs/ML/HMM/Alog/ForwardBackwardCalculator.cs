using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        protected double[][] alpha = null;

        /// <summary>
        /// Beta matrix.
        /// </summary>
        protected double[][] beta = null;

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
            if (this.alpha == null)
            {
                throw new EncogError("Alpha array has not "
                        + "been computed");
            }

            return this.alpha[t][i];
        }

        /**
         * Beta element, best element.
         * @param t From.
         * @param i To.
         * @return The element.
         */
        public double BetaElement(int t, int i)
        {
            if (this.beta == null)
            {
                throw new EncogError("Beta array has not "
                        + "been computed");
            }

            return this.beta[t][i];
        }

        /**
         * Compute alpha.
         * @param hmm The hidden markov model.
         * @param oseq The sequence.
         */
        protected void ComputeAlpha(HiddenMarkovModel hmm,
                IMLDataSet oseq)
        {
            this.alpha = EngineArray.AllocateDouble2D((int)oseq.Count, hmm.StateCount);

            for (int i = 0; i < hmm.StateCount; i++)
            {
                ComputeAlphaInit(hmm, oseq[0], i);
            }

            IEnumerator<IMLDataPair> seqIterator = oseq.GetEnumerator();
            if (seqIterator.MoveNext())
            {
                for (int t = 1; t < oseq.Count; t++)
                {
                    seqIterator.MoveNext();/////
                    IMLDataPair observation = seqIterator.Current;

                    for (int i = 0; i < hmm.StateCount; i++)
                    {
                        ComputeAlphaStep(hmm, observation, t, i);
                    }
                }

            }


        }

        /**
         * Compute the alpha init.
         * @param hmm THe hidden markov model.
         * @param o The element.
         * @param i The state.
         */
        protected void ComputeAlphaInit(HiddenMarkovModel hmm,
                 IMLDataPair o, int i)
        {
            this.alpha[0][i] = hmm.GetPi(i)
                    * hmm.getStateDistribution(i).Probability(o);
        }

        /**
         * Compute the alpha step.
         * @param hmm The hidden markov model.
         * @param o The sequence element.
         * @param t The alpha step.
         * @param j Thr column.
         */
        protected void ComputeAlphaStep(HiddenMarkovModel hmm,
                IMLDataPair o, int t, int j) {
		double sum = 0.0;

		for (int i = 0; i < hmm.StateCount; i++) {
			sum += this.alpha[t - 1][i] * hmm.getTransitionProbability(i, j);
		}

		this.alpha[t][j] = sum * hmm.getStateDistribution(j).Probability(o);
	}

        /**
         * Compute the beta step.
         * @param hmm The hidden markov model.
         * @param oseq The sequence.
         */
        protected void ComputeBeta(HiddenMarkovModel hmm, IMLDataSet oseq) {
		this.beta = EngineArray.AllocateDouble2D((int)oseq.Count,hmm.StateCount);

		for (int i = 0; i < hmm.StateCount; i++) {
			this.beta[oseq.Count - 1][i] = 1.0;
		}

		for (int t = (int)(oseq.Count - 2); t >= 0; t--) {
			for (int i = 0; i < hmm.StateCount; i++) {
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
                sum += this.beta[t + 1][j] * hmm.getTransitionProbability(i, j)
                        * hmm.getStateDistribution(j).Probability(o);
            }

            this.beta[t][i] = sum;
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
            this.probability = 0.0;

            if (doAlpha)
            {
                for (int i = 0; i < hmm.StateCount; i++)
                {
                    this.probability += this.alpha[oseq.Count - 1][i];
                }
            }
            else
            {
                for (int i = 0; i < hmm.StateCount; i++)
                {
                    this.probability += hmm.GetPi(i)
                            * hmm.getStateDistribution(i).Probability(oseq[0])
                            * this.beta[0][i];
                }
            }
        }

        /// <summary>
        /// The probability.
        /// </summary>
        /// <returns>The probability.</returns>
        public double Probability()
        {
            return this.probability;
        }
    }
}
