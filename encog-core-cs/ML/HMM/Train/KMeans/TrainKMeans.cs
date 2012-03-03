using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Train;
using Encog.ML.Data;
using Encog.ML.Train.Strategy;
using Encog.ML.Data.Basic;
using Encog.ML.HMM.Distributions;
using Encog.ML.HMM.Alog;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.HMM.Train.KMeans
{
    /// <summary>
    /// Train a Hidden Markov Model (HMM) with the KMeans algorithm. Makes use of
    /// KMeans clustering to estimate the transitional and observational
    /// probabilities for the HMM.
    /// 
    /// Unlike Baum Welch training, this method does not require a prior estimate of
    /// the HMM model, it starts from scratch.
    /// 
    /// Faber, Clustering and the Continuous k-Means Algorithm, Los Alamos Science,
    /// no. 22, 1994.
    /// </summary>
    public class TrainKMeans : IMLTrain
    {
        /// <summary>
        /// The clusters.
        /// </summary>
        private Clusters clusters;

        /// <summary>
        /// The number of states.
        /// </summary>
        private int states;

        /// <summary>
        /// Keep track of if we are done.
        /// </summary>
        private bool done;

        /// <summary>
        /// The HMM to use as a model.
        /// </summary>
        private HiddenMarkovModel modelHMM;

        /// <summary>
        /// The iteration number that we are currently on.
        /// </summary>
        public int IterationNumber { get; set; }

        /// <summary>
        /// The current HMM.
        /// </summary>
        private HiddenMarkovModel method;

        /// <summary>
        /// The training data.
        /// </summary>
        private IMLSequenceSet training;

        /// <summary>
        /// Construct a KMeans trainer. 
        /// </summary>
        /// <param name="method">The HMM.</param>
        /// <param name="sequences">The training data.</param>
        public TrainKMeans(HiddenMarkovModel method,
                IMLSequenceSet sequences)
        {
            this.method = method;
            this.modelHMM = method;
            this.states = method.StateCount;
            this.training = sequences;
            this.clusters = new Clusters(this.states, sequences);
            this.done = false;
        }

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {

        }

        /// <inheritdoc/>
        public double Error
        {
            get
            {
                return this.done ? 0 : 100;
            }
            set
            {
            }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get
            {
                return TrainingImplementationType.Iterative;
            }
        }


        /// <inheritdoc/>
        public IMLMethod Method
        {
            get
            {
                return this.method;
            }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public IMLDataSet Training
        {
            get
            {
                return this.training;
            }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get
            {
                return this.done;
            }
        }

        /// <inheritdoc/>
        public void Iteration()
        {
            HiddenMarkovModel hmm = this.modelHMM.CloneStructure();

            LearnPi(hmm);
            LearnTransition(hmm);
            LearnOpdf(hmm);

            this.done = OptimizeCluster(hmm);

            this.method = hmm;
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }

        }

        /// <summary>
        /// Learn the distribution. 
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        private void LearnOpdf(HiddenMarkovModel hmm)
        {
            for (int i = 0; i < hmm.StateCount; i++)
            {
                ICollection<IMLDataPair> clusterObservations = this.clusters
                        .Cluster(i);

                if (clusterObservations.Count < 1)
                {
                    IStateDistribution o = this.modelHMM.CreateNewDistribution();
                    hmm.StateDistributions[i] = o;
                }
                else
                {
                    IMLDataSet temp = new BasicMLDataSet();
                    foreach (IMLDataPair pair in clusterObservations)
                    {
                        temp.Add(pair);
                    }
                    hmm.StateDistributions[i].Fit(temp);
                }
            }
        }

        /// <summary>
        /// Learn Pi, the starting probabilities. 
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        private void LearnPi(HiddenMarkovModel hmm)
        {
            double[] pi = new double[this.states];

            for (int i = 0; i < this.states; i++)
            {
                pi[i] = 0.0;
            }

            foreach (IMLDataSet sequence in this.training.Sequences)
            {
                pi[this.clusters.Cluster(sequence[0])]++;
            }

            for (int i = 0; i < this.states; i++)
            {
                hmm.Pi[i] = pi[i] / (int)this.training.Count;
            }
        }
        
        /// <summary>
        /// Learn the state transitions. 
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        private void LearnTransition(HiddenMarkovModel hmm)
        {
            for (int i = 0; i < hmm.StateCount; i++)
            {
                for (int j = 0; j < hmm.StateCount; j++)
                {
                    hmm.TransitionProbability[i][j] = 0.0;
                }
            }

            foreach (IMLDataSet obsSeq in this.training.Sequences)
            {
                if (obsSeq.Count < 2)
                {
                    continue;
                }

                int first_state;
                int second_state = this.clusters.Cluster(obsSeq[0]);
                for (int i = 1; i < obsSeq.Count; i++)
                {
                    first_state = second_state;
                    second_state = this.clusters.Cluster(obsSeq[i]);

                    hmm.TransitionProbability[first_state][second_state] =
                            hmm.TransitionProbability[first_state][second_state] + 1.0;
                }
            }

            /* Normalize Aij array */
            for (int i = 0; i < hmm.StateCount; i++)
            {
                double sum = 0;

                for (int j = 0; j < hmm.StateCount; j++)
                {
                    sum += hmm.TransitionProbability[i][j];
                }

                if (sum == 0.0)
                {
                    for (int j = 0; j < hmm.StateCount; j++)
                    {
                        hmm.TransitionProbability[i][j] = 1.0 / hmm.StateCount;
                    }
                }
                else
                {
                    for (int j = 0; j < hmm.StateCount; j++)
                    {
                        hmm.TransitionProbability[i][j] /= sum;
                    }
                }
            }
        }


        /// <summary>
        /// Optimize the clusters. 
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        /// <returns>True if the cluster was not modified.</returns>
        private bool OptimizeCluster(HiddenMarkovModel hmm)
        {
            bool result = false;

            foreach (IMLDataSet obsSeq in this.training.Sequences)
            {
                ViterbiCalculator vc = new ViterbiCalculator(obsSeq, hmm);
                int[] states = vc.CopyStateSequence();

                for (int i = 0; i < states.Length; i++)
                {
                    IMLDataPair o = obsSeq[i];

                    if (this.clusters.Cluster(o) != states[i])
                    {
                        result = true;
                        this.clusters.Remove(o, this.clusters.Cluster(o));
                        this.clusters.Put(o, states[i]);
                    }
                }
            }

            return !result;
        }

        /// <inheritdoc/>
        public TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public void Resume(TrainingContinuation state)
        {
        }



    }
}
