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
using System.Collections.Generic;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.HMM.Alog;
using Encog.ML.HMM.Distributions;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
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
        private readonly Clusters _clusters;

        /// <summary>
        /// The HMM to use as a model.
        /// </summary>
        private readonly HiddenMarkovModel _modelHmm;

        /// <summary>
        /// The number of states.
        /// </summary>
        private readonly int _states;

        /// <summary>
        /// The training data.
        /// </summary>
        private readonly IMLSequenceSet _training;

        /// <summary>
        /// Keep track of if we are done.
        /// </summary>
        private bool _done;

        /// <summary>
        /// The current HMM.
        /// </summary>
        private HiddenMarkovModel _method;

        /// <summary>
        /// Construct a KMeans trainer. 
        /// </summary>
        /// <param name="method">The HMM.</param>
        /// <param name="sequences">The training data.</param>
        public TrainKMeans(HiddenMarkovModel method,
                           IMLSequenceSet sequences)
        {
            _method = method;
            _modelHmm = method;
            _states = method.StateCount;
            _training = sequences;
            _clusters = new Clusters(_states, sequences);
            _done = false;
        }

        #region IMLTrain Members

        /// <summary>
        /// The iteration number that we are currently on.
        /// </summary>
        public int IterationNumber { get; set; }

        /// <inheritdoc/>
        public void AddStrategy(IStrategy strategy)
        {
        }

        /// <inheritdoc/>
        public bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public void FinishTraining()
        {
        }

        /// <inheritdoc/>
        public double Error
        {
            get { return _done ? 0 : 100; }
            set { }
        }

        /// <inheritdoc/>
        public TrainingImplementationType ImplementationType
        {
            get { return TrainingImplementationType.Iterative; }
        }


        /// <inheritdoc/>
        public IMLMethod Method
        {
            get { return _method; }
        }

        /// <inheritdoc/>
        public IList<IStrategy> Strategies
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public IMLDataSet Training
        {
            get { return _training; }
        }

        /// <inheritdoc/>
        public bool TrainingDone
        {
            get { return _done; }
        }

        /// <inheritdoc/>
        public void Iteration()
        {
            HiddenMarkovModel hmm = _modelHmm.CloneStructure();

            LearnPi(hmm);
            LearnTransition(hmm);
            LearnOpdf(hmm);

            _done = OptimizeCluster(hmm);

            _method = hmm;
        }

        /// <inheritdoc/>
        public void Iteration(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Iteration();
            }
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

        #endregion

        /// <summary>
        /// Learn the distribution. 
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        private void LearnOpdf(HiddenMarkovModel hmm)
        {
            for (int i = 0; i < hmm.StateCount; i++)
            {
                ICollection<IMLDataPair> clusterObservations = _clusters
                    .Cluster(i);

                if (clusterObservations.Count < 1)
                {
                    IStateDistribution o = _modelHmm.CreateNewDistribution();
                    hmm.StateDistributions[i] = o;
                }
                else
                {
                    var temp = new BasicMLDataSet();
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
            var pi = new double[_states];

            for (int i = 0; i < _states; i++)
            {
                pi[i] = 0.0;
            }

            foreach (IMLDataSet sequence in _training.Sequences)
            {
                pi[_clusters.Cluster(sequence[0])]++;
            }

            for (int i = 0; i < _states; i++)
            {
                hmm.Pi[i] = pi[i]/(int) _training.Count;
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

            foreach (IMLDataSet obsSeq in _training.Sequences)
            {
                if (obsSeq.Count < 2)
                {
                    continue;
                }

                int secondState = _clusters.Cluster(obsSeq[0]);
                for (int i = 1; i < obsSeq.Count; i++)
                {
                    int firstState = secondState;
                    secondState = _clusters.Cluster(obsSeq[i]);

                    hmm.TransitionProbability[firstState][secondState] =
                        hmm.TransitionProbability[firstState][secondState] + 1.0;
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
                        hmm.TransitionProbability[i][j] = 1.0/hmm.StateCount;
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

            foreach (IMLDataSet obsSeq in _training.Sequences)
            {
                var vc = new ViterbiCalculator(obsSeq, hmm);
                int[] states = vc.CopyStateSequence();

                for (int i = 0; i < states.Length; i++)
                {
                    IMLDataPair o = obsSeq[i];

                    if (_clusters.Cluster(o) != states[i])
                    {
                        result = true;
                        _clusters.Remove(o, _clusters.Cluster(o));
                        _clusters.Put(o, states[i]);
                    }
                }
            }

            return !result;
        }
    }
}
