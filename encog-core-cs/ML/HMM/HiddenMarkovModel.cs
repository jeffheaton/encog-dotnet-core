using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.HMM.Distributions;
using Encog.Util;
using Encog.ML.Data;
using Encog.ML.HMM.Alog;

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
    public class HiddenMarkovModel : BasicML, IMLStateSequence
    {
        public const String TAG_STATES = "sates";

        public const String TAG_ITEMS = "items";

        public const String TAG_PI = "pi";

        public const String TAG_TRANSITION = "transition";

        public const String TAG_DIST_TYPE = "type";

        public const String TAG_MEAN = "mean";

        public const String TAG_COVARIANCE = "covariance";

        public const String TAG_PROBABILITIES = "probabilities";

        /**
         * The initial probabilities for each state.
         */
        private double[] pi;

        /**
         * The transitional probabilities between the states.
         */
        private double[][] transitionProbability;

        /**
         * The mapping of observation probabilities to the
         * states.
         */
        private readonly IStateDistribution[] stateDistributions;

        /**
         * The counts for each item in a discrete HMM.
         */
        private int[] items;

        /**
         * Construct a discrete HMM with the specified number of states.
         * @param states The number of states.
         */
        public HiddenMarkovModel(int states)
        {
            this.items = null;
            this.pi = new double[states];
            this.transitionProbability = EngineArray.AllocateDouble2D(states, states);
            this.stateDistributions = new IStateDistribution[states];

            for (int i = 0; i < states; i++)
            {
                this.pi[i] = 1.0 / states;

                this.stateDistributions[i] = new ContinousDistribution(
                        StateCount);

                for (int j = 0; j < states; j++)
                {
                    this.transitionProbability[i][j] = 1.0 / states;
                }
            }
        }

        public HiddenMarkovModel(int theStates, int theItems)
            : this(theStates, new int[] { theItems })
        {

        }

        public HiddenMarkovModel(int theStates, int[] theItems)
        {
            this.items = theItems;
            this.pi = new double[theStates];
            this.transitionProbability = EngineArray.AllocateDouble2D(theStates, theStates);
            this.stateDistributions = new IStateDistribution[theStates];

            for (int i = 0; i < theStates; i++)
            {
                this.pi[i] = 1.0 / theStates;
                this.stateDistributions[i] = new DiscreteDistribution(this.items);

                for (int j = 0; j < theStates; j++)
                {
                    this.transitionProbability[i][j] = 1.0 / theStates;
                }
            }
        }

        public HiddenMarkovModel Clone()
        {
            HiddenMarkovModel hmm = CloneStructure();

            hmm.pi = (double[])this.pi.Clone();
            hmm.transitionProbability = (double[][])this.transitionProbability.Clone();

            for (int i = 0; i < this.transitionProbability.Length; i++)
            {
                hmm.transitionProbability[i] = (double[])this.transitionProbability[i].Clone();
            }

            for (int i = 0; i < hmm.stateDistributions.Length; i++)
            {
                hmm.stateDistributions[i] = this.stateDistributions[i].Clone();
            }

            return hmm;
        }

        public HiddenMarkovModel CloneStructure()
        {
            HiddenMarkovModel hmm;

            if (IsDiscrete)
            {
                hmm = new HiddenMarkovModel(StateCount, this.items);
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
                return new DiscreteDistribution(this.items);
            }
        }

        public double GetPi(int i)
        {
            return this.pi[i];
        }

        public int StateCount
        {
            get
            {
                return this.pi.Length;
            }
        }

        public IStateDistribution getStateDistribution(int i)
        {
            return this.stateDistributions[i];
        }

        public int[] GetStatesForSequence(IMLDataSet seq)
        {
            return (new ViterbiCalculator(seq, this)).CopyStateSequence();
        }

        public double getTransitionProbability(int i, int j)
        {
            return this.transitionProbability[i][j];
        }

        public bool IsContinuous
        {
            get
            {
                return this.items == null;
            }
        }

        public bool IsDiscrete
        {
            get
            {
                return !IsContinuous;
            }
        }

        public double LnProbability(IMLDataSet seq)
        {
            return (new ForwardBackwardScaledCalculator(seq, this)).LnProbability();
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
                probability *= getStateDistribution(states[i]).Probability(
                        oseqIterator.Current)
                        * getTransitionProbability(states[i], states[i + 1]);
            }

            return probability
                    * getStateDistribution(states[states.Length - 1]).Probability(
                            seq[states.Length - 1]);
        }

        public override void UpdateProperties()
        {

        }

        public int[] Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Initial state probabilities.
        /// </summary>
        public double[] Pi
        {
            get
            {
                return this.pi;
            }
            set
            {
                if (value.Length != this.pi.Length)
                {
                    throw new EncogError("The length of pi, must match the number of states.");
                }
                this.pi = value;
            }
        }

        /// <summary>
        /// The probabilities of moving from one state to another.
        /// </summary>
        public double[][] TransitionProbability
        {
            get
            {
                return this.transitionProbability;
            }
            set
            {
                if (value.Length != this.transitionProbability.Length || value[0].Length != this.transitionProbability[0].Length)
                {
                    throw new EncogError("Dimensions of transationalProbability must match number of states.");
                }
                this.transitionProbability = value;
            }
        }
    }
}
