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
        public KullbackLeiblerDistanceCalculator()
        {
            Len = 1000;
            SequenceCount = 10;
        }

        public int Len { get; set; }
        public int SequenceCount { get; set; }

        public double Distance(HiddenMarkovModel hmm1,
                               HiddenMarkovModel hmm2)
        {
            double distance = 0.0;

            for (int i = 0; i < SequenceCount; i++)
            {
                IMLDataSet oseq = new MarkovGenerator(hmm1)
                    .ObservationSequence(Len);

                distance += (new ForwardBackwardScaledCalculator(oseq, hmm1)
                                 .LnProbability() - new ForwardBackwardScaledCalculator(
                                                        oseq, hmm2).LnProbability())
                            /Len;
            }

            return distance/SequenceCount;
        }
    }
}