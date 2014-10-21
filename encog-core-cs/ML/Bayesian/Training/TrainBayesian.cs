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
using Encog.ML.Bayesian.Training.Estimator;
using Encog.ML.Bayesian.Training.Search;
using Encog.ML.Bayesian.Training.Search.k2;
using Encog.ML.Data;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation;

namespace Encog.ML.Bayesian.Training
{
    /// <summary>
    /// Train a Bayesian network.
    /// </summary>
    public sealed class TrainBayesian : BasicTraining
    {
        /// <summary>
        /// The data used for training.
        /// </summary>
        private readonly IMLDataSet _data;

        /// <summary>
        /// The method used to estimate the probabilities.
        /// </summary>
        private readonly IBayesEstimator _estimator;

        /// <summary>
        /// The maximum parents a node should have.
        /// </summary>
        private readonly int _maximumParents;

        /// <summary>
        /// The network to train.
        /// </summary>
        private readonly BayesianNetwork _network;

        /// <summary>
        /// The method used to search for the best network structure.
        /// </summary>
        private readonly IBayesSearch _search;

        /// <summary>
        /// Used to hold the query.
        /// </summary>
        private String _holdQuery;

        /// <summary>
        /// The method used to setup the initial Bayesian network.
        /// </summary>
        private BayesianInit _initNetwork = BayesianInit.InitNaiveBayes;

        /// <summary>
        /// The phase that training is currently in.
        /// </summary>
        private Phase _p = Phase.Init;

        /// <summary>
        /// Construct a Bayesian trainer. Use K2 to search, and the SimpleEstimator
        /// to estimate probability.  Init as Naive Bayes
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theData">The data to train.</param>
        /// <param name="theMaximumParents">The max number of parents.</param>
        public TrainBayesian(BayesianNetwork theNetwork, IMLDataSet theData,
                             int theMaximumParents)
            : this(theNetwork, theData, theMaximumParents,
                   BayesianInit.InitNaiveBayes, new SearchK2(),
                   new SimpleEstimator())
        {
        }

        /// <summary>
        /// Construct a Bayesian trainer. 
        /// </summary>
        /// <param name="theNetwork">The network to train.</param>
        /// <param name="theData">The data to train with.</param>
        /// <param name="theMaximumParents">The maximum number of parents.</param>
        /// <param name="theInit">How to init the new Bayes network.</param>
        /// <param name="theSearch">The search method.</param>
        /// <param name="theEstimator">The estimation mehod.</param>
        public TrainBayesian(BayesianNetwork theNetwork, IMLDataSet theData,
                             int theMaximumParents, BayesianInit theInit, IBayesSearch theSearch,
                             IBayesEstimator theEstimator)
            : base(TrainingImplementationType.Iterative)
        {
            _network = theNetwork;
            _data = theData;
            _maximumParents = theMaximumParents;

            _search = theSearch;
            _search.Init(this, theNetwork, theData);

            _estimator = theEstimator;
            _estimator.Init(this, theNetwork, theData);

            _initNetwork = theInit;
            Error = 1.0;
        }

        /// <inheritdoc/>
        public override bool TrainingDone
        {
            get { return base.TrainingDone || _p == Phase.Terminated; }
        }

        /// <inheritdoc/>
        public override bool CanContinue
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override IMLMethod Method
        {
            get { return _network; }
        }

        /// <summary>
        /// Returns the network.
        /// </summary>
        public BayesianNetwork Network
        {
            get { return _network; }
        }

        /// <summary>
        /// The maximum parents a node can have.
        /// </summary>
        public int MaximumParents
        {
            get { return _maximumParents; }
        }

        /// <summary>
        /// The search method.
        /// </summary>
        public IBayesSearch Search
        {
            get { return _search; }
        }

        /// <summary>
        /// The init method.
        /// </summary>
        public BayesianInit InitNetwork
        {
            get { return _initNetwork; }
            set { _initNetwork = value; }
        }

        /// <summary>
        /// Init to Naive Bayes.
        /// </summary>
        private void InitNaiveBayes()
        {
            // clear out anything from before
            _network.RemoveAllRelations();

            // locate the classification target event
            BayesianEvent classificationTarget = _network
                .ClassificationTargetEvent;

            // now link everything to this event
            foreach (BayesianEvent e in _network.Events)
            {
                if (e != classificationTarget)
                {
                    _network.CreateDependency(classificationTarget, e);
                }
            }

            _network.FinalizeStructure();
        }

        /// <summary>
        /// Handle iterations for the Init phase.
        /// </summary>
        private void IterationInit()
        {
            _holdQuery = _network.ClassificationStructure;

            switch (_initNetwork)
            {
                case BayesianInit.InitEmpty:
                    _network.RemoveAllRelations();
                    _network.FinalizeStructure();
                    break;
                case BayesianInit.InitNoChange:
                    break;
                case BayesianInit.InitNaiveBayes:
                    InitNaiveBayes();
                    break;
            }
            _p = Phase.Search;
        }

        /// <summary>
        /// Handle iterations for the Search phase.
        /// </summary>
        private void IterationSearch()
        {
            if (!_search.Iteration())
            {
                _p = Phase.SearchDone;
            }
        }

        /// <summary>
        /// Handle iterations for the Search Done phase.
        /// </summary>
        private void IterationSearchDone()
        {
            _network.FinalizeStructure();
            _network.Reset();
            _p = Phase.Probability;
        }

        /// <summary>
        /// Handle iterations for the Probability phase.
        /// </summary>
        private void IterationProbability()
        {
            if (!_estimator.Iteration())
            {
                _p = Phase.Finish;
            }
        }

        /// <summary>
        /// Handle iterations for the Finish phase.
        /// </summary>
        private void IterationFinish()
        {
            _network.DefineClassificationStructure(_holdQuery);
            Error = _network.CalculateError(_data);
            _p = Phase.Terminated;
        }

        /// <inheritdoc/>
        public override void Iteration()
        {
            PreIteration();

            switch (_p)
            {
                case Phase.Init:
                    IterationInit();
                    break;
                case Phase.Search:
                    IterationSearch();
                    break;
                case Phase.SearchDone:
                    IterationSearchDone();
                    break;
                case Phase.Probability:
                    IterationProbability();
                    break;
                case Phase.Finish:
                    IterationFinish();
                    break;
            }

            PostIteration();
        }

        /// <inheritdoc/>
        public override TrainingContinuation Pause()
        {
            return null;
        }

        /// <inheritdoc/>
        public override void Resume(TrainingContinuation state)
        {
        }

        #region Nested type: Phase

        /// <summary>
        /// What phase of training are we in?
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// Init phase.
            /// </summary>
            Init,
            /// <summary>
            /// Searching for a network structure.
            /// </summary>
            Search,
            /// <summary>
            /// Search complete.
            /// </summary>
            SearchDone,
            /// <summary>
            /// Finding probabilities.
            /// </summary>
            Probability,
            /// <summary>
            /// Finished training.
            /// </summary>
            Finish,
            /// <summary>
            /// Training terminated.
            /// </summary>
            Terminated
        } ;

        #endregion
    }
}
