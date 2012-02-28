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
    /// given a sequence of observations. This version makes use of scaling, and will
    /// not generate underflows with long sequences.
    /// </summary>
    public class ForwardBackwardScaledCalculator : ForwardBackwardCalculator
    {
        private double[] ctFactors;
        private double lnProbability;

        public ForwardBackwardScaledCalculator(IMLDataSet oseq,
                HiddenMarkovModel hmm)
            : this(oseq, hmm, true, false)
        {

        }

        public ForwardBackwardScaledCalculator(
                IMLDataSet oseq, HiddenMarkovModel hmm,
                bool doAlpha, bool doBeta)
        {
            if (oseq.Count < 1)
            {
                throw new EncogError("Count cannot be less than one.");
            }

            this.ctFactors = new double[oseq.Count];
            EngineArray.Fill(this.ctFactors, 0.0);

            ComputeAlpha(hmm, oseq);

            if (doBeta)
            {
                ComputeBeta(hmm, oseq);
            }

            ComputeProbability(oseq, hmm, doAlpha, doBeta);
        }

        protected void ComputeAlpha(HiddenMarkovModel hmm,
                IMLDataSet oseq)
        {
            this.alpha = EngineArray.AllocateDouble2D((int)oseq.Count, hmm.StateCount);

            for (int i = 0; i < hmm.StateCount; i++)
            {
                ComputeAlphaInit(hmm, oseq[0], i);
            }
            Scale(this.ctFactors, this.alpha, 0);

            IEnumerator<IMLDataPair> seqIterator = oseq.GetEnumerator();
            if (seqIterator.MoveNext())
            {
                for (int t = 1; t < oseq.Count; t++)
                {
                    seqIterator.MoveNext();
                    IMLDataPair observation = seqIterator.Current;

                    for (int i = 0; i < hmm.StateCount; i++)
                    {
                        ComputeAlphaStep(hmm, observation, t, i);
                    }
                    Scale(this.ctFactors, this.alpha, t);
                }
            }


        }

        protected void ComputeBeta(HiddenMarkovModel hmm, IMLDataSet oseq) {
		this.beta = EngineArray.AllocateDouble2D((int)oseq.Count,hmm.StateCount);

		for (int i = 0; i < hmm.StateCount; i++) {
			this.beta[oseq.Count - 1][i] = 1.0 / this.ctFactors[oseq.Count - 1];
		}

		for (int t = (int)(oseq.Count - 2); t >= 0; t--) {
			for (int i = 0; i < hmm.StateCount; i++) {
				ComputeBetaStep(hmm, oseq[t + 1], t, i);
				this.beta[t][i] /= this.ctFactors[t];
			}
		}
	}

        private void ComputeProbability(IMLDataSet oseq,
                HiddenMarkovModel hmm, bool doAlha, bool doBeta)
        {
            this.lnProbability = 0.0;

            for (int t = 0; t < oseq.Count; t++)
            {
                this.lnProbability += Math.Log(this.ctFactors[t]);
            }

            this.probability = Math.Exp(this.lnProbability);
        }

        public double LnProbability()
        {
            return this.lnProbability;
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
