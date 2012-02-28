using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;

namespace Encog.ML.HMM.Alog
{
    /// <summary>
    /// This class produces a Kullback-Leibler estimation of the distance between two
    /// HMMs. This allows the similarity of two different HMM's to be evaluated.
    /// 
    /// ^ Kullback, S.; Leibler, R.A. (1951). "On Information and Sufficiency".
    /// Annals of Mathematical Statistics 22 (1): 79-86. doi:10.1214/aoms/1177729694.
    /// MR39968.
    /// </summary>
    public class KullbackLeiblerDistanceCalculator
    {
        public int Len { get; set; }
        public int SequenceCount { get; set; }

        public KullbackLeiblerDistanceCalculator()
        {
            Len = 1000;
            SequenceCount = 10;
        }

        public double Distance(HiddenMarkovModel hmm1,
             HiddenMarkovModel hmm2)
        {
            double distance = 0.0;

            for (int i = 0; i < this.SequenceCount; i++)
            {

                IMLDataSet oseq = new MarkovGenerator(hmm1)
                        .ObservationSequence(this.Len);

                distance += (new ForwardBackwardScaledCalculator(oseq, hmm1)
                        .LnProbability() - new ForwardBackwardScaledCalculator(
                        oseq, hmm2).LnProbability())
                        / this.Len;
            }

            return distance / this.SequenceCount;
        }

    }
}
