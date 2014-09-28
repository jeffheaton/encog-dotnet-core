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
