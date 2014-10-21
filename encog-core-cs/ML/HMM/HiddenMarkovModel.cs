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
using Encog.ML.HMM.Alog;
using Encog.ML.HMM.Distributions;
using Encog.Util;

namespace Encog.ML.HMM
{
    /// <summary>
    /// A Hidden Markov Model (HMM) is a Machine Learning Method that allows for
    /// predictions to be made about the hidden states and observations of a given
    /// system over time. A HMM can be thought of as a simple dynamic Bayesian
    /// network. The HMM is dynamic as it deals with changes that unfold over time.
    /// 
    /// The Hidden Markov Model is made up of a number of states and observations. A
    /// simple example might be the state of the economy. There are three hidden
    /// states, such as bull market, bear market and level. We do not know which
    /// state we are currently in. However, there are observations that can be made
    /// such as interest rate and the level of the S&P500. The HMM learns what state
    /// we are in by seeing how the observations change over time.
    /// 
    /// The HMM is only in one state at a given time. There is a percent probability
    /// that the HMM will move from one state to any of the other states. These
    /// probabilities are arranged in a grid, and are called the state transition
    /// probabilities.
    /// 
    /// Observations can be discrete or continuous. These observations allow the HMM
    /// to predict state transitions.
    /// 
    /// The HMM can handle single-value or multivariate observations.
    /// 
    /// http://www.heatonresearch.com/wiki/Hidden_Markov_Model
    /// 
    /// Rabiner, Juang, An introduction to Hidden Markov Models, IEEE ASSP Mag.,pp
    /// 4-16, June 1986.
    ///
    /// Baum, L. E.; Petrie, T. (1966).
    /// "Statistical Inference for Probabilistic Functions of Finite State Markov Chains"
    /// The Annals of Mathematical Statistics 37 (6): 1554-1563.
    /// 
    /// </summary>
    [Serializable]
    public class HiddenMarkovModel : BasicML, IMLStateSequence, ICloneable
    {
        public const String TAG_STATES = "sates";

        public const String TAG_ITEMS = "items";

        public const String TAG_PI = "pi";

        public const String TAG_TRANSITION = "transition";

        public const String TAG_DIST_TYPE = "type";

        public const String TAG_MEAN = "mean";

        public const String TAG_COVARIANCE = "covariance";

        public const String TAG_PROBABILITIES = "probabilities";
        private readonly int[] _items;
        private readonly IStateDistribution[] _stateDistributions;

        /// <summary>
        /// The initial probabilities for each state.
        /// </summary>
        private double[] pi;

        /// <summary>
        /// The transitional probabilities between the states.
        /// </summary>
        private double[][] _transitionProbability;


        /// <summary>
        /// Construct a discrete HMM with the specified number of states.
        /// </summary>
        /// <param name="states">The number of states.</param>
        public HiddenMarkovModel(int states)
        {
            _items = null;
            pi = new double[states];
            _transitionProbability = EngineArray.AllocateDouble2D(states, states);
            _stateDistributions = new IStateDistribution[states];

            for (int i = 0; i < states; i++)
            {
                pi[i] = 1.0/states;

                _stateDistributions[i] = new ContinousDistribution(
                    StateCount);

                for (int j = 0; j < states; j++)
                {
                    _transitionProbability[i][j] = 1.0/states;
                }
            }
        }

        public HiddenMarkovModel(int theStates, int theItems)
            : this(theStates, new[] {theItems})
        {
        }

        public HiddenMarkovModel(int theStates, int[] theItems)
        {
            _items = theItems;
            pi = new double[theStates];
            _transitionProbability = EngineArray.AllocateDouble2D(theStates, theStates);
            _stateDistributions = new IStateDistribution[theStates];

            for (int i = 0; i < theStates; i++)
            {
                pi[i] = 1.0/theStates;
                _stateDistributions[i] = new DiscreteDistribution(_items);

                for (int j = 0; j < theStates; j++)
                {
                    _transitionProbability[i][j] = 1.0/theStates;
                }
            }
        }

        public int StateCount
        {
            get { return pi.Length; }
        }

        public IStateDistribution[] StateDistributions
        {
            get { return _stateDistributions; }
        }

        public bool IsContinuous
        {
            get { return _items == null; }
        }

        public bool IsDiscrete
        {
            get { return !IsContinuous; }
        }

        public int[] Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Initial state probabilities.
        /// </summary>
        public double[] Pi
        {
            get { return pi; }
            set
            {
                if (value.Length != pi.Length)
                {
                    throw new EncogError("The length of pi, must match the number of states.");
                }
                pi = value;
            }
        }

        /// <summary>
        /// The probabilities of moving from one state to another.
        /// </summary>
        public double[][] TransitionProbability
        {
            get { return _transitionProbability; }
            set
            {
                if (value.Length != _transitionProbability.Length || value[0].Length != _transitionProbability[0].Length)
                {
                    throw new EncogError("Dimensions of transationalProbability must match number of states.");
                }
                _transitionProbability = value;
            }
        }

        #region IMLStateSequence Members

        public int[] GetStatesForSequence(IMLDataSet seq)
        {
            return (new ViterbiCalculator(seq, this)).CopyStateSequence();
        }

        public double Probability(IMLDataSet seq)
        {
            return (new ForwardBackwardCalculator(seq, this)).Probability();
        }

        public double Probability(IMLDataSet seq, int[] states)
        {
            if ((seq.Count != states.Length) || (seq.Count < 1))
            {
                new EncogError("Invalid count");
            }

            double probability = GetPi(states[0]);

            IEnumerator<IMLDataPair> oseqIterator = seq.GetEnumerator();

            for (int i = 0; i < (states.Length - 1); i++)
            {
                oseqIterator.MoveNext();
                probability *= _stateDistributions[i].Probability(
                    oseqIterator.Current)
                               *_transitionProbability[states[i]][states[i + 1]];
            }

            return probability
                   *_stateDistributions[states.Length - 1].Probability(
                       seq[states.Length - 1]);
        }

        #endregion

        public HiddenMarkovModel Clone()
        {
            HiddenMarkovModel hmm = CloneStructure();

            hmm.pi = (double[]) pi.Clone();
            hmm._transitionProbability = (double[][]) _transitionProbability.Clone();

            for (int i = 0; i < _transitionProbability.Length; i++)
            {
                hmm._transitionProbability[i] = (double[]) _transitionProbability[i].Clone();
            }

            for (int i = 0; i < hmm._stateDistributions.Length; i++)
            {
                hmm._stateDistributions[i] = _stateDistributions[i].Clone();
            }

            return hmm;
        }

        public HiddenMarkovModel CloneStructure()
        {
            HiddenMarkovModel hmm;

            if (IsDiscrete)
            {
                hmm = new HiddenMarkovModel(StateCount, _items);
            }
            else
            {
                hmm = new HiddenMarkovModel(StateCount);
            }

            return hmm;
        }

        public IStateDistribution CreateNewDistribution()
        {
            if (IsContinuous)
            {
                return new ContinousDistribution(StateCount);
            }
            else
            {
                return new DiscreteDistribution(_items);
            }
        }

        public double GetPi(int i)
        {
            return pi[i];
        }

        public double LnProbability(IMLDataSet seq)
        {
            return (new ForwardBackwardScaledCalculator(seq, this)).LnProbability();
        }

        public override void UpdateProperties()
        {
        }

        object ICloneable.Clone()
        {
            var result = new HiddenMarkovModel(StateCount);
            EngineArray.ArrayCopy(Pi, result.Pi);
            EngineArray.ArrayCopy(TransitionProbability, result.TransitionProbability);
            for (int i = 0; i < StateCount;i++)
            {
                result.StateDistributions[i] = StateDistributions[i].Clone();
            }
            return result;
        }
    }
}
