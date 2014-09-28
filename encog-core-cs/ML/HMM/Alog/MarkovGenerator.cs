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
using Encog.MathUtil;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.ML.HMM.Alog
{
    /// <summary>
    /// This class is used to generate random sequences based on a Hidden Markov
    /// Model. These sequences represent the random probabilities that the HMM
    /// models.
    /// </summary>
    public class MarkovGenerator
    {
        private readonly HiddenMarkovModel _hmm;
        private int _currentState;

        public MarkovGenerator(HiddenMarkovModel hmm)
        {
            this._hmm = hmm;
            NewSequence();
        }

        public int CurrentState
        {
            get { return _currentState; }
        }

        public IMLSequenceSet GenerateSequences(int observationCount,
                                                int observationLength)
        {
            IMLSequenceSet result = new BasicMLSequenceSet();

            for (int i = 0; i < observationCount; i++)
            {
                result.StartNewSequence();
                result.Add(ObservationSequence(observationLength));
            }

            return result;
        }

        public void NewSequence()
        {
            double rand = ThreadSafeRandom.NextDouble();
            double current = 0.0;

            for (int i = 0; i < (_hmm.StateCount - 1); i++)
            {
                current += _hmm.GetPi(i);

                if (current > rand)
                {
                    _currentState = i;
                    return;
                }
            }

            _currentState = _hmm.StateCount - 1;
        }

        public IMLDataPair Observation()
        {
            IMLDataPair o = _hmm.StateDistributions[_currentState].Generate();
            double rand = ThreadSafeRandom.NextDouble();

            for (int j = 0; j < (_hmm.StateCount - 1); j++)
            {
                if ((rand -= _hmm.TransitionProbability[_currentState][j]) < 0)
                {
                    _currentState = j;
                    return o;
                }
            }

            _currentState = _hmm.StateCount - 1;
            return o;
        }

        public IMLDataSet ObservationSequence(int length)
        {
            var sequence = new BasicMLDataSet();
            while (length-- > 0)
            {
                sequence.Add(Observation());
            }
            NewSequence();

            return sequence;
        }
    }
}
