using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.MathUtil;

namespace Encog.ML.HMM.Alog
{
    /// <summary>
    /// This class is used to generate random sequences based on a Hidden Markov
    /// Model. These sequences represent the random probabilities that the HMM
    /// models.
    /// </summary>
    public class MarkovGenerator
    {
        private HiddenMarkovModel hmm;
        private int currentState;

        public MarkovGenerator(HiddenMarkovModel hmm)
        {
            this.hmm = hmm;
            NewSequence();
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

        public int CurrentState
        {
            get
            {
                return this.currentState;
            }
        }

        public void NewSequence()
        {
            double rand = ThreadSafeRandom.NextDouble();
            double current = 0.0;

            for (int i = 0; i < (this.hmm.StateCount - 1); i++)
            {
                current += this.hmm.GetPi(i);

                if (current > rand)
                {
                    this.currentState = i;
                    return;
                }
            }

            this.currentState = this.hmm.StateCount - 1;
        }

        public IMLDataPair Observation()
        {
            IMLDataPair o = this.hmm.StateDistributions[this.currentState].Generate();
            double rand = ThreadSafeRandom.NextDouble();

            for (int j = 0; j < (this.hmm.StateCount - 1); j++)
            {
                if ((rand -= this.hmm.TransitionProbability[this.currentState][j]) < 0)
                {
                    this.currentState = j;
                    return o;
                }
            }

            this.currentState = this.hmm.StateCount - 1;
            return o;
        }

        public IMLDataSet ObservationSequence(int length)
        {
            IMLDataSet sequence = new BasicMLDataSet();
            while (length-- > 0)
            {
                sequence.Add(Observation());
            }
            NewSequence();

            return sequence;
        }
    }
}
