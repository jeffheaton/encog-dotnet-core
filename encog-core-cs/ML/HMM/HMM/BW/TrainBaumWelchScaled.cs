using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.HMM.Alog;
using Encog.Util;

namespace Encog.ML.HMM.HMM.BW
{
    /// <summary>
    /// Baum Welch Learning allows a HMM to be constructed from a series of sequence
    /// observations. This implementation of Baum Welch scales and is not as 
    /// susceptible to underflows in long sequences of data as the regular Baum Welch
    /// algorithm.
    /// 
    /// Baum Welch requires a starting point.  You should create a HMM that has a
    /// reasonable guess as to the observation and transition probabilities.  If you 
    /// can make no such guess, you should consider using KMeans training.
    /// 
    /// L. E. Baum, T. Petrie, G. Soules, and N. Weiss,
    /// "A maximization technique occurring in the statistical analysis of probabilistic functions of Markov chains"
    /// , Ann. Math. Statist., vol. 41, no. 1, pp. 164-171, 1970.
    /// 
    /// Hidden Markov Models and the Baum-Welch Algorithm, IEEE Information Theory
    /// Society Newsletter, Dec. 2003.
    /// </summary>
    public class TrainBaumWelchScaled : BaseBaumWelch
    {
        public TrainBaumWelchScaled(HiddenMarkovModel hmm,
             IMLSequenceSet training)
            : base(hmm, training)
        {

        }

        public override double[][][] EstimateXi(IMLDataSet sequence,
                ForwardBackwardCalculator fbc, HiddenMarkovModel hmm)
        {
            if (sequence.Count <= 1)
            {
                throw new EncogError(
                        "Must have more than one observation");
            }

            double[][][] xi = EngineArray.AllocDouble3D((int)sequence.Count - 1, hmm
                    .StateCount, hmm.StateCount);

            for (int t = 0; t < (sequence.Count - 1); t++)
            {
                IMLDataPair observation = sequence[t];

                for (int i = 0; i < hmm.StateCount; i++)
                {
                    for (int j = 0; j < hmm.StateCount; j++)
                    {
                        xi[t][i][j] = fbc.AlphaElement(t, i)
                                * hmm.TransitionProbability[i][j]
                                * hmm.StateDistributions[j].Probability(
                                        observation) * fbc.BetaElement(t + 1, j);
                    }
                }
            }

            return xi;
        }

        public override ForwardBackwardCalculator GenerateForwardBackwardCalculator(
                IMLDataSet sequence, HiddenMarkovModel hmm)
        {
            return new ForwardBackwardScaledCalculator(sequence, hmm,
                    true, true);
        }
    }
}
